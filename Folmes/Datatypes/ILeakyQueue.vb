Imports Folmes

Public Interface ILeakyQueue(Of T)
    ReadOnly Property Capacity As Integer
    Property Count As Integer
    Sub Clear()
    Sub Enqueue(item As T)
    Function Dequeue() As T
    Function Peek() As T
End Interface
