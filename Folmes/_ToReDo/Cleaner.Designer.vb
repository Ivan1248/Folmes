Imports Folmes.GUI.Controls

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Cleaner
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Cleaner))
        Me.Output = New MessagesDisplay()
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
        Me.DeleteBtn = New System.Windows.Forms.ToolStripButton()
        Me.Toggle = New System.Windows.Forms.ToolStripButton()
        Me.ToolStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Output
        '
        Me.Output.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Output.Location = New System.Drawing.Point(0, 28)
        Me.Output.MinimumSize = New System.Drawing.Size(20, 20)
        Me.Output.Size = New System.Drawing.Size(304, 194)
        Me.Output.TabIndex = 0
        '
        'ToolStrip1
        '
        Me.ToolStrip1.AutoSize = False
        Me.ToolStrip1.BackColor = System.Drawing.Color.FromArgb(CType(CType(48, Byte), Integer), CType(CType(48, Byte), Integer), CType(CType(48, Byte), Integer))
        Me.ToolStrip1.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel)
        Me.ToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DeleteBtn, Me.Toggle})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
        Me.ToolStrip1.Size = New System.Drawing.Size(304, 28)
        Me.ToolStrip1.TabIndex = 12
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'DeleteBtn
        '
        Me.DeleteBtn.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.DeleteBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.DeleteBtn.Image = Global.Folmes.My.Resources.Resources.X
        Me.DeleteBtn.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.DeleteBtn.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.DeleteBtn.Margin = New System.Windows.Forms.Padding(0)
        Me.DeleteBtn.Name = "DeleteBtn"
        Me.DeleteBtn.Padding = New System.Windows.Forms.Padding(2)
        Me.DeleteBtn.Size = New System.Drawing.Size(26, 28)
        Me.DeleteBtn.Text = "ToolStripButton1"
        Me.DeleteBtn.ToolTipText = "Delete"
        '
        'Toggle
        '
        Me.Toggle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.Toggle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(176, Byte), Integer), CType(CType(176, Byte), Integer), CType(CType(176, Byte), Integer))
        Me.Toggle.Image = CType(resources.GetObject("Toggle.Image"), System.Drawing.Image)
        Me.Toggle.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.Toggle.Name = "Toggle"
        Me.Toggle.Size = New System.Drawing.Size(34, 25)
        Me.Toggle.Text = "Files"
        '
        'Cleaner
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(304, 222)
        Me.Controls.Add(Me.Output)
        Me.Controls.Add(Me.ToolStrip1)
        Me.Name = "Cleaner"
        Me.Text = "Cleaner"
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Private ToolStrip1 As System.Windows.Forms.ToolStrip
    Private WithEvents DeleteBtn As System.Windows.Forms.ToolStripButton
    Private WithEvents Toggle As System.Windows.Forms.ToolStripButton
    Friend WithEvents Output As MessagesDisplay
End Class
