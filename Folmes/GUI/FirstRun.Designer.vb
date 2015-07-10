<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FirstRun
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
        Me.Label1 = New System.Windows.Forms.Label()
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.Username = New System.Windows.Forms.TextBox()
        Me.FlowLayoutPanel5 = New System.Windows.Forms.FlowLayoutPanel()
        Me.CancButton = New System.Windows.Forms.Button()
        Me.OKButton = New System.Windows.Forms.Button()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.FlowLayoutPanel5.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(9, 9)
        Me.Label1.Margin = New System.Windows.Forms.Padding(0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(166, 65)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Hello, and welcome to Folmes!" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "To get started, enter your desired nickname into" & _
    " the textbox below." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.AutoSize = True
        Me.FlowLayoutPanel1.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.FlowLayoutPanel1.Controls.Add(Me.Label1)
        Me.FlowLayoutPanel1.Controls.Add(Me.Username)
        Me.FlowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FlowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.FlowLayoutPanel1.MaximumSize = New System.Drawing.Size(186, 0)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Padding = New System.Windows.Forms.Padding(9)
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(184, 123)
        Me.FlowLayoutPanel1.TabIndex = 1
        '
        'Username
        '
        Me.Username.Location = New System.Drawing.Point(12, 74)
        Me.Username.Margin = New System.Windows.Forms.Padding(3, 0, 3, 6)
        Me.Username.MaxLength = 16
        Me.Username.Name = "Username"
        Me.Username.Size = New System.Drawing.Size(162, 20)
        Me.Username.TabIndex = 1
        '
        'FlowLayoutPanel5
        '
        Me.FlowLayoutPanel5.AutoSize = True
        Me.FlowLayoutPanel5.Controls.Add(Me.CancButton)
        Me.FlowLayoutPanel5.Controls.Add(Me.OKButton)
        Me.FlowLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.FlowLayoutPanel5.FlowDirection = System.Windows.Forms.FlowDirection.BottomUp
        Me.FlowLayoutPanel5.Location = New System.Drawing.Point(0, 123)
        Me.FlowLayoutPanel5.Name = "FlowLayoutPanel5"
        Me.FlowLayoutPanel5.Padding = New System.Windows.Forms.Padding(3, 0, 3, 3)
        Me.FlowLayoutPanel5.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.FlowLayoutPanel5.Size = New System.Drawing.Size(184, 41)
        Me.FlowLayoutPanel5.TabIndex = 9
        '
        'CancButton
        '
        Me.CancButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.CancButton.FlatAppearance.BorderColor = System.Drawing.Color.DimGray
        Me.CancButton.FlatAppearance.BorderSize = 0
        Me.CancButton.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.CancButton.Location = New System.Drawing.Point(105, 9)
        Me.CancButton.Margin = New System.Windows.Forms.Padding(3, 9, 3, 6)
        Me.CancButton.Name = "CancButton"
        Me.CancButton.Size = New System.Drawing.Size(70, 23)
        Me.CancButton.TabIndex = 6
        Me.CancButton.Text = "Cancel"
        Me.CancButton.UseVisualStyleBackColor = True
        '
        'OKButton
        '
        Me.OKButton.FlatAppearance.BorderColor = System.Drawing.Color.DimGray
        Me.OKButton.FlatAppearance.BorderSize = 0
        Me.OKButton.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.OKButton.Location = New System.Drawing.Point(29, 9)
        Me.OKButton.Margin = New System.Windows.Forms.Padding(3, 9, 3, 6)
        Me.OKButton.Name = "OKButton"
        Me.OKButton.Size = New System.Drawing.Size(70, 23)
        Me.OKButton.TabIndex = 1
        Me.OKButton.Text = "OK"
        Me.OKButton.UseVisualStyleBackColor = True
        '
        'FirstRun
        '
        Me.AcceptButton = Me.OKButton
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.CancelButton = Me.CancButton
        Me.ClientSize = New System.Drawing.Size(184, 164)
        Me.Controls.Add(Me.FlowLayoutPanel1)
        Me.Controls.Add(Me.FlowLayoutPanel5)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Name = "FirstRun"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Welcome!"
        Me.TopMost = True
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.FlowLayoutPanel1.PerformLayout()
        Me.FlowLayoutPanel5.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private Label1 As System.Windows.Forms.Label
    Private FlowLayoutPanel1 As System.Windows.Forms.FlowLayoutPanel
    Private WithEvents Username As System.Windows.Forms.TextBox
    Private FlowLayoutPanel5 As System.Windows.Forms.FlowLayoutPanel
    Private WithEvents CancButton As System.Windows.Forms.Button
    Private WithEvents OKButton As System.Windows.Forms.Button
End Class
