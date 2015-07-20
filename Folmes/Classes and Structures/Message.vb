Public Class Message
    Public Sender As String
    Public Type As MessageType
    Public Content As String
    Public Time As Long
End Class
Public Enum MessageType As Short
    Normal
    Highlighted
    FolmesDeclaration
    Reflexive
End Enum

Public Class OrderedMessageList
    Public Count As Integer = 0
    Dim _newest As MessageListNode
    Dim _oldest As MessageListNode

    Public Function PopOldest() As Message
        PopOldest = _oldest.Message
        RemoveOldest()
    End Function

    Public Sub Add(message As Message)
        If Count = 0 Then
            _newest = New MessageListNode(message)
            _oldest = _newest
        Else
            Dim mln As New MessageListNode(message)
            Select Case message.Time
                Case Is > _newest.Message.Time
                    _newest.Succeeding = mln
                    mln.Preceeding = _newest
                    _newest = mln
                Case Is < _oldest.Message.Time
                    _oldest.Preceeding = mln
                    mln.Succeeding = _newest
                    _oldest = mln
                Case Else
                    Dim preceeding As MessageListNode = _newest.Preceeding
                    While preceeding.Message.Time > message.Time
                        preceeding = preceeding.Preceeding
                    End While
                    mln.Preceeding = preceeding
                    mln.Succeeding = preceeding.Succeeding

                    mln.Succeeding.Preceeding = mln
                    preceeding.Succeeding = mln
            End Select
        End If
        Count += 1
    End Sub

    Public Sub RemoveOldest()
        _oldest = _oldest.Succeeding
        _oldest.Preceeding = Nothing
    End Sub

    Public Sub Clear()
        _newest = Nothing
        _oldest = Nothing
    End Sub

    Private Class MessageListNode
        Public ReadOnly Message As Message
        Public Preceeding As MessageListNode
        Public Succeeding As MessageListNode
        Sub New(message As Message)
            Me.Message = message
        End Sub
    End Class
End Class
