Imports System.IO

Public MustInherit Class MessagesManager
    Private Shared CommonNewQueue As New MessageQueue(My.Settings.NofMsgs)
    Private Shared PrivateNewQueue As New List(Of MessageQueue)

    Public Shared Sub AddPrivateNew(channel As String, message As Message)
        If Channels.Current = channel Then
            MainGUI.Output.AddMessage(message)
            Exit Sub
        End If
        Dim oml As MessageQueue = Nothing
        For Each m As MessageQueue In PrivateNewQueue
            If m.Id = channel Then
                oml = m
                Exit For
            End If
        Next
        If oml Is Nothing Then
            oml = New MessageQueue(My.Settings.NofMsgs) With {.Id = channel}
            PrivateNewQueue.Add(oml)
        End If
        oml.Enqueue(message)
    End Sub

    Public Shared Sub AddCommonNew(message As Message)
        If Channels.Current = Channels.Common Then
            MainGUI.Output.AddMessage(message)
        Else
            CommonNewQueue.Enqueue(message)
        End If
    End Sub

    Private Shared Function MessageFileComparison(file1 As String, file2 As String) As Integer
        Dim a As Integer = file1.Length - Extension.Message.Length - 16
        Dim b As Integer = file2.Length - Extension.Message.Length - 16
        For i As Integer = 0 To 15
            Select Case Asc(file1(a + i)) - Asc(file2(b + i))
                Case Is > 0 : Return 1
                Case Is < 0 : Return -1
            End Select
        Next
        Return 0
    End Function
    Delegate Sub LoadSub(m As Message)
    Public Shared Sub LoadInitial(channel As String) ' TODO optimizirati - ne mora se sortirati cijeli niz
        Dim msgFilePaths As List(Of String)
        If channel = Channels.Common Then
            msgFilePaths = New List(Of String)(Directory.GetFiles(Dirs.CommonChannel,
                                                              "*" & Extension.Message, SearchOption.AllDirectories))
        Else
            Dim messagesPath As String = Path.Combine(Dirs.PrivateMessages, channel, My.Settings.Username)
            Dirs.Create(messagesPath)
            msgFilePaths = New List(Of String)(Directory.GetFiles(messagesPath))
            messagesPath = Path.Combine(Dirs.PrivateMessages, My.Settings.Username, channel)
            Dirs.Create(messagesPath)
            msgFilePaths.AddRange(Directory.GetFiles(messagesPath))
            For Each m As MessageQueue In PrivateNewQueue
                If m.Id = channel Then m.Clear()
                Exit For
            Next
        End If
        msgFilePaths.Sort(AddressOf MessageFileComparison)
        Dim oldestToDisplay As Integer = Math.Max(0, msgFilePaths.Count - 1 - My.Settings.NofMsgs)
        For i As Integer = oldestToDisplay To msgFilePaths.Count - 1
            MainGUI.Output.AddMessage(MessageFile.LoadMessage(msgFilePaths(i)))
        Next
        For i As Integer = 0 To oldestToDisplay - 1 'deletes all not displayed messages
            Try
                File.Delete(msgFilePaths(i))
            Catch ex As Exception
            End Try
        Next
    End Sub

    Public Shared Sub LoadNew(channel As String)
        Dim oml As MessageQueue = Nothing
        If channel = Channels.Common Then
            oml = CommonNewQueue
        Else
            For Each m As MessageQueue In PrivateNewQueue
                If m.Id = channel Then
                    oml = m
                    Exit For
                End If
            Next
            If oml Is Nothing Then Exit Sub
        End If
        While oml.Count > 0
            MainGUI.Output.AddMessage(oml.Dequeue)
        End While
    End Sub

    Public Shared Function LoadMessage(fpath As String) As Message
        Return MessageFile.LoadMessage(fpath)
        'TODO: ++
    End Function

    Public Shared Sub CreateMessageFile(channel As String, msg As Message)
        Dim dirPath As String
        If channel = Channels.Common Then
            dirPath = Path.Combine(Dirs.CommonChannel, msg.Sender)
        Else
            dirPath = Path.Combine(Dirs.PrivateMessages, channel, msg.Sender)
        End If
        Dirs.Create(dirPath)
        Dim filePath As String = Path.Combine(dirPath, Convert.ToString(msg.Time, 16) & Extension.Message)
        MessageFile.Create(filePath, msg)
        'TODO: ++
    End Sub

End Class
