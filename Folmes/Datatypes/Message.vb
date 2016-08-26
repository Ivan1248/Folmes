Public Class Message
    Public Sender As String
    Public Flags As MessageFlags
    Public HtmlContent As String
    Public Time As Long

    Public Shared Operator =(message1 As Message, message2 As Message) As Boolean
        Return message1.Time = message2.Time AndAlso message1.Sender = message2.Sender
    End Operator

    Public Shared Operator <>(message1 As Message, message2 As Message) As Boolean
        Return message1.Time <> message2.Time OrElse message1.Sender <> message2.Sender
    End Operator
End Class

Public Enum MessageFlags As Integer
    None = 0
    FolmesSystemMessage = 1
    Privat = 2
    MeIs = 4
    NonFolmesIrc = 8
    Highlighted = 16
End Enum
