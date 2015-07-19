Imports System.IO
Imports System.Text
Imports Folmes.Datatypes

Public MustInherit Class NewMessageFile
    Public Shared Sub Create(channel As String, msg As Message)
        Dim filePath As String = Path.Combine(MessagesDir, channel, msg.Sender, Convert.ToString(msg.Time, 16) & ".msg")
        Dim sb As New StringBuilder
        sb.Append(ChrW(msg.Type))
        sb.AppendLine(msg.Sender)
        sb.AppendLine(msg.Content)
        Dim fs As New FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read)
        Using sw As New StreamWriter(fs) With {.AutoFlush = False}
            sw.Write(sb)
        End Using
    End Sub

    Public Shared Function Load(fpath As String) As Message
        Dim m As New Message() With {.Time = Convert.ToInt64(Path.GetFileNameWithoutExtension(fpath), 16)}
        Using sr As New StreamReader(New FileStream(fpath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            m.Type = CType(sr.Read(), MessageType)
            m.Sender = sr.ReadLine()
            m.Content = sr.ReadLine()
        End Using
        Return m
    End Function
End Class
