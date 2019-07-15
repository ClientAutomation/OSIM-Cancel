'make sure you open windowsApp1>properties>settings and change the value of WindowsApp1_clientauto_DSMWebServiceAPIService to point to the directory the mod_gsoap_utf8.dll is on your system
'this app logs to a file c:\PurgeOSIMJobs.log you can change the name in the logcreate function on this page
'i have the values of host, user, domain and password prepopulated  you can change them or wait the gui will try and fail 
'you can then poulate new values that will be retained
'those values are just below

'I have created the app so each discrete type of action is one function so you can take the modules out and use them 

' basiclly when it starts tries to login with the predefined credentials  if it fails the gui will prompt for new credentials
'once credentials are correct the app will

'Prompt the user for number of hours of elapsed time the job needs to be active for to be considerd for deletion


'   get the current system time   
'   get a list of all scheduled osim jobs (configstate=8)
'   Parse the list get the osim job properties specificlly activation time
'   convert the time format to the datetime format of mm/dd/YYYY HH:mm:ss
'compare the current time with the job activation time in hours
'determine if the difference between activation time time and current time is greater then specified deletion interval
'   if so execute a cancel OSIM job request

'
'

'


'limitations

'by no means is this app hardened i trap for webservice failures and log most actions 
'but any other unforseen coding errors the app will choke



'I am trying to figure out how to thell the app tat the mod_gsoap file is compiled into the app itself
'I tried off and on to create an installer that will put the file somewhere



'near future changes
'


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
Imports System.Environment



Module common


    Public CurVal(1) As String 'used to pass extra values out of backgroundworker2
    Public Host As String  'entered on startup screen or saved in settings
    Public UserENC As String 'created in WSlogin
    Public User As String  'entered on startup screen or save in settings
    Public Domain As String  ''entered on startup screen or save in settings
    Public Password As String   'entered on startup screen or save in settings
    Public PasswordENC As String 'created in WSlogin
    Public CredValidated As Boolean 'set in sartup.emvalidate if credentials are good
    Public FirstPass As Integer = 0 ' used to see if this is the first time the app has run on this session it sets things like mapped drives
    Public dsms As New WindowsApp1.clientauto.DSMWebServiceAPIService
    Public SessionID As New Object 'web service session ID
    Public SessionIDset As Boolean = False 'used to determine if WSlogoff is needed at exit
    Public JobExpireTime As Integer '(hours)
    Public OSIMJobList(1000, 1) As String
    Public OSIMJobCount As Integer
    Public LogName As String = "c:\CancelOSIMJobStartup.log" 'uses this until variables are read in
    Public lastModule As String
    Public lastfunction As String
    Public fatalError As String
    Public IsLocal As Boolean
    Public myargs As String()

    Public PasswordFound As Integer
    Public UserFound As Integer
    Public HostFound As Integer
    Public DomainFound As Integer
    Public TimeFound As Integer
    Public LogNameFound As Integer
    Public EncryptionFound As Integer
    Public LogSize As String = 10 '10 meg
    Public LogSizeFound As Integer
    Public LogCount As String = 3 'number of log files to keep
    Public LogCountFound As Integer



    Public Uselocal As Boolean 'tells logins to assume credentials are local not domain so sets user as hostname\user instade of emdomain\user

    Declare Sub Sleep Lib "kernel32" Alias "Sleep" (ByVal dwMilliseconds As Long)
    Sub Main()
        Dim rtn As String
        Try
            myargs = System.Environment.GetCommandLineArgs()
        Catch ex As Exception
            'step 1
            DisplayUseage(0)

        End Try

        LogCreate(0)
        rtn = ParseArgs()



        UpdateLog(1, "Module1.Main", "Found HostName=" + Host, 0)
        UpdateLog(1, "Module1.Main", "Found UserName", 0)
        UpdateLog(1, "Module1.Main", "Found Password", 0)
        UpdateLog(1, "Module1.Main", "Found JobExpireTime=" + Str(JobExpireTime), 0)
        rtn = ManagerValidate1()
        If rtn <> "OK" Then
            UpdateLog(1, "Module1.Main", "Unable to log in to Client Auto", 2)
            lastModule = "main"
            lastfunction = "Return from ManagerValidate1 "

            Exitapp(20)

        End If
        rtn = WSGetOsim()
        If rtn = "FAILED" Then
            UpdateLog(1, "Module1.Main", "Unable to GetOSIM jobs", 2)
            Exitapp(30)
        ElseIf rtn = "NOTHINGTODO" Then
            UpdateLog(1, "Module1.Main", "Found No OSIM jobs to cancel", 1)
            Exitapp(0)
        End If
        rtn = WSCancelOSInstallRequest()
        If rtn <> "OK" Then
            UpdateLog(1, "Module1.Main", "Unable to  Cancel OSIM jobs", 2)
            Exitapp(40)

        End If
        Exitapp(0)
        End
    End Sub

    Function ManagerValidate1()
        Dim rtn As String
        Dim am As Integer = 1


        If SessionIDset = True Then
            rtn = UpdateLog(am, "Module1.Managervalidate", "Existing web services session exists will log off", 0)
            rtn = WSLogout()
            If rtn = "OK" Then
                SessionIDset = False
                rtn = UpdateLog(am, "Module1.Managervalidate", "log off OK", 0)
            Else
                rtn = UpdateLog(am, "Module1.Managervalidate", "log off failed", 2)

            End If

        End If


        rtn = UpdateLog(am, "Module1.Managervalidate", "Testing webservices connection by logging in", 0)
        rtn = WSLogin(Host)

        If rtn <> "OK" Then
            rtn = UpdateLog(1, "Module1.Managervalidate", "login failed check your settings and ensure you have rights in client auto", 2)

            CredValidated = False
            Return "NotOK"
        Else
            rtn = UpdateLog(am, "Module1.Managervalidate", "log in OK", 0)

            CredValidated = True



            Return "OK"

        End If
    End Function

    Function WSLogin(HostName As String) As String 'called by startup.validate to see if credentials are valid
        Dim tmpuser As String
        UpdateLog(1, "common.WSlogin", "Attemping to get sessionID from " + HostName, 0)
        SessionID = ""
        If Uselocal = True Then
            tmpuser = HostName + "/" + User
        Else
            tmpuser = Domain + "/" + User
        End If
        Try
            dsms.Url = "http://" + HostName + "/DSM_Webservice/mod_gsoap.dll"
            If PasswordFound = 1 Then
                PasswordENC = Convert.ToBase64String(New System.Text.ASCIIEncoding().GetBytes(Password))
            Else
                PasswordENC = Password
            End If
            If UserFound = 1 Then
                UserENC = Convert.ToBase64String(New System.Text.ASCIIEncoding().GetBytes("winnt://" + tmpuser))
            Else
                UserENC = User
            End If
            ' UserENC = Convert.ToBase64String(New System.Text.ASCIIEncoding().GetBytes("winnt://" + tmpuser))
            UpdateLog(1, "common.WSlogin", "Trying To login To WebServices On Server " + HostName, 0)
            SessionID = dsms.Login2(UserENC, PasswordENC, HostName)
        Catch ex As Exception

            UpdateLog(1, "common.WSlogin", "Trying To login To WebServices On Server " + HostName + " Process returned the following error" + ex.Message, 2)
            lastModule = "WSLogin"
            lastfunction = "Login2"
            Return "NOTOK"

        End Try
        UpdateLog(1, "common.WSlogin", "Obtained sessionid " + SessionID.ToString, 0)

        Return "OK"
    End Function

    Function WSLogout() As String  'called when exit button is hit

        UpdateLog(1, "common.logout", "Attemping to release sessionID from " + Host, 0)

        If SessionIDset = True Then
            Try
                dsms.Logout(SessionID.ToString)
            Catch ex As Exception
                UpdateLog(1, "common.logout", "Failed to release sessionID from " + Host, 1)
                Return "NOTOK"
            End Try
            UpdateLog(1, "common.logout", "SessionID Released ", 0)
            SessionIDset = False
        End If
        Return "OK"
    End Function


    Function WSGetOsim() As String
        Dim CurrentTime As String = DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss")
        UpdateLog(1, "common.WSGetOSIM", "Current time is  " + CurrentTime, 0)
        UpdateLog(1, "common.WSGetOSIM", "Job Expiration time is " + Str(JobExpireTime) + " hours", 0)
        Dim DeleteJob As Boolean


        Dim OSIMCount As Long = 2000
        Dim TargetsRequired As Long = 2000
        Dim Index As Long = 0
        Dim k As Integer
        Dim JobTime As String
        Dim FilterArray(0) As clientauto.OSIMTargetFilter2

        ' .
        FilterArray(0) = New clientauto.OSIMTargetFilter2() With { 'gets all osim targets
        .osimTargetProperty = clientauto.OSIMTARGETPROPERTY2.TARGETNAME,
        .condition = clientauto.FILTERCONDITION.FILTERWILDCARDEQ,
        .searchString = "*"}

        Dim OSIMTargetProperties2() As clientauto.OSIMTargetProperties2


        Dim OSIMProperties() As clientauto.OSIMProperties
        '
        UpdateLog(1, "common.WSGetOSIM", "Getting All OSIM targets", 0)
        Try


            OSIMTargetProperties2 = dsms.GetOSIMTargetList(SessionID.ToString, False, "", FilterArray, True, clientauto.OSIMTARGETPROPERTY2.TARGETNAME, True, Index, TargetsRequired, True, OsimCount)

        Catch ex As Exception
            UpdateLog(1, "common.WSGetOSIM", "GetOSIMTargetList returned the following error" + ex.Message, 2)
            lastModule = "WSOSIM"
            lastfunction = "GetOSIMTargetList"
            Return "NOTOK"
        End Try
        If OSIMCount > 0 Then
                OSIMCount = OSIMCount - 1
            Else
            Return "NOTHINGTODO"
        End If

        UpdateLog(1, "common.WSGetOSIM", "Found " + Str(OSIMCount + 1) + " OSIM targets", 0)
        Console.WriteLine("Found " + Str(OSIMCount + 1) + " OSIM targets")
        OSIMJobCount = 0
        UpdateLog(1, "common.WSGetOSIM", "Parsing output of all OSIM targets to determine how many are scheduled", 0)


        For j = 0 To OSIMCount
            If OSIMTargetProperties2(j).configType = 8 Then
                Try
                    OSIMProperties = dsms.GetOSInstallationRequestByName(SessionID.ToString, OSIMTargetProperties2(j).targetName, clientauto.WSOSIMCONFTYPE.WSOSIMCTSCHEDULED, True)

                Catch ex As Exception
                    UpdateLog(1, "common.WSGetOSIM", "GetInstallationRequestByName returned the following error" + ex.Message, 2)
                    lastModule = "GetOSIMTargets"
                    lastfunction = "GetInstallationRequestByName"
                    Return "NOTOK"
                End Try
                UpdateLog(1, "common.WSGetOSIM", "found the OSIM target " + OSIMTargetProperties2(j).targetName + " is scheduled for installation", 0)
                    For k = 1 To OSIMProperties.Length - 1
                    If OSIMProperties(k).propertyName = "Activation time" Then
                        JobTime = ConvertOSIMTime(OSIMProperties(k).propertyValue)
                        UpdateLog(1, "common.WSGetOSIM", "Found the activation time for " + OSIMTargetProperties2(j).targetName + " is " + OSIMProperties(k).propertyValue, 0)

                        DeleteJob = DateDifference(CurrentTime, JobTime, JobExpireTime)
                        If DeleteJob = True Then
                            UpdateLog(1, "common.WSGetOSIM", "Job to be canceled", 0)
                            OSIMJobList(OSIMJobCount, 0) = OSIMTargetProperties2(j).targetName
                            OSIMJobList(OSIMJobCount, 1) = JobTime
                            OSIMJobCount = OSIMJobCount + 1
                        Else
                            UpdateLog(1, "common.WSGetOSIM", "Job current", 0)
                        End If
                    End If
                Next k



            End If

        Next j
        UpdateLog(1, "common.WSGetOSIM", "There are " + Str(OSIMJobCount) + " Jobs to be canceled", 0)
        Console.WriteLine("There are " + Str(OSIMJobCount) + " Jobs to be canceled")
        OSIMJobCount = OSIMJobCount - 1
        Return "OK"
    End Function

    Function WSCancelOSInstallRequest() As String
        For j = 0 To OSIMJobCount
            Try
                dsms.CancelOSInstallationByName(SessionID.ToString, OSIMJobList(j, 0), True)
            Catch ex As Exception
                UpdateLog(1, "common.WSCancelOSInstallRequest", "CancelOSInstallationRequest returned the following error" + ex.Message, 2)
                lastModule = "WSDeleteOSInstallationRequest"
                lastfunction = "DeleteOSInstallationRequestByName"
                Console.WriteLine("Job cancel failed  for " + OSIMJobList(j, 0))
                Return "NOTOK"
            End Try
            UpdateLog(1, "common.WSCancelOSInstallRequest", "CancelOSInstallationRequest accepted for " + OSIMJobList(j, 0), 0)
            Console.WriteLine("CancelOSInstallationRequest accepted for " + OSIMJobList(j, 0))
        Next
        Return "OK"
    End Function

    Public Sub Exitapp(errorcode As Integer)
        If SessionIDset = True Then
            WSLogout()


        End If

        If errorcode = 0 Then
            UpdateLog(1, "common.exitapp", "application ended normally", 0)
            Console.WriteLine("Application ended normally")
        ElseIf errorcode = 1 Then
            UpdateLog(1, "common.exitapp", "application ended but had nothing to do", 0)
            Console.WriteLine("Application ended but had nothing to do")
        Else
            Console.WriteLine("Application failed with error code " + Str(errorcode))
            UpdateLog(1, "common.exitapp", "application failed with error code " + Str(errorcode), 2)
            UpdateLog(1, "common.exitapp", "Failed in module " + lastModule, 2)
            UpdateLog(1, "common.exitapp", "Failed in function " + lastfunction, 2)
            Environment.ExitCode = errorcode
        End If
        Try
            Using log As StreamWriter = File.AppendText(LogName)
                log.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + " LOGGING ENDS")
            End Using
        Catch ex As Exception
        End Try
        Application.Exit()
        End


    End Sub



    Function LogCreate(lognum As Integer) As String
        Dim LogNameShort As String = LogName
        Dim Extension As String = String.Empty

        If LogNameFound = 2 Then 'append=true
            Try


                If GetFileInfo(LogNameShort, Extension, True) > CLng(LogSize) * 1000000 Then 'log full
                    Dim k As Integer
                    Dim LogCounter As Integer = Int(LogCount)
                    For k = LogCounter To 1 Step -1

                        If File.Exists(LogNameShort + "_" + Str(k + 1) + Extension) Then
                            File.Delete(LogNameShort + "_" + Str(k + 1) + Extension)

                        End If

                        IO.File.Move(LogNameShort + "_" + Str(k) + Extension, LogNameShort + "_" + Str(k + 1) + Extension)


                    Next
                    If File.Exists(LogNameShort + "_1" + Extension) Then
                        File.Delete(LogNameShort + "_1" + Extension)
                    End If
                    IO.File.Move(LogNameShort + Extension, LogNameShort + "_1" + Extension)

                End If
            Catch ex As Exception
                Console.WriteLine("detected request to append log file but there is an issue with existing log file(s)")
                Exitapp(10)

            End Try
            Try
                If Not File.Exists(LogName) Or LogNameFound = 1 Then

                    Using log As StreamWriter = File.CreateText(LogName)
                        log.WriteLine("{0,-18}{1,-30}{2,-90}", "Severity", "Calling Module", "Message")
                    End Using



                End If
            Catch ex As Exception
                Console.WriteLine("Error trying to create log file")
                Sleep(5000)
                Exitapp(10)

            End Try

            Try


                Using log As StreamWriter = File.AppendText(LogName)
                    log.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + " LOGGING BEGINS")
                End Using

            Catch ex As Exception
                Console.WriteLine("Error trying to write log file")
                Sleep(5000)
                Exitapp(10)

            End Try
        Else
            Try
                Using log As StreamWriter = File.CreateText(LogName)
                    log.WriteLine("{0,-18}{1,-30}{2,-90}", "Severity", "Calling Module", "Message")
                    log.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + " LOGGING BEGINS")
                End Using
            Catch ex As Exception
                Console.WriteLine("Error trying to create log file")

                Exitapp(10)
            End Try
        End If

        Return "OK"
    End Function

    Function GetFileInfo(ByRef LogNameShort As String, ByRef Extension As String, flag As Boolean) As Long

        Dim loginfo As New IO.FileInfo(LogNameShort)

        Dim exists As Boolean = loginfo.Exists
        If exists = True Then
            Try


                If flag = True Then
                    Extension = Path.GetExtension(LogNameShort)
                    LogNameShort = Path.GetFullPath(LogNameShort)


                End If
            Catch ex As Exception
                Console.WriteLine("logfile " + LogNameShort + " exists but sytem flagged an error getting file properties")
            End Try

            Return loginfo.Length

        Else
            Return 0
        End If

    End Function




    Function UpdateLog(lognum As Integer, CallingModule As String, Ts As String, Severity As Integer) As String


        Try

            Using log As StreamWriter = File.AppendText(logname)
                If Severity = 2 Then
                    log.WriteLine("{0,-18}{1,-30}{2,-90}", "***FATAL***", CallingModule, Ts)
                ElseIf Severity = 1 Then
                    log.WriteLine("{0,-18}{1,-30}{2,-90}", " Warning", CallingModule, Ts)
                Else
                    log.WriteLine("{0,-18}{1,-30}{2,-90}", "", CallingModule, Ts)
                End If
            End Using
        Catch ex As Exception
            Console.WriteLine("unable to update log file " + LogName)
            Return "NOTOK"
        End Try
        Return "OK"
    End Function
    'extra functions not used by this app at this time
    Function DateDifference(CurrentTime As String, JobToCompare As String, DifferenceVariable As Integer) As Boolean
        Dim CurrentTimeDT As DateTime = DateTime.ParseExact(CurrentTime, "MM/dd/yyyy HH:mm:ss", Nothing)
        Dim JobToCompareDT As DateTime = DateTime.ParseExact(JobToCompare, "MM/dd/yyyy HH:mm:ss", Nothing)
        Dim HourDifference As Long = DateDiff(DateInterval.Hour, JobToCompareDT, CurrentTimeDT, )
        UpdateLog(1, "common.DateDifference", "Job has been active for " + Str(HourDifference) + " hours", 0)
        If HourDifference >= DifferenceVariable Then
            Return True
        Else
            Return False
        End If

    End Function
    Function ConvertOSIMTime(OSIMTime As String) As String
        UpdateLog(1, "common.ConvertOSIMTime", "Jin Convert OSIM Time", 0)
        Dim Hour As String
        Dim Minute As String
        Dim Second As String
        Dim Month As String
        Dim DayOfMonth As String
        Dim Year As String
        Year = Left(OSIMTime, InStr(OSIMTime, "-") - 1).Trim
        OSIMTime = Mid(OSIMTime, InStr(OSIMTime, "-") + 1).Trim
        Month = Left(OSIMTime, InStr(OSIMTime, "-") - 1).Trim
        OSIMTime = Mid(OSIMTime, InStr(OSIMTime, "-") + 1).Trim
        DayOfMonth = Left(OSIMTime, InStr(OSIMTime, " ") - 1).Trim
        OSIMTime = Mid(OSIMTime, InStr(OSIMTime, " ") + 1).Trim
        Hour = Left(OSIMTime, InStr(OSIMTime, ":") - 1).Trim
        OSIMTime = Mid(OSIMTime, InStr(OSIMTime, ":") + 1).Trim
        Minute = Left(OSIMTime, InStr(OSIMTime, ":") - 1).Trim
        OSIMTime = Mid(OSIMTime, InStr(OSIMTime, ":") + 1).Trim
        Second = Left(OSIMTime, InStr(OSIMTime, " ") - 1).Trim
        Return Month + "/" + DayOfMonth + "/" + Year + " " + Hour + ":" + Minute + ":" + Second

    End Function


    Function ParseArgs() As String
        Dim Rtn As String
        Dim lognametemp As String
        Try

            UpdateLog(1, "Module1.ParseArgs", "Array length=" + Str(myargs.Length), 0)


            If myargs.Length < 9 Then
                DisplayUseage(0)

            End If
            For I As Integer = 1 To myargs.Length - 2 Step 2
                UpdateLog(1, "Module1.ParseArgs", "Array " + Str(I) + "=" + myargs(I), 0)
                UpdateLog(1, "Module1.ParseArgs", "Array " + Str(I + 1) + "=" + myargs(I + 1), 0)

                If myargs(I).ToUpper = "-M" Then
                    Host = myargs(I + 1)
                    UpdateLog(1, "Module1.ParseArgs", "Found HostName=" + Host, 0)

                    HostFound = 1
                ElseIf Left(myargs(I), 2).ToUpper = "-U" Then

                    User = myargs(I + 1)
                    UpdateLog(1, "Module1.ParseArgs", "Found user", 0)
                    If InStr(myargs(I).ToUpper, "E") > 0 Then
                        UpdateLog(1, "Module1.ParseArgs", "Found user is encrypted", 0)
                        UserFound = 2
                    Else
                        UpdateLog(1, "Module1.ParseArgs", "Found plain text user", 0)
                        UserFound = 1
                    End If

                ElseIf Left(myargs(I), 2).ToUpper = "-P" Then

                    Password = myargs(I + 1)
                    UpdateLog(1, "Module1.ParseArgs", "Found password", 0)
                    If InStr(myargs(I).ToUpper, "E") > 0 Then
                        UpdateLog(1, "Module1.ParseArgs", "Found password is encrypted", 0)
                        PasswordFound = 2
                    Else
                        UpdateLog(1, "Module1.ParseArgs", "Found plain text password", 0)
                        PasswordFound = 1
                    End If

                ElseIf myargs(I).ToUpper = "-T" Then


                    JobExpireTime = Int(myargs(I + 1))

                    UpdateLog(1, "Module1.ParseArgs", "Found time parameter=" + Str(JobExpireTime), 0)

                ElseIf myargs(I).ToUpper = "-D" Then

                    Domain = myargs(I + 1)

                    UpdateLog(1, "Module1.ParseArgs", "Found domain parameter=" + Domain, 0)
                    DomainFound = 1
                ElseIf myargs(I).ToUpper = "-L" Or myargs(I).ToUpper = "-LA" Then
                    UpdateLog(1, "Module1.ParseArgs", "Found logname parameter", 0)
                    lognametemp = myargs(I + 1)
                    UpdateLog(1, "Module1.ParseArgs", "Found logname=" + lognametemp, 0)
                    If InStr(myargs(I).ToUpper, "A") > 0 Then
                        UpdateLog(1, "Module1.ParseArgs", "Found log append", 0)
                        LogNameFound = 2
                    Else
                        UpdateLog(1, "Module1.ParseArgs", "Found log create", 0)
                        LogNameFound = 1
                    End If
                ElseIf myargs(I).ToUpper = "-E" Then
                    UpdateLog(1, "Module1.ParseArgs", "Found encrypt flag", 0)
                    EncryptionFound = 1
                ElseIf myargs(I).ToUpper = "-LS" Then

                    LogSize = myargs(I + 1)
                    UpdateLog(1, "Module1.ParseArgs", "Found log size parameter=" + LogSize, 0)
                    LogSizeFound = 1
                ElseIf myargs(I).ToUpper = "-LC" Then
                    LogCount = myargs(I + 1)
                    UpdateLog(1, "Module1.ParseArgs", "Found log count parameter=" + LogCount, 0)
                    LogCountFound = 1


                End If
            Next
            LogName = lognametemp
            Console.WriteLine("logname=" + LogName + " lognamefound=" + Str(LogNameFound))


        Catch ex As Exception
            Console.WriteLine("Unable to parse input Array")
            UpdateLog(1, "Module1.ParseArgs", "Unable to parse input array", 0)

            DisplayUseage(20)

        End Try

        If LogNameFound > 0 Then
            UpdateLog(1, "Module1.ParseArgs", "calling log create", 0)
            Rtn = LogCreate(0)
        End If
        If EncryptionFound = 1 Then
            UpdateLog(1, "common.ParseArgs", "Detected -E flag set preparing to provide encrypted UserName and Password", 0)
            If UserFound > 0 And PasswordFound > 0 And DomainFound > 0 Then
                GetEncryption()
            Else
                Console.WriteLine("-U UserName , -P Password and/or -D Domain was not provided unable to encrypt them")
                UpdateLog(1, "common.ParseArgs", "-U UserName, -P Passwordand or -D  is not provided unable to encrypt them", 2)
                DisplayUseage(20)
            End If

            UpdateLog(1, "common.ParseArgs", "detected this is a normal run validating all parameters are present", 0)
            If HostFound = 0 Or UserFound = 0 Or PasswordFound = 0 Or TimeFound = 0 Or LogNameFound = 0 Or (DomainFound = 0 And UserFound <> 2) Then
                UpdateLog(1, "common.ParseArgs", "detected this is a normal run but one or more variables are missing", 2)
                DisplayUseage(20)
            End If

        End If

        Return "OK"
    End Function

    Sub GetEncryption()
        UpdateLog(1, "common.GetEncryption", "Preparing to encrypt user and password", 0)
        Console.WriteLine("Encrypted User Name = " + Convert.ToBase64String(New System.Text.ASCIIEncoding().GetBytes("winnt://" + Domain + "/" + User)))
        Console.WriteLine("Encrypted Password = " + Convert.ToBase64String(New System.Text.ASCIIEncoding().GetBytes(Password)))
        Console.WriteLine("Enter any key to exit")
        UpdateLog(1, "common.GetEncryption", "credentials encrypted", 0)
        Console.ReadLine()
        Exitapp(0)

        End




    End Sub


    Sub DisplayUseage(UseType As Integer)
        Console.WriteLine("This application has 2 modes encrypt credentials or run")
        Console.WriteLine("To encrypt credentials the syntax is ")
        Console.WriteLine("CancelOSIMJobs -E Y|Yes -D UserWindowsDomain -U UserName -P Password")
        Console.WriteLine("no log created encrypted passwrd sent to console")

        Console.WriteLine("Normal execution plain text is:")
        Console.WriteLine("CancelOSIMJobs -M ClientAutoManagerName -D WindowsDomain -U UserName")
        Console.WriteLine("       -P Password -T MaxAgeOfJob(0=all) -L LogFileName")
        Console.WriteLine("Normal executtion using encrypted credentials (encryted with the -E Option is")
        Console.WriteLine("CancelOSIMJobs -M ClientAutoManagerName -UE EnryptedUserName")
        Console.WriteLine("       -PE Encrypted Password -T MaxAgeOfJobs -L LogFileName")
        Console.WriteLine("Optional Parameters")
        Console.WriteLine("       -LA(in place of -L appends existing log file of the name specified)")
        Console.WriteLine("        In addition to -L[A]")
        Console.WriteLine("      -LL MaxLengthOfLogFile in megabytes default=10")
        Console.WriteLine("          only checked if append option was specified")
        Console.WriteLine("      -LC number of log file kept default=3 ")
        Console.WriteLine("          only checked if append option is specified")

        Sleep(5000)
        Exitapp(10)
        End
    End Sub

End Module