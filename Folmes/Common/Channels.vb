Imports System.Linq

Public MustInherit Class Channels
    Public Const Common As String = "Common"

    Public Shared Current As String = Common 'ovo treba srediti (BOOL)

    Public Shared Sub Switch(channel As String)
        SetLastRead()
        Current = channel
    End Sub

    Public Shared Sub SetLastRead()
        Dim lrt As ChannelLastReadTime = My.Settings.LastReadTimes.FirstOrDefault(Function(t) t.Channel.Equals(Current))
        If lrt IsNot Nothing Then
            lrt.Time = NetworkTime.UtcNow.ToBinary
        Else
            My.Settings.LastReadTimes.Add(New ChannelLastReadTime With {.Channel = Current, .Time = NetworkTime.UtcNow.ToBinary})
        End If
    End Sub

    Public Shared Function AnyNewMessages(channel As String) As Boolean ' TODO prepraviti
        Return My.Settings.LastReadTimes.Find(Function(e) e.Channel = channel) Is Nothing
    End Function
End Class

Public Class ChannelLastReadTime
    Public Channel As String
    Public Time As Long
End Class

