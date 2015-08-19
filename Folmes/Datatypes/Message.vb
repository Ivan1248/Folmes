Public Class FolMessage
    Public Sender As String
    Public Flags As FolMessageFlags
    Public HtmlContent As String
    Public Time As Long

    Public Shared Operator =(message1 As FolMessage, message2 As FolMessage) As Boolean
        Return message1.Time = message2.Time AndAlso message1.Sender = message2.Sender
    End Operator

    Public Shared Operator <>(message1 As FolMessage, message2 As FolMessage) As Boolean
        Return message1.Time <> message2.Time OrElse message1.Sender <> message2.Sender
    End Operator
End Class

Public Enum FolMessageFlags As Integer
    None = 0
    FolmesSystemMessage = 1
    Privat = 2
    MeIs = 4
    NonFolmesIrc = 8
    Highlighted = 16
End Enum
