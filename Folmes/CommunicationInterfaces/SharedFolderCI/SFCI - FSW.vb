Imports System.IO
Imports Folmes

Partial Public Class SharedFolderCI
    Private WithEvents MessagesWatcher As FileSystemWatcher
    Private WithEvents PingPongWatcher As FileSystemWatcher

    Sub EnableFSWatchers(SynchronizingObject As Form)
        MessagesWatcher = New FileSystemWatcher(Dirs.Messages, "*" & MessageFile.Extension) With {
            .IncludeSubdirectories = True,
            .NotifyFilter = NotifyFilters.LastWrite,
            .EnableRaisingEvents = True,
            .SynchronizingObject = SynchronizingObject
        }
        PingPongWatcher = New FileSystemWatcher(Path.Combine(Dirs.PingPong, My.Settings.Username), "*.*") With {
            .IncludeSubdirectories = False,
            .NotifyFilter = NotifyFilters.LastWrite,
            .EnableRaisingEvents = True,
            .SynchronizingObject = SynchronizingObject
        }
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

        Dim message As Message = MessageFile.LoadMessage(e.FullPath)
        If recipientChannel = Channels.Common Then
            RaiseEvent NewCommonMessage(message)
        Else
            RaiseEvent NewPrivateMessage(message)
        End If
    End Sub

    Private Sub PingPongWatcher_Created(senderObject As Object, e As FileSystemEventArgs) Handles PingPongWatcher.Changed
        File.Delete(e.FullPath)
        If Path.GetExtension(e.Name) = PingPongFile.Extension.Ping Then
            Dim sender As String = e.Name.Substring(0, e.Name.IndexOf("."c))
            PingPong.Pong(sender)
        Else
            RaiseEvent PongReceived(PingPong.GetFileRtt())
        End If
    End Sub

End Class
