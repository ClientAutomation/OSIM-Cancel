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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Startup))
        Me.GBEM = New System.Windows.Forms.GroupBox()
        Me.TBEMHost = New System.Windows.Forms.TextBox()
        Me.CheckBoxLocal = New System.Windows.Forms.CheckBox()
        Me.Bukeep = New System.Windows.Forms.Button()
        Me.checkEM = New System.Windows.Forms.CheckBox()
        Me.LBCAvalidation = New System.Windows.Forms.Label()
        Me.tbempassword = New System.Windows.Forms.TextBox()
        Me.tbemuser = New System.Windows.Forms.TextBox()
        Me.BUValidate = New System.Windows.Forms.Button()
        Me.tbEMdomain = New System.Windows.Forms.TextBox()
        Me.TBCAPassword = New System.Windows.Forms.Label()
        Me.TBCAUser = New System.Windows.Forms.Label()
        Me.TBCAdomain = New System.Windows.Forms.Label()
        Me.LBEMHost = New System.Windows.Forms.Label()
        Me.BUexit = New System.Windows.Forms.Button()
        Me.LBAllSoftware = New System.Windows.Forms.Label()
        Me.CBAllSoftware = New System.Windows.Forms.ComboBox()
        Me.GBpackageSeal = New System.Windows.Forms.GroupBox()
        Me.buAnother = New System.Windows.Forms.Button()
        Me.Buexport = New System.Windows.Forms.Button()
        Me.buUnseal = New System.Windows.Forms.Button()
        Me.TbselectedVersion = New System.Windows.Forms.TextBox()
        Me.TBSelectedPackage = New System.Windows.Forms.TextBox()
        Me.Lbversionselectedlabel = New System.Windows.Forms.Label()
        Me.Lbpackselectlabel = New System.Windows.Forms.Label()
        Me.Lbvalidated = New System.Windows.Forms.Label()
        Me.burevalidate = New System.Windows.Forms.Button()
        Me.Lbvalidated1 = New System.Windows.Forms.Label()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.TreeView1 = New System.Windows.Forms.TreeView()
        Me.LBBGStatus = New System.Windows.Forms.Label()
        Me.BackgroundWorker1 = New System.ComponentModel.BackgroundWorker()
        Me.BackgroundWorker2 = New System.ComponentModel.BackgroundWorker()
        Me.lbfinalstat = New System.Windows.Forms.Label()
        Me.GBEM.SuspendLayout()
        Me.GBpackageSeal.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'GBEM
        '
        Me.GBEM.Controls.Add(Me.TBEMHost)
        Me.GBEM.Controls.Add(Me.CheckBoxLocal)
        Me.GBEM.Controls.Add(Me.Bukeep)
        Me.GBEM.Controls.Add(Me.checkEM)
        Me.GBEM.Controls.Add(Me.LBCAvalidation)
        Me.GBEM.Controls.Add(Me.tbempassword)
        Me.GBEM.Controls.Add(Me.tbemuser)
        Me.GBEM.Controls.Add(Me.BUValidate)
        Me.GBEM.Controls.Add(Me.tbEMdomain)
        Me.GBEM.Controls.Add(Me.TBCAPassword)
        Me.GBEM.Controls.Add(Me.TBCAUser)
        Me.GBEM.Controls.Add(Me.TBCAdomain)
        Me.GBEM.Controls.Add(Me.LBEMHost)
        Me.GBEM.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GBEM.Location = New System.Drawing.Point(24, 59)
        Me.GBEM.Name = "GBEM"
        Me.GBEM.Size = New System.Drawing.Size(438, 283)
        Me.GBEM.TabIndex = 0
        Me.GBEM.TabStop = False
        Me.GBEM.Text = "Client Auto Enterprise Credentials"
        '
        'TBEMHost
        '
        Me.TBEMHost.Location = New System.Drawing.Point(178, 17)
        Me.TBEMHost.Name = "TBEMHost"
        Me.TBEMHost.Size = New System.Drawing.Size(237, 20)
        Me.TBEMHost.TabIndex = 12
        '
        'CheckBoxLocal
        '
        Me.CheckBoxLocal.AutoSize = True
        Me.CheckBoxLocal.Location = New System.Drawing.Point(178, 136)
        Me.CheckBoxLocal.Name = "CheckBoxLocal"
        Me.CheckBoxLocal.Size = New System.Drawing.Size(150, 17)
        Me.CheckBoxLocal.TabIndex = 11
        Me.CheckBoxLocal.Text = "Use Local Credentials"
        Me.CheckBoxLocal.UseVisualStyleBackColor = True
        '
        'Bukeep
        '
        Me.Bukeep.Location = New System.Drawing.Point(230, 215)
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
        Me.checkEM.Location = New System.Drawing.Point(178, 166)
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
        Me.LBCAvalidation.Location = New System.Drawing.Point(28, 199)
        Me.LBCAvalidation.Name = "LBCAvalidation"
        Me.LBCAvalidation.Size = New System.Drawing.Size(184, 13)
        Me.LBCAvalidation.TabIndex = 8
        Me.LBCAvalidation.Text = "Client Auto Credentials Not Evaluated"
        '
        'tbempassword
        '
        Me.tbempassword.Location = New System.Drawing.Point(178, 72)
        Me.tbempassword.Name = "tbempassword"
        Me.tbempassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.tbempassword.Size = New System.Drawing.Size(237, 20)
        Me.tbempassword.TabIndex = 7
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
        Me.BUValidate.Location = New System.Drawing.Point(329, 215)
        Me.BUValidate.Name = "BUValidate"
        Me.BUValidate.Size = New System.Drawing.Size(93, 48)
        Me.BUValidate.TabIndex = 2
        Me.BUValidate.Text = "Validate Credentials"
        Me.BUValidate.UseVisualStyleBackColor = True
        Me.BUValidate.Visible = False
        '
        'tbEMdomain
        '
        Me.tbEMdomain.Location = New System.Drawing.Point(178, 98)
        Me.tbEMdomain.Name = "tbEMdomain"
        Me.tbEMdomain.Size = New System.Drawing.Size(237, 20)
        Me.tbEMdomain.TabIndex = 5
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
        'LBEMHost
        '
        Me.LBEMHost.AutoSize = True
        Me.LBEMHost.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LBEMHost.Location = New System.Drawing.Point(7, 20)
        Me.LBEMHost.Name = "LBEMHost"
        Me.LBEMHost.Size = New System.Drawing.Size(164, 13)
        Me.LBEMHost.TabIndex = 0
        Me.LBEMHost.Text = "Client Auto Enterprise Host Name"
        '
        'BUexit
        '
        Me.BUexit.Location = New System.Drawing.Point(695, 518)
        Me.BUexit.Name = "BUexit"
        Me.BUexit.Size = New System.Drawing.Size(93, 48)
        Me.BUexit.TabIndex = 3
        Me.BUexit.Text = "EXIT"
        Me.BUexit.UseVisualStyleBackColor = True
        '
        'LBAllSoftware
        '
        Me.LBAllSoftware.AutoSize = True
        Me.LBAllSoftware.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LBAllSoftware.Location = New System.Drawing.Point(483, 49)
        Me.LBAllSoftware.Name = "LBAllSoftware"
        Me.LBAllSoftware.Size = New System.Drawing.Size(168, 13)
        Me.LBAllSoftware.TabIndex = 10
        Me.LBAllSoftware.Text = "Getting List of Software Packages"
        Me.LBAllSoftware.Visible = False
        '
        'CBAllSoftware
        '
        Me.CBAllSoftware.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append
        Me.CBAllSoftware.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.CBAllSoftware.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CBAllSoftware.FormattingEnabled = True
        Me.CBAllSoftware.Location = New System.Drawing.Point(486, 65)
        Me.CBAllSoftware.Name = "CBAllSoftware"
        Me.CBAllSoftware.Size = New System.Drawing.Size(400, 21)
        Me.CBAllSoftware.TabIndex = 11
        Me.CBAllSoftware.Visible = False
        '
        'GBpackageSeal
        '
        Me.GBpackageSeal.Controls.Add(Me.buAnother)
        Me.GBpackageSeal.Controls.Add(Me.Buexport)
        Me.GBpackageSeal.Controls.Add(Me.buUnseal)
        Me.GBpackageSeal.Controls.Add(Me.TbselectedVersion)
        Me.GBpackageSeal.Controls.Add(Me.TBSelectedPackage)
        Me.GBpackageSeal.Controls.Add(Me.Lbversionselectedlabel)
        Me.GBpackageSeal.Controls.Add(Me.Lbpackselectlabel)
        Me.GBpackageSeal.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GBpackageSeal.Location = New System.Drawing.Point(486, 96)
        Me.GBpackageSeal.Name = "GBpackageSeal"
        Me.GBpackageSeal.Size = New System.Drawing.Size(387, 166)
        Me.GBpackageSeal.TabIndex = 12
        Me.GBpackageSeal.TabStop = False
        Me.GBpackageSeal.Text = "Package Selected"
        Me.GBpackageSeal.Visible = False
        '
        'buAnother
        '
        Me.buAnother.Location = New System.Drawing.Point(74, 117)
        Me.buAnother.Name = "buAnother"
        Me.buAnother.Size = New System.Drawing.Size(93, 38)
        Me.buAnother.TabIndex = 15
        Me.buAnother.Text = "Select Another"
        Me.buAnother.UseVisualStyleBackColor = True
        Me.buAnother.Visible = False
        '
        'Buexport
        '
        Me.Buexport.Location = New System.Drawing.Point(173, 117)
        Me.Buexport.Name = "Buexport"
        Me.Buexport.Size = New System.Drawing.Size(93, 38)
        Me.Buexport.TabIndex = 14
        Me.Buexport.Text = "Export"
        Me.Buexport.UseVisualStyleBackColor = True
        Me.Buexport.Visible = False
        '
        'buUnseal
        '
        Me.buUnseal.Location = New System.Drawing.Point(273, 117)
        Me.buUnseal.Name = "buUnseal"
        Me.buUnseal.Size = New System.Drawing.Size(114, 38)
        Me.buUnseal.TabIndex = 13
        Me.buUnseal.Text = "Reseal/ Redistribute"
        Me.buUnseal.UseVisualStyleBackColor = True
        Me.buUnseal.Visible = False
        '
        'TbselectedVersion
        '
        Me.TbselectedVersion.Location = New System.Drawing.Point(15, 73)
        Me.TbselectedVersion.Name = "TbselectedVersion"
        Me.TbselectedVersion.ReadOnly = True
        Me.TbselectedVersion.Size = New System.Drawing.Size(152, 20)
        Me.TbselectedVersion.TabIndex = 9
        '
        'TBSelectedPackage
        '
        Me.TBSelectedPackage.Location = New System.Drawing.Point(15, 29)
        Me.TBSelectedPackage.Name = "TBSelectedPackage"
        Me.TBSelectedPackage.ReadOnly = True
        Me.TBSelectedPackage.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal
        Me.TBSelectedPackage.Size = New System.Drawing.Size(351, 20)
        Me.TBSelectedPackage.TabIndex = 8
        Me.TBSelectedPackage.WordWrap = False
        '
        'Lbversionselectedlabel
        '
        Me.Lbversionselectedlabel.AutoSize = True
        Me.Lbversionselectedlabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Lbversionselectedlabel.Location = New System.Drawing.Point(17, 57)
        Me.Lbversionselectedlabel.Name = "Lbversionselectedlabel"
        Me.Lbversionselectedlabel.Size = New System.Drawing.Size(42, 13)
        Me.Lbversionselectedlabel.TabIndex = 3
        Me.Lbversionselectedlabel.Text = "Version"
        '
        'Lbpackselectlabel
        '
        Me.Lbpackselectlabel.AutoSize = True
        Me.Lbpackselectlabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Lbpackselectlabel.Location = New System.Drawing.Point(17, 18)
        Me.Lbpackselectlabel.Name = "Lbpackselectlabel"
        Me.Lbpackselectlabel.Size = New System.Drawing.Size(35, 13)
        Me.Lbpackselectlabel.TabIndex = 2
        Me.Lbpackselectlabel.Text = "Name"
        '
        'Lbvalidated
        '
        Me.Lbvalidated.AutoSize = True
        Me.Lbvalidated.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Lbvalidated.Location = New System.Drawing.Point(31, 13)
        Me.Lbvalidated.Name = "Lbvalidated"
        Me.Lbvalidated.Size = New System.Drawing.Size(257, 13)
        Me.Lbvalidated.TabIndex = 13
        Me.Lbvalidated.Text = "Connection validated to Client Auto Enterprise Server"
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
        'PictureBox1
        '
        Me.PictureBox1.ErrorImage = CType(resources.GetObject("PictureBox1.ErrorImage"), System.Drawing.Image)
        Me.PictureBox1.Image = CType(resources.GetObject("PictureBox1.Image"), System.Drawing.Image)
        Me.PictureBox1.InitialImage = CType(resources.GetObject("PictureBox1.InitialImage"), System.Drawing.Image)
        Me.PictureBox1.Location = New System.Drawing.Point(287, 411)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(80, 81)
        Me.PictureBox1.TabIndex = 16
        Me.PictureBox1.TabStop = False
        Me.PictureBox1.Visible = False
        Me.PictureBox1.WaitOnLoad = True
        '
        'TreeView1
        '
        Me.TreeView1.BackColor = System.Drawing.SystemColors.ControlLight
        Me.TreeView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TreeView1.Location = New System.Drawing.Point(501, 279)
        Me.TreeView1.Name = "TreeView1"
        Me.TreeView1.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.TreeView1.Size = New System.Drawing.Size(385, 213)
        Me.TreeView1.TabIndex = 17
        Me.TreeView1.Visible = False
        '
        'LBBGStatus
        '
        Me.LBBGStatus.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LBBGStatus.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LBBGStatus.Location = New System.Drawing.Point(2, 495)
        Me.LBBGStatus.Name = "LBBGStatus"
        Me.LBBGStatus.Size = New System.Drawing.Size(677, 20)
        Me.LBBGStatus.TabIndex = 18
        Me.LBBGStatus.Text = "Background Worker Status"
        Me.LBBGStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.LBBGStatus.Visible = False
        '
        'BackgroundWorker1
        '
        '
        'BackgroundWorker2
        '
        '
        'lbfinalstat
        '
        Me.lbfinalstat.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lbfinalstat.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbfinalstat.Location = New System.Drawing.Point(12, 524)
        Me.lbfinalstat.Name = "lbfinalstat"
        Me.lbfinalstat.Size = New System.Drawing.Size(677, 74)
        Me.lbfinalstat.TabIndex = 19
        Me.lbfinalstat.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.lbfinalstat.Visible = False
        '
        'Startup
        '
        Me.ClientSize = New System.Drawing.Size(934, 607)
        Me.ControlBox = False
        Me.Controls.Add(Me.lbfinalstat)
        Me.Controls.Add(Me.LBBGStatus)
        Me.Controls.Add(Me.TreeView1)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.Lbvalidated1)
        Me.Controls.Add(Me.burevalidate)
        Me.Controls.Add(Me.Lbvalidated)
        Me.Controls.Add(Me.GBpackageSeal)
        Me.Controls.Add(Me.CBAllSoftware)
        Me.Controls.Add(Me.LBAllSoftware)
        Me.Controls.Add(Me.BUexit)
        Me.Controls.Add(Me.GBEM)
        Me.MaximizeBox = False
        Me.Name = "Startup"
        Me.ShowIcon = False
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.GBEM.ResumeLayout(False)
        Me.GBEM.PerformLayout()
        Me.GBpackageSeal.ResumeLayout(False)
        Me.GBpackageSeal.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents GBEM As GroupBox
    Friend WithEvents tbempassword As TextBox
    Friend WithEvents tbemuser As TextBox
    Friend WithEvents tbEMdomain As TextBox
    Friend WithEvents TBCAPassword As Label
    Friend WithEvents TBCAUser As Label
    Friend WithEvents TBCAdomain As Label
    Friend WithEvents LBEMHost As Label
    Friend WithEvents BUValidate As Button
    Friend WithEvents BUexit As Button
    Friend WithEvents LBCAvalidation As Label
    Friend WithEvents LBAllSoftware As Label
    Friend WithEvents CBAllSoftware As ComboBox
    Friend WithEvents checkEM As CheckBox
    Friend WithEvents GBpackageSeal As GroupBox
    Friend WithEvents buUnseal As Button
    Friend WithEvents TbselectedVersion As TextBox
    Friend WithEvents TBSelectedPackage As TextBox
    Friend WithEvents Lbversionselectedlabel As Label
    Friend WithEvents Lbpackselectlabel As Label
    Friend WithEvents Buexport As Button
    Friend WithEvents buAnother As Button
    Friend WithEvents Lbvalidated As Label
    Friend WithEvents burevalidate As Button
    Friend WithEvents Bukeep As Button
    Friend WithEvents Lbvalidated1 As Label
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents CheckBoxLocal As CheckBox
    Friend WithEvents LBBGStatus As Label
    Friend WithEvents TBEMHost As TextBox
    Friend WithEvents BackgroundWorker1 As System.ComponentModel.BackgroundWorker
    Friend WithEvents BackgroundWorker2 As System.ComponentModel.BackgroundWorker
    Private WithEvents TreeView1 As TreeView
    Friend WithEvents lbfinalstat As Label
End Class
