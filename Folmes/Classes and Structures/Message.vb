Namespace Datatypes
    Public Class Message
        Public Sender As String
        Public Type As MessageType
        Public Content As String
        Public Time As Long
    End Class
    Public Enum MessageType As Short
        Normal
        Reflexive
        Declaration
    End Enum
End Namespace