Imports System.IO
Imports Folmes

Partial Public Class SharedFolderCI : Implements ICommunicationInterface
    Public Event NewCommonMessage(message As Message) Implements ICommunicationInterface.NewCommonMessage
    Public Event NewPrivateMessage(message As Message) Implements ICommunicationInterface.NewPrivateMessage
    Public Event PongReceived(rtt_in_ms As Long) Implements ICommunicationInterface.PongReceived
    Public Event PongTimeout() Implements ICommunicationInterface.PongTimeout
    Public Event PingError(message As String) Implements ICommunicationInterface.PingError

    'TODO: automatic deletion of old message files

    Private Structure ChannelNewMessagesStart
        Dim Channel As String
        Dim NewMessagesStart As Long
    End Structure

    Private ChannelNewMessagesStarts As New List(Of ChannelNewMessagesStart)

    Public Sub Start(SynchronizingObject As Form) Implements ICommunicationInterface.Start
        EnableFSWatchers(SynchronizingObject)
        PingPong.CleanPing()
    End Sub

    Public Sub SendMessage(channel As String, message As Message) Implements ICommunicationInterface.SendMessage
        Dim dirPath As String
        If channel = Channels.Common Then
            dirPath = Path.Combine(Dirs.CommonChannel, message.Sender.Name)
        Else
            dirPath = Path.Combine(Dirs.PrivateMessages, channel, message.Sender.Name)
        End If
        Dirs.Create(dirPath)
        Dim filePath As String = Path.Combine(dirPath, Convert.ToString(message.Time, 16) & MessageFile.Extension)
        MessageFile.Create(filePath, message)
    End Sub

    Public Function Ping(username As String) As Boolean Implements ICommunicationInterface.Ping
        Return PingPong.Ping(username)
    End Function

    Public Sub GetOldMessages(channel As String, count As Integer, loadSubRef As ICommunicationInterface.MessageLoadingSub) Implements ICommunicationInterface.GetOldMessages
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
        msgFilePaths.Sort(AddressOf MessageFileComparison)
        Dim oldestToDisplay As Integer = Math.Max(0, msgFilePaths.Count - 1 - count)
        Dim NewBeginningTime As Long = Long.MaxValue
        For Each cnms As ChannelNewMessagesStart In ChannelNewMessagesStarts
            If cnms.Channel = channel Then
                NewBeginningTime = cnms.NewMessagesStart
            End If
        Next
        For i As Integer = oldestToDisplay To msgFilePaths.Count - 1
            Dim msg As Message = MessageFile.LoadMessage(msgFilePaths(i))
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

    Private Shared Function MessageFileComparison(file1 As String, file2 As String) As Integer
        Dim a As Integer = file1.Length - MessageFile.Extension.Length - 16
        Dim b As Integer = file2.Length - MessageFile.Extension.Length - 16
        For i As Integer = 0 To 15
            Select Case Asc(file1(a + i)) - Asc(file2(b + i))
                Case Is > 0 : Return 1
                Case Is < 0 : Return -1
            End Select
        Next
        Return 0
    End Function

End Class
