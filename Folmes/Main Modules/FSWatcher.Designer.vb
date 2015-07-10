Imports System.IO
Imports Folmes.Classes

Partial Class MainGUI
    Private WithEvents MessagesWatcher As FileSystemWatcher
    Private WithEvents UserFilesWatcher As FileSystemWatcher
    Private WithEvents DirectoriesWatcher As FileSystemWatcher
    Private Sub LoadFSWatchers()
        MessagesWatcher = New FileSystemWatcher(MessagesDir, "*.*") With {
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
        MessagesWatcher.EnableRaisingEvents = False

        Dim Name As String = Path.GetFileNameWithoutExtension(e.Name)
        Dim DirPath As String = Path.GetDirectoryName(e.FullPath)
        Dim DirName As String = Path.GetFileName(DirPath)
        Select Case Path.GetExtension(e.FullPath)
            Case Files.Extension.Message
                Dim NotificationType As Notifications.Notifications
                Dim Files As List(Of MessageFile)
                If DirPath = MessagesDir Then
                    If Name = My.Settings.Username Then Exit Sub
                    NotificationType = Notifications.Notifications.PublicMessage
                    Files = MessageFiles.IngoingCommon
                ElseIf DirName = My.Settings.Username Then
                    NotificationType = Notifications.Notifications.PrivateMessage
                    Files = MessageFiles.IngoingPrivate
                Else
                    Exit Sub
                End If
                Dim File As MessageFile = Files.Find(Function(fi) fi.Sender = Name)
                If File Is Nothing Then 'ako nema
                    File = New MessageFile(e.FullPath, False, Name, My.Settings.Username)
                    Files.Add(File)
                Else
                    File.ReadNew()
                End If
                Notify(NotificationType, Name)
                If Channels.Current = Channels.PublicChannel OrElse Channels.Current = Name Then 'ako je korisnik na kanalu pošiljatelja
                    While File.NewQueueLength > 0
                        OutputHtmlMessages.LoadMessageToOutput(File.GetNextNewer)
                    End While
                End If
            Case Files.Extension.Ping
                If DirName = My.Settings.Username And e.ChangeType = WatcherChangeTypes.Created Then
                    MoveFile(e.FullPath, Path.Combine(MessagesDir, Name, My.Settings.Username & Files.Extension.Pong))
                End If
            Case Files.Extension.Pong
                If DirName = My.Settings.Username And e.ChangeType = WatcherChangeTypes.Created Then
                    GetPing()
                    File.Delete(e.FullPath)
                End If
        End Select

        MessagesWatcher.EnableRaisingEvents = True
    End Sub

    Private Sub UserFilesWatcher_Changed(sender As Object, e As FileSystemEventArgs) Handles UserFilesWatcher.Changed
        Dim Name As String = Path.GetFileNameWithoutExtension(e.Name)

        If Name = My.Settings.Username Then Exit Sub

        If Path.GetExtension(e.FullPath) <> Files.Extension.UserInfo Then Exit Sub

        Dim Foo As UserInfoFile = UserInfoFiles.Others.Find(Function(x) x.Name = Name)
        If Foo Is Nothing Then
            UserInfoFiles.Others.Add(New UserInfoFile(e.FullPath) With {.Name = Name})
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
        UserInfoFiles.Others.Remove(UserInfoFiles.Others.Find(Function(x) x.Name = Name))
        MessageFiles.OutgoingPrivate.Remove(MessageFiles.OutgoingPrivate.Find(Function(x) x.Sender = Name))
        MessageFiles.IngoingPrivate.Remove(MessageFiles.IngoingPrivate.Find(Function(x) x.Sender = Name))

        If Channels.Current = Name Then SwitchChannel(Channels.PublicChannel)
    End Sub

    Private Sub DirectoriesWatcher_Renamed(sender As Object, e As RenamedEventArgs) Handles DirectoriesWatcher.Renamed
        If Channels.Current = e.OldName Then SwitchChannel(Path.GetFileName(e.FullPath))
    End Sub
End Class
