Imports System.IO

Partial Public Class SharedFolderCI : Implements ICommunicationInterface

    Private Structure ChannelNewMessagesStart
        Dim Channel As String
        Dim NewMessagesStart As Long
    End Structure

    Private ChannelNewMessagesStarts As New List(Of ChannelNewMessagesStart)

    Public Event NewMessage(message As FolMessage) Implements ICommunicationInterface.NewMessage
    Public Event PongReceived(rtt_in_ms As Long) Implements ICommunicationInterface.PongReceived
    Public Event PongTimeout(username As String) Implements ICommunicationInterface.PongTimeout
    Public Event PingError(message As String) Implements ICommunicationInterface.PingError

    Public Sub Start(synchronizingObject As Form) Implements ICommunicationInterface.Start
        EnableFSWatchers(SynchronizingObject)
        CleanPing()
    End Sub

    Public Sub SendMessage(channel As String, message As FolMessage) Implements ICommunicationInterface.SendMessage
        Dim dirPath As String
        If channel = Channels.Common Then
            dirPath = Path.Combine(Dirs.CommonChannel, message.Sender)
        Else
            dirPath = Path.Combine(Dirs.PrivateMessages, channel, message.Sender)
        End If
        Dirs.Create(dirPath)
        MessageFile.Create(dirPath, message)
    End Sub

    Public Sub Ping(username As String) Implements ICommunicationInterface.Ping
        _Ping(username)
    End Sub

    Public Sub LoadOldMessages(channel As String, count As Integer, loadSubRef As ICommunicationInterface.MessageLoadingSub) Implements ICommunicationInterface.LoadOldMessages
        Dim msgFilePaths As List(Of String)
        If channel = Channels.Common Then
            msgFilePaths = New List(Of String)(Directory.GetFiles(Dirs.CommonChannel,
                                                              "*" & MessageFile.Extension, SearchOption.AllDirectories))
        Else
            Dim messagesPath As String = Path.Combine(Dirs.PrivateMessages, channel, My.Settings.Username)
            Dirs.Create(messagesPath)
            msgFilePaths = New List(Of String)(Directory.GetFiles(messagesPath))
            messagesPath = Path.Combine(Dirs.PrivateMessages, My.Settings.Username, channel)
            Dirs.Create(messagesPath)
            msgFilePaths.AddRange(Directory.GetFiles(messagesPath))
        End If
        msgFilePaths.Sort(AddressOf MessageFile.Comparison)
        Dim oldestToDisplay As Integer = Math.Max(0, msgFilePaths.Count - 1 - count)
        Dim NewBeginningTime As Long = Long.MaxValue
        For Each cnms As ChannelNewMessagesStart In ChannelNewMessagesStarts
            If cnms.Channel = channel Then
                NewBeginningTime = cnms.NewMessagesStart
            End If
        Next
        For i As Integer = oldestToDisplay To msgFilePaths.Count - 1
            Dim msg As FolMessage = MessageFile.LoadMessage(msgFilePaths(i))
            If msg.Time > NewBeginningTime Then Exit For
            loadSubRef(msg)
        Next
        For i As Integer = 0 To oldestToDisplay - 1 'deletes all not displayed messages
            Try
                File.Delete(msgFilePaths(i))
            Catch ex As Exception
            End Try
        Next
    End Sub

End Class
