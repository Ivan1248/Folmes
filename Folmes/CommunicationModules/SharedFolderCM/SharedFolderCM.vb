Imports System.IO

Partial Public Class SharedFolderCM : Implements ICommunicationModule
    Private Structure ChannelNewMessagesStart
        Dim Channel As String
        Dim NewMessagesStartTime As Long  ' Time in ticks that displayed messasges are newer than
    End Structure

    Private ChannelNewMessagesStarts As New List(Of ChannelNewMessagesStart)

    Public Event MessageReceived(message As Message) Implements ICommunicationModule.MessageReceived
    Public Event PongReceived(rtt_in_ms As Long) Implements ICommunicationModule.PongReceived
    Public Event PongTimeout(username As String) Implements ICommunicationModule.PongTimeout
    Public Event PingError(message As String) Implements ICommunicationModule.PingError
    Public Event Connected(userName As String) Implements ICommunicationModule.Connected
    Public Event LostConnection() Implements ICommunicationModule.LostConnection

    Public ReadOnly Property IsConnected As Boolean = True Implements ICommunicationModule.IsConnected

    Public Sub Initialize(synchronizingObject As Form) Implements ICommunicationModule.Initialize
        EnableFSWatchers(synchronizingObject)
        InitializePingPong()
    End Sub

    Public Sub SendMessage(channel As String, message As Message) Implements ICommunicationModule.SendMessage
        Dim dirPath As String
        If channel = Channels.Common Then
            dirPath = IO.Path.Combine(Dirs.CommonChannel, message.Sender)
        Else
            dirPath = IO.Path.Combine(Dirs.PrivateMessages, channel, message.Sender)
        End If
        Dirs.Create(dirPath)
        MessageFile.Create(dirPath, message)
    End Sub

    Public Sub Ping(username As String) Implements ICommunicationModule.Ping
        _Ping(username)
    End Sub

    Public Sub LoadOldMessages(channel As String, count As Integer, loader As ICommunicationModule.MessageLoader) Implements ICommunicationModule.LoadOldMessages
        Dim msgFilePaths As List(Of String)
        If channel = Channels.Common Then
            msgFilePaths = New List(Of String)(Directory.GetFiles(Dirs.CommonChannel,
                                                              "*" & MessageFile.Extension, SearchOption.AllDirectories))
        Else
            Dim messagesPath As String = IO.Path.Combine(Dirs.PrivateMessages, channel, My.Settings.Username)
            Dirs.Create(messagesPath)
            msgFilePaths = New List(Of String)(Directory.GetFiles(messagesPath))
            messagesPath = IO.Path.Combine(Dirs.PrivateMessages, My.Settings.Username, channel)
            Dirs.Create(messagesPath)
            msgFilePaths.AddRange(Directory.GetFiles(messagesPath))
        End If
        msgFilePaths.Sort(AddressOf MessageFile.RecentnessComparison)
        Dim oldestToDisplay As Integer = Math.Max(0, msgFilePaths.Count - 1 - count)
        Dim newBeginningTime As Long = Long.MaxValue
        For Each cnms As ChannelNewMessagesStart In ChannelNewMessagesStarts
            If cnms.Channel = channel Then newBeginningTime = cnms.NewMessagesStartTime
        Next
        For i As Integer = oldestToDisplay To msgFilePaths.Count - 1
            Dim msg As Message = MessageFile.LoadMessage(msgFilePaths(i))
            If msg.Time > newBeginningTime Then Exit For
            loader(msg)
        Next
        For i As Integer = 0 To oldestToDisplay - 1 ' deletes all not displayed messages
            Try
                File.Delete(msgFilePaths(i))
            Catch ex As Exception
                Debug.WriteLine(Me.GetType().ToString() + ": " + ex.Message)
            End Try
        Next
    End Sub
End Class
