Imports System.Diagnostics.Eventing.Reader
Imports System.IO
Imports Folmes.Classes

Partial Class Box
    Protected Friend MustInherit Class UserInfoFiles
        Friend Shared Others As New List(Of UserInfoFile)
        Friend Shared Mine As UserInfoFile

        Shared Sub GetAll()
            Dim Files As String() = Directory.GetFiles(MessagesDir, "*.info")
            Dim Username As String
            Dim NewFile As UserInfoFile
            For Each File As String In Files
                Username = Path.GetFileNameWithoutExtension(File)
                NewFile = New UserInfoFile(File) With {.Name = Username}
                If Username <> My.Settings.Username Then
                    Others.Add(NewFile)
                ElseIf Mine Is Nothing Then
                    Mine = NewFile
                End If
            Next
            If Mine Is Nothing Then
                Mine = New UserInfoFile(Path.Combine(MessagesDir, My.Settings.Username & ".info")) With {.Name = My.Settings.Username}
            End If
        End Sub
        Shared Function IsOnline(userName As String) As Boolean
            Dim Foo As UserInfoFile = Others.Find(Function(x) x.Name = userName)
            Return If(Foo IsNot Nothing, Foo.Online, False)
        End Function
    End Class
End Class
