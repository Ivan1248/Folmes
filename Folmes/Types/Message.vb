Public Class Message
    Public Sender As User
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
