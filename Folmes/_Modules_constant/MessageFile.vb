Imports System.Text

Public MustInherit Class MessageFile
    Public Shared Sub Create(filePath As String, msg As Message)
        Dim sb As New StringBuilder
        sb.Append(ChrW(msg.Type))
        sb.AppendLine(msg.Sender)
        sb.AppendLine(msg.Content)
        Dim fs As New IO.FileStream(filePath, IO.FileMode.Create, IO.FileAccess.Write, IO.FileShare.Read)
        Using sw As New IO.StreamWriter(fs) With {.AutoFlush = False}
            sw.Write(sb)
        End Using
    End Sub

    Public Shared Function LoadMessage(fpath As String) As Message
        Dim m As New Message() With {.Time = Convert.ToInt64(IO.Path.GetFileNameWithoutExtension(fpath), 16)}
        Using sr As New IO.StreamReader(New IO.FileStream(fpath, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.ReadWrite))
            m.Type = CType(sr.Read(), MessageType)
            m.Sender = sr.ReadLine()
            m.Content = sr.ReadLine()
        End Using
        Return m
    End Function
End Class
