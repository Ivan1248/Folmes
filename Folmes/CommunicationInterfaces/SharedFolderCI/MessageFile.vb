Imports System.Text
Imports System.IO

Public MustInherit Class MessageFile
    Public Const Extension As String = ".fmsg"

    Public Shared Sub Create(dirPath As String, msg As Message)
        Dim filePath As String = Path.Combine(dirPath, Convert.Int64ToBase32String(msg.Time) & Extension)
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
        Dim m As New Message() With {.Time = Convert.Base32StringToInt64(Path.GetFileNameWithoutExtension(fpath))}
        Using sr As New StreamReader(New FileStream(fpath, FileMode.Open, FileAccess.Read, FileShare.Read))
            m.Type = CType(sr.Read(), MessageType)
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

    Private MustInherit Class Convert

        Shared Function Int64ToBase32String(time As Long) As String
            Const mask As Byte = 31
            Dim chars(12) As Byte
            For i As Integer = 0 To 12
                chars(i) = CByte((time >> (5 * (12 - i))) And mask)
                If chars(i) < 10 Then
                    chars(i) += CByte(AscW("0"c))
                Else
                    chars(i) += CByte(AscW("a"c) - 10)
                End If
            Next
            Return Encoding.ASCII.GetString(chars)
        End Function

        Shared Function Base32StringToInt64(time As String) As Long
            Base32StringToInt64 = 0
            For i As Integer = 0 To 12
                Base32StringToInt64 <<= 5
                Dim b As Byte = CByte(AscW(time(i)))
                If b >= AscW("a"c) Then
                    b -= CByte(AscW("a"c) - 10)
                Else
                    b -= CByte(AscW("0"c))
                End If
                Base32StringToInt64 = Base32StringToInt64 Or b
            Next
        End Function

    End Class


End Class
