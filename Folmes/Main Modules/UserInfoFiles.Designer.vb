Imports System.Diagnostics.Eventing.Reader
Imports System.IO
Imports Folmes.Classes

Partial Class Box
    Protected Friend MustInherit Class UserInfoFiles
        Friend Shared Others As New List(Of UserInfoFile)
        Friend Shared Mine As UserInfoFile

        Shared Sub GetAll()
            Dim UIFiles As String() = Directory.GetFiles(UsersDir, "*" & Files.Extension.UserInfo)
            For Each File As String In UIFiles
                Dim Username As String = Path.GetFileNameWithoutExtension(File)
                Dim NewFile As UserInfoFile = New UserInfoFile(File) With {.Name = Username}
                If Username <> My.Settings.Username Then
                    Others.Add(NewFile)
                ElseIf Mine Is Nothing Then
                    Mine = NewFile
                End If
            Next
            If Mine Is Nothing Then
                Mine = New UserInfoFile(Path.Combine(UsersDir, My.Settings.Username & Files.Extension.UserInfo)) With {.Name = My.Settings.Username}
            End If
        End Sub
        Shared Function IsOnline(userName As String) As Boolean
            Dim Foo As UserInfoFile = Others.Find(Function(x) x.Name = userName)
            Return If(Foo IsNot Nothing, Foo.Online, False)
        End Function
    End Class
End Class
