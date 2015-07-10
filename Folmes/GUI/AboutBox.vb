Public NotInheritable Class AboutBox

    Private Sub AboutBox1_Load(sender As System.Object, e As EventArgs) Handles MyBase.Load
        PictureBox1.Image = New Icon(My.Resources.DBM, New Size(48, 48)).ToBitmap
        Me.Owner = MainGUI
        ' Initialize all of the text displayed on the About Box.
        ' TODO: Customize the application's assembly information in the "Application" pane of the project 
        '    properties dialog (under the "Project" menu).
        'Me.LabelCopyright.Text = My.Application.Info.Copyright
    End Sub

End Class
