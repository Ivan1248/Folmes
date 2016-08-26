Imports System.IO

Public MustInherit Class Users

    Public Shared Others As New List(Of User)
    Public Shared MyUser As User

    Public Shared Sub Initialize()
        For Each userDir As String In Directory.GetDirectories(Dirs.Users)
            Dim user As New User(IO.Path.GetFileName(userDir), UserKind.OldReal)
            If user.Name <> My.Settings.Username Then
                Others.Add(user)
            Else
                MyUser = user
            End If
        Next
        If MyUser Is Nothing Then MyUser = Create(My.Settings.Username)
    End Sub

    Public Shared Function GetByName(name As String) As User
        If name = My.Settings.Username Then Return MyUser
        Return Others.Find(Function(u) u.Name = name)
    End Function

    Public Shared Function GetByIrcNick(nick As String) As User
        If nick = MyUser.IrcNick Then Return MyUser
        Return Others.Find(Function(u) u.IrcNick = nick)
    End Function

    Public Shared Sub AddNew(name As String)
        Others.Add(New User(name, UserKind.OldReal))
    End Sub

    Public Shared Sub Remove(username As String)
        Dim i As Integer = Others.FindIndex(Function(u) u.Name = username)
        If i >= 0 Then Others.RemoveAt(i)
    End Sub

    Public Shared Function Create(username As String) As User
        Dirs.Create(IO.Path.Combine(Dirs.PrivateMessages, username))
        Dirs.Create(IO.Path.Combine(Dirs.Users, username))
        Return New User(My.Settings.Username, UserKind.NewReal)

    End Function

    Public Shared Sub Delete(username As String)
        Dim dirsToDelete As New List(Of String)
        dirsToDelete.Add(IO.Path.Combine(Dirs.Users, username))                        ' User folder
        dirsToDelete.Add(IO.Path.Combine(Dirs.PrivateMessages, username))              ' Private inbox
        dirsToDelete.Add(IO.Path.Combine(Dirs.CommonChannel, username))                ' Common outbox
        For Each userCh As String In Directory.GetDirectories(Dirs.PrivateMessages) ' Private outbox
            dirsToDelete.Add(IO.Path.Combine(userCh, username))
        Next
        For Each dir As String In dirsToDelete
            Try
                Directory.Delete(dir, True)
            Catch ex As Exception
            End Try
        Next
        Remove(username)
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