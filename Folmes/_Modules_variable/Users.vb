Imports System.IO

Public MustInherit Class Users

    Public Shared Others As New List(Of User)
    Public Shared MyUser As User

    Public Shared Sub Initialize()
        For Each userDir As String In Directory.GetDirectories(Dirs.Users)
            Dim user As New User(userDir)
            If user.Name <> My.Settings.Username Then
                Others.Add(user)
            Else
                MyUser = user
            End If
        Next
        If MyUser Is Nothing Then MyUser = Create(My.Settings.Username)
    End Sub

    Public Shared Function Create(username As String) As User
        Dirs.Create(Path.Combine(Dirs.PrivateMessages, username))
        Dirs.Create(Path.Combine(Dirs.Users, username))
        Return New User(Path.Combine(Dirs.Users, My.Settings.Username))
    End Function

    Public Shared Sub Delete(username As String)
        Dim dirsToDelete As New List(Of String)
        dirsToDelete.Add(Path.Combine(Dirs.Users, username))                        ' User folder
        dirsToDelete.Add(Path.Combine(Dirs.PrivateMessages, username))              ' Private inbox
        dirsToDelete.Add(Path.Combine(Dirs.CommonChannel, username))                ' Common outbox
        For Each userCh As String In Directory.GetDirectories(Dirs.PrivateMessages) ' Private outbox
            dirsToDelete.Add(Path.Combine(userCh, username))
        Next
        For Each dir As String In dirsToDelete
            Try
                Directory.Delete(dir, True)
            Catch ex As Exception
            End Try
        Next
    End Sub

    Public Shared Function IsOnline(username As String) As Boolean
        Dim user As User = Others.Find(Function(u) u.Name = username)
        If user IsNot Nothing Then
            Return user.IsOnline()
        Else
            Return username = My.Settings.Username
        End If
    End Function

End Class