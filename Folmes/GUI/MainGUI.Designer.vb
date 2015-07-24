Imports Folmes.GUI

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MainGUI
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainGUI))
        Me.Button2 = New System.Windows.Forms.Button()
        Me.InputBGPanel = New System.Windows.Forms.Panel()
        Me.InputPaddingPanel = New System.Windows.Forms.Panel()
        Me.InputContMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.CutBtn = New System.Windows.Forms.ToolStripMenuItem()
        Me.CopyBtn = New System.Windows.Forms.ToolStripMenuItem()
        Me.PasteBtn = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.SelectAllBtn = New System.Windows.Forms.ToolStripMenuItem()
        Me.OutputContMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.CopyO = New System.Windows.Forms.ToolStripMenuItem()
        Me.TrayIcon = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.TrayIconMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.CMSettings = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.CMOpenFolder = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.CMExit = New System.Windows.Forms.ToolStripMenuItem()
        Me.Output = New Folmes.GUI.MessagesDisplay()
        Me.TS = New Folmes.NFToolStrip()
        Me.TSTools = New System.Windows.Forms.ToolStripDropDownButton()
        Me.TSSettings = New System.Windows.Forms.ToolStripMenuItem()
        Me.TSCleaner = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripSeparator()
        Me.TSHelp = New System.Windows.Forms.ToolStripMenuItem()
        Me.TSAbout = New System.Windows.Forms.ToolStripMenuItem()
        Me.TSChannels = New System.Windows.Forms.ToolStripDropDownButton()
        Me.PublicChannel = New System.Windows.Forms.ToolStripMenuItem()
        Me.PubPrivChSeparator = New System.Windows.Forms.ToolStripSeparator()
        Me.TSChat = New System.Windows.Forms.ToolStripButton()
        Me.Input = New Folmes.GUI.InputTextBox()
        Me.InputBGPanel.SuspendLayout()
        Me.InputPaddingPanel.SuspendLayout()
        Me.InputContMenu.SuspendLayout()
        Me.OutputContMenu.SuspendLayout()
        Me.TrayIconMenu.SuspendLayout()
        Me.TS.SuspendLayout()
        Me.SuspendLayout()
        '
        'Button2
        '
        Me.Button2.Dock = System.Windows.Forms.DockStyle.Right
        Me.Button2.FlatAppearance.BorderColor = System.Drawing.Color.Black
        Me.Button2.FlatAppearance.BorderSize = 0
        Me.Button2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(48, Byte), Integer), CType(CType(48, Byte), Integer), CType(CType(48, Byte), Integer))
        Me.Button2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer))
        Me.Button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Button2.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!)
        Me.Button2.ForeColor = System.Drawing.Color.DarkGray
        Me.Button2.ImageKey = "(none)"
        Me.Button2.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Button2.Location = New System.Drawing.Point(358, 0)
        Me.Button2.Margin = New System.Windows.Forms.Padding(0)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(16, 38)
        Me.Button2.TabIndex = 4
        Me.Button2.Text = "*"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'InputBGPanel
        '
        Me.InputBGPanel.BackColor = System.Drawing.Color.FromArgb(CType(CType(24, Byte), Integer), CType(CType(24, Byte), Integer), CType(CType(24, Byte), Integer))
        Me.InputBGPanel.Controls.Add(Me.InputPaddingPanel)
        Me.InputBGPanel.Controls.Add(Me.Button2)
        Me.InputBGPanel.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.InputBGPanel.Location = New System.Drawing.Point(0, 224)
        Me.InputBGPanel.Margin = New System.Windows.Forms.Padding(4, 0, 4, 4)
        Me.InputBGPanel.Name = "InputBGPanel"
        Me.InputBGPanel.Size = New System.Drawing.Size(374, 38)
        Me.InputBGPanel.TabIndex = 1
        '
        'InputPaddingPanel
        '
        Me.InputPaddingPanel.Controls.Add(Me.Input)
        Me.InputPaddingPanel.Cursor = System.Windows.Forms.Cursors.WaitCursor
        Me.InputPaddingPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.InputPaddingPanel.Location = New System.Drawing.Point(0, 0)
        Me.InputPaddingPanel.Name = "InputPaddingPanel"
        Me.InputPaddingPanel.Padding = New System.Windows.Forms.Padding(3, 5, 0, 0)
        Me.InputPaddingPanel.Size = New System.Drawing.Size(358, 38)
        Me.InputPaddingPanel.TabIndex = 5
        '
        'InputContMenu
        '
        Me.InputContMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CutBtn, Me.CopyBtn, Me.PasteBtn, Me.ToolStripMenuItem1, Me.SelectAllBtn})
        Me.InputContMenu.Name = "TextContMenu"
        Me.InputContMenu.Size = New System.Drawing.Size(121, 98)
        '
        'CutBtn
        '
        Me.CutBtn.ForeColor = System.Drawing.Color.FromArgb(CType(CType(176, Byte), Integer), CType(CType(176, Byte), Integer), CType(CType(176, Byte), Integer))
        Me.CutBtn.Name = "CutBtn"
        Me.CutBtn.Size = New System.Drawing.Size(120, 22)
        Me.CutBtn.Text = "Cut"
        '
        'CopyBtn
        '
        Me.CopyBtn.ForeColor = System.Drawing.Color.FromArgb(CType(CType(176, Byte), Integer), CType(CType(176, Byte), Integer), CType(CType(176, Byte), Integer))
        Me.CopyBtn.Name = "CopyBtn"
        Me.CopyBtn.Size = New System.Drawing.Size(120, 22)
        Me.CopyBtn.Text = "Copy"
        '
        'PasteBtn
        '
        Me.PasteBtn.ForeColor = System.Drawing.Color.FromArgb(CType(CType(176, Byte), Integer), CType(CType(176, Byte), Integer), CType(CType(176, Byte), Integer))
        Me.PasteBtn.Name = "PasteBtn"
        Me.PasteBtn.Size = New System.Drawing.Size(120, 22)
        Me.PasteBtn.Text = "Paste"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(117, 6)
        '
        'SelectAllBtn
        '
        Me.SelectAllBtn.ForeColor = System.Drawing.Color.FromArgb(CType(CType(176, Byte), Integer), CType(CType(176, Byte), Integer), CType(CType(176, Byte), Integer))
        Me.SelectAllBtn.Name = "SelectAllBtn"
        Me.SelectAllBtn.Size = New System.Drawing.Size(120, 22)
        Me.SelectAllBtn.Text = "Select all"
        '
        'OutputContMenu
        '
        Me.OutputContMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CopyO})
        Me.OutputContMenu.Name = "TextContMenu"
        Me.OutputContMenu.Size = New System.Drawing.Size(103, 26)
        '
        'CopyO
        '
        Me.CopyO.ForeColor = System.Drawing.Color.FromArgb(CType(CType(176, Byte), Integer), CType(CType(176, Byte), Integer), CType(CType(176, Byte), Integer))
        Me.CopyO.Name = "CopyO"
        Me.CopyO.Size = New System.Drawing.Size(102, 22)
        Me.CopyO.Text = "Copy"
        '
        'TrayIcon
        '
        Me.TrayIcon.ContextMenuStrip = Me.TrayIconMenu
        Me.TrayIcon.Text = "Folmes"
        Me.TrayIcon.Visible = True
        '
        'TrayIconMenu
        '
        Me.TrayIconMenu.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.TrayIconMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CMSettings, Me.ToolStripSeparator1, Me.CMOpenFolder, Me.ToolStripSeparator2, Me.CMExit})
        Me.TrayIconMenu.Name = "ContextMenuStrip1"
        Me.TrayIconMenu.Size = New System.Drawing.Size(176, 82)
        '
        'CMSettings
        '
        Me.CMSettings.Name = "CMSettings"
        Me.CMSettings.Size = New System.Drawing.Size(175, 22)
        Me.CMSettings.Text = "Settings"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(172, 6)
        '
        'CMOpenFolder
        '
        Me.CMOpenFolder.Name = "CMOpenFolder"
        Me.CMOpenFolder.Size = New System.Drawing.Size(175, 22)
        Me.CMOpenFolder.Text = "Open shared folder"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(172, 6)
        '
        'CMExit
        '
        Me.CMExit.Name = "CMExit"
        Me.CMExit.Size = New System.Drawing.Size(175, 22)
        Me.CMExit.Text = "Exit"
        '
        'Output
        '
        Me.Output.AllowNavigation = False
        Me.Output.AllowWebBrowserDrop = False
        Me.Output.CausesValidation = False
        Me.Output.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Output.IsWebBrowserContextMenuEnabled = False
        Me.Output.Location = New System.Drawing.Point(0, 24)
        Me.Output.Margin = New System.Windows.Forms.Padding(0)
        Me.Output.MinimumSize = New System.Drawing.Size(20, 20)
        Me.Output.Name = "Output"
        Me.Output.ScrollBarsEnabled = False
        Me.Output.Size = New System.Drawing.Size(374, 200)
        Me.Output.TabIndex = 12
        '
        'TS
        '
        Me.TS.AutoSize = False
        Me.TS.BackColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.TS.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel)
        Me.TS.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.TS.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.TSTools, Me.TSChannels, Me.TSChat})
        Me.TS.Location = New System.Drawing.Point(0, 0)
        Me.TS.Name = "TS"
        Me.TS.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
        Me.TS.Size = New System.Drawing.Size(374, 24)
        Me.TS.TabIndex = 11
        '
        'TSTools
        '
        Me.TSTools.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.TSTools.AutoToolTip = False
        Me.TSTools.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.TSTools.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.TSSettings, Me.TSCleaner, Me.ToolStripMenuItem2, Me.TSHelp, Me.TSAbout})
        Me.TSTools.ForeColor = System.Drawing.Color.DarkGray
        Me.TSTools.Image = Global.Folmes.My.Resources.Resources.menu
        Me.TSTools.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.TSTools.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.TSTools.Margin = New System.Windows.Forms.Padding(0)
        Me.TSTools.Name = "TSTools"
        Me.TSTools.Padding = New System.Windows.Forms.Padding(1)
        Me.TSTools.ShowDropDownArrow = False
        Me.TSTools.Size = New System.Drawing.Size(24, 24)
        Me.TSTools.Text = "Tools"
        '
        'TSSettings
        '
        Me.TSSettings.ForeColor = System.Drawing.Color.DarkGray
        Me.TSSettings.Name = "TSSettings"
        Me.TSSettings.Size = New System.Drawing.Size(116, 22)
        Me.TSSettings.Text = "Settings"
        '
        'TSCleaner
        '
        Me.TSCleaner.ForeColor = System.Drawing.Color.DarkGray
        Me.TSCleaner.Name = "TSCleaner"
        Me.TSCleaner.Size = New System.Drawing.Size(116, 22)
        Me.TSCleaner.Text = "Cleaner"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(113, 6)
        '
        'TSHelp
        '
        Me.TSHelp.ForeColor = System.Drawing.Color.DarkGray
        Me.TSHelp.Name = "TSHelp"
        Me.TSHelp.Size = New System.Drawing.Size(116, 22)
        Me.TSHelp.Text = "Help"
        '
        'TSAbout
        '
        Me.TSAbout.ForeColor = System.Drawing.Color.DarkGray
        Me.TSAbout.Name = "TSAbout"
        Me.TSAbout.Size = New System.Drawing.Size(116, 22)
        Me.TSAbout.Text = "About"
        '
        'TSChannels
        '
        Me.TSChannels.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.TSChannels.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.PublicChannel, Me.PubPrivChSeparator})
        Me.TSChannels.ForeColor = System.Drawing.Color.FromArgb(CType(CType(176, Byte), Integer), CType(CType(176, Byte), Integer), CType(CType(176, Byte), Integer))
        Me.TSChannels.Image = CType(resources.GetObject("TSChannels.Image"), System.Drawing.Image)
        Me.TSChannels.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.TSChannels.Margin = New System.Windows.Forms.Padding(0)
        Me.TSChannels.Name = "TSChannels"
        Me.TSChannels.ShowDropDownArrow = False
        Me.TSChannels.Size = New System.Drawing.Size(62, 24)
        Me.TSChannels.Text = "Common"
        Me.TSChannels.ToolTipText = "Messaging channel"
        '
        'PublicChannel
        '
        Me.PublicChannel.ForeColor = System.Drawing.Color.FromArgb(CType(CType(176, Byte), Integer), CType(CType(176, Byte), Integer), CType(CType(176, Byte), Integer))
        Me.PublicChannel.Name = "PublicChannel"
        Me.PublicChannel.Size = New System.Drawing.Size(125, 22)
        Me.PublicChannel.Text = "Common"
        '
        'PubPrivChSeparator
        '
        Me.PubPrivChSeparator.Name = "PubPrivChSeparator"
        Me.PubPrivChSeparator.Size = New System.Drawing.Size(122, 6)
        '
        'TSChat
        '
        Me.TSChat.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.TSChat.Image = Global.Folmes.My.Resources.Resources.chat
        Me.TSChat.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.TSChat.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.TSChat.Margin = New System.Windows.Forms.Padding(0)
        Me.TSChat.Name = "TSChat"
        Me.TSChat.Padding = New System.Windows.Forms.Padding(1)
        Me.TSChat.Size = New System.Drawing.Size(24, 24)
        Me.TSChat.Text = "Chat"
        '
        'Input
        '
        Me.Input.AcceptsTab = True
        Me.Input.AllowDrop = True
        Me.Input.BackColor = System.Drawing.Color.FromArgb(CType(CType(24, Byte), Integer), CType(CType(24, Byte), Integer), CType(CType(24, Byte), Integer))
        Me.Input.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.Input.ContextMenuStrip = Me.InputContMenu
        Me.Input.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Input.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel)
        Me.Input.ForeColor = System.Drawing.Color.DarkGray
        Me.Input.Location = New System.Drawing.Point(3, 5)
        Me.Input.Multiline = True
        Me.Input.Name = "Input"
        Me.Input.Size = New System.Drawing.Size(355, 33)
        Me.Input.TabIndex = 0
        '
        'MainGUI
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer))
        Me.CausesValidation = False
        Me.ClientSize = New System.Drawing.Size(374, 262)
        Me.Controls.Add(Me.Output)
        Me.Controls.Add(Me.TS)
        Me.Controls.Add(Me.InputBGPanel)
        Me.DoubleBuffered = True
        Me.Margin = New System.Windows.Forms.Padding(2, 3, 2, 3)
        Me.MinimumSize = New System.Drawing.Size(200, 200)
        Me.Name = "MainGUI"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.Text = "Folmes"
        Me.InputBGPanel.ResumeLayout(False)
        Me.InputPaddingPanel.ResumeLayout(False)
        Me.InputPaddingPanel.PerformLayout()
        Me.InputContMenu.ResumeLayout(False)
        Me.OutputContMenu.ResumeLayout(False)
        Me.TrayIconMenu.ResumeLayout(False)
        Me.TS.ResumeLayout(False)
        Me.TS.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Private WithEvents InputBGPanel As System.Windows.Forms.Panel
    Private WithEvents InputPaddingPanel As System.Windows.Forms.Panel
    Private WithEvents Button2 As System.Windows.Forms.Button
    Private WithEvents InputContMenu As System.Windows.Forms.ContextMenuStrip
    Private WithEvents CutBtn As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents CopyBtn As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents PasteBtn As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents ToolStripMenuItem1 As System.Windows.Forms.ToolStripSeparator
    Private WithEvents SelectAllBtn As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents OutputContMenu As System.Windows.Forms.ContextMenuStrip
    Private WithEvents CopyO As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents TrayIconMenu As System.Windows.Forms.ContextMenuStrip
    Private WithEvents CMSettings As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents CMExit As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents CMOpenFolder As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Private WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
    Private WithEvents TS As NFToolStrip
    Private WithEvents TSTools As System.Windows.Forms.ToolStripDropDownButton
    Private WithEvents TSChannels As System.Windows.Forms.ToolStripDropDownButton
    Private WithEvents PublicChannel As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents TSChat As System.Windows.Forms.ToolStripButton
    Friend WithEvents Output As MessagesDisplay
    Friend WithEvents TrayIcon As System.Windows.Forms.NotifyIcon
    Friend WithEvents PubPrivChSeparator As System.Windows.Forms.ToolStripSeparator
    Private WithEvents TSSettings As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents TSCleaner As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents TSHelp As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TSAbout As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Input As Folmes.GUI.InputTextBox
End Class
