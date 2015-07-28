Imports System.IO

Partial Class MainGUI
    Private WithEvents MessagesWatcher As FileSystemWatcher
    Private WithEvents PingPongWatcher As FileSystemWatcher
    Private WithEvents UsersWatcher As FileSystemWatcher
    Private Sub LoadFSWatchers()
        MessagesWatcher = New FileSystemWatcher(Dirs.Messages, "*" & Extension.Message) With {
            .IncludeSubdirectories = True,
            .NotifyFilter = NotifyFilters.FileName
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
    Private Sub MessagesWatcher_Created(senderObject As Object, e As FileSystemEventArgs) Handles MessagesWatcher.Created
        Dim DirPath As String = Path.GetDirectoryName(e.FullPath)
        Dim Sender As String = Path.GetFileName(DirPath)
        Dim RecipientChannel As String = Path.GetFileName(Path.GetDirectoryName(DirPath))
        Dim NotificationType As Notifications.Notifications
        If RecipientChannel = Channels.Common AndAlso Sender <> My.Settings.Username Then
            NotificationType = Notifications.Notifications.PublicMessage
        ElseIf RecipientChannel = My.Settings.Username Then
            NotificationType = Notifications.Notifications.PrivateMessage
        Else
            Exit Sub
        End If
        Dim message As Message = MessageFile.LoadMessage(e.FullPath)
        If Sender = Channels.Current OrElse (RecipientChannel = Channels.Current AndAlso Channels.Current = Channels.Common) Then
            Me.Output.AddMessage(message)
        Else
            If RecipientChannel = Channels.Common Then
                MessagesManager.CommonNewQueue.Enqueue(message)
            Else
                MessagesManager.AddPrivateNew(RecipientChannel, message)
            End If
        End If
        Notify(NotificationType, Sender)
    End Sub

    Private Sub PingPongWatcher_Created(senderObject As Object, e As FileSystemEventArgs) Handles PingPongWatcher.Created
        If Path.GetFileName(Path.GetDirectoryName(e.FullPath)) <> My.Settings.Username Then
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
        If e.ChangeType <> WatcherChangeTypes.Changed Then Exit Sub

        Dim Dir As String = Path.GetDirectoryName(e.FullPath)
        Dim Name As String = Path.GetFileNameWithoutExtension(Dir)

        If Name = My.Settings.Username Then
            Exit Sub
        End If

        Dim Foo As User = Users.Others.Find(Function(u) u.Name = Name)
        If Foo Is Nothing Then
            Users.Others.Add(New User(e.FullPath))
            Notify(Notifications.Notifications.Joined, Name)
        Else
            Dim PrevStatus As Boolean = Foo.IsOnline()
            Foo.RefreshStatus()
            If PrevStatus <> Foo.IsOnline() Then
                Notify(If(Not PrevStatus, Notifications.Notifications.LoggedIn, Notifications.Notifications.LoggedOut), Name)
            End If
        End If
    End Sub

    Private Sub UsersWatcher_Deleted(sender As Object, e As FileSystemEventArgs) Handles UsersWatcher.Deleted
        If e.Name IsNot Nothing Then Exit Sub ' not a directory
        Users.Others.RemoveAt(Users.Others.FindIndex(Function(u) u.Name = e.Name))
        If Channels.Current = e.Name Then SwitchChannel(Channels.Common)
    End Sub
End Class
