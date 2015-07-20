Imports System.IO
Imports System.Text

Public MustInherit Class MessageFile
    Public Shared Sub Create(channel As String, msg As Message)
        Dim dirPath As String = Path.Combine(MessagesDir, channel, msg.Sender)
        MakeDir(dirPath)
        Dim filePath As String = Path.Combine(dirPath, Convert.ToString(msg.Time, 16) & Extension.Message)
        Dim sb As New StringBuilder
        sb.Append(ChrW(msg.Type))
        sb.AppendLine(msg.Sender)
        sb.AppendLine(msg.Content)
        Dim fs As New FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read)
        Using sw As New StreamWriter(fs) With {.AutoFlush = False}
            sw.Write(sb)
        End Using
    End Sub

    Public Shared Function LoadMessage(fpath As String) As Message
        Dim m As New Message() With {.Time = Convert.ToInt64(Path.GetFileNameWithoutExtension(fpath), 16)}
        Using sr As New StreamReader(New FileStream(fpath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            m.Type = CType(sr.Read(), MessageType)
            m.Sender = sr.ReadLine()
            m.Content = sr.ReadLine()
        End Using
        Return m
    End Function
End Class
