


Imports System
Imports System.IO
Imports System.Net
Imports System.String
Imports System.ComponentModel
Imports System.Windows.Forms
Imports System.Data.SqlClient
Imports System.Runtime.InteropServices '   DllImport
Imports System.Security.Principal '  WindowsImpersonationContext
Imports System.Security.Permissions
Imports System.Text



Module common
    Public lock As New Object()
    Public FinalStat As String = ""
    Public CurVal(99) As String 'used to pass extra values out of backgroundworker2
    Public curval1 As Integer
    Public EMHost As String  '"lecri01-141em" 'entered on startup screen or saved in settings
    Public EMUserENC As String 'created in WSlogin
    Public EMUser As String  '"Administrator" 'entered on startup screen or save in settings
    Public EMDomain As String '"lecri01-141em" ''entered on startup screen or save in settings
    Public EMPassword As String ' "CAdemo123" 'entered on startup screen or save in settings
    Public EMPasswordENC As String 'created in WSlogin
    Public EMValidated As Boolean 'set in sartup.emvalidate if credentials are good
    Public FirstPass As Integer = 0 ' used to see if this is the first time the app has run on this session it sets things like mapped drives
    Public dsms As New clientauto.DSMWebServiceAPIService
    Public Alreadymapped As Boolean = False 'check if there is already a drive mapped to emhost\sdlibrary if set it does not unmap at exit
    Public SessionID As New Object 'web service session ID
    Public SessionIDset As Boolean = False 'used to determine if WSlogoff is needed at exit
    Public TotalPackageCount As Long 'Total number of sd packages registered on the EM
    Public PackageNameList(10000) As String 'empackage names
    Public PackageVersionList(10000) As String 'empackage versions
    Public PackageUUIDList(10000) As String 'empackage uuids
    Public Suggestions(10000) As String 'concantinated package name and version for display in gui
    Public SelectedPackageName As String
    Public SelectedPackageVersion As String
    Public SelectedPackageUUID As String
    Public MyHostName As String = My.Computer.Name 'used to determine if this app is running on an em or dm to determine where to log and map drives
    Public SDPath As String 'path of the seleted sd package in the sd library on the em
    Public MyDrive As String 'specifies where emhost\sdlibrary is mapped
    Public IsLocal As Boolean
    Public SelectedPackageNameTmp As String 'modified package name since package name is used to creae working directories this name removes characters that cannot be in a directory path 
    Public SelectedPackageversionTmp As String 'same as above
    Public TotalDM As Long 'number of total dms registered to the em
    Public DMName(100, 1) As String ' domain name and domainuuid
    Public Workingdir As String 'the path where working files are placed it is mydrive + \exports\ + selectedpackagenametmp + selectedpackageversiontmp
    Public IsDistName(100, 3) As String ' the list of domains packge is distributed to, domain uuid,online=TRUE or FALSE,containerid
    Public DistCount As Integer 'to count of domains package is distributed to
    Public sslist(100, 2) As String 'list of all ss, what dm is it registered to , yes if pacjkage is staged to it
    Public TotalSS As Integer 'count of ss list
    Public ModifyPackagename As String 'name of the sd package created to update package content =update + selectedpackagename + selected package version
    Public ModifyPackageVersion As String 'version = date/time package was modified
    Public DistContainerName As String 'name of the distrubution container modifypackagename +modifypackageversion
    Public Uselocal As Boolean 'tells logins to assume credentials are local not domain so sets user as hostname\user instade of emdomain\user
    Public sdpackagepath As String
    Public differencespath As String
    Public BGStatus As String
    Public AllServerUUID As String 'uuid of the alldomain group on the em used in wsgetdistrubutionstatus


    Function WSLogin(HostName As String) As String
        Dim tmpuser As String
        UpdateLog(1, "common.WSlogin", "Attemping to get sessionID from " + HostName, 0)
        SessionID = ""
        If Uselocal = True Then
            tmpuser = HostName + "/" + EMUser
        Else
            tmpuser = EMDomain + "/" + EMUser
        End If
        Try
            dsms.Url = "http://" + HostName + "/DSM_Webservice/mod_gsoap.dll"
            EMPasswordENC = Convert.ToBase64String(New System.Text.ASCIIEncoding().GetBytes(EMPassword))
            EMUserENC = Convert.ToBase64String(New System.Text.ASCIIEncoding().GetBytes("winnt://" + tmpuser))
            UpdateLog(1, "common.WSlogin", "Trying To login To WebServices On Server " + HostName + " as user " + tmpuser, 0)
            SessionID = dsms.Login2(EMUserENC, EMPasswordENC, HostName)
        Catch ex As Exception
            MessageBox.Show("Trying To login To WebServices On Server " + HostName + vbCrLf + "as user " + tmpuser + vbCrLf + "Process returned the following error" + vbCrLf + ex.Message)
            UpdateLog(1, "common.WSlogin", "Trying To login To WebServices On Server " + HostName + "as user " + tmpuser + "Process returned the following error" + ex.Message, 2)
            Return "NOTOK"

        End Try
        UpdateLog(1, "common.WSlogin", "Obtained sessionid " + SessionID.ToString, 0)

        Return "OK"
    End Function

    Function WSLogout() As String
        If InStr(MyDrive, ":") > 0 Then
            UpdateLog(1, "common.logout", "Attemping to release sessionID from " + EMHost, 0)
        End If
        If SessionIDset = True Then
            Try
                dsms.Logout(SessionID.ToString)
            Catch ex As Exception
                UpdateLog(1, "common.logout", "Failed to release sessionID from " + EMHost, 1)
                Return "NOTOK"
            End Try
            UpdateLog(1, "common.logout", "SessionID Released ", 0)
            SessionIDset = False
        End If
        Return "OK"
    End Function
    Function WSGetAllSoftware() As String
        UpdateLog(1, "common.GetAllSoftware", "Attemping Get total number of software pacackages registered on " + EMHost, 0)
        Dim WSoutSoftwarePackageList() As clientauto.SoftwarePackageProperties2
        Dim ReturnText As String = Nothing
        Dim Failed As Boolean = False
        Dim index As Long = 0
        Dim numrequired As Long
        Dim SoftwarePackageGroupID As String = Nothing 'if groupuuid is specified then the search will only get packages from that group if set to nothing it will get all packages (NOTE nothing is not the same as "")
        Dim TotalCount As Long = Nothing
        Dim j As Integer
        Dim packpropreq As New clientauto.SoftwarePackagePropertiesRequired With {
        .softwarePackageNameRequired = True,
        .softwarePackageVersionRequired = True,
        .softwarePackageIdRequired = True}

        Dim sortprop As clientauto.SoftwarePackageProperty = clientauto.SoftwarePackageProperty.SDPKGNAME

        Dim packfilter(0) As clientauto.SoftwarePackageFilter
        packfilter(0) = New clientauto.SoftwarePackageFilter() With {
        .swPkgProperty = clientauto.SoftwarePackageProperty.SDPKGNAME,
        .condition = clientauto.FILTERCONDITION.FILTERWILDCARDEQ,
        .searchString = "*"}

        Dim ArrayOfSoftwarePackageFilter As clientauto.ArrayOfSoftwarePackageFilter = New clientauto.ArrayOfSoftwarePackageFilter With {
        .filter = packfilter,
        .matchAll = True}
        Try
            WSoutSoftwarePackageList = dsms.GetSoftwarePackageList(SessionID.ToString, SoftwarePackageGroupID, packpropreq, ArrayOfSoftwarePackageFilter, clientauto.SoftwarePackageProperty.SDPKGBASEDONPKGNAME, True, index, numrequired, True, TotalCount)
        Catch ex As Exception
            UpdateLog(1, "common.GetAllSoftware", "Trying to Get total number of software packages registered returned the following error" + ex.Message, 0)
            MessageBox.Show("Trying to Get Total number of software packages" + vbCrLf + "Process returned the following error" + vbCrLf + ex.Message)
            Return "NOTOK"
            Exit Function
        End Try
        If TotalCount = 0 Then
            UpdateLog(1, "common.GetAllSoftware", "There are " + Str(TotalCount) + " registered packages on EM " + EMHost, 2)
            MessageBox.Show("There are no software packages")
            Return "NOTOK"
            Exit Function
        End If
        numrequired = TotalCount

        UpdateLog(1, "common.GetAllSoftware", "There are " + Str(TotalCount) + " registered packages on EM " + EMHost, 0)
        UpdateLog(1, "common.GetAllSoftware", "Trying to Get list of  all software packages on EM " + EMHost, 0)
        Try
            WSoutSoftwarePackageList = dsms.GetSoftwarePackageList(SessionID.ToString, SoftwarePackageGroupID, packpropreq, ArrayOfSoftwarePackageFilter, clientauto.SoftwarePackageProperty.SDPKGBASEDONPKGNAME, True, index, numrequired, True, TotalCount)
        Catch ex As Exception
            UpdateLog(1, "common.GetAllSoftware", "Process returned the following error" + vbCrLf + ex.Message, 2)
            MessageBox.Show("Trying to logoff from WebServices" + vbCrLf + "Process returned the following error" + vbCrLf + ex.Message)
            Return "NOTOK"
            Exit Function
        End Try
        If TotalCount = 0 Then
            MessageBox.Show("There are no software packages")
            Return "NOTOK"
            Exit Function
        End If
        TotalPackageCount = TotalCount - 1
        For j = 0 To TotalPackageCount
            PackageNameList(j) = WSoutSoftwarePackageList(j).softwarePackageName
            PackageVersionList(j) = WSoutSoftwarePackageList(j).softwarePackageVersion
            Suggestions(j) = PackageNameList(j) + "'" + PackageVersionList(j)
            PackageUUIDList(j) = WSoutSoftwarePackageList(j).softwarePackageId
        Next
        UpdateLog(1, "common.GetAllSoftware", "Get list of  all software packages completed " + EMHost, 0)
        Return "OK"
    End Function
    Function WSUNChecksumsoftwarePackage(hostname As String, IsEM As Boolean) As String
        Dim rtn As String
        Dim PackageUUID As String
        UpdateLog(1, "common.UnChecksumSoftwarePackage", "Disabling the checksum on host " + hostname, 0)
        rtn = WSLogin(hostname)
        If rtn <> "OK" Then
            Return "NOTOK"
            Exit Function
        End If
        If IsEM = True Then
            PackageUUID = SelectedPackageUUID
        Else
            PackageUUID = WSGetSDPackageByName(SelectedPackageName, SelectedPackageVersion)
        End If
        Dim SetSoftwarePackageProperties3 As clientauto.SetSoftwarePackageProperties3 = New clientauto.SetSoftwarePackageProperties3 With {
        .softwarePackageId = PackageUUID,
        .enableCheckSum = False,
        .enableCheckSumSupplied = True}
        Try
            dsms.SetSoftwarePackage3(SessionID.ToString, SetSoftwarePackageProperties3, False)

        Catch ex As Exception
            MessageBox.Show("Trying to unChecksum the Package" + vbCrLf + "Process returned the following error" + vbCrLf + ex.Message)
            Return "NOTOK"

        End Try
        Return "OK"
        rtn = WSLogout()
        If rtn <> "OK" Then
            Return "NOTOK"
            Exit Function
        End If
        Return "OK"
    End Function
    Function WSExportSDPackage() As String
        UpdateLog(1, "common.ExportSDPackage", "Attempting to export " + SelectedPackageName + ":" + SelectedPackageVersion + " to " + MyDrive + "\sdlibrary\exports\" & SelectedPackageNameTmp & "'" & SelectedPackageversionTmp & "\original", 0)
        Dim rtn As String = WSLogin(EMHost)
        If rtn <> "OK" Then
            UpdateLog(1, "common.ExportSDPackage", "Failed to login into web services", 2)
            Return "NOTOK"
            Exit Function
        End If

        Dim exportSoftwarePackageProperties As clientauto.ExportSoftwarePkgProperties = New clientauto.ExportSoftwarePkgProperties With {
        .targetOnServer = True,
        .exportAs = clientauto.PackageExportType.LIBRARYIMAGE,
        .targetPath = "\\" + EMHost + "\sdlibrary\exports\" & SelectedPackageNameTmp & "'" & SelectedPackageversionTmp & "\original"}
        Try
            dsms.ExportSoftwarePackage(SessionID.ToString, SelectedPackageUUID, exportSoftwarePackageProperties)
        Catch ex As Exception
            UpdateLog(1, "common.ExportSDPackage", "Export failed process returned the following error", 2)
            UpdateLog(1, "common.ExportSDPackage", ex.Message, 2)
            MessageBox.Show("Trying to Export the Package" + vbCrLf + "Process returned the following error" + vbCrLf + ex.Message)
            Return "NOTOK"

        End Try
        UpdateLog(1, "common.ExportSDPackage", "Export complete", 0)
        Return "OK"
        rtn = WSLogout()
        Return "OK"
    End Function
    Function WSGetSDPackageByName(PN As String, pV As String) As String

        UpdateLog(1, "common.GetSDPackagebyName", "Getting the uuid of the SD package " + PN + ":" + pV, 0)
        Dim rtn As String
        Try
            rtn = dsms.FindSoftwarePackage(SessionID.ToString, PN, pV)
        Catch ex As Exception
            UpdateLog(1, "common.GetSDPackagebyName", "Getting the uuid of the SD package " + PN + ":" + pV + " failed with the following error", 2)
            UpdateLog(1, "common.GetSDPackagebyName", ex.Message, 2)
            MessageBox.Show("Getting UUID for selected package from the domain failed with error" + vbCrLf + ex.Message)
            Return "NOTOK"
            Exit Function
        End Try
        UpdateLog(1, "common.GetSDPackagebyName", "UUID of the selected SD package is " + rtn, 0)
        Return rtn





    End Function
    Function WSGetAllDomains() As String
        Dim DN As String = "*"
        Dim dpr As clientauto.DomainPropertiesRequired = New clientauto.DomainPropertiesRequired
        Dim tc As Long = 100

        Dim DP() As clientauto.DomainProperties
        dpr.domainUUIDRequired = True
        dpr.domainLabelRequired = True
        UpdateLog(1, "common.WSGetAllDomains", "Attemping to get all domain names and their UUIDs", 0)
        Dim tmpcount As Long
        Try
            DP = dsms.FindDomain(SessionID.ToString, DN, tc, dpr, tmpcount, True)
        Catch ex As Exception
            UpdateLog(1, "common.WSGetAllDomains", "Getting all domains failed with the following error", 2)
            UpdateLog(1, "common.GetAllDomains", ex.Message, 2)
            MessageBox.Show("Getting All Domains failed with error" + vbCrLf + ex.Message)
            Return "NOTOK"
            Exit Function
        End Try
        If tmpcount > 1 Then
            TotalDM = -1
            Dim j As Integer
            For j = 0 To tmpcount - 1
                If EMHost <> DP(j).domainLabel Then
                    TotalDM = TotalDM + 1
                    DMName(TotalDM, 0) = DP(j).domainLabel
                    DMName(TotalDM, 1) = DP(j).domainUUID
                    UpdateLog(1, "common.WSGetAllDomains", "Found domain " + DP(j).domainLabel + "  Domain UUID " + DP(j).domainUUID, 0)
                End If

            Next

        End If
        If TotalDM = -1 Then
            UpdateLog(1, "common.WSGetAllDomains", "Found no registered domains", 1)
        Else
            UpdateLog(1, "common.WSGetAllDomains", "Found a total of " + Str(TotalDM + 1) + " registered domains", 0)
        End If
        Return "OK"
    End Function
    Function WSFindDMGroup() As String
        UpdateLog(1, "common.WSFindDMGroup", "Getting the $AllServers group UUID", 0)
        Dim DGPR As clientauto.DomainGroupPropertiesRequired = New clientauto.DomainGroupPropertiesRequired With {
        .groupUUIDRequired = 1,
        .groupLabelRequired = 1}

        Dim DP() As clientauto.DomainGroupProperties
        Dim Count As Long = 100
        Dim Gname As String = "*"
        Dim TF As Long
        Try
            DP = dsms.FindDomainGroup(SessionID.ToString, Gname, Count, DGPR, TF, True)
        Catch ex As Exception
            UpdateLog(1, "common.WSFindDMGroup", "Getting UUID for $AllServers group failed with the following error", 2)
            UpdateLog(1, "common.WSFindDMGroup", ex.Message, 2)
            MessageBox.Show("Getting The AllServers Group UUID failed with error" + vbCrLf + ex.Message)
            Return "NOTOK"
            Exit Function
        End Try
        Dim j As Integer

        For j = 0 To TF - 1
            'UpdateLog(1, "common.WSFindDMGroup", DP(j).groupLabel, 0)
            If DP(j).groupLabel = "FDU" Then
                AllServerUUID = DP(j).groupUUID
                UpdateLog(1, "common.WSFindDMGroup", "$AllDomains group UUID=" + AllServerUUID, 0)
                Return "OK"
                Exit Function
            End If
        Next


        UpdateLog(1, "common.WSFindDMGroup", "Unable to find the  group UUID for the $Allservers group", 0)
        Return "NOTOK"
    End Function

    Function MapnetworkDrive(DriveLetter As String, UNCPath As String, password As String, domain As String, user As String) As String
        Dim rtn As String = MakeCADSMCMDCred(EMHost)
        Dim oneline As String = "use " & DriveLetter & " " & UNCPath & " " & password & " /user:" & rtn & "\" & user

        UpdateLog(0, "common.MapNetworkDrive", "Attemping to Map drive to " + DriveLetter & " " & UNCPath, 0)
        rtn = DoMapNetworkDrive(DriveLetter, oneline, True)
        If rtn = "OK" Then
            UpdateLog(0, "common.MapNetworkDrive", "drive " + DriveLetter & " mapped", 0)

            Return "0K"
            Exit Function
        ElseIf rtn = "1219" Then
            UpdateLog(0, "common.MapNetworkDrive", "Mapping failed with error 1219 will attempt to delete all mappings and remap", 1)
            rtn = Unmapforce()
            If rtn = "OK" Then
                UpdateLog(0, "common.MapNetworkDrive", "Unmapping reoported OK", 1)
                Alreadymapped = False
                MyDrive = ""
                UpdateLog(0, "common.MapNetworkDrive", "Mydive and alreadymapped are clear calling nextdriveavalibe", 1)
                rtn = NextAvailableDrive()
                If rtn <> "OK" And rtn <> "NOTOK" Then
                    UpdateLog(0, "common.MapNetworkDrive", "nextdriveavalibe=" + rtn, 1)
                    DriveLetter = rtn
                    oneline = "use " & DriveLetter & " " & UNCPath & " " & password & " /user:" & domain & "\" & user
                    UpdateLog(0, "common.MapNetworkDrive", "Attempting to map the drive again after delete all zombies", 1)
                    rtn = DoMapNetworkDrive(DriveLetter, oneline, False)
                    If rtn = "OK" Then
                        UpdateLog(0, "common.MapNetworkDrive", "drive " + DriveLetter & " mapped", 0)
                        Return "0K"
                        Exit Function
                    Else
                        UpdateLog(0, "common.MapNetworkDrive", "drive mapping failed again resolve the issues and rerun the application", 2)
                        Return "NOTOK"
                    End If
                Else
                    UpdateLog(0, "common.MapNetworkDrive", "NextAvalibedrive failed", 2)
                    Return "NOTOK"
                End If
            Else
                UpdateLog(0, "common.MapNetworkDrive", "unmap all failed with an error this app cannot handle", 2)
                Return "NOTOK"
            End If
        Else
            UpdateLog(0, "common.MapNetworkDrive", "drive mapping failed with an error this app cannot handle", 2)
            Return "NOTOK"
        End If

    End Function
    Function DoMapNetworkDrive(Driveletter As String, OneLine As String, firstpass As Boolean) As String

        Dim myprocess As New Process()
        Dim ostartinfo As New ProcessStartInfo("net", OneLine) With {
        .UseShellExecute = False,
        .RedirectStandardError = True}
        myprocess.StartInfo = ostartinfo
        myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
        myprocess.StartInfo.CreateNoWindow = True
        myprocess.Start()
        myprocess.WaitForExit()
        If myprocess.ExitCode <> 0 Then
            Dim myprocessoutput As String
            Using oStreamReader As System.IO.StreamReader = myprocess.StandardError
                myprocessoutput = oStreamReader.ReadToEnd()
            End Using
            UpdateLog(0, "common.DoMapNetworkDrive", "failed with an exit code of " + myprocess.ExitCode.ToString + " and returned " + myprocessoutput, 1)
            If firstpass = False Then
                UpdateLog(0, "common.DoMapNetworkDrive", "failed with an exit code of " + myprocess.ExitCode.ToString + " and returned " + myprocessoutput, 2)
                MessageBox.Show("Mapping drive  " + Driveletter + " failed with an exit code of " + myprocess.ExitCode.ToString + " and returned the following message" + vbCrLf + myprocessoutput)
                Return "NOTOK"
            Else
                If InStr(myprocessoutput, "1219") > 0 Then
                    Return "1219"
                End If
            End If
        End If
        MyDrive = Driveletter
        Return "OK"


    End Function

    Function Unmapnetworkdrive(driveletter As String) As String
        UpdateLog(0, "common.Unmapnetworkdrive", "Will attempt to unmap network drive it it has been mapped by this application", 0)
        If InStr(driveletter, ":") > 0 And Alreadymapped = False Then
            UpdateLog(0, "common.Unmapnetworkdrive", "Will attempt to unmap network drive " + driveletter, 0)
            Try


                Dim oneline As String = "use " & driveletter & " /delete /y"
                Dim myprocess As New Process
                myprocess.StartInfo.FileName = "net"
                myprocess.StartInfo.Arguments = oneline
                myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                myprocess.StartInfo.CreateNoWindow = True
                myprocess.Start()
                myprocess.WaitForExit()
            Catch ex As Exception
                UpdateLog(0, "common.Unmapnetworkdrive", "Drive unmapping failed ", 1)
                Return "NOTOK"

            End Try
            UpdateLog(0, "common.Unmapnetworkdrive", "Drive unmapping OK ", 0)
        End If
        Return "OK"

    End Function

    Function Unmapforce() As String
        UpdateLog(0, "common.Unmapforce", "Attempting to unmap all drives", 1)
        Try

            Dim oneline As String = "use * /delete /n"
            Dim myprocess As New Process
            myprocess.StartInfo.FileName = "net"
            myprocess.StartInfo.Arguments = oneline
            myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
            myprocess.StartInfo.CreateNoWindow = True
            myprocess.Start()
            myprocess.WaitForExit()
            If myprocess.ExitCode <> 0 Then
                UpdateLog(0, "common.Unmapforce", "Failed to unmap all drives", 2)
                MessageBox.Show("failed to un map all drives")
                Return "NOTOK"
            End If
            UpdateLog(0, "common.Unmapforce", "Unmap all drives OK", 1)
            Return "OK"
        Catch ex As Exception
            UpdateLog(0, "common.Unmapforce", "Failed to unmap all drives", 2)
            MessageBox.Show("failed to un map all drives")
            Return "NOTOK"
        End Try
        UpdateLog(0, "common.Unmapforce", "Unmap all drives OK", 1)
        Return "OK"
    End Function


    Public Function NextAvailableDrive() As String
        Dim rtn As String
        UpdateLog(0, "common.NextAvalibleDrive", "Getting the next avalible drive letter for mapping", 0)
        rtn = DriveAlreadyMapped()
        If Alreadymapped = True Then
            UpdateLog(0, "common.NextAvalibleDrive", "drive already mapped", 0)
            Return "OK"
            Exit Function
        End If
        Dim iDrive As Integer
        Dim sNextDrive As DriveInfo

        UpdateLog(0, "common.NextAvalibleDrive", "Drive not previously mapped looking for next avalible letter", 0)
        For iDrive = 67 To 90 'starts looking at c:\ check to see if sdlibrary is already mapped



            sNextDrive = My.Computer.FileSystem.GetDriveInfo(Chr(iDrive) + ":\")
            If sNextDrive.DriveType.ToString = "NoRootDirectory" Then

                NextAvailableDrive = Chr(iDrive) + ":"
                UpdateLog(0, "common.NextAvalibleDrive", "next avalible drive=" + NextAvailableDrive, 0)
                Return NextAvailableDrive
                Exit Function
            End If

        Next
        Return "NOTOK"
        UpdateLog(0, "common.NextAvalibleDrive", "No drive letters between c and z are free very unusual application can not continue", 0)
    End Function




    Public Sub Exitapp()
        If SessionIDset = True Then
            WSLogout()
        End If
        If System.IO.File.Exists(MyDrive + "\PackageModifyApp\" + EMUser + "\logs\debug.log") Then
            Try

                Using log As StreamWriter = File.AppendText(MyDrive + "\PackageModifyApp\" + EMUser + "\logs\debug.log")
                    log.WriteLine("Logging complete")
                End Using
            Catch ex As Exception

            End Try
        End If


        If MyDrive <> vbNullString Or Alreadymapped = False Then
            Unmapnetworkdrive(MyDrive)
        End If

        Application.Exit()

    End Sub

    Public Function DateCalculator(ByRef year As Integer, ByRef month As Integer, ByRef day As Integer, ByRef hour As Integer, ByRef minute As Integer, ByRef second As Integer, add As String, howlong As Integer, ByRef fyear As Integer, ByRef fmonth As Integer, ByRef fday As Integer, ByRef fhour As Integer, ByRef fminute As Integer, ByRef fsecond As Integer) As String
        Dim today As System.DateTime = System.DateTime.Now
        hour = today.Hour
        minute = today.Minute
        second = today.Second
        month = today.Month
        day = today.Day
        year = today.Year

        Dim future As System.DateTime
        Select Case add
            Case "min"
                future = today.AddMinutes(howlong)
            Case "hour"
                future = today.AddHours(howlong)
            Case "day"
                future = today.AddDays(howlong)

        End Select


        fhour = future.Hour
        fminute = future.Minute
        fsecond = future.Second
        fmonth = future.Month
        fday = future.Day
        fyear = future.Year
        Return "OK"
    End Function


    Public Function GetAllDomains() As String
        Dim rtn = MakeCADSMCMDCred(EMHost)
        Dim oneline As String = "/c cadsmcmd local " + EMHost + " area action=list login=\""winnt://" + rtn + "/" + EMUser + "\"":" + EMPassword + "  >""" + Workingdir + "AllDomains.lst"""
        Dim lookingfor As String
        UpdateLog(1, "common.GetAllDomains", "Getting list of All domains known to the enterprise", 0)
        TotalDM = -1

        If System.IO.File.Exists(Workingdir + "Alldomains.lst") Then
            System.IO.File.Delete(Workingdir + "Alldomains.lst")
        End If

        Dim myprocess As New Process
        myprocess.StartInfo.FileName = "cmd.exe"
        myprocess.StartInfo.Arguments = oneline
        myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
        myprocess.StartInfo.CreateNoWindow = True
        myprocess.Start()
        myprocess.WaitForExit()

        If myprocess.ExitCode <> 0 Then
            UpdateLog(1, "common.GetAllDomains", "Failed Getting list of All domains known to the enterprise check the file " + Workingdir + "Alldomains.lst if it exists for details", 2)
            MessageBox.Show("failed to Get list of all registered domains")
            Return "NOTOK"
            Exit Function
        End If


        If Not System.IO.File.Exists(Workingdir + "Alldomains.lst") Then
            UpdateLog(1, "common.GetAllDomains", "Getting list of All domains completed but the output file " + Workingdir + "Alldomains.lst does not exist is is unreadable", 2)
            MessageBox.Show("Failed to get list of all domains using the following command " + vbCrLf + oneline)
            Return "NOTOK"
            Exit Function
        End If

        Try

            UpdateLog(1, "common.GetAllDomains", "Beginning to read the file " + Workingdir + "Alldomains.lst", 0)
            lookingfor = "List of areas"
            Dim Lookingforchange As Boolean = False
            Dim FileReader As StreamReader = New StreamReader(Workingdir + "Alldomains.lst")
            Dim newlookingfor As String
            Do While FileReader.Peek() >= 0
                oneline = FileReader.ReadLine()
                If lookingfor = "List of areas" Then
                    If InStr(oneline, lookingfor) > 0 Then
                        UpdateLog(1, "common.GetAllDomains", "Found the line " + lookingfor, 0)
                        oneline = FileReader.ReadLine()
                        Lookingforchange = True
                        newlookingfor = "Number of items listed"
                    End If
                ElseIf lookingfor = "Number of items listed" Then

                    If oneline <> "" Or oneline <> vbNullString Then
                        TotalDM = TotalDM + 1
                        DMName(TotalDM, 0) = oneline
                        UpdateLog(1, "common.GetAllDomains", "Found DM " + oneline, 0)
                    Else
                        UpdateLog(1, "common.GetAllDomains", "No more domains to find", 0)
                        Exit Do
                    End If
                End If
                If Lookingforchange = True Then
                    Lookingforchange = False
                    lookingfor = "Number of items listed"
                End If
            Loop
            FileReader.Close()
            UpdateLog(1, "common.GetAllDomains", "Found a total of " + Str(TotalDM - 1) + "domains registered", 0)
        Catch ex As Exception
            MessageBox.Show("unable to read the file " + Workingdir + "Alldomains.lst recived the following error" + vbCrLf + ex.Message)


            UpdateLog(1, "common.GetAllDomains", "Uunable To read the file " + Workingdir + "Alldomains.lst recived the following Error" + ex.Message, 2)
            Return "NOTOK"
            Exit Function
        End Try

        Return "OK"


    End Function
    Function GetDistRecords() As String
        Dim OneLine As String
        Dim Workingon As String
        Dim j As Integer
        DistCount = -1
        Try
            UpdateLog(1, "common.GetDistRecords", "Getting a list of all the domains " + SelectedPackageName + ":" + SelectedPackageVersion + " is distributed to", 0)

            Dim FileWriter As StreamWriter = New StreamWriter(Workingdir + "GetDistribute.tsk")
            For j = 0 To TotalDM
                UpdateLog(1, "common.GetDistRecords", "Adding Get record for " + DMName(j, 0), 0)
                FileWriter.WriteLine("area action=listDistSW name=" + DMName(j, 0) + " filter=""(Item name=" + SelectedPackageName + " && Item version=" + SelectedPackageVersion + ")""")
            Next
            FileWriter.Close()
        Catch ex As Exception
            Return "NOTOK"
            UpdateLog(1, "common.GetDistRecords", "Unable to create or write to the file " + Workingdir + "distribute.tsk, got the following error " + ex.Message, 2)
            MessageBox.Show("unable to create or write to the file " + Workingdir + "GetDistribute.tsk, got the following error" + vbCrLf + ex.Message)
            Exit Function

        End Try
        Dim rtn = MakeCADSMCMDCred(EMHost)
        OneLine = "/c set SDCMD_CONTINUE=ON && cadsmcmd local " + EMHost + " batch """ + Workingdir + "GetDistribute.tsk"" login=\""winnt://" + rtn + "/" + EMUser + "\"":" + EMPassword + "  >""" + Workingdir + "GetDistribute.lst"""
        Try
            UpdateLog(1, "common.GetDistRecords", "Attempting to get the distribute records", 0)
            Dim myprocess As New Process
            myprocess.StartInfo.FileName = "cmd.exe"
            myprocess.StartInfo.Arguments = OneLine
            myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
            myprocess.StartInfo.CreateNoWindow = True
            myprocess.Start()
            myprocess.WaitForExit()

            If myprocess.ExitCode <> 0 Then
                UpdateLog(1, "common.GetDistRecords", "Unable to get the distribute records from " + EMHost, 2)
                MessageBox.Show("failed to Get disrubution records")
                Return "NOTOK"
                Exit Function
            End If
        Catch ex As Exception
            UpdateLog(1, "common.GetDistRecords", "Unable to get the distribute records from " + EMHost + "received the error " + ex.Message, 2)
        End Try
        UpdateLog(1, "common.GetDistRecords", "Get distribute records Completed ", 0)
        If Not System.IO.File.Exists(Workingdir + "GetDistribute.lst") Then
            UpdateLog(1, "common.GetDistRecords", "Get distribute records output file " + Workingdir + "GetDistribute.lst cannot be found", 2)
            MessageBox.Show("Failed to get list of all domains using the following command " + vbCrLf + OneLine)
            Return "NOTOK"
            Exit Function
        End If
        Dim FileReader As StreamReader = New StreamReader(Workingdir + "GetDistribute.lst")
        UpdateLog(1, "common.GetDistRecords", "Preparing to parse the file " + Workingdir + "GetDistribute.lst", 0)
        j = 0
        Workingon = DMName(j, 0)
        UpdateLog(1, "common.GetDistRecords", "Looking for distubution records for " + DMName(j, 0), 0)
        Do While FileReader.Peek() >= 0
            OneLine = FileReader.ReadLine()

            If InStr(OneLine, "List of distributed software of the area") > 0 And InStr(OneLine, Workingon) > 0 Then

                OneLine = FileReader.ReadLine()

                OneLine = FileReader.ReadLine()


                If InStr(OneLine, SelectedPackageName) > 0 Then
                    DistCount = DistCount + 1
                    IsDistName(DistCount, 0) = Workingon
                    UpdateLog(1, "common.GetDistRecords", "Package is distubuted to " + Workingon, 0)
                End If
                If j < TotalDM Then
                    j = j + 1
                    Workingon = DMName(j, 0)
                    UpdateLog(1, "common.GetDistRecords", "Looking for distubution records for " + DMName(j, 0), 0)
                Else
                    Exit Do

                End If
            End If

        Loop
        FileReader.Close()
        UpdateLog(1, "common.GetDistRecords", "Package is distributed to " + Str(DistCount + 1) + " domains", 0)
        Dim K As Integer
        For K = 0 To DistCount
            For j = 0 To TotalDM

                If DMName(j, 0) = IsDistName(K, 0) Then
                    IsDistName(K, 1) = DMName(j, 1)
                    Exit For
                End If
            Next
        Next

        Return "OK"
    End Function
    Function GetAllSS() As String
        Dim rtn As String = MakeCADSMCMDCred(EMHost)
        Dim oneline As String = "/c cadsmcmd local " + EMHost + " servergroup action=listMem name=$Allservers login=\""winnt://" + rtn + "/" + EMUser + "\"":" + EMPassword + "  >""" + Workingdir + "AllSS.lst"""
        UpdateLog(1, "common.GetAllSS", "Getting list of a SS registered on the " + EMHost, 0)
        Try



            TotalSS = -1

            If System.IO.File.Exists(Workingdir + "Allss.lst") Then
                System.IO.File.Delete(Workingdir + "AllSS.lst")
            End If

            UpdateLog(1, "common.GetAllSS", "Getting list with the command " + oneline, 0)
            Dim myprocess As New Process
            myprocess.StartInfo.FileName = "cmd.exe"
            myprocess.StartInfo.Arguments = oneline
            myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
            myprocess.StartInfo.CreateNoWindow = True
            myprocess.Start()
            myprocess.WaitForExit()

            If myprocess.ExitCode <> 0 Then
                UpdateLog(1, "common.GetAllSS", "Getting list failed check the file " + Workingdir + "Allss.lst if it exists for details", 2)
                MessageBox.Show("failed to Get list of all Scalability Servers")
                Return "NOTOK"
                Exit Function
            End If

        Catch ex As Exception
            UpdateLog(1, "common.GetAllSS", "Getting list failed check the file " + Workingdir + "Allss.lst if it exists for details", 2)
            MessageBox.Show("failed to Get list of all Scalability Servers")
            Return "NOTOK"
            Exit Function
        End Try
        If Not System.IO.File.Exists(Workingdir + "AllSS.lst") Then
            UpdateLog(1, "common.GetAllSS", Workingdir + "Allss.lst does not exist or cannot be opened, cannot get the list of all register ss, must abort", 2)
            MessageBox.Show("Failed to get list of all SS using the following command " + vbCrLf + oneline)
            Return "NOTOK"
            Exit Function
        End If
        UpdateLog(1, "common.GetAllSS", "Prparing to parse the file " + Workingdir + "Allss.lst", 0)

        Try



            Dim FileReader As StreamReader = New StreamReader(Workingdir + "AllSS.lst")
            Dim found As Boolean = False
            Dim sstmp As String
            Dim DMtmp As String
            TotalSS = -1

            Do While FileReader.Peek() >= 0
                oneline = FileReader.ReadLine()
                If InStr(oneline, "Members of server group") > 0 Then

                    UpdateLog(1, "common.GetAllSS", "Found line Members of server group", 0)
                    oneline = FileReader.ReadLine()
                    found = True
                ElseIf found = True Then
                    If InStr(oneline, "(") > 0 Then
                        sstmp = Left(oneline, InStr(oneline, "(") - 1)
                        sstmp = sstmp.Trim
                        DMtmp = Mid(oneline, InStr(oneline, "(") + 1)
                        DMtmp = Left(DMtmp, InStr(DMtmp, ")") - 1)
                        UpdateLog(1, "common.GetAllSS", "SS=" + sstmp + ",DM=" + DMtmp, 0)



                        If sstmp <> DMtmp Then
                            For j = 0 To DistCount
                                If DMtmp = IsDistName(j, 0) Then
                                    TotalSS = TotalSS + 1
                                    sslist(TotalSS, 0) = sstmp
                                    sslist(TotalSS, 1) = DMtmp
                                    Exit For
                                End If
                            Next
                        Else
                            UpdateLog(1, "common.GetAllSS", "Found SS=DM record will be ignored", 0)
                        End If

                    Else
                        Exit Do
                    End If

                End If

            Loop

            FileReader.Close()
            Return "OK"

        Catch ex As Exception
            UpdateLog(1, "common.GetAllSS", "Unable to parse file recieved the error " + ex.Message, 2)
            MessageBox.Show("Unable to parse file " + Workingdir + "GetAllSS.lst recieved the error" + vbCrLf + ex.Message)
            Return "NOTOK"

        End Try
        UpdateLog(1, "common.GetAllSS", "Found a total of " + Str(TotalSS - 1), 0)
    End Function

    Function GetStageRecords(DMpointer As Integer) As String
        Dim Tmpss(100) As String
        Dim tmpcount As Integer = -1
        Dim oneline As String
        Dim WorkingDM As String = IsDistName(DMpointer, 0)
        Dim j As Integer
        UpdateLog(1, "common.GetStageRecords", "Getting stage records for all SS on DM" + WorkingDM, 0)

        Try


            Dim FileWriter As StreamWriter = New StreamWriter(Workingdir + WorkingDM + "_GetStage.tsk")
            For j = 0 To TotalSS

                If sslist(j, 1) = WorkingDM Then
                    tmpcount = tmpcount + 1
                    Tmpss(tmpcount) = sslist(j, 0)
                    UpdateLog(1, "common.GetStageRecords", "Found SS " + sslist(j, 0) + " belongs to this DM adding its record to the file " + Workingdir + WorkingDM + "_GetStage.tsk", 0)
                    FileWriter.WriteLine("stagingserver action=listitem name=" + sslist(j, 0))
                End If
            Next
            FileWriter.Close()
        Catch ex As Exception
            UpdateLog(1, "common.GetStageRecords", "unable to create or write to the file " + Workingdir + WorkingDM + "_GetStage.tsk got the following error" + ex.Message, 2)
            MessageBox.Show("unable to create or write to the file " + Workingdir + WorkingDM + "_GetStage.tsk got the following error" + vbCrLf + ex.Message)
            Return "NOTOK"
            Exit Function

        End Try
        If tmpcount = -1 Then
            UpdateLog(1, "common.GetStageRecords", "Found no scalability servers on this domain manager, nothing to do", 1)
            Return "N/A"
            Exit Function
        End If
        Dim rtn = MakeCADSMCMDCred(WorkingDM)
        oneline = "/c set SDCMD_CONTINUE=ON && cadsmcmd local " + WorkingDM + " batch """ + Workingdir + WorkingDM + "_GetStage.tsk"" login=\""winnt://" + rtn + "/" + EMUser + "\"":" + EMPassword + "  >""" + Workingdir + WorkingDM + "_GetStage.lst"""
        UpdateLog(1, "common.GetStageRecords", "Running the following command to get the stage records", 0)
        UpdateLog(1, "common.GetStageRecords", oneline, 0)

        Try



            Dim myprocess As New Process
            myprocess.StartInfo.FileName = "cmd.exe"
            myprocess.StartInfo.Arguments = oneline
            myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
            myprocess.StartInfo.CreateNoWindow = True
            myprocess.Start()
            myprocess.WaitForExit()

            If myprocess.ExitCode <> 0 Then
                UpdateLog(1, "common.GetStageRecords", "Failed to get stage records from DM " + WorkingDM, 2)
                MessageBox.Show("failed to Get stage records for DM" + WorkingDM)
                Return "NOTOK"
                Exit Function
            End If

        Catch ex As Exception
            UpdateLog(1, "common.GetStageRecords", "Failed to get stage records from DM " + WorkingDM + "Process returned " + ex.Message, 2)
            MessageBox.Show("failed to Get stage records for DM" + WorkingDM)
            Return "NOTOK"
            Exit Function
        End Try

        Try


            UpdateLog(1, "common.GetStageRecords", "Parsing the get stage records output file " + Workingdir + WorkingDM + "_Getstage.lst", 0)
            j = 0
            UpdateLog(1, "common.GetStageRecords", "Looking in the file for " + Tmpss(j), 0)
            Dim lookingfor As String = Tmpss(j)
            Dim FileReader As StreamReader = New StreamReader(Workingdir + WorkingDM + "_GetStage.lst")
            Dim found As Boolean = False


            Do While FileReader.Peek() >= 0
                oneline = FileReader.ReadLine()
                If InStr(oneline, "List of items available on staging server") > 0 And InStr(oneline, lookingfor) > 0 Then
                    UpdateLog(1, "common.GetStageRecords", "Found the " + Tmpss(j) + " section", 0)
                    oneline = FileReader.ReadLine()
                    found = True
                ElseIf found = True Then
                    If oneline = "" Or oneline = vbNullString Then
                        found = False
                        UpdateLog(1, "common.GetStageRecords", Tmpss(j) + " section complete", 0)
                        If j < tmpcount Then
                            j = j + 1
                            lookingfor = Tmpss(j)
                            UpdateLog(1, "common.GetStageRecords", "Looking in the file for " + Tmpss(j), 0)
                        Else
                            UpdateLog(1, "common.GetStageRecords", "Found all sections", 0)
                            Exit Do
                        End If
                    Else
                        If InStr(oneline, SelectedPackageName) > 0 And InStr(oneline, SelectedPackageVersion) > 0 Then
                            UpdateLog(1, "common.GetStageRecords", "Found " + SelectedPackageName + ":" + SelectedPackageVersion + " is staged to " + Tmpss(j), 0)
                            For j = 0 To TotalSS
                                If sslist(j, 0) = lookingfor Then
                                    sslist(j, 2) = "staged"
                                    Exit For
                                End If
                            Next
                        End If
                    End If
                End If
            Loop
            FileReader.Close()
        Catch ex As Exception
            UpdateLog(1, "common.GetStageRecords", "Unable to parse the file " + Workingdir + WorkingDM + "_getstage.lst, recieved the error " + ex.Message, 2)
            MessageBox.Show("Unable to parse the file " + Workingdir + WorkingDM + "_getstage.lst, recieved the error " + vbCrLf + ex.Message)
        End Try


        Dim testcount As Integer = 0
        For j = 0 To TotalSS
            If sslist(j, 2) = "staged" Then
                testcount = testcount + 1
            End If
        Next
        ' MessageBox.Show("testcount=" + Str(testcount))
        If testcount > 0 Then
            UpdateLog(1, "common.GetStageRecords", "The packages is staged to " + Str(testcount) + " ss on the dm " + WorkingDM, 0)
        Else
            UpdateLog(1, "common.GetStageRecords", "The packages is not staged to any ss on the dm " + WorkingDM, 1)
        End If

        Return "OK"
    End Function

    Function DriveAlreadyMapped() As String
        'Then to use it in a method:

        For Each drv In IO.DriveInfo.GetDrives()

            If drv.DriveType = IO.DriveType.Network Then
                Dim UncPath As New StringBuilder(255)
                Test.WNetGetConnection(drv.Name.Replace("\", ""), UncPath, UncPath.Capacity)

                If (String.Compare(UncPath.ToString, "\\" + EMHost + "\sdLibrary", True)) = 0 Then
                    MyDrive = Left(drv.Name, 2)
                    Alreadymapped = True

                    Return "Mapped"
                    Exit Function
                End If

            End If
        Next
        Return "NotMapped"
    End Function
    Function LogCreate(lognum As Integer) As String
        Dim logname As String
        If lognum = 1 Then
            logname = "c:\PackageModifyApp.log"

            'logname = MyDrive + "\PackageModifyApp\" + EMUser + "\logs\debug.log"

        Else
            logname = "C:\packageModifyPreMap.log"
        End If

        Try

            Using log As StreamWriter = File.CreateText(logname)
                log.WriteLine("Logging Begins")
                log.WriteLine("{0,-18}{1,-30}{2,-90}", "Severity", "Calling Module", "Message")
                log.Close()
            End Using
        Catch ex As Exception
            MessageBox.Show("unable to create log file " + logname)
            Return "NOTOK"
        End Try
        Return "OK"
    End Function
    Function UpdateLog(lognum As Integer, CallingModule As String, Ts As String, Severity As Integer) As String

        SyncLock (lock)
            Dim logname As String
            If lognum = 1 Then
                logname = "c:\PackageModifyApp.log"
                'logname = MyDrive + "\PackageModifyApp\" + EMUser + "\logs\debug.log"

            Else
                logname = "C:\PackageModifyPreMap.log"
            End If

            Try

                Using log As StreamWriter = File.AppendText(logname)
                    If Severity = 2 Then
                        log.WriteLine("{0,-18}{1,-30}{2,-90}", "***FATAL***", CallingModule, Ts)
                    ElseIf Severity = 1 Then
                        log.WriteLine("{0,-18}{1,-30}{2,-90}", " Warning", CallingModule, Ts)
                    Else
                        log.WriteLine("{0,-18}{1,-30}{2,-90}", "", CallingModule, Ts)
                    End If
                    log.Close()
                End Using
            Catch ex As Exception
                MessageBox.Show("unable to update log file " + logname)
                Return "NOTOK"
            End Try
            Return "OK"
        End SyncLock
    End Function
    Function GetPackagePath() As String
        UpdateLog(1, "Common.GetPackagePath", "finding the package path in the file " + MyDrive + "\library.dct", 0)
        Dim oneline As String
        Dim lookingfor As String = "[" + SelectedPackageName + ":" + SelectedPackageVersion
        Dim found As Boolean = False

        Try
            Dim FileReader As StreamReader = New StreamReader(MyDrive + "\library.dct")
            UpdateLog(1, "Common.GetPackagePath", "Parsing the file looking for the line  """ + lookingfor + "", 0)

            Do While FileReader.Peek() >= 0
                oneline = FileReader.ReadLine
                If InStr(oneline, lookingfor) > 0 Then
                    UpdateLog(1, "Common.GetPackagePath", "Found the section in the file for the specified patch", 0)
                    oneline = FileReader.ReadLine
                    If InStr(oneline, "Path") = 0 Then
                        UpdateLog(1, "Common.GetPackagePath", "Expected to find the path varible and did not", 2)
                        Return "NOTOK"
                        Exit Function
                    Else
                        UpdateLog(1, "Common.GetPackagePath", "Found the path for the package specified", 0)
                        oneline = Mid(oneline, InStr(oneline, "=") + 1)
                        oneline = oneline.Trim
                        UpdateLog(1, "Common.GetPackagePath", "package path=" + oneline, 0)
                        FileReader.Close()
                        Return oneline
                        Exit Function
                    End If
                End If
            Loop
            UpdateLog(1, "Common.GetPackagePath", "Unable to find package specified cannot continue", 2)
            MessageBox.Show("Unable to find package specified cannot continue")
            Return "NOTOK"
            Exit Function
        Catch ex As Exception
            UpdateLog(1, "Common.GetPackagePath", "Unable to parse the file " + MyDrive + "\library.dct recieved the following error, " + ex.Message, 2)
            MessageBox.Show("Unable to parse the file " + MyDrive + "\library.dct recieved the following error" + vbCrLf + ex.Message)
            Return "NOTOK"
            Exit Function
        End Try
        Return "NOTOK"
    End Function
    Function FindAndCopyDifferences(packagePath As String) As String
        sdpackagepath = packagePath

        Try
            If Directory.Exists(MyDrive + "\Exports\" + SelectedPackageNameTmp + "'" + SelectedPackageversionTmp + "\differences") Then
                Dim myprocess As New Process
                myprocess.StartInfo.FileName = "cmd.exe"
                myprocess.StartInfo.Arguments = "/c rd /s /q """ + MyDrive + "\Exports\" + SelectedPackageNameTmp + "'" + SelectedPackageversionTmp + "\differences"
                myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                myprocess.StartInfo.CreateNoWindow = True
                myprocess.Start()
                myprocess.WaitForExit()
            End If


            System.IO.Directory.CreateDirectory(MyDrive + "\Exports\" + SelectedPackageNameTmp + "'" + SelectedPackageversionTmp + "\differences")
            System.IO.Directory.CreateDirectory(MyDrive + "\Exports\" + SelectedPackageNameTmp + "'" + SelectedPackageversionTmp + "\differences\content")
            differencespath = MyDrive + "\Exports\" + SelectedPackageNameTmp + "'" + SelectedPackageversionTmp + "\differences\content"
        Catch ex As Exception
            UpdateLog(1, "common.FindAndCopyDifferences", "Unable to Create directory " + MyDrive + "\exports" + SelectedPackageNameTmp + "'" + SelectedPackageversionTmp + "\differences received the following Error, " + ex.Message, 2)
            MessageBox.Show("Unable to create the directory " & MyDrive + "\Exports" + SelectedPackageNameTmp + "'" + SelectedPackageversionTmp + "\differences received the following error" + vbCrLf + ex.Message)
            Return "NotOK"
            Exit Function
        End Try
        UpdateLog(1, "common.FindAndCopyDifferences", "Created directory " + MyDrive + "\exports" + SelectedPackageNameTmp + "'" + SelectedPackageversionTmp + "\differences", 0)
        UpdateLog(1, "common.FindAndCopyDifferences", "Prepare to get the differences from the exported package and the original", 0)


        Dim changed As String = MyDrive + "\exports\" + SelectedPackageNameTmp + "'" + SelectedPackageversionTmp + "\original\" + packagePath
        Dim oneline As String = "/c robocopy """ + changed + """ """ + MyDrive + "\" + packagePath + """ /mir /E /L /NJH /NJS /NDL /NS /NC /Log:""" + Workingdir + "Differences.lst"""
        UpdateLog(1, "common.FindAndCopyDifferences", "Getting list of differences with the following command", 0)
        UpdateLog(1, "common.FindAndCopyDifferences", "Robocopy " + oneline, 0)

        Try

            Dim myprocess As New Process()
            Dim ostartinfo As New ProcessStartInfo("cmd.exe", oneline) With {
            .UseShellExecute = False,
            .RedirectStandardError = True,
            .RedirectStandardOutput = True}
            myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
            myprocess.StartInfo.CreateNoWindow = True
            myprocess.StartInfo = ostartinfo
            myprocess.Start()
            myprocess.WaitForExit()
            If myprocess.ExitCode = 0 Then
                UpdateLog(1, "common.FindAndCopyDifferences", "Detected no changed files not continuing", 2)
                MessageBox.Show("detected no changed files cannot continue")
                Return "NOTOK"
                Exit Function
            ElseIf myprocess.ExitCode = 4 Then

                UpdateLog(1, "common.FindAndCopyDifferences", "Mismatched files Or directories were detected. Examine the log file" + Workingdir + "Differences.lst for more information", 2)
                MessageBox.Show("Mismatched files Or directories were detected. Examine the log file" + Workingdir + "Differences.lst for more information")
                Return "NOTOK"
                Exit Function
            ElseIf myprocess.ExitCode > 4 Then
                Dim myprocessoutput As String
                Using oStreamReader As System.IO.StreamReader = myprocess.StandardError
                    myprocessoutput = oStreamReader.ReadToEnd()
                End Using
                Using oStreamReader As System.IO.StreamReader = myprocess.StandardOutput
                    myprocessoutput = myprocessoutput & " " & oStreamReader.ReadToEnd()
                End Using

                UpdateLog(1, "common.FindAndCopyDifferences", "Getting list of differences generated the following error", 2)
                UpdateLog(1, "common.FindAndCopyDifferences", myprocessoutput, 2)
                MessageBox.Show("Getting list of differences generated the following error" + vbCrLf + myprocessoutput)
                Return "NOTOK"
                Exit Function
            End If

        Catch ex As Exception
            UpdateLog(1, "common.FindAndCopyDifferences", "Getting list of differences generated the following error", 2)
            UpdateLog(1, "common.FindAndCopyDifferences", ex.Message, 2)
            MessageBox.Show("Getting list of differences generated the following error" + vbCrLf + ex.Message)
            Return "NOTOK"
        End Try
        UpdateLog(1, "common.FindAndCopyDifferences", "Created a list of changed Or added file they are in the file " + MyDrive + "\exports" + SelectedPackageNameTmp + "'" + SelectedPackageversionTmp + "\differences.lst", 0)

        UpdateLog(1, "common.FindAndCopyDifferences", "Preparing to copy the files that have changed to the differences folder", 0)

        Try
            Dim FileReader As StreamReader = New StreamReader(MyDrive + "\exports\" + SelectedPackageNameTmp + "'" + SelectedPackageversionTmp + "\differences.lst")
            oneline = FileReader.ReadLine
            oneline = FileReader.ReadLine
            If oneline = "" Or oneline = vbNullString Then
                UpdateLog(1, "common.FindAndCopyDifferences", "No files have changed", 1)
                FileReader.Close()
                Return "NOCHANGE"
                Exit Function
            End If
            Dim rtn As String



            rtn = ParseFileCopy(oneline, packagePath)
            If rtn <> "OK" Then
                FileReader.Close()
                Return "NOTOK"
                Exit Function
            End If

            Do While FileReader.Peek() >= 0

                ParseFileCopy(FileReader.ReadLine, packagePath)
                If rtn <> "OK" Then
                    FileReader.Close()
                    Return "NOTOK"
                    Exit Function
                End If

            Loop
            FileReader.Close()
        Catch ex As Exception
            UpdateLog(1, "common.FindAndCopyDifferences", "Unable to read or process changed file list received the error " + ex.Message, 2)
            MessageBox.Show("Unable to read or process changed file list received the error " + vbCrLf + ex.Message)
            Return "NOTOK"
            Exit Function

        End Try

        Return ("OK")




    End Function
    Function UpdateEM() As String
        UpdateLog(1, "Common.UpdateEM", "Preparing to copy changes back into the SD package", 0)
        Dim oneline As String = "/c echo d|xcopy """ + differencespath + """ """ + MyDrive + "\" + sdpackagepath + """ /e /Y"
        UpdateLog(1, "Common.UpdateEM", "Updating the package with the following line", 0)
        UpdateLog(1, "Common.UpdateEM", oneline, 0)

        Try
            Dim myprocess As New Process()
            Dim ostartinfo As New ProcessStartInfo("cmd.exe", oneline) With {
            .UseShellExecute = False,
            .RedirectStandardError = True}
            myprocess.StartInfo = ostartinfo
            myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
            myprocess.StartInfo.CreateNoWindow = True
            myprocess.Start()
            myprocess.WaitForExit()
            If myprocess.ExitCode <> 0 Then
                Dim myprocessoutput As String
                Using oStreamReader As System.IO.StreamReader = myprocess.StandardError
                    myprocessoutput = oStreamReader.ReadToEnd()
                End Using
                UpdateLog(1, "common.UpdateEM", "Process generated the following error", 2)
                UpdateLog(1, "common.UpdateEM", myprocessoutput, 2)
                MessageBox.Show("process xcopy in UpdateEM generated the following error" + vbCrLf + myprocessoutput)
                Return "NOTOK"
            End If

        Catch ex As Exception
            UpdateLog(1, "common.UPdateEM", "Process xcopy generated the following error", 2)
            UpdateLog(1, "common.Update", ex.Message, 2)
            MessageBox.Show("process xcopy in UpdateEM generated the following error" + vbCrLf + ex.Message)
            Return "NOTOK"



        End Try
        UpdateLog(1, "common.UPdateEM", "Differences have been copied back into the package on the EM", 0)
        Dim linein(1) As String
        linein(0) = "/c rmdir /s /q """ + MyDrive + "\Exports\" + SelectedPackageNameTmp + "'" + SelectedPackageversionTmp + "\differences"
        linein(1) = "/c rmdir /s /q """ + MyDrive + "\Exports\" + SelectedPackageNameTmp + "'" + SelectedPackageversionTmp + "\original"

        Dim j As Integer
        For j = 0 To 1
            UpdateLog(1, "common.UPdateEM", "Preparing to delete a directory with the following command", 0)
            UpdateLog(1, "common.UPdateEM", linein(j), 0)
            Try
                Dim myprocess As New Process()
                Dim ostartinfo As New ProcessStartInfo("cmd.exe", linein(j)) With {
                .UseShellExecute = False,
                .RedirectStandardError = True}
                myprocess.StartInfo = ostartinfo
                myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                myprocess.StartInfo.CreateNoWindow = True
                myprocess.Start()
                myprocess.WaitForExit()
                If myprocess.ExitCode <> 0 Then
                    Dim myprocessoutput As String
                    Using oStreamReader As System.IO.StreamReader = myprocess.StandardError
                        myprocessoutput = oStreamReader.ReadToEnd()
                    End Using
                    UpdateLog(1, "common.UpdateEM", "Deleting the directory generated the following error", 2)
                    UpdateLog(1, "common.UpdateEM", myprocessoutput, 2)
                    MessageBox.Show("process rmdir in UpdateEM generated the following error" + vbCrLf + myprocessoutput)
                    Return "NOTOK"
                End If

            Catch ex As Exception
                UpdateLog(1, "common.UPdateEM", "Deleting the directory generated the following error", 2)
                UpdateLog(1, "common.Update", ex.Message, 2)
                MessageBox.Show("process rmdir in UpdateEM generated the following error" + vbCrLf + ex.Message)
                Return "NOTOK"



            End Try

        Next

        Return "OK"
    End Function

    Function ParseFileCopy(linein As String, packagepath As String) As String
        Dim PartLine As String

        Dim Oneline As String
        linein = linein.Trim
        If InStr(linein, "reginfo") < 1 Then
            PartLine = Mid(linein, Len(MyDrive + "\exports\" + SelectedPackageNameTmp + "'" + SelectedPackageversionTmp + "\original\" + packagepath) + 2)

            Oneline = "/c ""echo f|xcopy """ + linein + """ """ + MyDrive + "\exports\" + SelectedPackageNameTmp + "'" + SelectedPackageversionTmp + "\differences\content\" + PartLine + """ /y"""
            UpdateLog(1, "common.ParseFileCopy", "Attempting to copy the file with the following line", 0)
            UpdateLog(1, "common.ParseFileCopy", Oneline, 0)

            Try
                Dim myprocess As New Process()
                Dim ostartinfo As New ProcessStartInfo("cmd.exe", Oneline) With {
                .UseShellExecute = False,
                .RedirectStandardError = True}
                myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                myprocess.StartInfo.CreateNoWindow = True
                myprocess.StartInfo = ostartinfo
                myprocess.Start()
                myprocess.WaitForExit()
                If myprocess.ExitCode <> 0 Then
                    Dim myprocessoutput As String
                    Using oStreamReader As System.IO.StreamReader = myprocess.StandardError
                        myprocessoutput = oStreamReader.ReadToEnd()
                    End Using
                    UpdateLog(1, "common.Parsefilecopy", "Process generated the following error", 2)
                    UpdateLog(1, "common.Parsefilecopy", myprocessoutput, 2)
                    MessageBox.Show("process xcopy generated the following error" + vbCrLf + myprocessoutput)
                    Return "NOTOK"
                End If

            Catch ex As Exception
                UpdateLog(1, "common.Parsefilecopy", "Process xcopy generated the following error", 2)
                UpdateLog(1, "common.Parsefilecopy", ex.Message, 2)
                MessageBox.Show("process xcopy generated the following error" + vbCrLf + ex.Message)
                Return "NOTOK"

            End Try
        End If

        Return "OK"
    End Function

    Function CreateSDPackage() As String
        Dim rtn As String = CreateSDscript()
        Dim oneline As String
        If rtn <> "OK" Then
            Return "NOTOK"
        End If
        Dim currentTime As System.DateTime = System.DateTime.Now
        ModifyPackagename = "Update_" + SelectedPackageName + "_" + SelectedPackageVersion
        Dim format As String = "yyMMdd-HHmmss"
        Dim nowtime As String = currentTime.ToString(format)

        ModifyPackageVersion = nowtime
        UpdateLog(1, "common.CreateSDpackage", "Preparing to create SD package " + ModifyPackagename + "," + ModifyPackageVersion, 0)
        Try
            Using Filewriter As StreamWriter = File.CreateText(Workingdir + "packagecreate.tsk")
                Filewriter.WriteLine("Regsw item=""" + ModifyPackagename + """ version=" + nowtime + " os=WINDOWS_32-bit path=""" + Workingdir + "differences""")
                Filewriter.WriteLine("regproc item=""" + ModifyPackagename + """ version=" + nowtime + " os=WINDOWS_32-bit task=install procedure=UpdateMedia type=ips file=update.dms parameters=$rf path=\")
                Filewriter.WriteLine("swlibrary action=sealitem item=""" + ModifyPackagename + """ version=" + nowtime)
            End Using
        Catch ex As Exception
            UpdateLog(1, "common.CreateSDpackage", "Creating the file " + Workingdir + "packagecreate.tsk generated the following error", 2)
            UpdateLog(1, "common.CreateSDPackage", ex.Message, 2)
            MessageBox.Show("process xcopy generated the following error" + vbCrLf + ex.Message)
            Return "NOTOK"

        End Try

        Dim rtn1 = MakeCADSMCMDCred(EMHost)
        oneline = "/c set SDCMD_CONTINUE=ON && cadsmcmd local " + EMHost + " batch """ + Workingdir + "packagecreate.tsk"" login=\""winnt://" + rtn1 + "/" + EMUser + "\"":" + EMPassword + "  >""" + Workingdir + "createSDpackage.result"""
        UpdateLog(1, "common.CreateSDPackage", "Attempting to crete the SD package with the following command", 0)
        UpdateLog(1, "common.CreateSDPackage", oneline, 0)
        Try
            Dim myprocess As New Process()
            Dim ostartinfo As New ProcessStartInfo("cmd.exe", oneline) With {
            .UseShellExecute = False,
            .RedirectStandardError = True}
            myprocess.StartInfo = ostartinfo
            myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
            myprocess.StartInfo.CreateNoWindow = True
            myprocess.Start()
            myprocess.WaitForExit()
            If myprocess.ExitCode <> 0 Then
                Dim myprocessoutput As String
                Using oStreamReader As System.IO.StreamReader = myprocess.StandardError
                    myprocessoutput = oStreamReader.ReadToEnd()
                End Using
                UpdateLog(1, "common.CreateSDPackage", "Process generated the following error", 2)
                UpdateLog(1, "common.CreateSDPackage", myprocessoutput, 2)
                MessageBox.Show("process CreateSDPackage generated the following error" + vbCrLf + myprocessoutput)
                Return "NOTOK"
            End If

        Catch ex As Exception
            UpdateLog(1, "common.CreateSDPackage", "Process xcopy generated the following error", 2)
            UpdateLog(1, "common.CreateSDPackage", ex.Message, 2)
            MessageBox.Show("process CreateSDPackage generated the following error" + vbCrLf + ex.Message)
            Return "NOTOK"

        End Try
        UpdateLog(1, "common.CreateSDPackage", "Created package " + ModifyPackagename + ":" + nowtime, 0)
        Return "OK"



    End Function
    Function CreateSDscript() As String
        Dim filename As String = MyDrive + "\exports\" + SelectedPackageNameTmp + "'" + SelectedPackageversionTmp + "\differences\Update.dms"
        UpdateLog(1, "Common.CreateSDScript", "Creating the DMS script to be run by the scalability servers to populate the changed files", 0)
        Try
            Using Filewriter As StreamWriter = File.CreateText(filename)
                Filewriter.WriteLine("Dim SDPath, SDPackagePath, Oneline, PackageName, PackageVersion, LogName As String")
                Filewriter.WriteLine("Dim Log, RC as integer")
                Filewriter.WriteLine("")
                Filewriter.WriteLine("FUNCTION Finalize(Message as String) as string")
                Filewriter.WriteLine("WriteFile(Log,Message)")
                Filewriter.WriteLine("CloseFile(Log)")
                Filewriter.WriteLine("Exit")
                Filewriter.WriteLine("END FUNCTION")
                Filewriter.WriteLine("'******************************MAIN*********************************")
                Filewriter.WriteLine("LogName=Argv(1)")
                Filewriter.WriteLine("PackageName=""" + SelectedPackageName + """")
                Filewriter.WriteLine("PackageVersion=""" + SelectedPackageVersion + """")
                Filewriter.WriteLine("Log=OpenFile(LogName,O_Write)")
                Filewriter.WriteLine("IF Log<1 THEN")
                Filewriter.WriteLine("	SetStatus(100)")
                Filewriter.WriteLine("	Exit")
                Filewriter.WriteLine("END IF")
                Filewriter.WriteLine("Writefile(log,""Getting the Software Delivery Library Path"")")
                Filewriter.WriteLine("IF CcnfGetParameterStr(""itrm/usd/shared/ARCHIVE"", SDPath) THEN")
                Filewriter.WriteLine("   SDPath=SDPath + ""\""")
                Filewriter.WriteLine("   WriteFile(Log,""SDPath="" + SDPath)")
                Filewriter.WriteLine("ELSE")
                Filewriter.WriteLine("  SetStatus(101)")
                Filewriter.WriteLine("  Finalize(""Could not detect the SD Library path with the command, [CcnfGetParameterStr(itrm/usd/shared/ARCHIVE,"" + SDPath + ""] aborting application"")")
                Filewriter.WriteLine("")
                Filewriter.WriteLine("End if")
                Filewriter.WriteLine("IF Not(ReadINIEntry(PackageName + "":"" + PackageVersion,""Path"",SDPackagePath,SDPath + ""Library.dct"")) THEN")
                Filewriter.WriteLine("	SetStatus(102)")
                Filewriter.WriteLine("	Finalize(""Unable to find the package, ""  + PackageName + "":"" + PackageVersion + "", in the file "" + SDPath + ""Library.dct aborting application"")")
                Filewriter.WriteLine("")
                Filewriter.WriteLine("ELSE")
                Filewriter.WriteLine("  SDPackagePath=SDPath + SDPackagePath")
                Filewriter.WriteLine("	WriteFile(Log,""SDPackagePath="" + SDPackagePath)")
                Filewriter.WriteLine("End If")
                Filewriter.WriteLine("oneline=""cmd /c echo d|xcopy content """""" + SDPackagePath + """""" /e /Y""")
                Filewriter.WriteLine("WriteFile(Log,""Will copy the contents with the following command"")")
                Filewriter.WriteLine("WriteFile(Log,OneLine)")
                Filewriter.WriteLine("RC=Exec(OneLine,True,7)")
                Filewriter.WriteLine("IF RC=0 THEN")
                Filewriter.WriteLine("	Finalize(""Files have been updated"")")
                Filewriter.WriteLine("Else")
                Filewriter.WriteLine("	SetStatus(104)")
                Filewriter.WriteLine("	Finalize(""Files were not copied"")")
                Filewriter.WriteLine("END IF")

            End Using


        Catch ex As Exception
            UpdateLog(1, "common.CreateSDScript", "Failed to create the file " + filename + " proccess returned the following error", 2)
            UpdateLog(1, "common.CreateSDScript", ex.Message, 2)
            MessageBox.Show("Failed to create the file " + filename + " proccess returned the following error" + vbCrLf + ex.Message)
            Return "NOTOK"
            Exit Function
        End Try
        UpdateLog(1, "common.CreateSDScript", "created the file " + filename, 0)
        Return "OK"

    End Function
    Function DistributeUpdatePackage() As String
        Dim j As Integer
        UpdateLog(1, "common.DistributeUpdatePackage", "Preparing to distribute the SD package " + ModifyPackagename, 0)

        Dim DistContainerName As String = ModifyPackagename + "_" + ModifyPackageVersion
        Dim OKtodeploy As Boolean = False
        Dim rtn As String = MakeCADSMCMDCred(EMHost)
        Dim oneline As String = "/c cadsmcmd local " + EMHost + " rregsw item=""" + ModifyPackagename + """ version=" + ModifyPackageVersion + " cname=""" + ModifyPackagename + "_" + ModifyPackageVersion + Chr(34)
        For j = 0 To DistCount
            If IsDistName(j, 2) = "TRUE" Then
                UpdateLog(1, "common.DistributeUpdatePackage", "Added DM " + IsDistName(j, 0) + " to the distribution order", 0)
                oneline = oneline + " area=" + IsDistName(j, 0)
                OKtodeploy = True
            End If

        Next
        If OKtodeploy = False Then
            UpdateLog(1, "common.DistributeUpdatePackage", "There are No DM online to distribute to or the package has not been distibuted to any domains", 1)
            MessageBox.Show("There are No DM online to distribute to or the package has not been distibuted to any domains")
            Return "NOTOK"
            Exit Function
        End If
        oneline = oneline + " login=\""winnt://" + rtn + "/" + EMUser + "\"":" + EMPassword + "  >""" + Workingdir + "Distributepackage.result"""
        UpdateLog(1, "common.DistributeUpdatePackage", "Attempting to distribute the SD package with the following command", 0)
        UpdateLog(1, "common.DistributeUpdatePackage", oneline, 0)
        Try
            Dim myprocess As New Process()
            Dim ostartinfo As New ProcessStartInfo("cmd.exe", oneline) With {
            .UseShellExecute = False,
            .RedirectStandardError = True,
            .RedirectStandardOutput = True}
            myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
            myprocess.StartInfo.CreateNoWindow = True
            myprocess.StartInfo = ostartinfo
            myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
            myprocess.StartInfo.CreateNoWindow = True
            myprocess.Start()
            myprocess.WaitForExit()
            If myprocess.ExitCode <> 0 Then
                Dim myprocessoutput As String
                Using oStreamReader As System.IO.StreamReader = myprocess.StandardError
                    myprocessoutput = oStreamReader.ReadToEnd()
                End Using
                Dim myprocessoutput1 As String
                Using oStreamReader As System.IO.StreamReader = myprocess.StandardOutput
                    myprocessoutput1 = oStreamReader.ReadToEnd()
                End Using



                UpdateLog(1, "common.DistributeUpdatePackage", "Process generated the following error", 2)
                UpdateLog(1, "common.DistributeUpdatePackage", myprocessoutput + ":" + myprocessoutput1, 2)
                MessageBox.Show("process DistributeUpdatePackage generated an error" + vbCrLf + myprocessoutput + ":" + myprocessoutput1)
                Return "NOTOK"
            End If

        Catch ex As Exception
            UpdateLog(1, "common.DistributeUpdatePackage", "Process generated the following error", 2)
            UpdateLog(1, "common.DistributeUpdatePackage", ex.Message + "here", 2)
            MessageBox.Show("process DistributeUpdatePackage generated the following error" + vbCrLf + ex.Message)
            Return "NOTOK"

        End Try
        UpdateLog(1, "common.DistributeUpdatePackage", "Created the distribution task " + DistContainerName, 0)
        Return "OK"
    End Function
    Function GetDistributeStatus() As String
        Dim containername As String = ModifyPackagename + "_" + ModifyPackageVersion
        UpdateLog(1, "common.GetDistributeStatus", "Getting Distibution status will stay in this loop for 30 minutes or until the distibution is complete", 0)
        Dim k As Integer
        Dim oneline As String
        Dim waiting As Boolean
        Dim rtn As String = MakeCADSMCMDCred(EMHost)
        Dim cmdline As String = "/c cadsmcmd local " + EMHost + " distribution action=showattr name=""" + containername + """ login=\""winnt://" + rtn + "/" + EMUser + "\"":" + EMPassword + "  >""" + Workingdir + "DistributeStatus.result"""
        Dim FileReader As StreamReader
        Dim WorkingArea As String
        Dim myprocess As New Process()
        myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
        myprocess.StartInfo.CreateNoWindow = True
        Dim ostartinfo As New ProcessStartInfo("cmd.exe", cmdline) With {
            .UseShellExecute = False,
            .RedirectStandardError = True}
        UpdateLog(1, "common.GetDistributeStatus", "will be calling the command line with the following command", 0)
        UpdateLog(1, "common.GetDistributeStatus", cmdline, 0)
        For k = 0 To DistCount
            If IsDistName(k, 2) = "DISTRIBUTED" Then
                UpdateLog(1, "common.GetDistributeStatus", "Getting the delivery status from DM " + IsDistName(k, 0), 0)
                rtn = GetSDJobStatus(containername, IsDistName(k, 0))
                If rtn <> "OK" Then
                    IsDistName(k, 2) = rtn
                End If
            End If
        Next
        Try

            myprocess.StartInfo = ostartinfo
            myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
            myprocess.StartInfo.CreateNoWindow = True
            myprocess.Start()
            myprocess.WaitForExit()


                If myprocess.ExitCode <> 0 Then
                    Dim myprocessoutput As String
                    Using oStreamReader As System.IO.StreamReader = myprocess.StandardError
                        myprocessoutput = oStreamReader.ReadToEnd()
                    End Using
                    UpdateLog(1, "common.GetDistributeStatus", "Process generated the following error", 2)
                    UpdateLog(1, "common.GetDistributeStatus", myprocessoutput, 2)
                    MessageBox.Show("process GetDistributeStatus generated an error" + vbCrLf + myprocessoutput)
                    Return "NOTOK"
                End If
                myprocess.Close()
            Catch ex As Exception
                UpdateLog(1, "common.GetDistributeStatus", "Process generated the following error", 2)
                UpdateLog(1, "common.GetDistributeStatus", ex.Message, 2)
                MessageBox.Show("process GetDistributeStatus generated the following error" + vbCrLf + ex.Message)
                Return "NOTOK"

            End Try

            Try

                FileReader = New StreamReader(Workingdir + "\Distributestatus.result")


                Do While FileReader.Peek() >= 0
                    oneline = FileReader.ReadLine
                    If InStr(oneline, "..Area") > 0 Then
                        WorkingArea = Mid(oneline, InStr(oneline, ":") + 1).Trim
                        UpdateLog(1, "Common.GetDistributeStatus", "Found the Domain " + WorkingArea, 0)
                    ElseIf InStr(oneline, "....Order status") > 0 Then
                        oneline = Mid(oneline, InStr(oneline, ":") + 1).Trim
                        UpdateLog(1, "Common.GetDistributeStatus", "The distibution status of " + WorkingArea + " is " + oneline, 0)
                        If oneline = "ok" Or oneline = "error" Then
                            For k = 0 To DistCount
                                If WorkingArea = IsDistName(k, 0) Then
                                    If oneline = "ok" And IsDistName(k, 2) = "TRUE" Then
                                        IsDistName(k, 2) = "DISTRIBUTED"
                                        UpdateLog(1, "Common.GetDistributeStatus", "The distibution status of " + WorkingArea + " has changed to " + oneline + " will now request the domain to update the proper SS libraries", 0)
                                        rtn = DeliverSoftware(WorkingArea)
                                    End If
                                    If oneline = "error" And IsDistName(k, 2) = "TRUE" Then
                                        IsDistName(k, 2) = "DISTFAILED"
                                        UpdateLog(1, "Common.GetDistributeStatus", "The distibution status of " + WorkingArea + " has changed to " + oneline, 0)
                                    End If
                                End If
                            Next
                        End If
                    End If
                Loop
                FileReader.Close()
            Catch ex As Exception
                UpdateLog(1, "common.GetDistributeStatus", "Parsing the status output file generated an error check the file " + Workingdir + "\Distributestatus.result", 2)
                MessageBox.Show("Getting distibutution status generated an error when try to read the file " + Workingdir + "\Distributestatus.result")
            End Try
            For k = 0 To DistCount
                If IsDistName(k, 2) = "TRUE" Then
                    waiting = True
                    UpdateLog(1, "Common.GetDistributeStatus", "Still waiting for distibution to finsh", 0)
                    Exit For
                End If

            Next
        If waiting = False Then
            Return "OK"
            Exit Function
        Else
            Return "WAITING"

        End If





    End Function

    Function DeliverSoftware(Hostname As String) As String
        Dim oneline As String
        Dim j As Integer
        Dim SDContainerName As String = ModifyPackagename + "_" + ModifyPackageVersion
        Dim rtn As String = MakeCADSMCMDCred(Hostname)
        Dim filename As String = MyDrive + "\exports\" + SelectedPackageNameTmp + "'" + SelectedPackageversionTmp + "\" + Hostname + "_SDDeploy.tsk"
        UpdateLog(1, "Common.DeliverSoftware", "Creating the Deliversoftware tasklist for DM " + Hostname, 0)
        Try
            Using Filewriter As StreamWriter = File.CreateText(filename)
                UpdateLog(1, "Common.DeliverSoftware", "Creating the job container " + SDContainerName, 0)
                Filewriter.WriteLine("jobcontainer action=create name=""" + SDContainerName + Chr(34))
                UpdateLog(1, "Common.DeliverSoftware", "Adding the job for " + Hostname, 0)
                Filewriter.WriteLine("jobcontainer action=addjob Name=""" + SDContainerName + """ item=""" + ModifyPackagename + """ version=" + ModifyPackageVersion + " Procedure=UpdateMedia Task=install computer=" + Hostname + " after=exacttime")
                For j = 0 To TotalSS
                    If sslist(j, 1) = Hostname And sslist(j, 2) = "staged" Then
                        UpdateLog(1, "Common.DeliverSoftware", "Adding the job for " + sslist(j, 0), 0)
                        Filewriter.WriteLine("jobcontainer action=addjob Name=""" + SDContainerName + """ item=""" + ModifyPackagename + """ version=" + ModifyPackageVersion + " Procedure=UpdateMedia Task=install computer=" + sslist(j, 0) + " after=exacttime")
                    End If
                Next
                UpdateLog(1, "Common.DeliverSoftware", "Adding the seal command to the task list", 0)
                Filewriter.WriteLine("jobcontainer action=seal name=""" + SDContainerName + Chr(34))


            End Using


        Catch ex As Exception
            UpdateLog(1, "common.DeliverSoftware", "Process on DM " + Hostname + " generated the following error", 2)
            UpdateLog(1, "common.DeliverSoftware", ex.Message, 2)
            MessageBox.Show("process DeliverSoftware on DM " + Hostname + " generated the following error" + vbCrLf + ex.Message)
            Return "NOTOK"
            Exit Function
        End Try
        oneline = "/C cadsmcmd local " + Hostname + " batch """ + filename + """ login=\""winnt://" + rtn + "/" + EMUser + "\"":" + EMPassword + "  >""" + Workingdir + Hostname + "_SDJobCreate.result"""
        UpdateLog(1, "Common.DeliverSoftware", "Creating SD job with the following command", 0)
        UpdateLog(1, "Common.DeliverSoftware", oneline, 0)
        Dim myprocess As New Process()
        myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
        myprocess.StartInfo.CreateNoWindow = True
        Dim ostartinfo As New ProcessStartInfo("cmd.exe", oneline) With {
            .UseShellExecute = False,
            .RedirectStandardError = True}
        myprocess.StartInfo = ostartinfo


        Try
            myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
            myprocess.StartInfo.CreateNoWindow = True
            myprocess.Start()
            myprocess.WaitForExit()
            If myprocess.ExitCode <> 0 Then
                Dim myprocessoutput As String
                Using oStreamReader As System.IO.StreamReader = myprocess.StandardError
                    myprocessoutput = oStreamReader.ReadToEnd()
                End Using
                UpdateLog(1, "common.DeliverSoftware", "Process generated the following error", 2)
                UpdateLog(1, "common.DeliverSoftware", myprocessoutput, 2)
                MessageBox.Show("process DeliverSoftware generated an error" + vbCrLf + myprocessoutput)
                Return "NOTOK"
            End If

        Catch ex As Exception
            UpdateLog(1, "common.DeliverSoftware", "Process generated the following error", 2)
            UpdateLog(1, "common.DeliverSoftware", ex.Message, 2)
            MessageBox.Show("process DeliverSoftware generated the following error" + vbCrLf + ex.Message)
            Return "NOTOK"

        End Try




        Return "OK"
    End Function
    Function GetSDJobStatus(Jobname As String, Hostname As String) As String
        UpdateLog(1, "common.GetSDJobStatus", "Getting SD job status for the job " + Jobname + " on DM " + Hostname, 0)
        Dim rtn As String = MakeCADSMCMDCred(Hostname)
        Dim oneline As String = "/c cadsmcmd local " + Hostname + " jobcontainer action=showattr name=""" + Jobname + """ login=\""winnt://" + rtn + "/" + EMUser + "\"":" + EMPassword + "  >""" + Workingdir + Hostname + "_SDJobStatus.result"""
        UpdateLog(1, "common.GetSDJobStatus", "Getting SD job status with the following command", 0)
        UpdateLog(1, "common.GetSDJobStatus", "Getting SD job status for the job " + Jobname + " on DM " + Hostname, 0)
        Dim j As Integer
        Dim ContainerState, WorkingOn, Status As String
        Dim myprocess As New Process()
        myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
        myprocess.StartInfo.CreateNoWindow = True
        Dim ostartinfo As New ProcessStartInfo("cmd.exe", oneline) With {
            .UseShellExecute = False,
            .RedirectStandardError = True}
        myprocess.StartInfo = ostartinfo
        Try
            myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
            myprocess.StartInfo.CreateNoWindow = True
            myprocess.Start()
            myprocess.WaitForExit()
            If myprocess.ExitCode <> 0 Then
                Dim myprocessoutput As String
                Using oStreamReader As System.IO.StreamReader = myprocess.StandardError
                    myprocessoutput = oStreamReader.ReadToEnd()
                End Using
                UpdateLog(1, "common.GetSDJobStatus", "CAdsmcmd Command generated the following error", 2)
                UpdateLog(1, "common.GetSDJobStatus", myprocessoutput, 2)
                MessageBox.Show("process GetSDJobStatus Cadsmcmd Command generated an error" + vbCrLf + myprocessoutput)
                Return "NOTOK"
            End If

        Catch ex As Exception
            UpdateLog(1, "common.GetSDJobStatus", "Process generated the following error", 2)
            UpdateLog(1, "common.GetSDJobStatus", ex.Message, 2)
            MessageBox.Show("process GetSDJobStatus generated the following error" + vbCrLf + ex.Message)
            Return "NOTOK"

        End Try
        Try


            Dim FileReader = New StreamReader(Workingdir + Hostname + "_SDJobStatus.result")

            Do While FileReader.Peek() >= 0
                oneline = FileReader.ReadLine
                If InStr(oneline, "Job container state") > 0 Then
                    ContainerState = Mid(oneline, InStr(oneline, ":") + 1).Trim
                    UpdateLog(1, "common.GetSDJobStatus", "Status of the container is " + ContainerState, 0)
                ElseIf InStr(oneline, "..Target") > 0 Then
                    WorkingOn = Mid(oneline, InStr(oneline, ":") + 1).Trim
                    UpdateLog(1, "common.GetSDJobStatus", "Found the section for " + WorkingOn, 0)
                ElseIf InStr(oneline, "....Job state") > 0 Then
                    Status = Mid(oneline, InStr(oneline, ":") + 1).Trim
                    If Status = "EXECUTION_ERROR" Or Status = "EXECUTION_OK" Then
                        If WorkingOn = Hostname Then
                            For j = 0 To DistCount
                                If IsDistName(j, 0) = WorkingOn Then
                                    IsDistName(j, 3) = Status
                                    If Status = "EXECUTION_OK" Then
                                        UpdateLog(1, "common.GetSDJobStatus", "The SD job for Domain" + WorkingOn + "completed OK", 0)
                                    Else
                                        UpdateLog(1, "common.GetSDJobStatus", "The SD job for Domain" + WorkingOn + "Failed", 2)
                                    End If
                                    Exit For
                                End If
                            Next
                        Else
                            For j = 0 To TotalSS
                                If sslist(j, 0) = WorkingOn Then
                                    If sslist(j, 2) <> Status Then
                                        sslist(j, 2) = Status
                                        If Status = "EXECUTION_OK" Then
                                            UpdateLog(1, "common.GetSDJobStatus", "The SD job for SS" + WorkingOn + "completed OK", 0)
                                        Else
                                            UpdateLog(1, "common.GetSDJobStatus", "The SD job for SS" + WorkingOn + "Failed", 2)
                                        End If
                                        Exit For
                                    End If
                                End If

                            Next
                        End If
                    End If

                End If
            Loop
            FileReader.Close()
        Catch ex As Exception
            UpdateLog(1, "common.GetSDJobStatus", "Parsing the output file generated the following error", 2)
            UpdateLog(1, "common.GetSDJobStatus", ex.Message, 2)
            MessageBox.Show("process GetSDJobStatus parsing the output file generated the following error" + vbCrLf + ex.Message)
            Return "NOTOK"

        End Try
        If ContainerState = "failed" Then
            Return "FAILED"
            Exit Function
        End If
        If ContainerState = "successfully completed" Then
            Return "COMPLETE"
            Exit Function
        End If

        Return "OK"


    End Function
    Function WSDeliverSoftware(PackageName As String, PackageVersion As String, ProcedureName As String, ContainerName As String, HostName As String) As String
        MessageBox.Show("see If package is on dm yet this is for debug")
        UpdateLog(1, "common.WSDeliverSoftware", "Preparing to login to " + HostName, 0)
        Dim Rtn As String = WSLogin(HostName)
        Dim ProcID As String
        If Rtn = "NOTOK" Then
            Return "NOTOK"
            Exit Function
        End If
        UpdateLog(1, "common.WSDeliverSoftware", "Preparing to Get the procedure UUID for " + PackageName + ":" + PackageVersion + ":" + ProcedureName, 0)
        ProcID = WSFindSDprocedure(PackageName, PackageVersion, ProcedureName)
        If ProcID = "NOTOK" Then
            Return "NOTOK"
            Exit Function
        End If

        Dim SJC As clientauto.CreateSoftwareJobContainerProperties = New clientauto.CreateSoftwareJobContainerProperties With {
        .name = "Update " + PackageName + " " + PackageVersion + " + " + ContainerName,
        .nameSupplied = True}
        Dim ContainerId As String
        Try
            ContainerId = dsms.CreateSoftwareJobContainer(SessionID.ToString, SJC)
        Catch ex As Exception

            UpdateLog(1, "common.wsDeliverSoftwate", "Create Software job Container Process generated the following error", 2)
            UpdateLog(1, "common.WsDeliverSoftware", ex.Message, 2)
            MessageBox.Show("process WSDeliverSoftware Creating software job container generated the following error" + vbCrLf + ex.Message)
            Return "NOTOK"


        End Try
        UpdateLog(1, "common.WSDeliverSoftware", "Created SD job delivery container " + ContainerName + " containerUUID=" + ContainerId, 0)


        Dim CSJOP As clientauto.CreateSoftwareJobOrderProperties = New clientauto.CreateSoftwareJobOrderProperties With {
        .jobName = "Update",
        .jobNameSupplied = True}




        Dim AllcomputerUUID(0) As String

        UpdateLog(1, "common.WSDeliverSoftware", "Preparing to Get the UUID for the $AllComputers group", 0)
        Rtn = WSGetAllComputerUUID()
        If Rtn = "NOTOK" Then
            Return "NOTOK"
            Exit Function
        End If
        AllcomputerUUID(0) = Rtn
        'AllcomputerUUID(0) = vbNull
        Dim SR As String
        Dim TmpCount As Integer = 0
        Dim J As Integer
        Dim sl(0) As String


        sl(0) = WSFindComputer(HostName)
        UpdateLog(1, "common.WSDeliverSoftware", "Adding " + HostName + " to the SD package target list", 0)
        UpdateLog(1, "common.WSDeliverSoftware", "TotalSS=" + Str(TotalSS), 0)
        For J = 0 To TotalSS
            UpdateLog(1, "common.WSDeliverSoftware", "working on SS " + sslist(J, 0), 0)
            If sslist(J, 1) = HostName And sslist(J, 2) = "staged" Then
                TmpCount = TmpCount + 1
                ReDim Preserve sl(TmpCount)
                sl(TmpCount) = WSFindComputer(sslist(J, 0))
                UpdateLog(1, "common.WSDeliverSoftware", "Adding " + sslist(J, 0) + " to the SD package target list", 0)
            End If
        Next

        UpdateLog(1, "common.WSDeliverSoftware", "Adding Software job to the container", 0)
        Try
            SR = dsms.CreateInstallSoftwareJob(SessionID.ToString, ProcID, CSJOP, ContainerId, AllcomputerUUID, sl)
        Catch ex As Exception
            UpdateLog(1, "common.wsDeliverSoftwate", "Create Install Software job  Process generated the following error", 2)
            UpdateLog(1, "common.WsDeliverSoftware", ex.Message, 2)
            MessageBox.Show("process WSDeliverSoftware Creating Install software job generated the following error" + vbCrLf + ex.Message)
            Return "NOTOK"
        End Try
        UpdateLog(1, "common.WSDeliverSoftware", "Software job added jobID= " + SR, 0)
        UpdateLog(1, "common.WSDeliverSoftware", "Preparing to seal and activate the container", 0)

        Try
            dsms.SealAndActivateSoftwareJobContainer(SessionID.ToString, ContainerId)
        Catch ex As Exception
            UpdateLog(1, "common.wsDeliverSoftwate", "SealAndActivateJobContainer process generated the following error", 2)
            UpdateLog(1, "common.WsDeliverSoftware", ex.Message, 2)
            MessageBox.Show("process WSDeliverSoftware SealAndActivateContainer process generated the following error" + vbCrLf + ex.Message)
            Return "NOTOK"
        End Try

        UpdateLog(1, "common.WSDeliverSoftware", "Software job sealed and activated", 0)
        Return ContainerId
    End Function
    Function WSFindComputer(Name As String) As String
        Dim CPR As clientauto.ComputerPropertiesRequired = New clientauto.ComputerPropertiesRequired With {
        .computerHostNameRequired = True,
        .computerHostUUIDRequired = True}
        Dim TmpCount As Long = 1
        Dim Count As Long
        Dim CP() As clientauto.ComputerProperties
        Try
            CP = dsms.FindComputer(SessionID.ToString, Name, TmpCount, CPR, Count, True)
        Catch ex As Exception
            UpdateLog(1, "common.wsFindComputer", "Process generated the following error", 2)
            UpdateLog(1, "common.WsFindComputer", ex.Message, 2)
            MessageBox.Show("process WSFindComputer generated the following error" + vbCrLf + ex.Message)
            Return "NOTOK"
        End Try
        Dim J As Integer
        For J = 0 To Count
            If CP(J).computerHostName = Name Then
                UpdateLog(1, "common.WsFindComputer", "UUID for " + Name + " = " + CP(J).computerHostUUID, 2)
                Return CP(J).computerHostUUID
                Exit Function
            End If
        Next

        UpdateLog(1, "common.WsFindComputer", "Could Not Find the UUID for " + Name, 2)
        MessageBox.Show("process WSFindComputer Could Not Find the UUID for " + Name)
        Return "NOTOK"
    End Function

    Function WSFindSDprocedure(PN As String, PV As String, ProcName As String) As String
        Dim SPPT As clientauto.SoftwarePackageProcedureTask = clientauto.SoftwarePackageProcedureTask.INSTALL
        Dim Rtn As String
        Try
            Rtn = dsms.FindSoftwarePackageProcedure(SessionID.ToString, PN, PV, ProcName, SPPT)
        Catch ex As Exception
            UpdateLog(1, "common.wsFindSDProcedure", "Process generated the following error", 2)
            UpdateLog(1, "common.WsFindSDProcedure", ex.Message, 2)
            MessageBox.Show("process WSFindSDProcedure generated the following error" + vbCrLf + ex.Message)
            Return "NOTOK"
        End Try
        UpdateLog(1, "common.wsFindSDProcedure", ProcName + " UUID=" + Rtn, 0)

        Return Rtn
    End Function
    Function WSGetAllComputerUUID() As String
        Dim SG As clientauto.SystemGroup = clientauto.SystemGroup.COALLCOMPUTERS


        Dim Rtn As String
        Try
            Rtn = dsms.GetSystemGroupUUID(SessionID.ToString, SG)
        Catch ex As Exception
            UpdateLog(1, "common.wsGetAllComputerUUID", "Process generated the following error", 2)
            UpdateLog(1, "common.WsGetAllComputerUUID", ex.Message, 2)
            MessageBox.Show("process WSGetAllComputerUUID generated the following error" + vbCrLf + ex.Message)
            Return "NOTOK"
        End Try
        UpdateLog(1, "common.WsGetAllComputerUUID", "$ALLCOMPUTERS UUID=" + Rtn, 0)
        Return Rtn

    End Function
    Function MakeCADSMCMDCred(HostName As String) As String
        If Uselocal = True Then
            Return HostName
        Else
            Return EMDomain
        End If

    End Function


    Class Test
        <DllImport("mpr.dll")>
        Public Shared Function WNetGetConnection(ByVal lpLocalName As String, ByVal lpRemoteName As StringBuilder, ByRef lpnLength As Integer) As Integer
        End Function

    End Class



End Module