<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Startup
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.GBM = New System.Windows.Forms.GroupBox()
        Me.TBHost = New System.Windows.Forms.TextBox()
        Me.CheckBoxLocal = New System.Windows.Forms.CheckBox()
        Me.Bukeep = New System.Windows.Forms.Button()
        Me.checkEM = New System.Windows.Forms.CheckBox()
        Me.LBCAvalidation = New System.Windows.Forms.Label()
        Me.tbpassword = New System.Windows.Forms.TextBox()
        Me.tbemuser = New System.Windows.Forms.TextBox()
        Me.BUValidate = New System.Windows.Forms.Button()
        Me.tbdomain = New System.Windows.Forms.TextBox()
        Me.TBCAPassword = New System.Windows.Forms.Label()
        Me.TBCAUser = New System.Windows.Forms.Label()
        Me.TBCAdomain = New System.Windows.Forms.Label()
        Me.LBHost = New System.Windows.Forms.Label()
        Me.BUexit = New System.Windows.Forms.Button()
        Me.Lbvalidated = New System.Windows.Forms.Label()
        Me.burevalidate = New System.Windows.Forms.Button()
        Me.Lbvalidated1 = New System.Windows.Forms.Label()
        Me.ButtonGetCandidates = New System.Windows.Forms.Button()
        Me.TextBoxHours = New System.Windows.Forms.TextBox()
        Me.LabelOSIMHours = New System.Windows.Forms.Label()
        Me.ButtonExecute = New System.Windows.Forms.Button()
        Me.ListBoxCandidates = New System.Windows.Forms.ListBox()
        Me.LabelCandidates = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.GBM.SuspendLayout()
        Me.SuspendLayout()
        '
        'GBM
        '
        Me.GBM.Controls.Add(Me.TBHost)
        Me.GBM.Controls.Add(Me.CheckBoxLocal)
        Me.GBM.Controls.Add(Me.Bukeep)
        Me.GBM.Controls.Add(Me.checkEM)
        Me.GBM.Controls.Add(Me.LBCAvalidation)
        Me.GBM.Controls.Add(Me.tbpassword)
        Me.GBM.Controls.Add(Me.tbemuser)
        Me.GBM.Controls.Add(Me.BUValidate)
        Me.GBM.Controls.Add(Me.tbdomain)
        Me.GBM.Controls.Add(Me.TBCAPassword)
        Me.GBM.Controls.Add(Me.TBCAUser)
        Me.GBM.Controls.Add(Me.TBCAdomain)
        Me.GBM.Controls.Add(Me.LBHost)
        Me.GBM.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GBM.Location = New System.Drawing.Point(24, 59)
        Me.GBM.Name = "GBM"
        Me.GBM.Size = New System.Drawing.Size(438, 228)
        Me.GBM.TabIndex = 0
        Me.GBM.TabStop = False
        Me.GBM.Text = "Client Auto Enterprise Credentials"
        '
        'TBHost
        '
        Me.TBHost.Location = New System.Drawing.Point(178, 17)
        Me.TBHost.Name = "TBHost"
        Me.TBHost.Size = New System.Drawing.Size(237, 20)
        Me.TBHost.TabIndex = 12
        '
        'CheckBoxLocal
        '
        Me.CheckBoxLocal.AutoSize = True
        Me.CheckBoxLocal.Location = New System.Drawing.Point(178, 124)
        Me.CheckBoxLocal.Name = "CheckBoxLocal"
        Me.CheckBoxLocal.Size = New System.Drawing.Size(150, 17)
        Me.CheckBoxLocal.TabIndex = 11
        Me.CheckBoxLocal.Text = "Use Local Credentials"
        Me.CheckBoxLocal.UseVisualStyleBackColor = True
        '
        'Bukeep
        '
        Me.Bukeep.Location = New System.Drawing.Point(209, 168)
        Me.Bukeep.Name = "Bukeep"
        Me.Bukeep.Size = New System.Drawing.Size(93, 48)
        Me.Bukeep.TabIndex = 10
        Me.Bukeep.Text = "Keep Current Credentials"
        Me.Bukeep.UseVisualStyleBackColor = True
        Me.Bukeep.Visible = False
        '
        'checkEM
        '
        Me.checkEM.AutoSize = True
        Me.checkEM.Location = New System.Drawing.Point(178, 148)
        Me.checkEM.Name = "checkEM"
        Me.checkEM.Size = New System.Drawing.Size(105, 17)
        Me.checkEM.TabIndex = 9
        Me.checkEM.Text = "Save Settings"
        Me.checkEM.UseVisualStyleBackColor = True
        '
        'LBCAvalidation
        '
        Me.LBCAvalidation.AutoSize = True
        Me.LBCAvalidation.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LBCAvalidation.Location = New System.Drawing.Point(19, 168)
        Me.LBCAvalidation.Name = "LBCAvalidation"
        Me.LBCAvalidation.Size = New System.Drawing.Size(184, 13)
        Me.LBCAvalidation.TabIndex = 8
        Me.LBCAvalidation.Text = "Client Auto Credentials Not Evaluated"
        '
        'tbpassword
        '
        Me.tbpassword.Location = New System.Drawing.Point(178, 72)
        Me.tbpassword.Name = "tbpassword"
        Me.tbpassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.tbpassword.Size = New System.Drawing.Size(237, 20)
        Me.tbpassword.TabIndex = 7
        '
        'tbemuser
        '
        Me.tbemuser.Location = New System.Drawing.Point(178, 46)
        Me.tbemuser.Name = "tbemuser"
        Me.tbemuser.Size = New System.Drawing.Size(237, 20)
        Me.tbemuser.TabIndex = 6
        '
        'BUValidate
        '
        Me.BUValidate.Location = New System.Drawing.Point(308, 168)
        Me.BUValidate.Name = "BUValidate"
        Me.BUValidate.Size = New System.Drawing.Size(93, 48)
        Me.BUValidate.TabIndex = 2
        Me.BUValidate.Text = "Validate Credentials"
        Me.BUValidate.UseVisualStyleBackColor = True
        Me.BUValidate.Visible = False
        '
        'tbdomain
        '
        Me.tbdomain.Location = New System.Drawing.Point(178, 98)
        Me.tbdomain.Name = "tbdomain"
        Me.tbdomain.Size = New System.Drawing.Size(237, 20)
        Me.tbdomain.TabIndex = 5
        '
        'TBCAPassword
        '
        Me.TBCAPassword.AutoSize = True
        Me.TBCAPassword.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TBCAPassword.Location = New System.Drawing.Point(19, 75)
        Me.TBCAPassword.Name = "TBCAPassword"
        Me.TBCAPassword.Size = New System.Drawing.Size(82, 13)
        Me.TBCAPassword.TabIndex = 3
        Me.TBCAPassword.Text = "Login Password"
        '
        'TBCAUser
        '
        Me.TBCAUser.AutoSize = True
        Me.TBCAUser.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TBCAUser.Location = New System.Drawing.Point(19, 49)
        Me.TBCAUser.Name = "TBCAUser"
        Me.TBCAUser.Size = New System.Drawing.Size(58, 13)
        Me.TBCAUser.TabIndex = 2
        Me.TBCAUser.Text = "Login User"
        '
        'TBCAdomain
        '
        Me.TBCAdomain.AutoSize = True
        Me.TBCAdomain.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TBCAdomain.Location = New System.Drawing.Point(19, 101)
        Me.TBCAdomain.Name = "TBCAdomain"
        Me.TBCAdomain.Size = New System.Drawing.Size(103, 13)
        Me.TBCAdomain.TabIndex = 1
        Me.TBCAdomain.Text = "Login Domain Name"
        '
        'LBHost
        '
        Me.LBHost.AutoSize = True
        Me.LBHost.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LBHost.Location = New System.Drawing.Point(7, 20)
        Me.LBHost.Name = "LBHost"
        Me.LBHost.Size = New System.Drawing.Size(114, 13)
        Me.LBHost.TabIndex = 0
        Me.LBHost.Text = "Client Auto Host Name"
        '
        'BUexit
        '
        Me.BUexit.Location = New System.Drawing.Point(373, 528)
        Me.BUexit.Name = "BUexit"
        Me.BUexit.Size = New System.Drawing.Size(93, 48)
        Me.BUexit.TabIndex = 3
        Me.BUexit.Text = "EXIT"
        Me.BUexit.UseVisualStyleBackColor = True
        '
        'Lbvalidated
        '
        Me.Lbvalidated.AutoSize = True
        Me.Lbvalidated.Cursor = System.Windows.Forms.Cursors.Cross
        Me.Lbvalidated.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Lbvalidated.Location = New System.Drawing.Point(31, 13)
        Me.Lbvalidated.Name = "Lbvalidated"
        Me.Lbvalidated.Size = New System.Drawing.Size(218, 13)
        Me.Lbvalidated.TabIndex = 13
        Me.Lbvalidated.Text = "Connection validated to Client Auto Manager"
        Me.Lbvalidated.Visible = False
        '
        'burevalidate
        '
        Me.burevalidate.Location = New System.Drawing.Point(367, 13)
        Me.burevalidate.Name = "burevalidate"
        Me.burevalidate.Size = New System.Drawing.Size(84, 43)
        Me.burevalidate.TabIndex = 14
        Me.burevalidate.Text = "Enter New Credentials"
        Me.burevalidate.UseVisualStyleBackColor = True
        Me.burevalidate.Visible = False
        '
        'Lbvalidated1
        '
        Me.Lbvalidated1.AutoSize = True
        Me.Lbvalidated1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Lbvalidated1.Location = New System.Drawing.Point(31, 28)
        Me.Lbvalidated1.Name = "Lbvalidated1"
        Me.Lbvalidated1.Size = New System.Drawing.Size(0, 13)
        Me.Lbvalidated1.TabIndex = 15
        Me.Lbvalidated1.Visible = False
        '
        'ButtonGetCandidates
        '
        Me.ButtonGetCandidates.Location = New System.Drawing.Point(38, 355)
        Me.ButtonGetCandidates.Name = "ButtonGetCandidates"
        Me.ButtonGetCandidates.Size = New System.Drawing.Size(87, 48)
        Me.ButtonGetCandidates.TabIndex = 33
        Me.ButtonGetCandidates.Text = "Get OSIM Job Delete Candidates"
        Me.ButtonGetCandidates.UseVisualStyleBackColor = True
        Me.ButtonGetCandidates.Visible = False
        '
        'TextBoxHours
        '
        Me.TextBoxHours.Location = New System.Drawing.Point(48, 329)
        Me.TextBoxHours.Name = "TextBoxHours"
        Me.TextBoxHours.Size = New System.Drawing.Size(66, 20)
        Me.TextBoxHours.TabIndex = 34
        Me.TextBoxHours.Visible = False
        '
        'LabelOSIMHours
        '
        Me.LabelOSIMHours.AutoSize = True
        Me.LabelOSIMHours.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelOSIMHours.Location = New System.Drawing.Point(45, 313)
        Me.LabelOSIMHours.Name = "LabelOSIMHours"
        Me.LabelOSIMHours.Size = New System.Drawing.Size(139, 13)
        Me.LabelOSIMHours.TabIndex = 35
        Me.LabelOSIMHours.Text = "Max OSIM Job Age in hours"
        Me.LabelOSIMHours.Visible = False
        '
        'ButtonExecute
        '
        Me.ButtonExecute.Location = New System.Drawing.Point(134, 355)
        Me.ButtonExecute.Name = "ButtonExecute"
        Me.ButtonExecute.Size = New System.Drawing.Size(87, 48)
        Me.ButtonExecute.TabIndex = 36
        Me.ButtonExecute.Text = "Delete  OSIM Jobs"
        Me.ButtonExecute.UseVisualStyleBackColor = True
        Me.ButtonExecute.Visible = False
        '
        'ListBoxCandidates
        '
        Me.ListBoxCandidates.FormattingEnabled = True
        Me.ListBoxCandidates.HorizontalScrollbar = True
        Me.ListBoxCandidates.Location = New System.Drawing.Point(233, 329)
        Me.ListBoxCandidates.Name = "ListBoxCandidates"
        Me.ListBoxCandidates.SelectionMode = System.Windows.Forms.SelectionMode.None
        Me.ListBoxCandidates.Size = New System.Drawing.Size(218, 186)
        Me.ListBoxCandidates.Sorted = True
        Me.ListBoxCandidates.TabIndex = 37
        Me.ListBoxCandidates.Visible = False
        '
        'LabelCandidates
        '
        Me.LabelCandidates.AutoSize = True
        Me.LabelCandidates.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelCandidates.Location = New System.Drawing.Point(230, 313)
        Me.LabelCandidates.Name = "LabelCandidates"
        Me.LabelCandidates.Size = New System.Drawing.Size(139, 13)
        Me.LabelCandidates.TabIndex = 38
        Me.LabelCandidates.Text = "Max OSIM Job Age in hours"
        Me.LabelCandidates.Visible = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(120, 332)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(64, 13)
        Me.Label1.TabIndex = 39
        Me.Label1.Text = "Integer Only"
        Me.Label1.Visible = False
        '
        'Startup
        '
        Me.ClientSize = New System.Drawing.Size(478, 588)
        Me.ControlBox = False
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.LabelCandidates)
        Me.Controls.Add(Me.ListBoxCandidates)
        Me.Controls.Add(Me.ButtonExecute)
        Me.Controls.Add(Me.LabelOSIMHours)
        Me.Controls.Add(Me.TextBoxHours)
        Me.Controls.Add(Me.ButtonGetCandidates)
        Me.Controls.Add(Me.Lbvalidated1)
        Me.Controls.Add(Me.burevalidate)
        Me.Controls.Add(Me.Lbvalidated)
        Me.Controls.Add(Me.BUexit)
        Me.Controls.Add(Me.GBM)
        Me.MaximizeBox = False
        Me.Name = "Startup"
        Me.ShowIcon = False
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.GBM.ResumeLayout(False)
        Me.GBM.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents GBM As GroupBox
    Friend WithEvents tbpassword As TextBox
    Friend WithEvents tbemuser As TextBox
    Friend WithEvents tbdomain As TextBox
    Friend WithEvents TBCAPassword As Label
    Friend WithEvents TBCAUser As Label
    Friend WithEvents TBCAdomain As Label
    Friend WithEvents LBHost As Label
    Friend WithEvents BUValidate As Button
    Friend WithEvents BUexit As Button
    Friend WithEvents LBCAvalidation As Label
    Friend WithEvents checkEM As CheckBox
    Friend WithEvents Lbvalidated As Label
    Friend WithEvents burevalidate As Button
    Friend WithEvents Bukeep As Button
    Friend WithEvents Lbvalidated1 As Label
    Friend WithEvents CheckBoxLocal As CheckBox
    Friend WithEvents TBHost As TextBox
    Friend WithEvents ButtonGetCandidates As Button
    Friend WithEvents TextBoxHours As TextBox
    Friend WithEvents LabelOSIMHours As Label
    Friend WithEvents ButtonExecute As Button
    Friend WithEvents ListBoxCandidates As ListBox
    Friend WithEvents LabelCandidates As Label
    Friend WithEvents Label1 As Label
End Class
