<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class HVACForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.LogoPictureBox = New System.Windows.Forms.PictureBox()
        Me.StatusStrip = New System.Windows.Forms.StatusStrip()
        Me.FaultToolStripStatusLabel = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ComsStatusToolStripStatusLabel = New System.Windows.Forms.ToolStripStatusLabel()
        Me.MachineTempLabel = New System.Windows.Forms.Label()
        Me.RoomTempLabel = New System.Windows.Forms.Label()
        Me.MachineTempRichTextBox = New System.Windows.Forms.RichTextBox()
        Me.RoomTempRichTextBox = New System.Windows.Forms.RichTextBox()
        Me.HighSetpointLabel = New System.Windows.Forms.Label()
        Me.LowSetpointLabel = New System.Windows.Forms.Label()
        Me.ComPort = New System.IO.Ports.SerialPort(Me.components)
        Me.HighSetpointRichTextBox = New System.Windows.Forms.RichTextBox()
        Me.LowSetpointRichTextBox = New System.Windows.Forms.RichTextBox()
        Me.HighSetpointUpButton = New System.Windows.Forms.Button()
        Me.HighSetpointDownButton = New System.Windows.Forms.Button()
        Me.LowSetpointUpButton = New System.Windows.Forms.Button()
        Me.LowSetpointDownButton = New System.Windows.Forms.Button()
        Me.HeatCoolOffGroupBox = New System.Windows.Forms.GroupBox()
        Me.OffRadioButton = New System.Windows.Forms.RadioButton()
        Me.CoolRadioButton = New System.Windows.Forms.RadioButton()
        Me.HeatRadioButton = New System.Windows.Forms.RadioButton()
        Me.ModeLabel = New System.Windows.Forms.Label()
        Me.FanLabel = New System.Windows.Forms.Label()
        Me.FanGroupBox = New System.Windows.Forms.GroupBox()
        Me.AutoRadioButton = New System.Windows.Forms.RadioButton()
        Me.OnRadioButton = New System.Windows.Forms.RadioButton()
        Me.ExitButton = New System.Windows.Forms.Button()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.SaveSettingsToolStripMenu = New System.Windows.Forms.ToolStripMenuItem()
        Me.SaveSettingsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExitToolStripMenu = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SendTimer = New System.Windows.Forms.Timer(Me.components)
        Me.TempTimer = New System.Windows.Forms.Timer(Me.components)
        Me.FiveSecondTimer = New System.Windows.Forms.Timer(Me.components)
        Me.TwoMinuteTimer = New System.Windows.Forms.Timer(Me.components)
        CType(Me.LogoPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.StatusStrip.SuspendLayout()
        Me.HeatCoolOffGroupBox.SuspendLayout()
        Me.FanGroupBox.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'LogoPictureBox
        '
        Me.LogoPictureBox.Image = Global.HVACSmartHomeController.My.Resources.Resources.Bengal_OrangeOutline
        Me.LogoPictureBox.Location = New System.Drawing.Point(13, 34)
        Me.LogoPictureBox.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.LogoPictureBox.Name = "LogoPictureBox"
        Me.LogoPictureBox.Size = New System.Drawing.Size(130, 94)
        Me.LogoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.LogoPictureBox.TabIndex = 0
        Me.LogoPictureBox.TabStop = False
        '
        'StatusStrip
        '
        Me.StatusStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FaultToolStripStatusLabel, Me.ComsStatusToolStripStatusLabel})
        Me.StatusStrip.Location = New System.Drawing.Point(0, 428)
        Me.StatusStrip.Name = "StatusStrip"
        Me.StatusStrip.Size = New System.Drawing.Size(933, 22)
        Me.StatusStrip.TabIndex = 12
        Me.StatusStrip.Text = "StatusStrip1"
        '
        'FaultToolStripStatusLabel
        '
        Me.FaultToolStripStatusLabel.Name = "FaultToolStripStatusLabel"
        Me.FaultToolStripStatusLabel.Size = New System.Drawing.Size(36, 17)
        Me.FaultToolStripStatusLabel.Text = "Fault:"
        '
        'ComsStatusToolStripStatusLabel
        '
        Me.ComsStatusToolStripStatusLabel.Name = "ComsStatusToolStripStatusLabel"
        Me.ComsStatusToolStripStatusLabel.Size = New System.Drawing.Size(132, 17)
        Me.ComsStatusToolStripStatusLabel.Text = "Communication Status:"
        '
        'MachineTempLabel
        '
        Me.MachineTempLabel.AutoSize = True
        Me.MachineTempLabel.Font = New System.Drawing.Font("Segoe UI", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MachineTempLabel.Location = New System.Drawing.Point(135, 131)
        Me.MachineTempLabel.Name = "MachineTempLabel"
        Me.MachineTempLabel.Size = New System.Drawing.Size(156, 30)
        Me.MachineTempLabel.TabIndex = 13
        Me.MachineTempLabel.Text = "Machine Temp"
        '
        'RoomTempLabel
        '
        Me.RoomTempLabel.AutoSize = True
        Me.RoomTempLabel.Font = New System.Drawing.Font("Segoe UI", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.RoomTempLabel.Location = New System.Drawing.Point(449, 132)
        Me.RoomTempLabel.Name = "RoomTempLabel"
        Me.RoomTempLabel.Size = New System.Drawing.Size(130, 30)
        Me.RoomTempLabel.TabIndex = 14
        Me.RoomTempLabel.Text = "Room Temp"
        '
        'MachineTempRichTextBox
        '
        Me.MachineTempRichTextBox.Font = New System.Drawing.Font("Segoe UI", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MachineTempRichTextBox.Location = New System.Drawing.Point(136, 165)
        Me.MachineTempRichTextBox.Name = "MachineTempRichTextBox"
        Me.MachineTempRichTextBox.Size = New System.Drawing.Size(151, 44)
        Me.MachineTempRichTextBox.TabIndex = 17
        Me.MachineTempRichTextBox.Text = ""
        '
        'RoomTempRichTextBox
        '
        Me.RoomTempRichTextBox.Font = New System.Drawing.Font("Segoe UI", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.RoomTempRichTextBox.Location = New System.Drawing.Point(438, 165)
        Me.RoomTempRichTextBox.Name = "RoomTempRichTextBox"
        Me.RoomTempRichTextBox.Size = New System.Drawing.Size(151, 44)
        Me.RoomTempRichTextBox.TabIndex = 18
        Me.RoomTempRichTextBox.Text = ""
        '
        'HighSetpointLabel
        '
        Me.HighSetpointLabel.AutoSize = True
        Me.HighSetpointLabel.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HighSetpointLabel.Location = New System.Drawing.Point(75, 319)
        Me.HighSetpointLabel.Name = "HighSetpointLabel"
        Me.HighSetpointLabel.Size = New System.Drawing.Size(116, 21)
        Me.HighSetpointLabel.TabIndex = 19
        Me.HighSetpointLabel.Text = "High Setpoint"
        '
        'LowSetpointLabel
        '
        Me.LowSetpointLabel.AutoSize = True
        Me.LowSetpointLabel.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LowSetpointLabel.Location = New System.Drawing.Point(438, 319)
        Me.LowSetpointLabel.Name = "LowSetpointLabel"
        Me.LowSetpointLabel.Size = New System.Drawing.Size(110, 21)
        Me.LowSetpointLabel.TabIndex = 20
        Me.LowSetpointLabel.Text = "Low Setpoint"
        '
        'ComPort
        '
        '
        'HighSetpointRichTextBox
        '
        Me.HighSetpointRichTextBox.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HighSetpointRichTextBox.Location = New System.Drawing.Point(79, 343)
        Me.HighSetpointRichTextBox.Name = "HighSetpointRichTextBox"
        Me.HighSetpointRichTextBox.Size = New System.Drawing.Size(100, 30)
        Me.HighSetpointRichTextBox.TabIndex = 21
        Me.HighSetpointRichTextBox.Text = ""
        '
        'LowSetpointRichTextBox
        '
        Me.LowSetpointRichTextBox.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LowSetpointRichTextBox.Location = New System.Drawing.Point(442, 343)
        Me.LowSetpointRichTextBox.Name = "LowSetpointRichTextBox"
        Me.LowSetpointRichTextBox.Size = New System.Drawing.Size(100, 30)
        Me.LowSetpointRichTextBox.TabIndex = 22
        Me.LowSetpointRichTextBox.Text = ""
        '
        'HighSetpointUpButton
        '
        Me.HighSetpointUpButton.Location = New System.Drawing.Point(35, 379)
        Me.HighSetpointUpButton.Name = "HighSetpointUpButton"
        Me.HighSetpointUpButton.Size = New System.Drawing.Size(75, 23)
        Me.HighSetpointUpButton.TabIndex = 23
        Me.HighSetpointUpButton.Text = "^"
        Me.HighSetpointUpButton.UseVisualStyleBackColor = True
        '
        'HighSetpointDownButton
        '
        Me.HighSetpointDownButton.Location = New System.Drawing.Point(136, 379)
        Me.HighSetpointDownButton.Name = "HighSetpointDownButton"
        Me.HighSetpointDownButton.Size = New System.Drawing.Size(75, 23)
        Me.HighSetpointDownButton.TabIndex = 24
        Me.HighSetpointDownButton.Text = "⌄"
        Me.HighSetpointDownButton.UseVisualStyleBackColor = True
        '
        'LowSetpointUpButton
        '
        Me.LowSetpointUpButton.Location = New System.Drawing.Point(413, 379)
        Me.LowSetpointUpButton.Name = "LowSetpointUpButton"
        Me.LowSetpointUpButton.Size = New System.Drawing.Size(75, 23)
        Me.LowSetpointUpButton.TabIndex = 25
        Me.LowSetpointUpButton.Text = "^"
        Me.LowSetpointUpButton.UseVisualStyleBackColor = True
        '
        'LowSetpointDownButton
        '
        Me.LowSetpointDownButton.Location = New System.Drawing.Point(504, 379)
        Me.LowSetpointDownButton.Name = "LowSetpointDownButton"
        Me.LowSetpointDownButton.Size = New System.Drawing.Size(75, 23)
        Me.LowSetpointDownButton.TabIndex = 26
        Me.LowSetpointDownButton.Text = "⌄"
        Me.LowSetpointDownButton.UseVisualStyleBackColor = True
        '
        'HeatCoolOffGroupBox
        '
        Me.HeatCoolOffGroupBox.Controls.Add(Me.OffRadioButton)
        Me.HeatCoolOffGroupBox.Controls.Add(Me.CoolRadioButton)
        Me.HeatCoolOffGroupBox.Controls.Add(Me.HeatRadioButton)
        Me.HeatCoolOffGroupBox.Location = New System.Drawing.Point(758, 75)
        Me.HeatCoolOffGroupBox.Name = "HeatCoolOffGroupBox"
        Me.HeatCoolOffGroupBox.Size = New System.Drawing.Size(89, 87)
        Me.HeatCoolOffGroupBox.TabIndex = 27
        Me.HeatCoolOffGroupBox.TabStop = False
        '
        'OffRadioButton
        '
        Me.OffRadioButton.AutoSize = True
        Me.OffRadioButton.Enabled = False
        Me.OffRadioButton.Location = New System.Drawing.Point(6, 64)
        Me.OffRadioButton.Name = "OffRadioButton"
        Me.OffRadioButton.Size = New System.Drawing.Size(41, 17)
        Me.OffRadioButton.TabIndex = 2
        Me.OffRadioButton.TabStop = True
        Me.OffRadioButton.Text = "Off"
        Me.OffRadioButton.UseVisualStyleBackColor = True
        '
        'CoolRadioButton
        '
        Me.CoolRadioButton.AutoSize = True
        Me.CoolRadioButton.Enabled = False
        Me.CoolRadioButton.Location = New System.Drawing.Point(6, 41)
        Me.CoolRadioButton.Name = "CoolRadioButton"
        Me.CoolRadioButton.Size = New System.Drawing.Size(66, 17)
        Me.CoolRadioButton.TabIndex = 1
        Me.CoolRadioButton.TabStop = True
        Me.CoolRadioButton.Text = "Cooling"
        Me.CoolRadioButton.UseVisualStyleBackColor = True
        '
        'HeatRadioButton
        '
        Me.HeatRadioButton.AutoSize = True
        Me.HeatRadioButton.Enabled = False
        Me.HeatRadioButton.Location = New System.Drawing.Point(6, 18)
        Me.HeatRadioButton.Name = "HeatRadioButton"
        Me.HeatRadioButton.Size = New System.Drawing.Size(66, 17)
        Me.HeatRadioButton.TabIndex = 0
        Me.HeatRadioButton.TabStop = True
        Me.HeatRadioButton.Text = "Heating"
        Me.HeatRadioButton.UseVisualStyleBackColor = True
        '
        'ModeLabel
        '
        Me.ModeLabel.AutoSize = True
        Me.ModeLabel.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ModeLabel.Location = New System.Drawing.Point(760, 51)
        Me.ModeLabel.Name = "ModeLabel"
        Me.ModeLabel.Size = New System.Drawing.Size(54, 21)
        Me.ModeLabel.TabIndex = 28
        Me.ModeLabel.Text = "Mode"
        '
        'FanLabel
        '
        Me.FanLabel.AutoSize = True
        Me.FanLabel.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FanLabel.Location = New System.Drawing.Point(760, 175)
        Me.FanLabel.Name = "FanLabel"
        Me.FanLabel.Size = New System.Drawing.Size(37, 21)
        Me.FanLabel.TabIndex = 29
        Me.FanLabel.Text = "Fan"
        '
        'FanGroupBox
        '
        Me.FanGroupBox.Controls.Add(Me.AutoRadioButton)
        Me.FanGroupBox.Controls.Add(Me.OnRadioButton)
        Me.FanGroupBox.Location = New System.Drawing.Point(758, 199)
        Me.FanGroupBox.Name = "FanGroupBox"
        Me.FanGroupBox.Size = New System.Drawing.Size(82, 72)
        Me.FanGroupBox.TabIndex = 30
        Me.FanGroupBox.TabStop = False
        '
        'AutoRadioButton
        '
        Me.AutoRadioButton.AutoSize = True
        Me.AutoRadioButton.Enabled = False
        Me.AutoRadioButton.Location = New System.Drawing.Point(6, 41)
        Me.AutoRadioButton.Name = "AutoRadioButton"
        Me.AutoRadioButton.Size = New System.Drawing.Size(51, 17)
        Me.AutoRadioButton.TabIndex = 1
        Me.AutoRadioButton.TabStop = True
        Me.AutoRadioButton.Text = "Auto"
        Me.AutoRadioButton.UseVisualStyleBackColor = True
        '
        'OnRadioButton
        '
        Me.OnRadioButton.AutoSize = True
        Me.OnRadioButton.Enabled = False
        Me.OnRadioButton.Location = New System.Drawing.Point(3, 18)
        Me.OnRadioButton.Name = "OnRadioButton"
        Me.OnRadioButton.Size = New System.Drawing.Size(40, 17)
        Me.OnRadioButton.TabIndex = 0
        Me.OnRadioButton.TabStop = True
        Me.OnRadioButton.Text = "On"
        Me.OnRadioButton.UseVisualStyleBackColor = True
        '
        'ExitButton
        '
        Me.ExitButton.Location = New System.Drawing.Point(846, 402)
        Me.ExitButton.Name = "ExitButton"
        Me.ExitButton.Size = New System.Drawing.Size(75, 23)
        Me.ExitButton.TabIndex = 31
        Me.ExitButton.Text = "E&xit"
        Me.ExitButton.UseVisualStyleBackColor = True
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SaveSettingsToolStripMenu, Me.ExitToolStripMenu})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(933, 24)
        Me.MenuStrip1.TabIndex = 32
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'SaveSettingsToolStripMenu
        '
        Me.SaveSettingsToolStripMenu.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SaveSettingsToolStripMenuItem})
        Me.SaveSettingsToolStripMenu.Name = "SaveSettingsToolStripMenu"
        Me.SaveSettingsToolStripMenu.Size = New System.Drawing.Size(88, 20)
        Me.SaveSettingsToolStripMenu.Text = "Save Settings"
        '
        'SaveSettingsToolStripMenuItem
        '
        Me.SaveSettingsToolStripMenuItem.Name = "SaveSettingsToolStripMenuItem"
        Me.SaveSettingsToolStripMenuItem.Size = New System.Drawing.Size(143, 22)
        Me.SaveSettingsToolStripMenuItem.Text = "Save Settings"
        '
        'ExitToolStripMenu
        '
        Me.ExitToolStripMenu.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ExitToolStripMenuItem})
        Me.ExitToolStripMenu.Name = "ExitToolStripMenu"
        Me.ExitToolStripMenu.Size = New System.Drawing.Size(37, 20)
        Me.ExitToolStripMenu.Text = "Exit"
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(92, 22)
        Me.ExitToolStripMenuItem.Text = "Exit"
        '
        'SendTimer
        '
        Me.SendTimer.Interval = 10
        '
        'TempTimer
        '
        Me.TempTimer.Interval = 30000
        '
        'FiveSecondTimer
        '
        Me.FiveSecondTimer.Interval = 5000
        '
        'TwoMinuteTimer
        '
        Me.TwoMinuteTimer.Interval = 120000
        '
        'HVACForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(933, 450)
        Me.Controls.Add(Me.ExitButton)
        Me.Controls.Add(Me.FanGroupBox)
        Me.Controls.Add(Me.FanLabel)
        Me.Controls.Add(Me.ModeLabel)
        Me.Controls.Add(Me.HeatCoolOffGroupBox)
        Me.Controls.Add(Me.LowSetpointDownButton)
        Me.Controls.Add(Me.LowSetpointUpButton)
        Me.Controls.Add(Me.HighSetpointDownButton)
        Me.Controls.Add(Me.HighSetpointUpButton)
        Me.Controls.Add(Me.LowSetpointRichTextBox)
        Me.Controls.Add(Me.HighSetpointRichTextBox)
        Me.Controls.Add(Me.LowSetpointLabel)
        Me.Controls.Add(Me.HighSetpointLabel)
        Me.Controls.Add(Me.RoomTempRichTextBox)
        Me.Controls.Add(Me.MachineTempRichTextBox)
        Me.Controls.Add(Me.RoomTempLabel)
        Me.Controls.Add(Me.MachineTempLabel)
        Me.Controls.Add(Me.StatusStrip)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.LogoPictureBox)
        Me.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.Name = "HVACForm"
        Me.Text = "Form1"
        CType(Me.LogoPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        Me.StatusStrip.ResumeLayout(False)
        Me.StatusStrip.PerformLayout()
        Me.HeatCoolOffGroupBox.ResumeLayout(False)
        Me.HeatCoolOffGroupBox.PerformLayout()
        Me.FanGroupBox.ResumeLayout(False)
        Me.FanGroupBox.PerformLayout()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents LogoPictureBox As PictureBox
    Friend WithEvents StatusStrip As StatusStrip
    Friend WithEvents FaultToolStripStatusLabel As ToolStripStatusLabel
    Friend WithEvents ComsStatusToolStripStatusLabel As ToolStripStatusLabel
    Friend WithEvents MachineTempLabel As Label
    Friend WithEvents RoomTempLabel As Label
    Friend WithEvents MachineTempRichTextBox As RichTextBox
    Friend WithEvents RoomTempRichTextBox As RichTextBox
    Friend WithEvents HighSetpointLabel As Label
    Friend WithEvents LowSetpointLabel As Label
    Friend WithEvents ComPort As IO.Ports.SerialPort
    Friend WithEvents HighSetpointRichTextBox As RichTextBox
    Friend WithEvents LowSetpointRichTextBox As RichTextBox
    Friend WithEvents HighSetpointUpButton As Button
    Friend WithEvents HighSetpointDownButton As Button
    Friend WithEvents LowSetpointUpButton As Button
    Friend WithEvents LowSetpointDownButton As Button
    Friend WithEvents HeatCoolOffGroupBox As GroupBox
    Friend WithEvents OffRadioButton As RadioButton
    Friend WithEvents CoolRadioButton As RadioButton
    Friend WithEvents HeatRadioButton As RadioButton
    Friend WithEvents ModeLabel As Label
    Friend WithEvents FanLabel As Label
    Friend WithEvents FanGroupBox As GroupBox
    Friend WithEvents AutoRadioButton As RadioButton
    Friend WithEvents OnRadioButton As RadioButton
    Friend WithEvents ExitButton As Button
    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents SaveSettingsToolStripMenu As ToolStripMenuItem
    Friend WithEvents ExitToolStripMenu As ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SaveSettingsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SendTimer As Timer
    Friend WithEvents TempTimer As Timer
    Friend WithEvents FiveSecondTimer As Timer
    Friend WithEvents TwoMinuteTimer As Timer
End Class
