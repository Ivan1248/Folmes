Imports System.IO
Imports Folmes.Classes
Imports Folmes.Datatypes

Partial Class MainGUI
    Private WithEvents MessagesWatcher As FileSystemWatcher
    Private WithEvents UserFilesWatcher As FileSystemWatcher
    Private WithEvents DirectoriesWatcher As FileSystemWatcher
    Private Sub LoadFSWatchers()
        'TODO PingPong
        MessagesWatcher = New FileSystemWatcher(MessagesDir, "*.fmsg") With {
            .IncludeSubdirectories = True,
            .NotifyFilter = NotifyFilters.LastWrite
        }
        DirectoriesWatcher = New FileSystemWatcher(MessagesDir, "*.*") With {
            .IncludeSubdirectories = False,
            .NotifyFilter = NotifyFilters.DirectoryName
        }
        UserFilesWatcher = New FileSystemWatcher(UsersDir, "*.*") With {
            .IncludeSubdirectories = False,
            .NotifyFilter = NotifyFilters.FileName Or NotifyFilters.LastWrite
        }
        For Each fsw As FileSystemWatcher In {MessagesWatcher, UserFilesWatcher, DirectoriesWatcher}
            fsw.SynchronizingObject = Me
            fsw.EnableRaisingEvents = True
        Next
    End Sub
    Private Sub MessagesWatcher_Changed(sender As Object, e As FileSystemEventArgs) Handles MessagesWatcher.Changed
        MessagesWatcher.EnableRaisingEvents = True

        Dim DirPath As String = Path.GetDirectoryName(e.FullPath)
        Dim SenderName As String = Path.GetFileName(DirPath)
        Dim ChannelName As String = Path.GetFileName(Path.GetDirectoryName(DirPath))
        Dim NotificationType As Notifications.Notifications
        If ChannelName = Channels.Common AndAlso SenderName <> My.Settings.Username Then
            NotificationType = Notifications.Notifications.PublicMessage
        ElseIf ChannelName = My.Settings.Username Then
            NotificationType = Notifications.Notifications.PrivateMessage
        Else
            GoTo exitr
        End If
        Dim message As Message = MessageFile.LoadMessage(e.FullPath)
        If Channels.Current = ChannelName Then
            Me.Output.InsertMessage(message)
        Else
            If ChannelName = Channels.Common Then
                Messages.CommonNewQueue.Add(message)
            Else
                Messages.AddPrivateNew(ChannelName, message)
            End If
        End If
        Notify(NotificationType, SenderName)

exitr:  MessagesWatcher.EnableRaisingEvents = True
    End Sub

    Private Sub UserFilesWatcher_Changed(sender As Object, e As FileSystemEventArgs) Handles UserFilesWatcher.Changed, UserFilesWatcher.Created
        Dim Name As String = Path.GetFileNameWithoutExtension(e.Name)

        If Name = My.Settings.Username Then
            Exit Sub
        End If

        If Path.GetExtension(e.FullPath) <> Files.Extension.UserInfo Then Exit Sub

        Dim Foo As UserInfoFile = UserInfoFiles.Others.Find(Function(x) x.Username = Name)
        If Foo Is Nothing Then
            UserInfoFiles.Others.Add(New UserInfoFile(e.FullPath) With {.Username = Name})
            Notify(Notifications.Notifications.Joined, Name)
        Else
            Dim PrevStatus As Boolean = Foo.Online
            Foo.Refresh()
            If PrevStatus <> Foo.Online Then
                Notify(If(Not PrevStatus, Notifications.Notifications.LoggedIn, Notifications.Notifications.LoggedOut), Name)
            End If
            If Name = Channels.Current Then TSChat.Visible = Not PrevStatus
        End If
    End Sub

    Private Sub UserFilesWatcher_Deleted(sender As Object, e As FileSystemEventArgs) Handles UserFilesWatcher.Deleted
        Dim Name As String = Path.GetFileNameWithoutExtension(e.Name)
        UserInfoFiles.Others.Remove(UserInfoFiles.Others.Find(Function(x) x.Username = Name))
        'TODO MessageFiles.OutgoingPrivate.Remove(MessageFiles.OutgoingPrivate.Find(Function(x) x.Sender = Name))
        'TODO MessageFiles.IngoingPrivate.Remove(MessageFiles.IngoingPrivate.Find(Function(x) x.Sender = Name))

        If Channels.Current = Name Then SwitchChannel(Channels.Common)
    End Sub

    Private Sub DirectoriesWatcher_Renamed(sender As Object, e As RenamedEventArgs) Handles DirectoriesWatcher.Renamed
        If Channels.Current = e.OldName Then SwitchChannel(Path.GetFileName(e.FullPath))
    End Sub
End Class
