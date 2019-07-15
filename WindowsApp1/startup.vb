Imports System.ComponentModel
Imports System.Drawing.Color
Imports System.Drawing
Imports System.IO
Imports System.Configuration
Imports System.Reflection
Public Class Startup




    Private Sub Startup_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        If FirstPass = 1 Then

            If My.Settings.CAhost <> vbNullString Then
                EMHost = My.Settings.CAhost
            End If
            If My.Settings.CAdomain <> vbNullString Then
                EMDomain = My.Settings.CAdomain
            End If
            If My.Settings.CAuser <> vbNullString Then
                EMUser = My.Settings.CAuser
            End If
            If My.Settings.CApassword <> vbNullString Then
                EMUser = My.Settings.CAuser
            End If

            If My.Settings.UseLocal = "True" Then
                Uselocal = True
                CheckBoxLocal.Checked = True
            Else
                Uselocal = False
                CheckBoxLocal.Checked = False
            End If
        End If
        If EMHost <> vbNullString And EMUser <> vbNullString And EMPassword <> vbNullString And EMDomain <> vbNullString Then
            UpdateLog(0, "startup.startup_load", "credentials are prepopulated testing them now", 0)
            TBEMHost.Text = EMHost
            tbEMdomain.Text = EMDomain
            tbemuser.Text = EMUser
            tbempassword.Text = EMPassword
            ValidateEM()
        End If
        FirstPass = 1
        PictureBox1.BringToFront()



    End Sub

    Private Sub TBEMHost_TextChanged(sender As Object, e As EventArgs) Handles TBEMHost.TextChanged
        EMHost = TBEMHost.Text
        CheckForEMValidation()
    End Sub
    Private Sub TbEMdomain_TextChanged(sender As Object, e As EventArgs) Handles tbEMdomain.TextChanged
        EMDomain = tbEMdomain.Text
        CheckForEMValidation()
    End Sub

    Private Sub TBEMUser_TextChanged(sender As Object, e As EventArgs) Handles tbemuser.TextChanged
        EMUser = tbemuser.Text
        CheckForEMValidation()
    End Sub

    Private Sub TBEMPassword_TextChanged(sender As Object, e As EventArgs) Handles tbempassword.TextChanged
        EMPassword = tbempassword.Text
        CheckForEMValidation()
    End Sub

    Private Sub CheckForEMValidation()
        EMValidated = False
        Bukeep.Visible = False
        If EMHost <> vbNullString And EMUser <> vbNullString And EMPassword <> vbNullString And EMDomain <> vbNullString Then
            LBCAvalidation.Text = "Client Auto Credentials need validating"
            BUValidate.Visible = True
        Else
            LBCAvalidation.Text = "All 4 fields must be populated"
            BUValidate.Visible = False
        End If

    End Sub

    Private Sub ValidateEM()
        Dim rtn As String
        Dim am As Integer

        If InStr(MyDrive, ":") > 0 Then
            am = 1
        Else
            am = 0
        End If

        LBAllSoftware.Visible = False
        LBAllSoftware.Text = "Getting List of software packages"
        If Alreadymapped = False And MyDrive <> vbNullString Then

            rtn = UpdateLog(am, "startup.validateEM", "Mydrive already Populated unmapping now", 0)
            rtn = Unmapnetworkdrive(MyDrive)
            am = 0
            If rtn = "OK" Then
                rtn = UpdateLog(am, "startup.validateEM", "Unmap OK", 0)
            Else
                rtn = UpdateLog(am, "startup.validateEM", "Unmap Failed", 2)
            End If
        End If
        If SessionIDset = True Then
            rtn = UpdateLog(am, "startup.validateEM", "Existing web services session exists will log off", 0)
            rtn = WSLogout()
            If rtn = "OK" Then
                SessionIDset = False
                rtn = UpdateLog(am, "startup.validateEM", "log off OK", 0)
            Else
                rtn = UpdateLog(am, "startup.validateEM", "log off failed", 2)

            End If

        End If
        rtn = UpdateLog(am, "startup.validateEM", "Clearing all arrays", 0)
        If CBAllSoftware.Items.Count > 0 Then
            CBAllSoftware.Items.Clear()
            TotalPackageCount = 0
            PackageNameList.Initialize()
            PackageUUIDList.Initialize()
            PackageVersionList.Initialize()
            Suggestions.Initialize()

        End If
        rtn = UpdateLog(am, "startup.validateEM", "Testing webservices connection by logging in", 0)
        rtn = WSLogin(EMHost)
        BUValidate.Visible = False
        If rtn <> "OK" Then
            rtn = UpdateLog(am, "startup.validateEM", "login failed check yot settings and ensure you have rights in client auto", 1)
            LBCAvalidation.Text = "Validation Failed"
            EMValidated = False
        Else
            rtn = UpdateLog(am, "startup.validateEM", "log in OK", 0)
            LBCAvalidation.Text = "Credentials Validated"
            EMValidated = True
            If checkEM.Checked = True Then
                rtn = UpdateLog(am, "startup.validateEM", "Save credentials is enabled saving now", 0)
                My.Settings.CAdomain = tbEMdomain.Text
                My.Settings.CAhost = TBEMHost.Text
                My.Settings.CApassword = tbempassword.Text
                My.Settings.CAuser = tbemuser.Text
                If CheckBoxLocal.Checked = True Then
                    My.Settings.UseLocal = "True"
                Else
                    My.Settings.UseLocal = "False"

                End If
                My.Settings.Save()

            End If
            GBEM.Visible = False
            Lbvalidated.Text = "Connection validated to Client Auto Enterprise Server"
            Lbvalidated1.Text = EMHost
            Lbvalidated.Visible = True
            burevalidate.Visible = True
            Lbvalidated1.Visible = True
            LBAllSoftware.Visible = True
            Dim rtn2 As String
            rtn = UpdateLog(am, "startup.validateEM", "Getting next avalible drive", 0)
            rtn2 = NextAvailableDrive()
            If rtn2 = "NOTOK" Then
                MessageBox.Show("Can not find an avalible drive letter to map")
                rtn = UpdateLog(am, "startup.validateEM", "Getting next avalible drive failed very unusual, use NET USE to figure it out", 2)
                Exit Sub
            ElseIf rtn2 <> "OK" Then
                rtn = UpdateLog(am, "startup.validateEM", "next avalible drive=" + rtn, 0)
                rtn = UpdateLog(am, "startup.validateEM", "Calling MapnetworkDrive", 0)
                rtn2 = MapnetworkDrive(rtn2, "\\" & EMHost & "\SDLibrary", EMPassword, EMDomain, EMUser)

                If rtn2 <> "OK" Then
                    MessageBox.Show("Cannot map drive to \\" & EMHost & "\SDLibrary ensure there Is a Sdlibraryshare on " + EMHost + "And the user " + EMDomain + "\" + EMUser + " has rights")
                    rtn = UpdateLog(am, "startup.validateEM", "Mapnetworkdrive failed", 2)
                    Exit Sub
                End If
            End If
            rtn = UpdateLog(am, "startup.validateEM", "Mydrive=" + MyDrive, 0)
            rtn = UpdateLog(am, "startup.validateEM", "checking if the directory " + MyDrive + "\PackageModifyApp exists", 0)
            If (Not System.IO.Directory.Exists(MyDrive + "\PackageModifyApp")) Then
                rtn = UpdateLog(am, "startup.validateEM", "directory " + MyDrive + "\PackageModifyApp does not exit will create it", 0)
                Try
                    System.IO.Directory.CreateDirectory(MyDrive + "\PackageModifyApp")
                Catch ex As Exception
                    MessageBox.Show("Unable to create the directory " & MyDrive + "\PackageModifyApp")
                    rtn = UpdateLog(am, "startup.validateEM", "directory creation failed", 2)
                    Exit Sub
                End Try
            End If
            rtn = UpdateLog(am, "startup.validateEM", "checking if the directory " + MyDrive + "\PackageModifyApp\ " + EMUser + " exists", 0)
            If (Not System.IO.Directory.Exists(MyDrive + "\PackageModifyApp" + EMUser)) Then
                rtn = UpdateLog(am, "startup.validateEM", "directory " + MyDrive + "\PackageModifyApp\" + EMUser + " does not exit will create it", 0)
                Try
                    System.IO.Directory.CreateDirectory(MyDrive + "\PackageModifyApp\" + EMUser)
                Catch ex As Exception
                    rtn = UpdateLog(am, "startup.validateEM", "Unable to create directory " + MyDrive + "\PackageModifyApp\" + EMUser, 2)
                    MessageBox.Show("Unable to create the directory " & MyDrive + "\PackageModifyApp\" + EMUser)
                    Exit Sub
                End Try
            End If
            rtn = UpdateLog(am, "startup.validateEM", "checking if the directory " + MyDrive + "\PackageModifyApp\ " + EMUser + " \logs exists", 0)
            If Not System.IO.Directory.Exists(MyDrive + "\PackageModifyApp\" + EMUser + "\Logs") Then
                rtn = UpdateLog(am, "startup.validateEM", "directory " + MyDrive + "\PackageModifyApp\" + EMUser + "\logs does not exit will create it", 0)
                Try
                    System.IO.Directory.CreateDirectory(MyDrive + "\PackageModifyApp\" + EMUser + "\logs")
                Catch ex As Exception
                    rtn = UpdateLog(am, "startup.validateEM", "Unable to create directory " + MyDrive + "\PackageModifyApp\" + EMUser + "\logs", 2)
                    MessageBox.Show("Unable To create the directory " & MyDrive + "\PackageModifyApp\" + EMUser + "\logs")
                    Exit Sub
                End Try
            End If
            am = 1
            LogCreate(am)
            rtn = UpdateLog(am, "startup.validateEM", "Getting list of All Software", 0)
            Dim RTN1 As String = WSGetAllSoftware()
            If RTN1 <> "OK" Then
                rtn = UpdateLog(am, "startup.validateEM", "Getting List of all software failed", 2)
                LBAllSoftware.Text = "Failed to get list of All software"
            Else
                LBAllSoftware.Text = "Choose the software package to unseal"
                CBAllSoftware.AutoCompleteSource = AutoCompleteSource.ListItems
                CBAllSoftware.AutoCompleteMode = AutoCompleteMode.Suggest
                Dim j As Integer
                For j = 0 To TotalPackageCount
                    CBAllSoftware.Items.Add(Suggestions(j))

                Next
                CBAllSoftware.Sorted = True
                LBAllSoftware.Text = "Start Typing the name of the software package to be unsealed"
                CBAllSoftware.Visible = True
                UpdateLog(am, "startup.validateEM", "Getting list of All Domains", 0)
                rtn = WSGetAllDomains()
                If rtn <> "OK" Then
                    WSLogout()
                    Exit Sub
                End If
                UpdateLog(am, "startup.validateEM", "Getting UUID of  AllServers Group", 0)
                rtn = WSFindDMGroup()
                WSLogout()

            End If
        End If

    End Sub


    Private Sub BUvalidate_Click(sender As Object, e As EventArgs) Handles BUValidate.Click
        If EMValidated = False Then
            LBCAvalidation.Text = "Evaluating Client Auto Credentials"

            LBBGStatus.Text = ""
            LBBGStatus.Tag = ""
            LBBGStatus.Visible = False
            lbfinalstat.Visible = False
            lbfinalstat.Text = ""
            FinalStat = ""


            ValidateEM()
        End If
    End Sub



    Private Sub BUexit_Click(sender As Object, e As EventArgs) Handles BUexit.Click
        Exitapp()
    End Sub

    Private Sub CheckEM_CheckedChanged(sender As Object, e As EventArgs) Handles checkEM.CheckedChanged

        If checkEM.Checked = False Then
            My.Settings.CAdomain = vbNullString
            My.Settings.CAhost = vbNullString
            My.Settings.CApassword = vbNullString
            My.Settings.CAuser = vbNullString
            My.Settings.UseLocal = vbNullString
            My.Settings.Save()

        Else
            If EMValidated = True Then

                My.Settings.CAdomain = tbEMdomain.Text
                My.Settings.CAhost = TBEMHost.Text
                My.Settings.CApassword = tbempassword.Text
                My.Settings.CAuser = tbemuser.Text
                If My.Settings.UseLocal = "True" Then
                    Uselocal = True
                    CheckBoxLocal.Checked = True
                Else
                    Uselocal = False
                    CheckBoxLocal.Checked = False
                End If
                My.Settings.Save()

            End If
        End If
    End Sub


    Private Sub CBAllSoftware_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CBAllSoftware.SelectedIndexChanged
        SelectedPackageName = Strings.Left(CBAllSoftware.SelectedItem, InStr(CBAllSoftware.SelectedItem, "'") - 1)
        Dim j As Integer
        For j = 0 To TotalPackageCount
            If SelectedPackageName = PackageNameList(j) Then

                SelectedPackageVersion = PackageVersionList(j)
                SelectedPackageUUID = PackageUUIDList(j)
                TBSelectedPackage.Text = SelectedPackageName
                TbselectedVersion.Text = SelectedPackageVersion
                SelectedPackageversionTmp = SelectedPackageVersion.Replace("/", "_")
                SelectedPackageNameTmp = SelectedPackageName.Replace("/", "_")
                SelectedPackageversionTmp = SelectedPackageversionTmp.Replace(":", "_")
                SelectedPackageNameTmp = SelectedPackageNameTmp.Replace(":", "_")

                LBAllSoftware.Text = "Select dropdown arrow to reselect"
                Exit For
            End If

        Next j


        If (Not System.IO.Directory.Exists(MyDrive + "\Exports")) Then

            Try
                System.IO.Directory.CreateDirectory(MyDrive + "\Exports")
            Catch ex As Exception
                MessageBox.Show("Unable to create the directory " & MyDrive + "\Exports")
                Exit Sub
            End Try
        End If

        If (Not System.IO.Directory.Exists(MyDrive + "\Exports\" + SelectedPackageNameTmp + "'" + SelectedPackageversionTmp)) Then
            Try
                System.IO.Directory.CreateDirectory(MyDrive + "\Exports\" + SelectedPackageNameTmp + "'" + SelectedPackageversionTmp)
            Catch ex As Exception
                MessageBox.Show("Unable to create the directory " & MyDrive + "\Exports" + SelectedPackageNameTmp + "'" + SelectedPackageversionTmp)
                Exit Sub
            End Try
        End If
        If (Not System.IO.Directory.Exists(MyDrive + "\Exports\" + SelectedPackageNameTmp + "'" + SelectedPackageversionTmp + "\original")) Then
            Try
                System.IO.Directory.CreateDirectory(MyDrive + "\Exports\" + SelectedPackageNameTmp + "'" + SelectedPackageversionTmp + "\original")
            Catch ex As Exception
                MessageBox.Show("Unable to create the directory " & MyDrive + "\Exports" + SelectedPackageNameTmp + "'" + SelectedPackageversionTmp + "\original")
                Exit Sub
            End Try

            GBpackageSeal.Visible = True
            Buexport.Visible = True
            buUnseal.Visible = False
        Else
            buUnseal.Visible = True
            GBpackageSeal.Visible = True
            Buexport.Visible = False

        End If
        Workingdir = MyDrive + "\Exports\" + SelectedPackageNameTmp + "'" + SelectedPackageversionTmp + "\"

    End Sub

    Private Sub CBAllSoftware_DropDown(sender As Object, e As EventArgs) Handles CBAllSoftware.DropDown
        GBpackageSeal.Visible = False
    End Sub

    Private Sub Buunseal_Click(sender As Object, e As EventArgs) Handles buUnseal.Click
        Buexport.Visible = False
        LBAllSoftware.Visible = False
        CBAllSoftware.Visible = False
        GBpackageSeal.Location = New Drawing.Point(483, 49)
        LBBGStatus.Visible = True
        LBBGStatus.Text = ""
        PictureBox1.Visible = True
        BUexit.Visible = False
        burevalidate.Visible = False
        buAnother.Visible = False
        Buexport.Visible = False
        buUnseal.Visible = False
        BackgroundWorker2.RunWorkerAsync()
    End Sub

    Private Sub Buexport_Click(sender As Object, e As EventArgs) Handles Buexport.Click
        Buexport.Visible = False
        LBAllSoftware.Visible = False
        CBAllSoftware.Visible = False
        GBpackageSeal.Location = New Drawing.Point(483, 49)
        LBBGStatus.Visible = True
        LBBGStatus.Text = "EXPORTING"
        PictureBox1.Visible = True
        BUexit.Visible = False
        burevalidate.Visible = False
        BackgroundWorker1.RunWorkerAsync()
        'WSExportSDPackage()



    End Sub

    Private Sub BUAnother_Click(sender As Object, e As EventArgs) Handles buAnother.Click
        LBBGStatus.Text = ""
        LBBGStatus.Tag = ""
        LBBGStatus.Visible = False
        lbfinalstat.Visible = False
        lbfinalstat.Text = ""
        FinalStat = ""
        GBpackageSeal.Visible = False
        LBAllSoftware.Visible = True
        CBAllSoftware.Visible = True
        buUnseal.Visible = False
        Buexport.Visible = False
        GBpackageSeal.Location = New Drawing.Point(486, 96)
        buAnother.Visible = False
    End Sub

    Private Sub Burevalidate_Click(sender As Object, e As EventArgs) Handles burevalidate.Click
        burevalidate.Visible = False
        Lbvalidated.Visible = False
        Lbvalidated1.Visible = False
        Bukeep.Visible = True
        BUValidate.Visible = False
        GBEM.Visible = True
    End Sub

    Private Sub Bukeep_Click(sender As Object, e As EventArgs) Handles Bukeep.Click
        GBEM.Visible = False
        burevalidate.Visible = True
        Lbvalidated.Visible = True
        Lbvalidated1.Visible = True
    End Sub




    Private Sub CheckBoxLocal_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxLocal.CheckedChanged
        If EMValidated = True Then
            GBEM.Visible = False
            burevalidate.Visible = True
            Lbvalidated.Visible = True
        End If
        If CheckBoxLocal.Checked = True Then
            tbEMdomain.Visible = False
            TBCAdomain.Visible = False
            Uselocal = True
        Else
            tbEMdomain.Visible = True
            TBCAdomain.Visible = True
            Uselocal = False
        End If
    End Sub



    Public Sub New()
        InitializeComponent()
        BackgroundWorker1.WorkerReportsProgress = True
        BackgroundWorker1.WorkerSupportsCancellation = True
        BackgroundWorker2.WorkerReportsProgress = True
        BackgroundWorker2.WorkerSupportsCancellation = True
    End Sub



    ' This event handler is where the time-consuming work is done.
    Private Sub BackgroundWorker1_DoWork(ByVal sender As System.Object,
    ByVal e As DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Dim worker As BackgroundWorker = CType(sender, BackgroundWorker)
        Dim rtn As String = WSExportSDPackage()
        If rtn = "NOTOK" Then
            worker.ReportProgress(99)
            Exit Sub
        End If

    End Sub

    ' This event handler updates the progress.
    Private Sub BackgroundWorker1_ProgressChanged(ByVal sender As System.Object,
    ByVal e As ProgressChangedEventArgs) Handles BackgroundWorker1.ProgressChanged
        BUexit.Visible = True
        buAnother.Visible = True
        LBBGStatus.Visible = False
        lbfinalstat.Visible = False
        lbfinalstat.Text = ""
        FinalStat = ""
        burevalidate.Visible = True
    End Sub

    ' This event handler deals with the results of the background operation.
    Private Sub BackgroundWorker1_RunWorkerCompleted(ByVal sender As System.Object,
    ByVal e As RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted

        PictureBox1.Visible = False
        LBBGStatus.Visible = False
        lbfinalstat.Visible = False
        lbfinalstat.Text = ""
        FinalStat = ""
        MessageBox.Show("Package has been exported to " & vbCrLf & "\\" & EMHost & "\sdlibrary\exports\" & SelectedPackageNameTmp & "'" & SelectedPackageversionTmp & "\original" & vbCrLf & "for your convieniance while this application is open that location is mapped to drive " & MyDrive)
        Buexport.Visible = False
        buUnseal.Visible = True
        buAnother.Visible = True
        BUexit.Visible = True
        burevalidate.Visible = True
    End Sub

    Private Sub BackgroundWorker2_DoWork(ByVal sender As System.Object,
       ByVal e As DoWorkEventArgs) Handles BackgroundWorker2.DoWork
        Dim worker As BackgroundWorker = CType(sender, BackgroundWorker)
        'If File.Exists("c:\bg.log") Then
        'File.Delete("c:\bg.log")

        'End If
        'Using log1 As StreamWriter = File.CreateText("c:\bg.log")

        'End Using
        Dim j As Integer

        Dim rtn As String = "NOTOK"
        UpdateLog(1, "BackgrounderWorker2_DoWork.BUUNSEAL_click", "Calling GetDistRecords", 0)
        worker.ReportProgress(2)
        rtn = GetDistRecords()

        If rtn <> "OK" Then
            FinalStat = "Unable to get Distribution records from " + EMHost + " nothing has been done"
            worker.ReportProgress(99)

            Exit Sub
        End If
        worker.ReportProgress(2)
        UpdateLog(1, "BackgrounderWorker2_DoWork", "Calling GetAllSS", 0)
        worker.ReportProgress(4)
        rtn = GetAllSS()
        UpdateLog(1, "BackgrounderWorker2_DoWork", "Total number of ss=" + Str(TotalSS + 1), 0)
        FinalStat = "Unable to get list of all SS from " + EMHost + " nothing has been done"
        If rtn <> "OK" Then
            worker.ReportProgress(99)
            Exit Sub
        End If
        worker.ReportProgress(6)
        For j = 0 To DistCount
            UpdateLog(1, "BackgrounderWorker2_DoWork", "Calling GetStagerecords for DM " + IsDistName(j, 0), 0)
            CurVal(0) = IsDistName(j, 0)

            rtn = GetStageRecords(j)
            If rtn = "NOTOK" Then
                UpdateLog(1, "BackgrounderWorker2_DoWork", "Failed GetStagerecords for DM " + IsDistName(j, 0), 2)
                FinalStat = "Unable to get stage records from DM " + CurVal(0) + " other actions on this domain will be ignored"
                worker.ReportProgress(7)
                Exit Sub
            End If
            worker.ReportProgress(5)
        Next

        worker.ReportProgress(9)
        UpdateLog(1, "BackgrounderWorker2_DoWork", "Calling GetPackagePath for DM " + IsDistName(j, 0), 0)
        worker.ReportProgress(8)
        rtn = GetPackagePath()
        If rtn = "NOTOK" Then
            UpdateLog(1, "BackgrounderWorker2_DoWork", "Unable to get package path", 2)
            FinalStat = "Unable to find directory path for the SD package in the SD library on the EM nothing has been done"
            worker.ReportProgress(99)
            Exit Sub
        End If
        UpdateLog(1, "BackgrounderWorker2_DoWork", "Calling FindAndCopyDifferences", 0)
        worker.ReportProgress(10)
        rtn = FindAndCopyDifferences(rtn)
        If rtn <> "OK" Then
            finalstat = "Unable to find any changes or unable to create the changes directory, If ""original"" directory still exists then resove the problem and redistribute else re-export"
            worker.ReportProgress(99)
            Exit Sub
        End If
        UpdateLog(1, "BackgrounderWorker2_DoWork", "CallingCreateSDPackage", 0)
        worker.ReportProgress(12)
        rtn = CreateSDPackage()
        If rtn <> "OK" Then
            FinalStat = "Unable to create SD package once issue is resolved delete the ""replaced"" folder and re-distribute"
            worker.ReportProgress(99)
            Exit Sub
        End If
        UpdateLog(1, "BackgrounderWorker2_DoWork", "CallingUnChecksumSoftwarepackage(" + EMHost + ")", 0)
        CurVal(0) = EMHost
        worker.ReportProgress(14)
        rtn = WSUNChecksumsoftwarePackage(EMHost, True)
        If rtn <> "OK" Then
            Finalstat = "SD pacakge has been created on the EM but was unable to update the checksum on the EM"

            worker.ReportProgress(15)
            worker.ReportProgress(93)
            Exit Sub

        End If
        worker.ReportProgress(17)
        For j = 0 To DistCount
            UpdateLog(1, "BackgrounderWorker2_DoWork", "CallingUnChecksumSoftwarepackage(" + IsDistName(j, 0) + ")", 0)
            CurVal(0) = IsDistName(j, 0)
            worker.ReportProgress(14)
            rtn = WSUNChecksumsoftwarePackage(IsDistName(j, 0), False)
            If rtn = "NOTOK" Then
                FinalStat = "Unable to UPdate checksum on DM " + CurVal(0) + " no further procesiing was done on this DM"
                IsDistName(j, 2) = "FALSE"
                curval1 = 0
                CurVal(j + 30) = IsDistName(j, 0)
                worker.ReportProgress(j + 30)
            Else
                curval1 = 1
                CurVal(j + 30) = IsDistName(j, 0)
                IsDistName(j, 2) = "TRUE"
                worker.ReportProgress(j + 30)
            End If
        Next
        UpdateLog(1, "BackgrounderWorker2_DoWork", "Calling UpdateEM", 0)
        worker.ReportProgress(16)
        rtn = UpdateEM()
        If rtn <> "OK" Then
            FinalStat = "Unable to update the package on the EM the update package with the changes should already exist on the EM distribute it and deliver it to the machines required"
            worker.ReportProgress(96)
            worker.ReportProgress(99)
            Exit Sub
        End If


        UpdateLog(1, "BackgrounderWorker2_DoWork", "Calling DistributeUpdatePackage", 0)
        worker.ReportProgress(18)
        rtn = DistributeUpdatePackage()
        If rtn = "NOTOK" Then
            finalstat = "Uanble to create a distribution job package can not be distributed or delivered but should be in the library on the EM"
            worker.ReportProgress(96)
            worker.ReportProgress(99)


            Exit Sub
        End If



        UpdateLog(1, "BackgrounderWorker2_DoWork", "Starting GetDistributionStatus loop", 0)
        worker.ReportProgress(20)
        Dim waiting As Boolean = True
        For j = 0 To 120
            CurVal(0) = Str(j)
            worker.ReportProgress(21)
            worker.ReportProgress(93)
            UpdateLog(1, "BackgrounderWorker2_DoWork", "Waiting 15 seconds to get Distrubution status", 0)
            System.Threading.Thread.Sleep(15000)
            CurVal(0) = "done"

            UpdateLog(1, "BackgrounderWorker2_DoWork", "Calling GetDistributionStatus", 0)
            rtn = GetDistributeStatus()

            If rtn = "NOTOK" Then
                UpdateLog(1, "BackgrounderWorker2_DoWork", "GetDistibutionStatus Failed", 0)
                FinalStat = "Unable to get distribution status package delivery has been aborted"
                worker.ReportProgress(96)
                Exit Sub
            ElseIf rtn = "OK" Then
                UpdateLog(1, "BackgrounderWorker2_DoWork", "Distibution Completed", 0)
                waiting = False
                Exit For

            End If
            worker.ReportProgress(21)
        Next
        worker.ReportProgress(21)
        If waiting = True Then
            waiting = False
            FinalStat = "Distubution Timed out can not continue delivering the update package check individual statuses on right for details"
            For j = 0 To DistCount
                If IsDistName(j, 2) = "DISTRIBUTED" Then
                    waiting = True
                    UpdateLog(1, "BackgrounderWorker2_DoWork", "Distibution timed out but at least 1 dm completed will proceed checking SD job status to the completed domains", 1)
                    Exit For
                End If
            Next
            If waiting = False Then
                UpdateLog(1, "BackgrounderWorker2_DoWork", "All Distubutions timed out cannot continue", 2)
                FinalStat = "All Distrubutions timed out update package was created on the EM but not delivered anywhere"
                worker.ReportProgress(91)
                worker.ReportProgress(99)
                Exit Sub
            End If

        End If
        worker.ReportProgress(22)
        For j = 0 To 120
            CurVal(0) = Str(j)

            worker.ReportProgress(22)
            UpdateLog(1, "BackgrounderWorker2_DoWork", "Waiting 15 seconds to get delivery status", 0)
            System.Threading.Thread.Sleep(15000)
            CurVal(0) = "done"

            UpdateLog(1, "BackgrounderWorker2_DoWork", "Calling GetDistributionStatus", 0)
            Dim NotDone As Boolean
            Dim containername As String = ModifyPackagename + "_" + ModifyPackageVersion

            NotDone = False
            For k = 0 To DistCount
                If IsDistName(k, 2) = "DISTRIBUTED" Then
                    UpdateLog(1, "Backgrounder2_dowork", "Getting the delivery status from DM " + IsDistName(k, 0), 0)
                    rtn = GetSDJobStatus(containername, IsDistName(k, 0))
                    If rtn <> "OK" Then
                        IsDistName(k, 2) = rtn
                    Else
                        NotDone = True
                    End If
                End If
                worker.ReportProgress(24)
                worker.ReportProgress(93)
            Next
            If NotDone = False Then
                If FinalStat = vbNullString Then
                    FinalStat = "Delivery to all machines complete"
                End If
                worker.ReportProgress(24)
                worker.ReportProgress(93)
                Exit Sub
            End If
        Next
        UpdateLog(1, "Backgrounder2_dowork", "One or more SD jobs timed out after 30 minutes", 2)
        FinalStat = "Delivery to one or more machnes timed out"
        worker.ReportProgress(25)
        worker.ReportProgress(93)
        MessageBox.Show("One or more SD jobs timed out after 30 minutes")





    End Sub

    ' This event handler updates the progress.
    Private Sub BackgroundWorker2_ProgressChanged(ByVal sender As System.Object,
    ByVal e As ProgressChangedEventArgs) Handles BackgroundWorker2.ProgressChanged

        Dim done As Boolean = True
        Dim j, k, L, M As Integer
        If e.ProgressPercentage = 2 Then
            LBBGStatus.Text = "Getting Distubution records"
            TreeView1.Visible = True

        ElseIf e.ProgressPercentage = 4 Then
            TreeView1.Visible = True
            Dim rootnode As TreeNode
            rootnode = New TreeNode() With {
            .Tag = EMHost,
            .Text = .Tag + " **(REQUIRES UPDATE)**"}
            TreeView1.Nodes.Add(rootnode)
            rootnode.ForeColor = Color.Blue
            UpdateLog(1, "BackgrounderWorker2_ProgressChanged", "4 Creating root node tag=" + TreeView1.Nodes(0).Tag + " Text=" + TreeView1.Nodes(0).Text, 0)


            For j = 0 To TotalDM
                TreeView1.Nodes(0).Nodes.Add(DMName(j, 0))
                TreeView1.Nodes(0).Nodes(j).Tag = DMName(j, 0)
                TreeView1.Nodes(0).Nodes(j).Text = TreeView1.Nodes(0).Nodes(j).Tag
                TreeView1.Nodes(0).Nodes(j).ForeColor = Color.Blue

                UpdateLog(1, "BackgrounderWorker2_ProgressChanged", "4 Adding DM node to Root tag=" + TreeView1.Nodes(0).Nodes(j).Tag + " Text=" + TreeView1.Nodes(0).Nodes(j).Text, 0)
            Next
            TreeView1.ExpandAll()
            LBBGStatus.Text = "Getting List of all Scalability Servers"
        ElseIf e.ProgressPercentage = 5 Then
            For j = 0 To TotalSS
                If sslist(j, 1) = CurVal(0) Then
                    If sslist(j, 2) = "staged" Then
                        For k = 0 To TreeView1.Nodes(0).Nodes.Count - 1
                            If sslist(j, 1) = TreeView1.Nodes(0).Nodes(k).Tag Then
                                If TreeView1.Nodes(0).Nodes(k).Nodes.Count > 0 Then
                                    For L = 0 To TreeView1.Nodes(0).Nodes(k).Nodes.Count - 1
                                        If TreeView1.Nodes(0).Nodes(k).Nodes(L).Tag = sslist(j, 0) Then
                                            TreeView1.Nodes(0).Nodes(k).Nodes(L).Text = TreeView1.Nodes(0).Nodes(k).Nodes(L).Tag + " **(REQUIRES UPDATE)**"
                                            TreeView1.Nodes(0).Nodes(k).Nodes(L).ForeColor = Color.Black
                                            UpdateLog(1, "BackgrounderWorker2_ProgressChanged", "5 Adding update required to ss tag=" + TreeView1.Nodes(0).Nodes(k).Nodes(L).Tag, 0)
                                        End If
                                    Next
                                End If
                            End If
                        Next
                    End If
                End If
            Next


        ElseIf e.ProgressPercentage = 6 Then
            For j = 0 To TotalSS
                For k = 0 To TreeView1.Nodes(0).Nodes.Count - 1
                    If sslist(j, 1) = TreeView1.Nodes(0).Nodes(k).Tag Then
                        TreeView1.Nodes(0).Nodes(k).Nodes.Add(sslist(j, 0))
                        TreeView1.Nodes(0).Nodes(k).Nodes(TreeView1.Nodes(0).Nodes(k).Nodes.Count - 1).Tag = sslist(j, 0)
                        TreeView1.Nodes(0).Nodes(k).Nodes(TreeView1.Nodes(0).Nodes(k).Nodes.Count - 1).Text = TreeView1.Nodes(0).Nodes(k).Nodes(TreeView1.Nodes(0).Nodes(k).Nodes.Count - 1).Tag
                        TreeView1.Nodes(0).Nodes(k).Nodes(TreeView1.Nodes(0).Nodes(k).Nodes.Count - 1).ForeColor = Color.Blue
                        UpdateLog(1, "BackgrounderWorker2_ProgressChanged", "6 Add SS node to DM " + TreeView1.Nodes(0).Nodes(k).Tag + " ss node tag=" + TreeView1.Nodes(0).Nodes(k).Nodes(TreeView1.Nodes(0).Nodes(k).Nodes.Count - 1).Tag + " Text=" + TreeView1.Nodes(0).Nodes(k).Nodes(TreeView1.Nodes(0).Nodes(k).Nodes.Count - 1).Text, 0)
                    End If
                Next
            Next
            For j = 0 To DistCount
                For k = 0 To TreeView1.Nodes(0).Nodes.Count - 1
                    If TreeView1.Nodes(0).Nodes(k).Tag = IsDistName(j, 0) Then
                        TreeView1.Nodes(0).Nodes(k).Text = TreeView1.Nodes(0).Nodes(k).Tag + " **(REQUIRES UPDATE)**"
                        TreeView1.Nodes(0).Nodes(k).ForeColor = Color.Black
                        UpdateLog(1, "BackgrounderWorker2_ProgressChanged", "6 Adding requires update to DM node tag=" + TreeView1.Nodes(0).Nodes(k).Tag, 0)
                        Exit For
                    End If
                Next
            Next
            TreeView1.ExpandAll()
            LBBGStatus.Text = "Getting Stage Records from " + IsDistName(Val(CurVal(0)), 0)
        ElseIf e.ProgressPercentage = 7 Then
            TreeView1.Nodes(0).ForeColor = Color.DarkRed

            For j = 0 To TreeView1.Nodes(0).Nodes.Count - 1
                If TreeView1.Nodes(0).Nodes(j).Tag = CurVal(0) Then
                    TreeView1.Nodes(0).Nodes(j).Text = TreeView1.Nodes(0).Nodes(j).Tag + " **(FAILED GET STAGE RECORDS)**"
                    TreeView1.Nodes(0).Nodes(j).ForeColor = DarkRed
                    UpdateLog(1, "BackgrounderWorker2_ProgressChanged", "7 Change Status Failed Get Stage on DM tag=" + TreeView1.Nodes(0).Nodes(j).Tag, 0)
                    If TreeView1.Nodes(0).Nodes(j).Nodes.Count > 0 Then
                        For k = 0 To TreeView1.Nodes(0).Nodes(j).Nodes.Count - 1
                            TreeView1.Nodes(0).Nodes(j).Nodes(k).Text = TreeView1.Nodes(0).Nodes(j).Nodes(k).Tag + " **(ABORTED FAILURE ABOVE)**"
                            TreeView1.Nodes(0).Nodes(j).Nodes(k).ForeColor = Color.DarkRed
                            UpdateLog(1, "BackgrounderWorker2_ProgressChanged", "7 Change Status Failed above on SS tag=" + TreeView1.Nodes(0).Nodes(j).Nodes(k).Tag, 0)
                        Next
                    End If
                    Exit For
                End If
            Next

        ElseIf e.ProgressPercentage = 8 Then

            For j = 0 To TotalSS
                If sslist(j, 2) = "staged" Then
                    TreeView1.Nodes(0).ForeColor = Color.Black
                    For k = 0 To TreeView1.Nodes(0).Nodes.Count - 1
                        If sslist(j, 1) = TreeView1.Nodes(0).Nodes(k).Tag Then
                            TreeView1.Nodes(0).Nodes(k).ForeColor = Color.Black
                            For L = 0 To TreeView1.Nodes(0).Nodes(k).Nodes.Count - 1
                                If sslist(j, 1) = TreeView1.Nodes(0).Nodes(k).Nodes(L).Tag Then
                                    TreeView1.Nodes(0).Nodes(k).Nodes(L).ForeColor = Color.Black
                                    TreeView1.Nodes(0).Nodes(k).Nodes(L).Text = TreeView1.Nodes(0).Nodes(k).Nodes(L).Tag + " **(REQUIRES UPDATE)** "

                                    UpdateLog(1, "BackgrounderWorker2_ProgressChanged", "8 Adding requires update to ss node tag=" + TreeView1.Nodes(0).Nodes(k).Nodes(L).Tag, 0)
                                    Exit For
                                    Exit For
                                End If
                            Next
                        End If
                    Next
                End If
            Next
            LBBGStatus.Text = "Getting Path of the software delivery package"
        ElseIf e.ProgressPercentage = 10 Then
            LBBGStatus.Text = "Finding the changed files"

        ElseIf e.ProgressPercentage = 12 Then
            LBBGStatus.Text = "Creating Software Delivery Package"
        ElseIf e.ProgressPercentage = 14 Then
            TreeView1.Nodes(0).Text = TreeView1.Nodes(0).Tag + " **(SD PACKAGE CREATED)**"
            LBBGStatus.Text = "Disabling the checksum on " + CurVal(0)
            UpdateLog(1, "BackgrounderWorker2_ProgressChanged", "14 Updating status to Package created on Node EM tag=" + TreeView1.Nodes(0).Tag, 0)

        ElseIf e.ProgressPercentage = 17 Then
            TreeView1.Nodes(0).Text = TreeView1.Nodes(0).Tag + " **(CHECKSUM UPDATED)**"
            UpdateLog(1, "BackgrounderWorker2_ProgressChanged", "17 Updating status to checksum updated on Node EM tag=" + TreeView1.Nodes(0).Tag, 0)
        ElseIf (e.ProgressPercentage >= 30 And e.ProgressPercentage < 60) Or e.ProgressPercentage = 15 Then
            If curval1 = 0 Then
                If CurVal(e.ProgressPercentage) = EMHost Then
                    TreeView1.Nodes(0).ForeColor = Color.DarkRed
                    TreeView1.Nodes(0).Text = TreeView1.Nodes(0).Tag + " **(UNABLE TO UPDATE CHECKSUM)**"
                    UpdateLog(1, "BackgrounderWorker2_ProgressChanged", "30_60 Changing unable to update checksum on em " + TreeView1.Nodes(0).Tag, 0)
                    For j = 0 To TreeView1.Nodes(0).Nodes.Count - 1
                        TreeView1.Nodes(0).Nodes(j).ForeColor = Color.DarkRed
                        TreeView1.Nodes(0).Nodes(j).Text = TreeView1.Nodes(0).Nodes(j).Tag + " **(ABORTED SEE ABOVE PROCESS)**"
                        UpdateLog(1, "BackgrounderWorker2_ProgressChanged", "30_60 Changing status to abort because of step above fro DM node tag=" + TreeView1.Nodes(0).Nodes(j).Tag, 0)
                        If TreeView1.Nodes(0).Nodes(j).Nodes.Count > 0 Then
                            For k = 0 To TreeView1.Nodes(0).Nodes(j).Nodes.Count - 1
                                TreeView1.Nodes(0).Nodes(j).Nodes(k).ForeColor = Color.DarkRed
                                TreeView1.Nodes(0).Nodes(j).Nodes(k).Text = TreeView1.Nodes(0).Nodes(j).Nodes(k).Tag + " **(ABORTED SEE ABOVE PROCESS)**"
                                UpdateLog(1, "BackgrounderWorker2_ProgressChanged", "30_60 Changing status to abort because of step above on SS node tag=" + TreeView1.Nodes(0).Nodes(j).Nodes(k).Tag, 0)
                            Next
                        End If
                    Next


                Else
                    TreeView1.Nodes(0).ForeColor = Color.DarkRed
                    UpdateLog(1, "BackgrounderWorker2_ProgressChanged", "30_60 Changing color to red because could not update checksum on 1 or more DMs on em tag=" + TreeView1.Nodes(0).Tag, 0)
                    For j = 0 To TreeView1.Nodes(0).Nodes.Count - 1
                        If CurVal(e.ProgressPercentage) = TreeView1.Nodes(0).Nodes(j).Tag Then
                            TreeView1.Nodes(0).Nodes(j).Text = TreeView1.Nodes(0).Nodes(j).Tag + " **(UNABLE TO UPDATE CHECKSUM)**"
                            TreeView1.Nodes(0).Nodes(j).ForeColor = Color.DarkRed

                            UpdateLog(1, "BackgrounderWorker2_ProgressChanged", "30_60 Changing status to unable to update checksum on dm tag=" + TreeView1.Nodes(0).Nodes(j).Tag, 0)
                            If TreeView1.Nodes(0).Nodes(j).Nodes.Count > 0 Then
                                For k = 0 To TreeView1.Nodes(0).Nodes(j).Nodes.Count - 1
                                    TreeView1.Nodes(0).Nodes(j).Nodes(k).Text = TreeView1.Nodes(0).Nodes(j).Nodes(k).Tag + " **(ABORTED SEE ABOVE PROCESS)**"
                                    TreeView1.Nodes(0).Nodes(j).Nodes(k).ForeColor = Color.DarkRed

                                    UpdateLog(1, "BackgrounderWorker2_ProgressChanged", "30-60 Changing status to abort because of steps above on ss node tag=" + TreeView1.Nodes(0).Nodes(j).Nodes(k).Tag, 0)
                                Next
                            End If
                            Exit For
                        End If
                    Next
                End If
            Else
                For j = 0 To TreeView1.Nodes(0).Nodes.Count - 1
                    If CurVal(e.ProgressPercentage) = TreeView1.Nodes(0).Nodes(j).Tag Then
                        TreeView1.Nodes(0).Nodes(j).Text = TreeView1.Nodes(0).Nodes(j).Tag + " **(UPDATED CHECKSUM)**"
                        UpdateLog(1, "BackgrounderWorker2_ProgressChanged", "13 Updating text Updated checksum to DM node tag=" + TreeView1.Nodes(0).Nodes(j).Tag, 0)
                        Exit For
                    End If
                Next
            End If
        ElseIf e.ProgressPercentage = 16 Then
            LBBGStatus.Text = "Updating the package on the enterprise"


        ElseIf e.ProgressPercentage = 18 Then
            TreeView1.Nodes(0).Text = TreeView1.Nodes(0).Tag + " **(PACKAGE UPDATED)**"
            LBBGStatus.Text = "Distributing the update to the Domains"
        ElseIf e.ProgressPercentage = 20 Then
            For j = 0 To DistCount
                If IsDistName(j, 2) = "TRUE" Then
                    For k = 0 To TreeView1.Nodes(0).Nodes.Count - 1
                        If IsDistName(j, 0) = TreeView1.Nodes(0).Nodes(k).Tag Then
                            TreeView1.Nodes(0).Nodes(k).Text = TreeView1.Nodes(0).Nodes(k).Tag + " **(DISTRIBUTING)**"
                            UpdateLog(1, "BackgrounderWorker2_ProgressChanged", "20 Changing status to distributing on DM node tag=" + TreeView1.Nodes(0).Nodes(k).Tag, 0)
                            Exit For
                        End If
                    Next
                End If
            Next
            LBBGStatus.Text = "Getting Distribution Status"
        ElseIf e.ProgressPercentage = 21 Then
            UpdateLog(1, "BackgrounderWorker2_ProgressChanged", "21", 0)

            If CurVal(0) <> "DONE" Then
                UpdateLog(1, "BackgrounderWorker2_ProgressChanged", "21 <>done", 0)

                Dim dec As Decimal = Val(CurVal(0)) / 4
                Dim tmpstring As String = dec.ToString("G")
                LBBGStatus.Text = "Waiting 15 seconds to check Distribution Status have been waiting " + tmpstring + " minutes"
            Else
                LBBGStatus.Text = "Getting Package Delivery Status"
            End If
            For j = 0 To DistCount
                UpdateLog(1, "BackgrounderWorker2_ProgressChanged", "21 isdistname(j,2=" + IsDistName(j, 2), 0)
                If IsDistName(j, 2) = "DISTRIBUTED" Then
                    UpdateLog(1, "BackgrounderWorker2_ProgressChanged", "21 isdistname(j,2=" + IsDistName(j, 2), 0)
                    For k = 0 To TreeView1.Nodes(0).Nodes.Count - 1
                        If IsDistName(j, 0) = TreeView1.Nodes(0).Nodes(k).Tag Then
                            TreeView1.Nodes(0).Nodes(k).Text = TreeView1.Nodes(0).Nodes(k).Tag + " **(DEPLOYING)**"
                            UpdateLog(1, "BackgrounderWorker2_ProgressChanged", "21 Changing status to DEPLOYING on DM node tag=" + TreeView1.Nodes(0).Nodes(k).Tag, 0)
                            For L = 0 To TotalSS
                                If sslist(L, 1) = IsDistName(j, 0) And sslist(L, 2) <> vbNullString Then
                                    For M = 0 To TreeView1.Nodes(0).Nodes(k).Nodes.Count - 1
                                        If sslist(L, 0) = TreeView1.Nodes(0).Nodes(k).Nodes(M).Tag Then
                                            If sslist(L, 2) = "staged" Then
                                                TreeView1.Nodes(0).Nodes(k).Nodes(M).Text = TreeView1.Nodes(0).Nodes(k).Nodes(M).Tag + " **(DEPLOYING)**"
                                                UpdateLog(1, "BackgrounderWorker2_ProgressChanged", " 21 Changing status to deploying on ss node tag=" + TreeView1.Nodes(0).Nodes(k).Nodes(M).Tag, 0)
                                            ElseIf sslist(L, 2) = "EXECUTION_OK" Then
                                                TreeView1.Nodes(0).Nodes(k).Nodes(M).Text = TreeView1.Nodes(0).Nodes(k).Nodes(M).Tag + " **(OK)**"
                                                TreeView1.Nodes(0).Nodes(k).Nodes(M).ForeColor = Color.ForestGreen
                                                UpdateLog(1, "BackgrounderWorker2_ProgressChanged", "21 Changing status to OK on ss node tag=" + TreeView1.Nodes(0).Nodes(k).Nodes(M).Tag, 0)

                                            Else
                                                TreeView1.Nodes(0).Nodes(k).Nodes(M).Text = TreeView1.Nodes(0).Nodes(k).Nodes(M).Tag + " **(FAILED)**"
                                                TreeView1.Nodes(0).Nodes(k).Nodes(M).ForeColor = Color.DarkRed
                                                UpdateLog(1, "BackgrounderWorker2_ProgressChanged", "21 Changing status to Failed on ss node tag=" + TreeView1.Nodes(0).Nodes(k).Nodes(M).Tag, 0)
                                                TreeView1.Nodes(0).Nodes(k).ForeColor = Color.DarkRed
                                                TreeView1.Nodes(0).ForeColor = Color.DarkRed
                                            End If
                                        End If
                                    Next
                                End If
                            Next
                        End If
                    Next
                End If
            Next


        ElseIf e.ProgressPercentage = 22 Then
            For j = 0 To DistCount
                UpdateLog(1, "BackgrounderWorker2_ProgressChanged", "22 isdistname(j,2)=" + IsDistName(j, 2), 0)
                If IsDistName(j, 2) = "DISTRIBUTED" Then
                    UpdateLog(1, "BackgrounderWorker2_ProgressChanged", "22 isdistname(j,2)=" + IsDistName(j, 2), 0)
                    For k = 0 To TreeView1.Nodes(0).Nodes.Count - 1
                        If IsDistName(j, 0) = TreeView1.Nodes(0).Nodes(k).Tag Then
                            TreeView1.Nodes(0).Nodes(k).Text = TreeView1.Nodes(0).Nodes(k).Tag + " **(DEPLOYING)**"
                            UpdateLog(1, "BackgrounderWorker2_ProgressChanged", "22 Changing status to DEPLOYING on DM node tag=" + TreeView1.Nodes(0).Nodes(k).Tag, 0)
                            For L = 0 To TotalSS
                                If sslist(L, 1) = IsDistName(j, 0) And sslist(L, 2) <> vbNullString Then
                                    For M = 0 To TreeView1.Nodes(0).Nodes(k).Nodes.Count - 1
                                        If sslist(L, 0) = TreeView1.Nodes(0).Nodes(k).Nodes(M).Tag Then
                                            If sslist(L, 2) = "staged" Then
                                                TreeView1.Nodes(0).Nodes(k).Nodes(M).Text = TreeView1.Nodes(0).Nodes(k).Nodes(M).Tag + " **(DEPLOYING)**"
                                                UpdateLog(1, "BackgrounderWorker2_ProgressChanged", " 22 Changing status to deploying on ss node tag=" + TreeView1.Nodes(0).Nodes(k).Nodes(M).Tag, 0)
                                            ElseIf sslist(L, 2) = "EXECUTION_OK" Then
                                                TreeView1.Nodes(0).Nodes(k).Nodes(M).Text = TreeView1.Nodes(0).Nodes(k).Nodes(M).Tag + " **(OK)**"
                                                TreeView1.Nodes(0).Nodes(k).Nodes(M).ForeColor = Color.ForestGreen
                                                UpdateLog(1, "BackgrounderWorker2_ProgressChanged", "22 Changing status to OK on ss node tag=" + TreeView1.Nodes(0).Nodes(k).Nodes(M).Tag, 0)

                                            Else
                                                TreeView1.Nodes(0).Nodes(k).Nodes(M).Text = TreeView1.Nodes(0).Nodes(k).Nodes(M).Tag + " **(FAILED)**"
                                                TreeView1.Nodes(0).Nodes(k).Nodes(M).ForeColor = Color.DarkRed
                                                UpdateLog(1, "BackgrounderWorker2_ProgressChanged", "22 Changing status to Failed on ss node tag=" + TreeView1.Nodes(0).Nodes(k).Nodes(M).Tag, 0)
                                                TreeView1.Nodes(0).Nodes(k).ForeColor = Color.DarkRed
                                                TreeView1.Nodes(0).ForeColor = Color.DarkRed
                                            End If
                                        End If
                                    Next
                                End If
                            Next
                        End If
                    Next
                End If
            Next
            UpdateLog(1, "BackgrounderWorker2_ProgressChanged", "22", 0)
            Dim dec As Decimal = Val(CurVal(0)) / 4
            Dim tmpstring As String = dec.ToString("G")
            LBBGStatus.Text = "Waiting 15 seconds to check Delivery Status  have been waiting " + tmpstring + " minutes"
        ElseIf e.ProgressPercentage = 24 Then
            For j = 0 To DistCount
                If IsDistName(j, 3) <> vbNullString Then

                    For k = 0 To TreeView1.Nodes(0).Nodes.Count - 1
                        If IsDistName(j, 0) = TreeView1.Nodes(0).Nodes(k).Tag Then
                            If IsDistName(j, 3) = "EXECUTION_OK" Then
                                TreeView1.Nodes(0).Nodes(k).Text = TreeView1.Nodes(0).Nodes(k).Tag + " **(COMPLETE)**"
                                TreeView1.Nodes(0).Nodes(k).ForeColor = Color.ForestGreen
                                Exit For
                            Else
                                TreeView1.Nodes(0).Nodes(k).Text = TreeView1.Nodes(0).Nodes(k).Tag + " **(UPDATE JOB FAILED)**"
                                TreeView1.Nodes(0).Nodes(k).ForeColor = Color.DarkRed
                                TreeView1.Nodes(0).ForeColor = Color.DarkRed
                            End If
                        End If
                    Next
                End If
            Next
                For j = 0 To TotalSS
                If sslist(j, 2) = "EXECUTION_ERROR" Or sslist(j, 2) = "EXECUTION_OK" Then
                    For k = 0 To DistCount
                        If sslist(j, 1) = IsDistName(k, 0) Then
                            For L = 0 To TreeView1.Nodes(0).Nodes.Count - 1
                                If TreeView1.Nodes(0).Nodes(L).Tag = sslist(j, 1) Then
                                    If TreeView1.Nodes(0).Nodes(L).Nodes.Count > 0 Then
                                        For M = 0 To TreeView1.Nodes(0).Nodes(L).Nodes.Count - 1
                                            If TreeView1.Nodes(0).Nodes(L).Nodes(M).Tag = sslist(j, 0) Then
                                                If sslist(j, 2) = "EXECUTION_ERROR" Then
                                                    TreeView1.Nodes(0).Nodes(L).Nodes(M).Text = TreeView1.Nodes(0).Nodes(L).Nodes(M).Tag + " **(UPDATE JOB FAILED)**"
                                                    TreeView1.Nodes(0).Nodes(L).Nodes(M).ForeColor = Color.DarkRed
                                                    TreeView1.Nodes(0).Nodes(L).ForeColor = Color.DarkRed
                                                    TreeView1.Nodes(0).ForeColor = Color.DarkRed
                                                Else
                                                    TreeView1.Nodes(0).Nodes(L).Nodes(M).Text = TreeView1.Nodes(0).Nodes(L).Nodes(M).Tag + " **(COMPLETE)**"
                                                    TreeView1.Nodes(0).Nodes(L).Nodes(M).ForeColor = Color.ForestGreen
                                                    Exit For
                                                    Exit For
                                                End If
                                            End If
                                        Next
                                    End If
                                End If


                            Next
                        End If
                    Next
                End If
            Next


        ElseIf e.ProgressPercentage = 25 Then
                For j = 0 To TotalSS
                    If sslist(j, 2) = vbNullString And sslist(j, 2) <> "EXECUTION_OK" Then
                        For k = 0 To DistCount
                            If sslist(j, 1) = IsDistName(k, 0) Then
                                For L = 0 To TreeView1.Nodes(0).Nodes.Count - 1
                                    If TreeView1.Nodes(0).Nodes(L).Tag = sslist(j, 1) Then
                                        If TreeView1.Nodes(0).Nodes(L).Nodes.Count > 0 Then
                                            For M = 0 To TreeView1.Nodes(0).Nodes(L).Nodes.Count - 1
                                                If TreeView1.Nodes(0).Nodes(L).Nodes(M).Tag = sslist(j, 0) Then
                                                    TreeView1.Nodes(0).Nodes(L).Nodes(M).Text = TreeView1.Nodes(0).Nodes(L).Nodes(M).Tag + " **(UPDATE JOB TIMED OUT)**"
                                                    TreeView1.Nodes(0).Nodes(L).Nodes(M).ForeColor = Color.DarkRed
                                                    TreeView1.Nodes(0).Nodes(L).ForeColor = Color.DarkRed
                                                    TreeView1.Nodes(0).ForeColor = Color.DarkRed
                                                    Exit For
                                                    Exit For

                                                End If
                                            Next
                                        End If
                                    End If


                                Next
                            End If
                        Next
                    End If
                Next


            ElseIf e.ProgressPercentage = 91 Then
                TreeView1.Nodes(0).ForeColor = Color.DarkRed
                UpdateLog(1, "BackgrounderWorker2_ProgressChanged ", "91 Changing color to red on em node tag=" + TreeView1.Nodes(0).Tag, 0)
                For j = 0 To DistCount
                    For k = 0 To TreeView1.Nodes(0).Nodes.Count - 1
                        TreeView1.Nodes(0).Nodes(k).ForeColor = Color.DarkRed
                        TreeView1.Nodes(0).Nodes(k).Text = TreeView1.Nodes(0).Nodes(k).Tag + " **(DISTUBUTION FAILED)**"
                        UpdateLog(1, "BackgrounderWorker2_ProgressChanged ", "91 Changing status to distribute failed on DM tag=" + TreeView1.Nodes(0).Nodes(k).Tag, 0)
                        If TreeView1.Nodes(0).Nodes(k).Nodes.Count > 0 Then
                            For L = 0 To TreeView1.Nodes(0).Nodes(k).Nodes.Count - 1
                                TreeView1.Nodes(0).Nodes(k).Nodes(L).ForeColor = Color.DarkRed
                                TreeView1.Nodes(0).Nodes(k).Text = TreeView1.Nodes(0).Nodes(k).Nodes(L).Tag + " **(DISTUBUTION FAILED)**"
                                UpdateLog(1, "BackgrounderWorker2_ProgressChanged ", "91 Changing status to distribute failed on ss tag=" + TreeView1.Nodes(0).Nodes(k).Nodes(L).Tag, 0)
                            Next
                        End If
                    Next
                Next

            ElseIf e.ProgressPercentage = 93 Then

                Dim donegoodss As Boolean = True
                Dim donegooddm As Boolean = True
                UpdateLog(1, "BackgrounderWorker2_ProgressChanged ", "93 done color check", 0)
            For j = 0 To TreeView1.Nodes(0).Nodes.Count - 1
                If TreeView1.Nodes(0).Nodes(j).Nodes.Count > 0 Then

                    For k = 0 To TreeView1.Nodes(0).Nodes(j).Nodes.Count - 1
                        donegoodss = True
                        If TreeView1.Nodes(0).Nodes(j).Nodes(k).ForeColor <> Color.ForestGreen Then
                            UpdateLog(1, "BackgrounderWorker2_ProgressChanged ", "93 setting ss color check=false", 0)
                            donegoodss = False
                            Exit For
                        End If
                    Next

                End If
                UpdateLog(1, "BackgrounderWorker2_ProgressChanged ", "93 ss check done status=" + donegoodss.ToString, 0)
                If donegoodss = True And TreeView1.Nodes(0).Nodes(j).ForeColor <> Color.DarkRed Then
                    UpdateLog(1, "BackgrounderWorker2_ProgressChanged ", "93 setting ss color to green on dm " + TreeView1.Nodes(0).Nodes(j).Tag, 0)
                    TreeView1.Nodes(0).Nodes(j).ForeColor = Color.ForestGreen
                End If
            Next
            For j = 0 To TreeView1.Nodes(0).Nodes.Count - 1
                If TreeView1.Nodes(0).Nodes(j).ForeColor <> Color.ForestGreen Then
                    UpdateLog(1, "BackgrounderWorker2_ProgressChanged ", "93 setting dm color check=false", 0)
                    donegooddm = False
                    Exit For
                End If
            Next
            UpdateLog(1, "BackgrounderWorker2_ProgressChanged ", "93 dm check done status=" + donegooddm.ToString, 0)
            If donegooddm = True And TreeView1.Nodes(0).ForeColor <> Color.DarkRed Then
                TreeView1.Nodes(0).ForeColor = Color.ForestGreen
            End If

        ElseIf e.ProgressPercentage = 99 Then
                LBBGStatus.Tag = "NOTOK"





        End If




    End Sub

    ' This event handler deals with the results of the background operation.
    Private Sub BackgroundWorker2_RunWorkerCompleted(ByVal sender As System.Object,
    ByVal e As RunWorkerCompletedEventArgs) Handles BackgroundWorker2.RunWorkerCompleted
        UpdateLog(1, "BackgrounderWorker2_completed ", "exiting background", 0)
        PictureBox1.Visible = False
        If FinalStat <> "" Then
            LBBGStatus.Text = "UPDATE ***PARTIALLY FAILED***"
            lbfinalstat.Visible = True
            lbfinalstat.Text = FinalStat
        Else
            LBBGStatus.Text = "UPDATE COMPLETED"
        End If


        Buexport.Visible = False
        buUnseal.Visible = False
        buAnother.Visible = True
        BUexit.Visible = True
        burevalidate.Visible = True
    End Sub
End Class

