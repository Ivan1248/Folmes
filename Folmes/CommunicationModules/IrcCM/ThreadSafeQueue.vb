Imports System.Threading

Public Class ThreadSafeQueue(Of T)

    Private ReadOnly _lockObject As New Object
    Private ReadOnly _queue As Queue(Of T)

    Sub New()
        _queue = New Queue(Of T)
    End Sub

    Sub New(capacity As Integer)
        _queue = New Queue(Of T)(capacity)
    End Sub

    Public ReadOnly Property Count As Integer
        Get
            SyncLock _lockObject
                Return _queue.Count
            End SyncLock
        End Get
    End Property

    Public Sub Enqueue(item As T)
        SyncLock _lockObject
            _queue.Enqueue(item)
        End SyncLock
    End Sub

    Public Function Dequeue() As T
        SyncLock _lockObject
            Return _queue.Dequeue()
        End SyncLock
    End Function

End Class