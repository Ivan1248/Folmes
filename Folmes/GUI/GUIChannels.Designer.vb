Imports Folmes.Classes

Partial Public Class MainGUI

    Public Function AnyNewMessages(channel As String) As Boolean
        Dim last As ChannelLastReadTime = My.Settings.LastReadTimes.Find(Function(e) e.Channel = channel)
        If last Is Nothing Then Return True
        Dim msgFile As MessageFile = MessageFiles.IngoingPrivate.Find(Function(e) e.Sender = channel)
        If msgFile Is Nothing Then Return False
        Return msgFile.NewQueueLength > 0 OrElse msgFile.NextUnreadOldTime > last.Time
    End Function

    Private Sub ReloadPrivateChannelsToMenu()
        While TSChannels.DropDownItems.Count > 2
            TSChannels.DropDownItems.Item(2).Dispose()
        End While
        Dim showSeparator As Boolean = False
        For Each userFile As UserInfoFile In UserInfoFiles.Others
            showSeparator = True
            Dim channelTsMenuItem As New ToolStripMenuItem(userFile.Username) With {.ForeColor = Color.FromArgb(176, 176, 176)}
            Dim status As Integer = 0
            If userFile.Online Then status = 1
            If Channels.Current <> userFile.Username AndAlso AnyNewMessages(userFile.Username) Then status = status Or 2
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
            SetLastRead()
            Me.Output.CacheChannelHtml(Channels.Current)
            With TSChannels
                If channel = Channels.Common Then
                    Channels.Current = Channels.Common
                    MessageFiles.SwitchCommonChannel()
                    .AutoSize = True
                    TSChat.Visible = False
                Else 'PRIVATE
                    Channels.Current = channel
                    MessageFiles.SwitchPrivateChannel(channel)
                    If .Width < 32 Then
                        .AutoSize = False
                        .Width = 32
                    Else
                        .AutoSize = True
                    End If
                    TSChat.Visible = UserInfoFiles.IsOnline(channel)
                End If
                .Text = channel
            End With
            If Not Me.Output.LoadCachedChannelHtml(Channels.Current) Then
                OutputHtmlMessages.LoadInitial_Once()
            End If
            OutputHtmlMessages.LoadNew()
        End If
    End Sub

    Private Sub TSChannels_Opening(sender As Object, e As EventArgs) Handles TSChannels.DropDownOpening
        ReloadPrivateChannelsToMenu()
    End Sub

    Private Sub Channel_Click(sender As Object, e As EventArgs) Handles PublicChannel.Click
        SwitchChannel(DirectCast(sender, ToolStripMenuItem).Text)
    End Sub

    Public Sub SetLastRead()
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