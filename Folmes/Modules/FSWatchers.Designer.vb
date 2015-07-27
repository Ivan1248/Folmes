Imports System.IO

Partial Class MainGUI
    Private WithEvents MessagesWatcher As FileSystemWatcher
    Private WithEvents PingPongWatcher As FileSystemWatcher
    Private WithEvents UserFilesWatcher As FileSystemWatcher
    Private WithEvents DirectoriesWatcher As FileSystemWatcher
    Private Sub LoadFSWatchers()
        MessagesWatcher = New FileSystemWatcher(Dirs.Messages, "*" & Extension.Message) With {
            .IncludeSubdirectories = True,
            .NotifyFilter = NotifyFilters.FileName
        }
        PingPongWatcher = New FileSystemWatcher(Path.Combine(Dirs.PingPong, My.Settings.Username), "*.*") With {
            .IncludeSubdirectories = False,
            .NotifyFilter = NotifyFilters.FileName
        }
        DirectoriesWatcher = New FileSystemWatcher(Dirs.Messages, "*.*") With {
            .IncludeSubdirectories = False,
            .NotifyFilter = NotifyFilters.DirectoryName
        }
        UserFilesWatcher = New FileSystemWatcher(Dirs.Users, "*.*") With {
            .IncludeSubdirectories = False,
            .NotifyFilter = NotifyFilters.FileName Or NotifyFilters.LastWrite Or NotifyFilters.DirectoryName
        }
        For Each fsw As FileSystemWatcher In {MessagesWatcher, PingPongWatcher, UserFilesWatcher, DirectoriesWatcher}
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
                MessagesManager.CommonNewQueue.Add(message)
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
            PingPong.GetFileRTT()
        End If
    End Sub

    Private Sub UserFilesWatcher_Changed(sender As Object, e As FileSystemEventArgs) Handles UserFilesWatcher.Changed, UserFilesWatcher.Created
        Dim Dir As String = Path.GetDirectoryName(e.FullPath)
        Dim Name As String = Path.GetFileNameWithoutExtension(Dir)

        If Name = My.Settings.Username Then
            Exit Sub
        End If

        Dim Foo As Users.User = Users.Others.Find(Function(u) u.Name = Name)
        If e.ChangeType = WatcherChangeTypes.Created AndAlso Directory.Exists(e.FullPath) Then
            Users.Others.Add(New Users.User(e.FullPath))
            Notify(Notifications.Notifications.Joined, Name)
        Else
            Dim PrevStatus As Boolean = Foo.IsOnline()
            Foo.RefreshStatus()
            If PrevStatus <> Foo.IsOnline() Then
                Notify(If(Not PrevStatus, Notifications.Notifications.LoggedIn, Notifications.Notifications.LoggedOut), Name)
            End If
            If Name = Channels.Current Then TSChat.Visible = Not PrevStatus
        End If
    End Sub

    Private Sub UserFilesWatcher_Deleted(sender As Object, e As FileSystemEventArgs) Handles UserFilesWatcher.Deleted
        Dim Name As String = Path.GetFileNameWithoutExtension(e.Name)
        Users.Others.RemoveAt(Users.Others.FindIndex(Function(x) x.Name = Name))
        If Channels.Current = Name Then SwitchChannel(Channels.Common)
    End Sub

    Private Sub DirectoriesWatcher_Renamed(sender As Object, e As RenamedEventArgs) Handles DirectoriesWatcher.Renamed
        If Channels.Current = e.OldName Then SwitchChannel(Path.GetFileName(e.FullPath))
    End Sub
End Class
