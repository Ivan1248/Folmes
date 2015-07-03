Public MustInherit Class Channels
    Public Const PublicChannel As String = "Public Channel"

    Public Shared Current As String = PublicChannel 'ovo treba srediti (BOOL)

End Class

Public Class ChannelLastReadTime
    Public Channel As String
    Public Time As Long
End Class

