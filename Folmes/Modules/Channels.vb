Public MustInherit Class Channels
    Public Const Common As String = "Common"

    Public Shared Current As String = Common 'ovo treba srediti (BOOL)

    Public Shared Sub Switch(channel As String)
        SetLastRead()
        Current = channel
    End Sub

    Public Shared Sub SetLastRead()
        If My.Settings.LastReadTimes IsNot Nothing Then
            For Each lrt As ChannelLastReadTime In My.Settings.LastReadTimes
                If lrt.Channel = Current Then
                    lrt.Time = Time.UtcNow.ToBinary
                    Exit Sub
                End If
            Next
        End If
        My.Settings.LastReadTimes.Add(
            New ChannelLastReadTime With {.Channel = Current, .Time = Time.UtcNow.ToBinary})
    End Sub

    Public Shared Function AnyNewMessages(channel As String) As Boolean ' TODO prepraviti
        Dim last As ChannelLastReadTime = My.Settings.LastReadTimes.Find(Function(e) e.Channel = channel)
        If last Is Nothing Then Return True
        Return False
    End Function

End Class

Public Class ChannelLastReadTime
    Public Channel As String
    Public Time As Long
End Class

