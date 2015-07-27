Imports System.IO

Public MustInherit Class MessagesManager
    Public Shared CommonNewQueue As New OrderedMessageList()
    Public Shared PrivateNewQueue As New Dictionary(Of String, OrderedMessageList)
    
    Public Shared Sub AddPrivateNew(channel As String, message As Message)
        Dim oml As OrderedMessageList = Nothing
        For Each m As KeyValuePair(Of String, OrderedMessageList) In PrivateNewQueue
            If m.Key = channel Then
                oml = m.Value
                Exit For
            End If
        Next
        If oml Is Nothing Then
            oml = New OrderedMessageList()
            PrivateNewQueue.Add(channel, oml)
        End If
        oml.Add(message)
    End Sub

    Delegate Sub LoadSub(m As Message)
    Public Shared Sub LoadInitial(channel As String, loadSub As LoadSub) ' TODO optimizirati - ne mora se sortirati cijeli niz
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
            For Each m As KeyValuePair(Of String, OrderedMessageList) In PrivateNewQueue
                If m.Key = channel Then m.Value.Clear()
                Exit For
            Next
        End If
        msgFilePaths.Sort(AddressOf MessageFileComparison)
        For i As Integer = Math.Max(msgFilePaths.Count - 1 - My.Settings.NofMsgs, 0) To msgFilePaths.Count - 1
            loadSub(MessageFile.LoadMessage(msgFilePaths(i)))
        Next
    End Sub

    Public Shared Sub LoadNew(channel As String, loadsub As LoadSub)
        Dim oml As OrderedMessageList = Nothing
        If channel = Channels.Common Then
            oml = CommonNewQueue
        Else
            For Each m As KeyValuePair(Of String, OrderedMessageList) In PrivateNewQueue
                If m.Key = channel Then
                    oml = m.Value
                    Exit For
                End If
            Next
            If oml Is Nothing Then Exit Sub
        End If
        While oml.Count > 0
            loadsub(oml.PopOldest)
        End While
    End Sub

    Private Shared Function MessageFileComparison(x As String, y As String) As Integer
        Dim a As Integer = x.Length - Extension.Message.Length - 16
        Dim b As Integer = y.Length - Extension.Message.Length - 16
        For i As Integer = 0 To 15
            Select Case Asc(x(a + 1)) - Asc(y(b + i))
                Case Is > 0 : Return 1
                Case Is < 0 : Return -1
            End Select
        Next
        Return 0
    End Function
End Class
