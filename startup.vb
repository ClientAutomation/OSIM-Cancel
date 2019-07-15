Imports System.ComponentModel
Imports System.Drawing.Color
Imports System.Drawing
Imports System.IO
Public Class Startup




    Private Sub Startup_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        LogCreate(0)
        If FirstPass = 1 Then

            If My.Settings.dd <> vbNullString Then
                Host = My.Settings.dd
            End If
            If My.Settings.CAdomain <> vbNullString Then
                Domain = My.Settings.CAdomain
            End If
            If My.Settings.CAuser <> vbNullString Then
                User = My.Settings.CAuser
            End If
            If My.Settings.CApassword <> vbNullString Then
                User = My.Settings.CAuser
            End If

            If My.Settings.UseLocal = "True" Then
                Uselocal = True
                CheckBoxLocal.Checked = True
            Else
                Uselocal = False
                CheckBoxLocal.Checked = False
            End If
        End If
        If Host <> vbNullString And User <> vbNullString And Password <> vbNullString And Domain <> vbNullString Then
            UpdateLog(1, "startup.startup_load", "credentials are prepopulated testing them now", 0)
            TBHost.Text = Host
            tbdomain.Text = Domain
            tbemuser.Text = User
            tbpassword.Text = Password
            ManagerValidate()
        End If
        FirstPass = 1



    End Sub

    Private Sub TBEMHost_TextChanged(sender As Object, e As EventArgs) Handles TBHost.TextChanged
        Host = TBHost.Text
        CheckForValidation()
    End Sub
    Private Sub TbEMdomain_TextChanged(sender As Object, e As EventArgs) Handles tbdomain.TextChanged
        Domain = tbdomain.Text
        CheckForValidation()
    End Sub

    Private Sub TBEMUser_TextChanged(sender As Object, e As EventArgs) Handles tbemuser.TextChanged
        User = tbemuser.Text
        CheckForValidation()
    End Sub

    Private Sub TBEMPassword_TextChanged(sender As Object, e As EventArgs) Handles tbpassword.TextChanged
        Password = tbpassword.Text
        CheckForValidation()
    End Sub

    Private Sub CheckForValidation()
        CredValidated = False
        Bukeep.Visible = False
        If Host <> vbNullString And User <> vbNullString And Password <> vbNullString And Domain <> vbNullString Then
            LBCAvalidation.Text = "Client Auto Credentials need validating"
            BUValidate.Visible = True
        Else
            LBCAvalidation.Text = "All 4 fields must be populated"
            BUValidate.Visible = False
        End If

    End Sub

    Private Sub ManagerValidate()
        Dim rtn As String
        Dim am As Integer = 1


        If SessionIDset = True Then
            rtn = UpdateLog(am, "startup.validate", "Existing web services session exists will log off", 0)
            rtn = WSLogout()
            If rtn = "OK" Then
                SessionIDset = False
                rtn = UpdateLog(am, "startup.validate", "log off OK", 0)
            Else
                rtn = UpdateLog(am, "startup.validate", "log off failed", 2)

            End If

        End If


        rtn = UpdateLog(am, "startup.validate", "Testing webservices connection by logging in", 0)
        rtn = WSLogin(Host)
        BUValidate.Visible = False
        If rtn <> "OK" Then
            rtn = UpdateLog(1, "startup.validate", "login failed check your settings and ensure you have rights in client auto", 1)
            LBCAvalidation.Text = "Validation Failed"
            CredValidated = False
        Else
            rtn = UpdateLog(am, "startup.validate", "log in OK", 0)
            LBCAvalidation.Text = "Credentials Validated"
            CredValidated = True
            If checkEM.Checked = True Then
                rtn = UpdateLog(1, "startup.validate", "Save credentials is enabled saving now", 0)
                My.Settings.CAdomain = tbdomain.Text
                My.Settings.dd = TBHost.Text
                My.Settings.CApassword = tbpassword.Text
                My.Settings.CAuser = tbemuser.Text
                If CheckBoxLocal.Checked = True Then
                    My.Settings.UseLocal = "True"
                Else
                    My.Settings.UseLocal = "False"

                End If
                My.Settings.Save()

            End If
            GBM.Visible = False
            Lbvalidated.Text = "Connection validated to Client Auto Server"
            Lbvalidated1.Text = Host
            Lbvalidated.Visible = True
            burevalidate.Visible = True
            Lbvalidated1.Visible = True


            am = 1
            LogCreate(am)
            rtn = UpdateLog(1, "startup.validate", "Validation Complete", 0)
            LabelOSIMHours.Visible = True
            TextBoxHours.Visible = True

        End If
    End Sub


    Private Sub BUvalidate_Click(sender As Object, e As EventArgs) Handles BUValidate.Click
        If CredValidated = False Then
            LBCAvalidation.Text = "Evaluating Client Auto Credentials"
            ManagerValidate()
        End If
    End Sub



    Private Sub BUexit_Click(sender As Object, e As EventArgs) Handles BUexit.Click
        Exitapp()
    End Sub

    Private Sub CheckEM_CheckedChanged(sender As Object, e As EventArgs) Handles checkEM.CheckedChanged

        If checkEM.Checked = False Then
            My.Settings.CAdomain = vbNullString
            My.Settings.dd = vbNullString
            My.Settings.CApassword = vbNullString
            My.Settings.CAuser = vbNullString
            My.Settings.UseLocal = vbNullString
            My.Settings.Save()

        Else
            If CredValidated = True Then

                My.Settings.CAdomain = tbdomain.Text
                My.Settings.dd = TBHost.Text
                My.Settings.CApassword = tbpassword.Text
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






    Private Sub Burevalidate_Click(sender As Object, e As EventArgs) Handles burevalidate.Click
        burevalidate.Visible = False
        Lbvalidated.Visible = False
        Lbvalidated1.Visible = False
        Bukeep.Visible = True
        BUValidate.Visible = False
        GBM.Visible = True
    End Sub

    Private Sub Bukeep_Click(sender As Object, e As EventArgs) Handles Bukeep.Click
        GBM.Visible = False
        burevalidate.Visible = True
        Lbvalidated.Visible = True
        Lbvalidated1.Visible = True
    End Sub




    Private Sub CheckBoxLocal_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxLocal.CheckedChanged
        If CredValidated = True Then
            GBM.Visible = False
            burevalidate.Visible = True
            Lbvalidated.Visible = True
        End If
        If CheckBoxLocal.Checked = True Then
            tbdomain.Visible = False
            TBCAdomain.Visible = False
            Uselocal = True
        Else
            tbdomain.Visible = True
            TBCAdomain.Visible = True
            Uselocal = False
        End If
    End Sub



    Public Sub New()
        InitializeComponent()

    End Sub




    Private Sub GBEM_Enter(sender As Object, e As EventArgs) Handles GBM.Enter

    End Sub







    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles ButtonGetCandidates.Click
        JobExpireTime = Int(TextBoxHours.Text)
        WSGetOsim()
        If OSIMJobList(0, 0) IsNot Nothing Then
            For j = 0 To OSIMJobCount
                ListBoxCandidates.Items.Add(OSIMJobList(j, 0) + " | " + OSIMJobList(j, 1))
            Next
            ButtonExecute.Visible = True
        Else
            ListBoxCandidates.Items.Add("There are no OSIM Jobs to delete")
        End If

        ListBoxCandidates.Visible = True
        ButtonGetCandidates.Visible = False
        TextBoxHours.ReadOnly = True


    End Sub

    Private Sub TextBoxHours_TextChanged(sender As Object, e As EventArgs) Handles TextBoxHours.TextChanged
        Dim testVar As Object = TextBoxHours.Text
        Dim numericCheck As Boolean
        numericCheck = IsNumeric(testVar)
        If numericCheck = True Then
            JobExpireTime = Int(numericCheck)
            ButtonGetCandidates.Visible = True
            Label1.Visible = False
        Else
            ButtonGetCandidates.Visible = False
            Label1.Visible = True
        End If
    End Sub

    Private Sub ButtonExecute_Click(sender As Object, e As EventArgs) Handles ButtonExecute.Click
        WSCancelOSInstallation()
    End Sub
End Class


