Imports System.Text

Module Html

    Public Const DefaultHtml As String = "<html><head></head><body style=""background-color:#222""></body></html>"

    Public Sub LoadBaseHtml(caller As Form)
        With New StringBuilder() 'kapacitet
            'HTML start
            .Append("<!DOCTYPE html>" &
                    "<html>" &
                    "<head>" &
                    "<title>Folmes</title>" &
                    "<meta name=""viewport"" content=""user-scalable=no,initial-scale=1"">" &
                    "<meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">" &
                    "<style type=""text/css"">")
            '       style
            .Append("body{font-size:").Append(My.Settings.FontSize)
            .Append("px;line-height:").Append(Math.Round(My.Settings.FontSize * 4 / 3))
            .Append("px} img{max-height:").Append(My.Settings.ThumbnailHeight).Append("px} ")
            .Append(My.Resources.Style)
            .Append("</style><script>")
            '       script
            .Append(My.Resources.ScrollScript).Append(My.Resources.Script)
            If caller Is Cleaner Then .Append(My.Resources.CleanerScripts)
            '   body
            .Append("</script></head>" &
                    "<body data-click="""" data-sel="""">" &
                    "<div id=""container"">")
            '' U ELEMENTU ID = "container" NALAZE SE PORUKE ''
            .Append(
                "</div><div id=""scroller"" onload = ""loadScroller()"" ></div><div id=""scrollertrack""></div><div id=""copybtn"">Copy</div></body></html>")
            If caller Is Box Then
                DirectCast(caller, Box).Output.DocumentText = .ToString
            Else 'caller is cleaner
                DirectCast(caller, Cleaner).Output.DocumentText = .ToString
            End If
        End With
    End Sub

    ' 2 točke ili protokol i jedna točka do prvog razmaka (/s ili /n ili /r ili kraj)
    ' {x}.{x}.{x} ili {x}://{x}.{x}
    ' {x} smije sadržavati znakove 33-122 /34,60,62,92 ("<>\)
    Private Function IsInvalidUrlChar(c As Char) As Boolean
        Return c = """" OrElse c = "<" OrElse c = ">" OrElse c = "\" OrElse Asc(c) < 33 OrElse Asc(c) > 122
    End Function

    Public Function HtmlizeMessageContent(content As String) As String 'NEDOSTAJU JOŠ SLIKE
        Const spaceChars As String = " " & vbCr & vbLf & vbTab
        Dim sb As New StringBuilder(244) 'kapacitet
        Dim dots As Integer   ' broj '://' i '.' - 1
        Dim replac As String
        Dim start As Integer = 0
        Dim lastEnd As Integer = -1
        Dim i, j As Integer
        For i = 0 To content.Length - 1
            If content(i) = "."c Then
                dots = 0
                j = i - 1
                While (j <> 0) ' lookbehind
                    If IsInvalidUrlChar(content(j)) Then : Continue For ' nije URI
                    ElseIf j > 2 AndAlso content(j) = "/"c AndAlso content(j - 1) = "/"c AndAlso content(j - 2) = ":"c Then
                        j -= 2
                        dots += 1
                    ElseIf spaceChars.Contains(content(j)) Then : Exit While
                    End If
                    j -= 1
                End While
                start = j
                j = i
                While (j < content.Length - 1) ' lookahead
                    j += 1
                    If IsInvalidUrlChar(content(j)) Then : Continue For ' nije URI
                    ElseIf content(j) = "."c Then : dots += 1
                    ElseIf spaceChars.Contains(content(j)) Then : Exit While
                    End If
                End While
                i = j - 1
                If dots > 0 Then
                    If lastEnd < start - 1 Then
                        sb.Append(content, lastEnd + 1, start - lastEnd - 1)
                    End If
                    sb.Append("<span class=""url"" OnClick=""clickO('")
                    sb.Append(content, start, j - start + 1).Append("')"">")
                    sb.Append(content, start, j - start + 1)
                    sb.Append("</span>") 'SLIKA !!!
                    If content(j) = Chr(13) OrElse content(j) = Chr(10) Then
                        If content(j + 1) = Chr(10) Then
                            i = j
                            j += 1
                        End If
                        sb.Append("<br>")
                    End If
                Else
                    Continue For
                End If
            Else
                Select Case content(i)
                    Case "&"c : replac = "&amp;"
                    Case """"c : replac = "&quot;"
                    Case "<"c : replac = "&lt;"
                    Case ">"c : replac = "&gt;"
                    Case Chr(13), Chr(10) : replac = "<br>" 'CR/LF
                    Case Else : Continue For
                End Select
                If lastEnd < start - 1 Then
                    sb.Append(content, lastEnd + 1, i - lastEnd - 1)
                End If
                sb.Append(replac)
                lastEnd = i
            End If
        Next
        If lastEnd < content.Length - 1 Then
            sb.Append(content, lastEnd + 1, content.Length - 1 - lastEnd)
        End If
        Return sb.ToString
    End Function
End Module
