Imports System.IO

Public MustInherit Class UsersWatcher
    Private Shared WithEvents fsw As New FileSystemWatcher With {
        .IncludeSubdirectories = True,
        .NotifyFilter = NotifyFilters.LastWrite Or NotifyFilters.DirectoryName
    }

    Public Shared Event Deleted(username As String)

    Public Shared Sub Start()
        fsw.SynchronizingObject = MainGUI
        fsw.EnableRaisingEvents = True
    End Sub

    Private Shared Sub Changed(sender As Object, e As FileSystemEventArgs) Handles fsw.Changed
        If Directory.Exists(e.FullPath) Then Exit Sub ' directory
        If e.ChangeType <> WatcherChangeTypes.Changed Then
            Exit Sub
        End If
        Dim ext As String = IO.Path.GetExtension(e.Name)
        If ext <> Files.Extension.UserStatus OrElse ext <> Files.Extension.UserInfo Then
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
            Select Case ext
                Case Files.Extension.UserStatus
                    Dim PrevStatus As Boolean = user.IsOnline()
                    user.RefreshStatus()
                    If PrevStatus <> user.IsOnline() Then
                        Notify(If(Not PrevStatus, NotificationType.LoggedIn, NotificationType.LoggedOut), name)
                    End If
                Case Files.Extension.UserInfo
                    user.RefreshSettings()
            End Select
        End If
    End Sub

    Private Shared Sub fsw_Deleted(sender As Object, e As FileSystemEventArgs) Handles fsw.Deleted
        If Not Directory.Exists(e.FullPath) Then Exit Sub ' not a directory
        RaiseEvent Deleted(e.Name)
    End Sub
End Class
