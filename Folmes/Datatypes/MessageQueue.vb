
Public Class MessageQueue : Inherits AbstractLeakyQueue(Of Message)
    Public Sub New(capacity As Integer)
        MyBase.New(capacity)
    End Sub

    Public Property Id As String
End Class