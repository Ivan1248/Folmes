Public Class Message
    Public Sender As String
    Public Flags As MessageFlags
    Public Content As String
    Public Time As Long
End Class

Public Enum MessageFlags As Integer
    FolmesSystemMessage = 1
    Privat = 2
    MeIs = 4
    Irc = 8
    Highlighted = 16
End Enum
