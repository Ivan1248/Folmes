Imports System.Globalization
Imports System.IO
Imports System.Net
Imports System.Text

Public MustInherit Class HtmlConverter

    Public Shared Function HtmlizeInputAndCopyFiles(input As String) As String
        Dim spans As List(Of InputParser.Span) = InputParser.Parse(input)
        Return HtmlizeParsedInputAndCopyFiles(input, spans)
    End Function

    Private Shared Function HtmlizeParsedInputAndCopyFiles(input As String, spans As List(Of InputParser.Span)) As String
        Dim sb As New StringBuilder(512) 'kapacitet
        For Each s As InputParser.Span In spans
            Select Case s.Type
                Case InputParser.SpanType.Text
                    EscapeHtml(input, s, sb)
                Case InputParser.SpanType.URL
                    HtmlizeUri(input, s, sb)
                Case InputParser.SpanType.File
                    HtmlizeFileTag(input, s, sb)
                    Attachments.CopyFile(s.GetSubstring(input))
            End Select
        Next
        Return sb.ToString
    End Function

    Private Shared Sub EscapeHtml(text As String, span As InputParser.Span, ByRef sb As StringBuilder)
        For i As Integer = span.Left To span.Right
            Select Case text(i)
                ' Case " "c : sb.Append("&nbsp;")
                Case Chr(13) : sb.Append("")        ' CR
                Case Chr(10) : sb.Append("<br>")    ' LF
                Case Chr(9) : sb.Append("&emsp;")   ' tab
                Case "&"c : sb.Append("&amp;")
                Case """"c : sb.Append("&quot;")
                Case "'"c : sb.Append("&#39;")
                Case "<"c : sb.Append("&lt;")
                Case ">"c : sb.Append("&gt;")
                Case Else : sb.Append(text(i))
            End Select
        Next
    End Sub

    Private Shared Sub HtmlizeUri(text As String, span As InputParser.Span, ByRef sb As StringBuilder)
        Dim uri As String = text.Substring(span.Left, span.Right - span.Left + 1)
        sb.Append("<span class=""url"" OnClick=""linkClick('").Append(uri).Append("')"">")
        If Attachments.IsImageFile(uri) OrElse IsImageUrl(uri) Then
            sb.Append("<img src=""").Append(uri)
            sb.Append(""" alt=""").Append(uri)
            sb.Append(""" onload=""refreshScroller()")
            sb.Append("""></img>")
        Else
            sb.Append(uri)
        End If
        sb.Append("</span>")
    End Sub

    Private Shared Sub HtmlizeFileTag(str As String, span As InputParser.Span, ByRef sb As StringBuilder)
        Dim fileName As String = Path.GetFileName(span.GetSubstring(str))
        sb.Append("<span class=""file"" onclick=""linkClick('")
        sb.Append(Replace(Path.Combine(Dirs.Attachments, fileName), "\", "\\"))
        sb.Append("')"">")
        sb.Append(fileName)
        sb.Append("</span>")
    End Sub

    Private Shared Function IsImageUrl(url As String) As Boolean
        Try
            Dim req As HttpWebRequest = CType(WebRequest.Create(url), HttpWebRequest)
            req.Method = "HEAD"
            req.Timeout = 1000
            Using resp As WebResponse = req.GetResponse()
                Return resp.ContentType.ToLower(CultureInfo.InvariantCulture).StartsWith("image/")
            End Using
        Catch
            Return False
        End Try
    End Function
End Class
