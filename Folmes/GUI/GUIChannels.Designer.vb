Partial Public Class MainGUI

    Public Function AnyNewMessages(channel As String) As Boolean ' TODO prepraviti
        Dim last As ChannelLastReadTime = My.Settings.LastReadTimes.Find(Function(e) e.Channel = channel)
        If last Is Nothing Then Return True
        'Dim msgFile As MessageFile = MessageFiles.IngoingPrivate.Find(Function(e) e.Sender = channel)
        'If msgFile Is Nothing Then Return False
        'Return msgFile.NewQueueLength > 0 OrElse msgFile.NextUnreadOldTime > last.Time
        Return False
    End Function

    Private Sub ReloadPrivateChannelsToMenu()
        While TSChannels.DropDownItems.Count > 2
            TSChannels.DropDownItems.Item(2).Dispose()
        End While
        Dim showSeparator As Boolean = False
        For Each user As Users.User In Users.Others
            showSeparator = True
            Dim channelTsMenuItem As New ToolStripMenuItem(user.Name) With {.ForeColor = Color.FromArgb(176, 176, 176)}
            Dim status As Integer = 0
            If user.IsOnline Then status = 1
            If Channels.Current <> user.Name AndAlso AnyNewMessages(user.Name) Then status = status Or 2
            With channelTsMenuItem
                Select Case status
                    Case 0 : .Image = Nothing
                    Case 1 : .Image = My.Resources.online
                    Case 2 : .Image = My.Resources.newmsg
                    Case 1 + 2 : .Image = My.Resources.newmsg_online
                End Select
            End With
            TSChannels.DropDownItems.Add(channelTsMenuItem)
            AddHandler channelTsMenuItem.Click, AddressOf Channel_Click
        Next
        PubPrivChSeparator.Visible = showSeparator
    End Sub

    Private Sub SwitchChannel(channel As String)
        If channel <> Channels.Current Then
            Channels.SetLastRead()
            Me.Output.CacheChannelHtml(Channels.Current)
            With TSChannels
                If channel = Channels.Common Then
                    Channels.Switch(channel)
                    .AutoSize = True
                    TSChat.Visible = False
                Else 'PRIVATE
                    Channels.Switch(channel)
                    If .Width < 32 Then
                        .AutoSize = False
                        .Width = 32
                    Else
                        .AutoSize = True
                    End If
                    TSChat.Visible = Users.IsOnline(channel)
                End If
                .Text = channel
            End With
        End If
    End Sub

    Private Sub TSChannels_Opening(sender As Object, e As EventArgs) Handles TSChannels.DropDownOpening
        ReloadPrivateChannelsToMenu()
    End Sub

    Private Sub Channel_Click(sender As Object, e As EventArgs) Handles PublicChannel.Click
        SwitchChannel(DirectCast(sender, ToolStripMenuItem).Text)
    End Sub

End Class