Imports System.IO

Public Class FirstRun

    Dim usernameIsValid As Boolean = False

    Private Sub OKButton_Click(sender As Object, e As EventArgs) Handles OKButton.Click
        If usernameIsValid Then
            If Not Directory.Exists(Path.Combine(Dirs.Users, Username.Text)) OrElse MessageBox.Show("The username '" & Username.Text & "' is already registered." & vbNewLine & "Is this your old username?", "Username exists", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) = Windows.Forms.DialogResult.Yes Then
                My.Settings.Username = Username.Text
                Me.DialogResult = DialogResult.OK
            End If
        Else
            MessageBox.Show("The username must contain at least 2 characters and may not include any of the following characters: \/:*?""<>!", "Invalid username", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1)
        End If
    End Sub

    Private Sub Username_TextChanged(sender As Object, e As EventArgs) Handles Username.TextChanged
        If Username.Text.Length > 1 And NameValid(Username.Text) Then
            usernameIsValid = True
            Username.ForeColor = Color.Black
        Else
            usernameIsValid = False
            Username.ForeColor = Color.FromArgb(192, 0, 0)
        End If
    End Sub

    Public Function NameValid(ByVal str As String) As Boolean
        Return str.Length > 1 AndAlso Not "\/:*?""<>!|".Any(Function(crt) str.Contains(crt))
    End Function

End Class