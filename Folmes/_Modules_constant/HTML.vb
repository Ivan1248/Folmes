Imports System.Globalization
Imports System.Net
Imports System.Text

Public MustInherit Class Html

    Const SpaceChars As String = " "c & vbCr & vbLf & vbTab

    ' 2 točke ili protokol i jedna točka do prvog razmaka (/s ili /n ili /r ili kraj)
    ' {x}.{x}.{x} ili {x}://{x}.{x}
    ' {x} smije sadržavati znakove 33-122 /34,60,62,92 ("<>\)
    '    Private Function IsInvalidUrlChar(c As Char) As Boolean
    '        Return c = """" OrElse c = "<" OrElse c = ">" OrElse c = "\" OrElse Asc(c) < 33 OrElse Asc(c) > 122
    '    End Function

    Public Shared Function HtmlizeMessageContent(content As String) As String 'NEDOSTAJU JOŠ SLIKE
        Dim sb As New StringBuilder(244) 'kapacitet
        Dim start As Integer
        Dim lastEnd As Integer = -1
        Dim canBeUri As Boolean = True

        For i As Integer = 0 To content.Length - 1
            If SpaceChars.Contains(content(i)) Then
                canBeUri = True
                Dim esc As String
                Select Case content(i)
                    Case " "c : esc = "&nbsp;" ' CR
                    Case Chr(13) : esc = "" ' CR
                    Case Chr(10) : esc = "<br>" ' LF
                    Case Chr(9) : esc = "&emsp;" ' tab
                    Case Else : Continue For
                End Select
                sb.Append(content, lastEnd + 1, i - 1 - lastEnd).Append(esc)
                lastEnd = i
            ElseIf canBeUri And content(i) = "."c And i > 0 Then
                Dim urlSpan As Tuple(Of Integer, Integer) = DetectUrlAroundDot(content, i)
                start = urlSpan.Item1
                If start = -1 Then
                    canBeUri = False
                    Continue For
                End If
                If lastEnd < start - 1 Then
                    sb.Append(content, lastEnd + 1, start - lastEnd - 1)
                End If
                lastEnd = urlSpan.Item2
                Dim uri As String = content.Substring(start, lastEnd - start + 1)

                sb.Append("<span class=""url"" OnClick=""linkClick('").Append(uri).Append("')"">")
                If IsImageFile(uri) OrElse IsImageUrl(uri) Then
                    sb.Append("<img src=""").Append(uri)
                    sb.Append(""" alt=""").Append(uri)
                    sb.Append(""" onload=""refreshScroller()")
                    sb.Append("""></img>")
                Else
                    sb.Append(uri)
                End If
                sb.Append("</span>") 'SLIKA !!!
                i = lastEnd + 1
            Else
                Dim esc As String
                Select Case content(i)
                    Case "&"c : esc = "&amp;"
                    Case """"c : esc = "&quot;"
                    Case "<"c : esc = "&lt;"
                    Case ">"c : esc = "&gt;"
                    Case Else : Continue For
                End Select
                sb.Append(esc)
                lastEnd = i
            End If
        Next
        If lastEnd < content.Length - 1 Then
            sb.Append(content, lastEnd + 1, content.Length - 1 - lastEnd)
        End If
        Return sb.ToString
    End Function

    Private Shared Function DetectUrlAroundDot(str As String, dotIndex As Integer) As Tuple(Of Integer, Integer)
        Dim erroret As Tuple(Of Integer, Integer) = New Tuple(Of Integer, Integer)(-1, -1)
        Dim l As Integer = dotIndex - 1
        Dim r As Integer = dotIndex + 1

        If l < 0 OrElse r >= str.Length OrElse Not IsAsciiLetterOrDigit(str(l)) OrElse Not IsAsciiLetterOrDigit(str(r)) Then
            Return erroret
        End If
        l -= 1
        While True
            If l < 0 OrElse SpaceChars.Contains(str(l)) Then
                l += 1
                Exit While
            End If
            If IsAsciiLetterOrDigit(str(l)) Then : l -= 1
            ElseIf l - 3 > 0 AndAlso str(l - 2) = ":"c AndAlso str(l - 1) = "/"c AndAlso str(l) = "/"c Then : l -= 3
            Else : Return erroret
            End If
        End While
        r += 1
        While True
            If r >= str.Length OrElse SpaceChars.Contains(str(r)) Then
                r -= 1
                Exit While
            End If
            If IsAsciiLetterOrDigit(str(r)) OrElse "-_.~!*'();:@&=+$,/?%#[]".Contains(str(r)) Then : r += 1
            ElseIf str(r) = "."c AndAlso Not "/.".Contains(str(r - 1)) Then : r += 1
            ElseIf str(r) = "/"c AndAlso str(r - 1) <> "/"c Then : r += 1
            Else : Return erroret
            End If
        End While
        Return New Tuple(Of Integer, Integer)(l, r)
    End Function

    Private Shared Function IsAsciiLetterOrDigit(c As Char) As Boolean
        Return (c >= "A"c AndAlso c <= "Z"c) OrElse (c >= "a"c AndAlso c <= "z"c) OrElse (c >= "0"c AndAlso c <= "9"c)
    End Function

    Private Shared Function IsImageUrl(url As String) As Boolean
        Try
            Dim req As HttpWebRequest = CType(HttpWebRequest.Create(url), HttpWebRequest)
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
