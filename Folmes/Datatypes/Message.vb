Namespace Datatypes
    Public Class Message
        Public Sender As String
        Public Type As Message.Kind
        Public Content As String
        Public Time As Long

        Enum Kind As Short
            Normal
            Reflexive
            Declaration
        End Enum
    End Class
End Namespace