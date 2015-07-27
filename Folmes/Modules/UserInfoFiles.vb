Imports System.IO
Imports Folmes.Classes

Public MustInherit Class UserInfoFiles
    Friend Shared Others As New List(Of UserInfoFile)
    Friend Shared Mine As UserInfoFile

    Shared Sub GetAll()
        Dim UIFiles As String() = Directory.GetFiles(Dirs.Users, "*" & Files.Extension.UserInfo)
        For Each File As String In UIFiles
            Dim Username As String = Path.GetFileNameWithoutExtension(File)
            Dim NewFile As UserInfoFile = New UserInfoFile(File) With {.Username = Username}
            If Username <> My.Settings.Username Then
                Others.Add(NewFile)
            ElseIf Mine Is Nothing Then
                Mine = NewFile
            End If
        Next
        If Mine Is Nothing Then
            Mine = New UserInfoFile(Path.Combine(Dirs.Users, My.Settings.Username & Files.Extension.UserInfo)) With {.Username = My.Settings.Username}
        End If
    End Sub
    Shared Function IsOnline(userName As String) As Boolean
        Dim UIFile As UserInfoFile = Others.Find(Function(x) x.Username = userName)
        Return If(UIFile IsNot Nothing, UIFile.Online, False)
    End Function

End Class