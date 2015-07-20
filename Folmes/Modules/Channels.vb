Public MustInherit Class Channels
    Public Const Common As String = "Common"
    Public Shared Users As Dictionary(Of String, Boolean)

    Public Shared Current As String = Common 'ovo treba srediti (BOOL)

    Public Shared Sub Switch(channel As String)
        Current = channel
        If MainGUI.Output.LoadCachedChannelHtml(Channels.Current) Then
            MessagesManager.LoadNew(channel, AddressOf MainGUI.Output.AddMessage)
        Else
            MessagesManager.LoadInitial(channel, AddressOf MainGUI.Output.AddMessage)
        End If
    End Sub

    Public Shared Sub SetLastRead()
        If My.Settings.LastReadTimes IsNot Nothing Then
            For Each lrt As ChannelLastReadTime In My.Settings.LastReadTimes
                If lrt.Channel = Channels.Current Then
                    lrt.Time = DateTime.UtcNow.ToBinary
                    Exit Sub
                End If
            Next
        End If
        My.Settings.LastReadTimes.Add(
            New ChannelLastReadTime With {.Channel = Channels.Current, .Time = DateTime.UtcNow.ToBinary})
    End Sub
End Class

Public Class ChannelLastReadTime
    Public Channel As String
    Public Time As Long
End Class

