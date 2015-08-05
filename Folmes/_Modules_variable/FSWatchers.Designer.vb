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
            .NotifyFilter = NotifyFilters.FileName
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

    Private Sub PingPongWatcher_Created(senderObject As Object, e As FileSystemEventArgs) Handles PingPongWatcher.Created
        Dim recipient As String = Path.GetFileName(Path.GetDirectoryName(e.FullPath))
        If recipient <> My.Settings.Username Then
            Exit Sub
        End If
        File.Delete(e.FullPath)
        If Path.GetExtension(e.Name) = Extension.Ping Then
            Dim Sender As String = e.Name.Substring(0, e.Name.IndexOf("."c))
            PingPong.PingFile(Sender, True)
        Else
            PingPong.GetFileRtt()
        End If
    End Sub

    Private Sub UsersWatcher_Changed(sender As Object, e As FileSystemEventArgs) Handles UsersWatcher.Changed
        If e.ChangeType <> WatcherChangeTypes.Changed Then
            Exit Sub
        End If
        Dim Dir As String = Path.GetDirectoryName(e.FullPath)
        Dim Name As String = Path.GetFileNameWithoutExtension(Dir)
        If Name = My.Settings.Username Then
            Exit Sub
        End If
        Dim Foo As User = Users.Others.Find(Function(u) u.Name = Name)
        If Foo Is Nothing Then
            Users.Others.Add(New User(e.FullPath))
            Notify(Notifications.NotificationType.Joined, Name)
        Else
            Dim PrevStatus As Boolean = Foo.IsOnline()
            Foo.RefreshStatus()
            If PrevStatus <> Foo.IsOnline() Then
                Notify(If(Not PrevStatus, Notifications.NotificationType.LoggedIn, Notifications.NotificationType.LoggedOut), Name)
            End If
        End If
    End Sub

    Private Sub UsersWatcher_Deleted(sender As Object, e As FileSystemEventArgs) Handles UsersWatcher.Deleted
        If e.Name IsNot Nothing Then Exit Sub ' not a directory
        Users.Others.RemoveAt(Users.Others.FindIndex(Function(u) u.Name = e.Name))
        If Channels.Current = e.Name Then SwitchChannel(Channels.Common)
    End Sub
End Class
