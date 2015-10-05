Imports System.Threading

Public Class ThreadSafeQueue(Of T)

    Private ReadOnly lockObject As New Object
    Private ReadOnly queue As Queue(Of T)

    Sub New()
        queue = New Queue(Of T)
    End Sub

    Sub New(capacity As Integer)
        queue = New Queue(Of T)(capacity)
    End Sub

    Public ReadOnly Property Count As Integer
        Get
            SyncLock lockObject
                Return queue.Count
            End SyncLock
        End Get
    End Property

    Public Sub Enqueue(ByVal request As T)
        SyncLock lockObject
            queue.Enqueue(request)
        End SyncLock
    End Sub

    Public Function Dequeue() As T
        SyncLock lockObject
            Return queue.Dequeue()
        End SyncLock
    End Function

End Class