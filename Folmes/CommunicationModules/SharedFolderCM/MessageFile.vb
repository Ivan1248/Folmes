Imports System.IO
Imports System.Text

Public MustInherit Class MessageFile
    Public Const Extension As String = ".fmsg"

    Public Shared Sub Create(dirPath As String, msg As Message)
        Dim filePath As String = IO.Path.Combine(dirPath, Converter.Int64ToBase32String(msg.Time) & Extension)
        Dim sb As New StringBuilder
        sb.Append(ChrW(msg.Flags))
        sb.AppendLine(msg.Sender)
        sb.AppendLine(msg.HtmlContent)
        Dim fs As New FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read)
        Using sw As New StreamWriter(fs) With {.AutoFlush = False}
            sw.Write(sb)
        End Using
    End Sub

    Public Shared Function LoadMessage(fpath As String) As Message
        Dim m As New Message()
        m.Time = Converter.Base32StringToInt64(IO.Path.GetFileNameWithoutExtension(fpath))
        Using sr As New StreamReader(New FileStream(fpath, FileMode.Open, FileAccess.Read, FileShare.Read))
            m.Flags = CType(sr.Read(), MessageFlags)
            Dim username As String = sr.ReadLine()
            m.Sender = username
            m.HtmlContent = sr.ReadLine()
        End Using
        Return m
    End Function


    ''' <summary>
    ''' A comparison that tells whether message file filePath1 is more recent than message file filePath2.
    ''' Returns 1 (&gt;), -1 (\&lt;) or 0 (=).
    ''' </summary>
    ''' <param name="filePath1">Path of the first file.</param>
    ''' <param name="filePath2">Path of the second file.</param>
    ''' <returns></returns>
    Public Shared Function RecentnessComparison(filePath1 As String, filePath2 As String) As Integer
        Dim a As Integer = filePath1.Length - Extension.Length - 16
        Dim b As Integer = filePath2.Length - Extension.Length - 16
        For i As Integer = 0 To 15
            Select Case Asc(filePath1(a + i)) - Asc(filePath2(b + i))
                Case Is > 0 : Return 1
                Case Is < 0 : Return -1
            End Select
        Next
        Return 0
    End Function

End Class
