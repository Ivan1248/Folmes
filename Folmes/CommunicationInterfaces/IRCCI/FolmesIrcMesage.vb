Imports System.Text

Public MustInherit Class FolmesIrcMesage
    Public Shared Function GetCommand(channel As String, message As Message) As String
        Dim sb As New StringBuilder()
        sb.Append("PRIVMSG ")

        If channel = Channels.Common Then
            If Users.Others.Count = 0 Then Return Nothing
            sb.Append(Users.Others(0).Name)
            For i As Integer = 1 To Users.Others.Count - 1
                sb.Append(","c).Append(Users.Others(i).Name)
            Next
        Else
            sb.Append(channel)
        End If
        sb.Append(" :")
        sb.Append(Converter.Int64ToBase32String(message.Time))
        sb.Append(" ").Append(CInt(message.Flags).ToString())
        sb.Append(" ").Append(message.Content.Replace(vbNewLine, "[newline]"))
        Return sb.ToString()
    End Function

    Public Shared Function GetMessage(ircCommand As String) As Message
        Dim sp As Integer() = {0, 0}
        sp(0) = ircCommand.IndexOf(" "c)
        sp(1) = ircCommand.IndexOf(" "c, sp(0) + 2)

        If ircCommand.Substring(sp(0) + 1, sp(1) - sp(0) - 1) <> "PRIVMSG" Then
            Return Nothing
        End If
        
        GetMessage = New Message() With {.Sender = ircCommand.Substring(1, ircCommand.IndexOf("!"c, 2) - 1)}

        Dim timei As Integer = ircCommand.IndexOf(":"c, sp(1) + 2) + 1
        Dim flagsi As Integer = timei + 13 + 1
        Try
            Dim contenti As Integer = ircCommand.IndexOf(" "c, flagsi + 1) + 1
            GetMessage.Time = Converter.Base32StringToInt64(ircCommand.Substring(timei, 13))
            GetMessage.Flags = CType(Integer.Parse(ircCommand.Substring(flagsi, contenti - flagsi - 1)), MessageFlags) And MessageFlags.Irc
            GetMessage.Content = ircCommand.Substring(contenti)
        Catch
            GetMessage.Content = ircCommand.Substring(timei)
        End Try

    End Function
End Class
