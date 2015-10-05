Public Class MessageQueue
    Public Id As String
    Public Count As Integer = 0
    Dim _array As FolMessage()
    Dim _modMask As Integer
    Dim _newest As Integer = -1
    Dim _oldest As Integer = 0

    Sub New(maxCount As Integer)
        _modMask = 1
        While _modMask < maxCount
            _modMask <<= 1
        End While
        _array = New FolMessage(_modMask) {}
        _modMask -= 1
    End Sub

    Public Function Dequeue() As FolMessage
        Dequeue = _array(_oldest)
        _oldest = Incr(_oldest)
        Count -= 1
    End Function

    Public Function Peek() As FolMessage
        Return _array(_oldest)
    End Function

    Public Sub Enqueue(message As FolMessage)
        _newest = Incr(_newest)
        _array(_newest) = message
        If Count = _modMask + 1 Then
            _oldest = Incr(_oldest)
        Else
            Count += 1
        End If
    End Sub

    Public Sub Clear()
        _newest = Nothing
        _oldest = Nothing
    End Sub

    Private Function Incr(i As Integer) As Integer
        Return (i + 1) And _modMask
    End Function

End Class
