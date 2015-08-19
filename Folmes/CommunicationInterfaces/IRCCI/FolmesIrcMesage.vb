﻿Imports System.Text

Public MustInherit Class IrcMesage
    Public Shared Function GetCommand(channel As String, message As FolMessage) As String
        Dim sb As New StringBuilder()
        Dim u As User

        sb.Append("PRIVMSG ")

        If channel = Channels.Common Then
            If Users.Others.Count = 0 Then
                Return Nothing
            End If
            u = Users.Others(0)
            If u.IrcNick IsNot Nothing Then sb.Append(u.IrcNick)
            For i As Integer = 1 To Users.Others.Count - 1
                u = Users.Others(i)
                If u.IrcNick IsNot Nothing Then sb.Append(","c).Append(u.IrcNick)
            Next
        Else
            u = Users.GetByName(channel)
            If u Is Nothing OrElse u.IrcNick Is Nothing Then
                Return Nothing
            End If
            sb.Append(channel)
        End If
        sb.Append(" :")
        sb.Append(Converter.Int64ToBase32String(message.Time))
        sb.Append(" ").Append(CInt(message.Flags).ToString())
        sb.Append(" ").Append(message.HtmlContent)
        Return sb.ToString()
    End Function

    Public Shared Function GetMessageFromCommand(ircCommand As String) As FolMessage
        Dim sp As Integer() = {0, 0}
        sp(0) = ircCommand.IndexOf(" "c)
        sp(1) = ircCommand.IndexOf(" "c, sp(0) + 2)

        If ircCommand.Substring(sp(0) + 1, sp(1) - sp(0) - 1) <> "PRIVMSG" Then
            Return Nothing
        End If

        GetMessageFromCommand = New FolMessage()

        Dim senderNick As String = ircCommand.Substring(1, ircCommand.IndexOf("!"c, 2) - 1)
        Dim sender As User = Users.GetByIrcNick(senderNick)
        GetMessageFromCommand.Sender = If(sender Is Nothing, senderNick & "[IRC]", sender.Name)

        Dim timei As Integer = ircCommand.IndexOf(":"c, sp(1) + 2) + 1
        Dim flagsi As Integer = timei + 13 + 1
        Try
            Dim contenti As Integer = ircCommand.IndexOf(" "c, flagsi + 1) + 1
            GetMessageFromCommand.Time = Converter.Base32StringToInt64(ircCommand.Substring(timei, 13))
            GetMessageFromCommand.Flags = CType(Integer.Parse(ircCommand.Substring(flagsi, contenti - flagsi - 1)), FolMessageFlags)
            GetMessageFromCommand.HtmlContent = ircCommand.Substring(contenti)
        Catch
            GetMessageFromCommand.Time = Date.UtcNow.ToBinary
            GetMessageFromCommand.Flags = FolMessageFlags.NonFolmesIrc
            GetMessageFromCommand.HtmlContent = ircCommand.Substring(timei)
        End Try
    End Function
End Class
