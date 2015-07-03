Imports Folmes.Classes

Partial Public Class Box
#Region "Prebacivanje i spremanje kanala, Toolstrip"

    Public Function AnyNewMessages(channel As String) As Boolean
        Dim last As ChannelLastReadTime = My.Settings.LastReadTimes.Find(Function(e) e.Channel = channel)
        If last Is Nothing Then Return True
        Dim msgFile As MessageFile = MessageFiles.IngoingPrivate.Find(Function(e) e.Sender = channel)
        If msgFile Is Nothing Then Return False
        Return msgFile.NewQueueLength > 0 OrElse msgFile.NextUnreadOldTime > last.Time
    End Function

    Private Sub LoadChannelsToMenu()
        While PrivateChannelMenu.DropDownItems.Count > 0
            PrivateChannelMenu.DropDownItems.Item(0).Dispose()
        End While
        Dim privateImage As Integer = 0
        Dim showPrivateChannels As Boolean = False
        Dim username As String
        For Each msgFile As MessageFile In MessageFiles.IngoingCommon
            username = msgFile.Sender
            showPrivateChannels = True
            Dim channelTsMenuItem As New ToolStripMenuItem(Name) With {.ForeColor = Color.FromArgb(176, 176, 176)}
            Dim image As Integer = 0
            If UserInfoFiles.IsOnline(username) Then
                image = 1
                privateImage = privateImage Or 1
            End If
            If Channels.Current <> username AndAlso AnyNewMessages(username) Then
                image = image Or 2
                privateImage = privateImage Or 2
            End If
            With channelTsMenuItem
                Select Case image
                    Case 0 : .Image = Nothing
                    Case 1 : .Image = My.Resources.online
                    Case 2 : .Image = My.Resources.newmsg
                    Case 3 : .Image = My.Resources.newmsg_online
                End Select
            End With
            PrivateChannelMenu.DropDownItems.Add(channelTsMenuItem)
            AddHandler channelTsMenuItem.Click, AddressOf Channel_Click
        Next
        With PrivateChannelMenu
            .Visible = showPrivateChannels
            Select Case privateImage
                Case 0 : .Image = Nothing
                Case 1 : .Image = My.Resources.online
                Case 2 : .Image = My.Resources.newmsg
                Case 3 : .Image = My.Resources.newmsg_online
            End Select
        End With
    End Sub

    Private Sub SwitchChannel(channel As String)
        If channel <> Channels.Current Then
            SetLastRead()
            OutputHtmlMessages.CacheChannelHtml()
            With TSChannels
                If channel = Channels.PublicChannel Then
                    Channels.Current = Channels.PublicChannel
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
            OutputHtmlMessages.LoadCachedChannelHtml()
            OutputHtmlMessages.LoadNew()
        End If
    End Sub

    Private Sub TSChannels_Opening(sender As Object, e As EventArgs) Handles TSChannels.DropDownOpening
        LoadChannelsToMenu()
    End Sub

    Private Sub Channel_Click(sender As Object, e As EventArgs) _
        Handles PublicChannel.Click, ChannelMenuItem.Click
        SwitchChannel(DirectCast(sender, ToolStripMenuItem).Text)
    End Sub

    Private Sub TSCMOptions_Click(sender As Object, e As EventArgs) Handles TSOptions.Click, CMOptions.Click
        Config.Show()
    End Sub

    Private Sub TSCleaner_Click(sender As Object, e As EventArgs) Handles TSCleaner.Click
        Cleaner.Show()
    End Sub

    Private Sub TSViewHelp_Click(sender As Object, e As EventArgs) Handles TSViewHelp.Click
        Help.Show()
    End Sub

    Private Sub About_Click(sender As Object, e As EventArgs) Handles About.Click
        AboutBox.Show()
    End Sub

    'Private Sub TSChat_Click(sender As Object, e As EventArgs) Handles TSChat.Click
    'For Each chatbox As BoxIM In IMBoxes
    'If chatbox.Channel = Name Then
    'chatbox.Focus()
    'Exit Sub
    'End If
    'Next
    'OpenNewIM(CurrentChannel)
    'End Sub

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


#End Region

End Class