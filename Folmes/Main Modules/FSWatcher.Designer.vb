Imports System.IO
Imports Folmes.Classes

Partial Class Box
    Private WithEvents FilesWatcher As FileSystemWatcher
    Private WithEvents DirectoriesWatcher As FileSystemWatcher
    Private Sub LoadFSWatchers()
        FilesWatcher = New FileSystemWatcher(MessagesDir, "*.*")
        With FilesWatcher
            .IncludeSubdirectories = True
            .NotifyFilter = NotifyFilters.FileName Or NotifyFilters.LastWrite
            .SynchronizingObject = Me
            .EnableRaisingEvents = True
        End With
        DirectoriesWatcher = New FileSystemWatcher(MessagesDir, "*.*")
        With DirectoriesWatcher
            .IncludeSubdirectories = False
            .NotifyFilter = NotifyFilters.DirectoryName
            .SynchronizingObject = Me
            .EnableRaisingEvents = True
        End With
    End Sub
    Private Sub FilesWatcher_ChangedOrCreated(sender As Object, e As FileSystemEventArgs) Handles FilesWatcher.Changed, FilesWatcher.Created
        Dim Name As String = Path.GetFileNameWithoutExtension(e.Name)
        Dim DirPath As String = Path.GetDirectoryName(e.FullPath)
        Dim DirName As String = Path.GetFileName(DirPath)
        Select Case Path.GetExtension(e.FullPath)
            Case "fmsg"
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
            Case "info"
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
            Case "ping"
                If DirName = My.Settings.Username And e.ChangeType = WatcherChangeTypes.Created Then
                    MoveFile(e.FullPath, Path.Combine(MessagesDir, Name, My.Settings.Username & ".pong"))
                End If
            Case "pong"
                If DirName = My.Settings.Username And e.ChangeType = WatcherChangeTypes.Created Then
                    GetPing()
                    File.Delete(e.FullPath)
                End If
        End Select
    End Sub

    Private Sub DirectoriesWatcher_Renamed(sender As Object, e As RenamedEventArgs) Handles DirectoriesWatcher.Renamed
        If Channels.Current = e.OldName Then SwitchChannel(Path.GetFileName(e.FullPath))
    End Sub
End Class
