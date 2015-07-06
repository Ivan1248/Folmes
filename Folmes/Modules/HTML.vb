Imports System.Text

Module Html

    Public Const DefaultHtml As String = "<html><head></head><body style=""background-color:#222""></body></html>"

    Const SpaceChars As String = " "c & vbCr & vbLf & vbTab

    Public Sub LoadBaseHtml(callerOutput As WebBrowser, scripts As String())
        With New StringBuilder() 'kapacitet
            'HTML start
            .Append("<!DOCTYPE html>" &
                    "<html>" &
                    "<head>" &
                    "<meta name=""viewport"" content=""user-scalable=no, initial-scale=1"">" &
                    "<meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">" &
                    "<style type=""text/css"">")
            '       style
            .Append("body{font-size:").Append(My.Settings.FontSize)
            .Append("px;line-height:").Append(Math.Round(My.Settings.FontSize * 4 / 3))
            .Append("px} img{max-height:").Append(My.Settings.ThumbnailHeight).Append("px} ")
            .Append(My.Resources.Style)
            .Append("</style><script>")
            '       script
            .Append(My.Resources.ScrollScript).Append(vbNewLine).Append(My.Resources.Script)
            For Each scr As String In scripts
                .Append(scr)
            Next
            '   body
            .Append("</script></head>" &
                    "<body data-click="""" data-sel="""">" &
                    "<div id=""container"">")
            '' U ELEMENTU ID = "container" NALAZE SE PORUKE ''
            .Append(
                "</div><div id=""scroller""></div><div id=""scrollertrack""></div><div id=""copybtn"">Copy</div></body></html>")
            callerOutput.DocumentText = .ToString()
        End With
    End Sub

    ' 2 točke ili protokol i jedna točka do prvog razmaka (/s ili /n ili /r ili kraj)
    ' {x}.{x}.{x} ili {x}://{x}.{x}
    ' {x} smije sadržavati znakove 33-122 /34,60,62,92 ("<>\)
    '    Private Function IsInvalidUrlChar(c As Char) As Boolean
    '        Return c = """" OrElse c = "<" OrElse c = ">" OrElse c = "\" OrElse Asc(c) < 33 OrElse Asc(c) > 122
    '    End Function

    Public Function HtmlizeMessageContent(content As String) As String 'NEDOSTAJU JOŠ SLIKE
        Dim sb As New StringBuilder(244) 'kapacitet
        Dim start As Integer = 0
        Dim lastEnd As Integer = -1
        Dim canBeUri As Boolean = True

        For i As Integer = 0 To content.Length - 1
            If SpaceChars.Contains(content(i)) Then
                canBeUri = True
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

                sb.Append("<span class=""url"" OnClick=""clickO('")
                sb.Append(uri).Append("')"">")
                If IsImage(uri) Then sb.Append("<img src =""")
                sb.Append(uri)
                If IsImage(uri) Then sb.Append("""></img>")
                sb.Append("</span>") 'SLIKA !!!
                i = lastEnd + 1
            Else
                Dim esc As String
                Select Case content(i)
                    Case "&"c : esc = "&amp;"
                    Case """"c : esc = "&quot;"
                    Case "<"c : esc = "&lt;"
                    Case ">"c : esc = "&gt;"
                    Case Chr(13) : esc = String.Empty
                    Case Chr(10) : esc = "<br>"
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

    Private Function DetectUrlAroundDot(str As String, dotIndex As Integer) As Tuple(Of Integer, Integer)
        Dim erroret As Tuple(Of Integer, Integer) = New Tuple(Of Integer, Integer)(0, 0)
        Dim l As Integer = dotIndex - 1
        Dim r As Integer = dotIndex + 1

        If l < 0 OrElse r >= str.Length OrElse Not IsAsciiLetter(str(l)) OrElse Not IsAsciiLetter(str(r)) Then
            Return erroret
        End If
        l -= 1
        While (True)
            If l < 0 OrElse SpaceChars.Contains(str(l)) Then
                l += 1
                Exit While
            End If
            If IsAsciiLetter(str(l)) Then : l -= 1
            ElseIf l - 3 > 0 AndAlso str(l - 2) = ":"c AndAlso str(l - 1) = "/"c AndAlso str(l) = "/"c Then : l -= 3
            Else : Return erroret
            End If
        End While
        r += 1
        While (True)
            If r >= str.Length OrElse SpaceChars.Contains(str(r)) Then
                r -= 1
                Exit While
            End If
            If IsAsciiLetter(str(r)) OrElse Char.IsDigit(str(r)) OrElse "-_.~!*'();:@&=+$,/?%#[]".Contains(str(r)) Then : r += 1
            ElseIf str(r) = "."c AndAlso Not "/.".Contains(str(r - 1)) Then : r += 1
            ElseIf str(r) = "/"c AndAlso str(r - 1) <> "/"c Then : r += 1
            Else : Return erroret
            End If
        End While
        Return New Tuple(Of Integer, Integer)(l, r)
    End Function

    Private Function IsAsciiLetter(c As Char) As Boolean
        Return (c >= "A"c AndAlso c <= "Z"c) OrElse (c >= "a"c AndAlso c <= "z"c)
    End Function

End Module
