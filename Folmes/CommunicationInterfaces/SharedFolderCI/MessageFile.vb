Imports System.IO
Imports System.Text

Public MustInherit Class MessageFile
    Public Const Extension As String = ".fmsg"

    Public Shared Sub Create(dirPath As String, msg As Message)
        Dim filePath As String = Path.Combine(dirPath, Converter.Int64ToBase32String(msg.Time) & Extension)
        Dim sb As New StringBuilder
        sb.Append(ChrW(msg.Flags))
        sb.AppendLine(msg.Sender)
        sb.AppendLine(msg.Content)
        Dim fs As New FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read)
        Using sw As New StreamWriter(fs) With {.AutoFlush = False}
            sw.Write(sb)
        End Using
    End Sub

    Public Shared Function LoadMessage(fpath As String) As Message
        Dim m As New Message() With {.Time = Converter.Base32StringToInt64(Path.GetFileNameWithoutExtension(fpath))}
        Using sr As New StreamReader(New FileStream(fpath, FileMode.Open, FileAccess.Read, FileShare.Read))
            m.Flags = CType(sr.Read(), MessageFlags)
            Dim username As String = sr.ReadLine()
            m.Sender = username
            m.Content = sr.ReadLine()
        End Using
        Return m
    End Function

    Public Shared Function Comparison(file1 As String, file2 As String) As Integer
        Dim a As Integer = file1.Length - Extension.Length - 16
        Dim b As Integer = file2.Length - Extension.Length - 16
        For i As Integer = 0 To 15
            Select Case Asc(file1(a + i)) - Asc(file2(b + i))
                Case Is > 0 : Return 1
                Case Is < 0 : Return -1
            End Select
        Next
        Return 0
    End Function

End Class
