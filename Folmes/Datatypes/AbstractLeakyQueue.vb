Public Class AbstractLeakyQueue(Of T) : Implements ILeakyQueue(Of T)
    ReadOnly _array As T()
    Dim _newest As Integer = -1
    Dim _oldest As Integer = 0

    Sub New(capacity As Integer)
        Me.Capacity = 1
        While Me.Capacity < capacity
            Me.Capacity <<= 1
        End While
        _array = New T(Me.Capacity) {}
        Me.Capacity -= 1
    End Sub

    Public ReadOnly Property Capacity As Integer Implements ILeakyQueue(Of T).Capacity
    Public Property Count As Integer Implements ILeakyQueue(Of T).Count

    Public Function Dequeue() As T Implements ILeakyQueue(Of T).Dequeue
        Dequeue = _array(_oldest)
        _oldest = Incr(_oldest)
        Count -= 1
    End Function

    Public Function Peek() As T Implements ILeakyQueue(Of T).Peek
        Return _array(_oldest)
    End Function

    Public Sub Enqueue(item As T) Implements ILeakyQueue(Of T).Enqueue
        _newest = Incr(_newest)
        _array(_newest) = item
        If Count = Capacity + 1 Then
            _oldest = Incr(_oldest)
        Else
            Count += 1
        End If
    End Sub

    Public Sub Clear() Implements ILeakyQueue(Of T).Clear
        _newest = Nothing
        _oldest = Nothing
    End Sub

    Private Function Incr(i As Integer) As Integer
        Return (i + 1) And Capacity
    End Function
End Class
