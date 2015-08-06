Imports System.ComponentModel
Imports System.IO

Partial Class MainGUI
    Private WithEvents MessagesWatcher As FileSystemWatcher
    Private WithEvents PingPongWatcher As FileSystemWatcher
    Private WithEvents UsersWatcher As FileSystemWatcher

    Private Sub LoadFSWatchers()
        MessagesWatcher = New FileSystemWatcher(Dirs.Messages, "*" & Extension.Message) With {
            .IncludeSubdirectories = True,
            .NotifyFilter = NotifyFilters.LastWrite
        }
        PingPongWatcher = New FileSystemWatcher(Path.Combine(Dirs.PingPong, My.Settings.Username), "*.*") With {
            .IncludeSubdirectories = False,
            .NotifyFilter = NotifyFilters.LastWrite
        }
        UsersWatcher = New FileSystemWatcher(Dirs.Users, "*.*") With {
            .IncludeSubdirectories = True,
            .NotifyFilter = NotifyFilters.LastWrite Or NotifyFilters.DirectoryName
        }
        For Each fsw As FileSystemWatcher In {MessagesWatcher, PingPongWatcher, UsersWatcher}
            fsw.SynchronizingObject = Me
            fsw.EnableRaisingEvents = True
        Next
    End Sub

    Private Sub MessagesWatcher_Created(senderObject As Object, e As FileSystemEventArgs) Handles MessagesWatcher.Changed
        Dim dirPath As String = Path.GetDirectoryName(e.FullPath)

        Dim sender As String = Path.GetFileName(dirPath)
        If sender = My.Settings.Username Then
            Exit Sub
        End If

        Dim recipientChannel As String = Path.GetFileName(Path.GetDirectoryName(dirPath))
        If recipientChannel <> Channels.Common AndAlso recipientChannel <> My.Settings.Username Then
            Exit Sub
        End If

        Dim message As Message = MessagesManager.LoadMessage(e.FullPath)
        If recipientChannel = Channels.Common Then
            MessagesManager.AddCommonNew(message)
            Notify(NotificationType.PublicMessage, sender)
        Else
            MessagesManager.AddPrivateNew(recipientChannel, message)
            Notify(NotificationType.PrivateMessage, sender)
        End If
    End Sub

    Private Sub PingPongWatcher_Created(senderObject As Object, e As FileSystemEventArgs) Handles PingPongWatcher.Changed
        File.Delete(e.FullPath)
        If Path.GetExtension(e.Name) = Extension.Ping Then
            Dim sender As String = e.Name.Substring(0, e.Name.IndexOf("."c))
            PingPong.Pong(sender)
        Else
            PingPong.GetFileRtt()
        End If
    End Sub

    Private Sub UsersWatcher_Changed(sender As Object, e As FileSystemEventArgs) Handles UsersWatcher.Changed
        If e.ChangeType <> WatcherChangeTypes.Changed Then
            Exit Sub
        End If
        If Path.GetExtension(e.Name) <> Extension.UserInfo Then
            Exit Sub
        End If
        Dim name As String = e.Name.Substring(0, e.Name.IndexOf("\"c))
        If name = My.Settings.Username Then
            Exit Sub
        End If
        Dim user As User = Users.GetUser(name)
        If user Is Nothing Then
            Users.AddNew(name)
            Notify(NotificationType.Joined, name)
        Else
            Dim PrevStatus As Boolean = user.IsOnline()
            user.RefreshStatus()
            If PrevStatus <> user.IsOnline() Then
                Notify(If(Not PrevStatus, NotificationType.LoggedIn, NotificationType.LoggedOut), name)
            End If
        End If
    End Sub

    Private Sub UsersWatcher_Deleted(sender As Object, e As FileSystemEventArgs) Handles UsersWatcher.Deleted
        If Not Directory.Exists(e.FullPath) Then Exit Sub ' not a directory
        Users.Remove(e.Name)
        If Channels.Current = e.Name Then SwitchChannel(Channels.Common)
    End Sub
End Class
