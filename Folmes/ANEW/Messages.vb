Imports System.IO
Imports Folmes.Datatypes

Public MustInherit Class Messages
    Public Shared CommonNewQueue As New OrderedMessageList()
    Public Shared PrivateNewQueue As Dictionary(Of String, OrderedMessageList)

    Public Shared Sub AddPrivateNew(channel As String, message As Message)
        Dim oml As OrderedMessageList = Nothing
        For Each m As KeyValuePair(Of String, OrderedMessageList) In PrivateNewQueue
            If m.Key = channel Then
                oml = m.Value
                Exit For
            End If
        Next
        If oml Is Nothing Then oml = New OrderedMessageList()
        PrivateNewQueue.Add(channel, oml)
        oml.Add(message)
    End Sub

    Delegate Sub LoadSub(m As Message)
    Public Shared Sub LoadInitial(channel As String, loadSub As LoadSub) ' TODO optimizirati - ne mora se sortirati cijeli niz
        If channel = Channels.Common Then
            Dim msgFilePaths As String() = Directory.GetFiles(Path.Combine(MessagesDir, Channels.Common))
            Array.Sort(Of String)(msgFilePaths, AddressOf MessageFileComparison)
            For i As Integer = msgFilePaths.Length - 1 - My.Settings.NofMsgs To msgFilePaths.Length - 1
                loadSub(MessageFile.LoadMessage(msgFilePaths(i)))
            Next
        Else
            Dim msgFilePaths As New List(Of String)(Directory.GetFiles(Path.Combine(MessagesDir, channel, My.Settings.Username)))
            msgFilePaths.AddRange(Directory.GetFiles(Path.Combine(MessagesDir, My.Settings.Username, channel)))
            msgFilePaths.Sort(AddressOf MessageFileComparison)
            For i As Integer = msgFilePaths.Count - 1 - My.Settings.NofMsgs To msgFilePaths.Count - 1
                loadSub(MessageFile.LoadMessage(msgFilePaths(i)))
            Next
            For Each m As KeyValuePair(Of String, OrderedMessageList) In PrivateNewQueue
                If m.Key = channel Then m.Value.Clear()
                Exit For
            Next
        End If
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
