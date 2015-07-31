Partial Public Class MainGUI

    Private Sub ReloadPrivateChannelsToMenu()
        While TSChannels.DropDownItems.Count > 2
            TSChannels.DropDownItems.Item(2).Dispose()
        End While
        Dim showSeparator As Boolean = False
        For Each user As User In Users.Others
            showSeparator = True
            Dim channelTsMenuItem As New ToolStripMenuItem(user.Name) With {.ForeColor = Color.FromArgb(176, 176, 176)}
            Dim status As Integer = 0
            If user.IsOnline Then status = 1
            If Channels.Current <> user.Name AndAlso Channels.AnyNewMessages(user.Name) Then status = status Or 2
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
            Me.Output.CacheChannelHtml(Channels.Current)
            With TSChannels
                Channels.Switch(channel)
                If Output.LoadCachedChannelHtml(channel) Then
                    MessagesManager.LoadNew(AddressOf Output.AddMessage, channel)
                Else
                    MessagesManager.LoadInitial(AddressOf Output.AddMessage, channel)
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