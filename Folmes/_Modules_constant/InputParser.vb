Imports System.Collections.Generic
Imports System.IO

Public MustInherit Class InputParser
    Public Structure Span
        Public Left, Right As Integer
        Public Type As SpanType
        Sub New(left As Integer, right As Integer)
            Me.Left = left
            Me.Right = right
        End Sub
        Sub New(left As Integer, right As Integer, type As SpanType)
            Me.Left = left
            Me.Right = right
            Me.Type = type
        End Sub
        Public Function GetSubstring(text As String) As String
            Return text.Substring(Left, Right - Left + 1)
        End Function
    End Structure

    Public Enum SpanType
        File
        URL
        Text
    End Enum

    Public Shared Function Parse(input As String) As List(Of Span)
        Parse = New List(Of Span)
        Dim nonFileBeginning As Integer = 0
        Dim i As Integer
        For i = 0 To input.Length - 8 - 1
            If input(i) <> "["c Then Continue For

            Dim j As Integer = i + 1
            While j < i + 6 AndAlso input(j) = "file:"(j - i - 1)
                j += 1
            End While
            If j <> i + 6 Then
                i += 5
                Continue For
            End If

            For j = 0 To input.Length - 1
                If input(j) = "]"c Then
                    If nonFileBeginning < i Then
                        FindURIsAndText(input, New Span(nonFileBeginning, i - 1), Parse)
                    End If
                    Parse.Add(New Span(i + 6, j - 1, SpanType.File))
                    i = j
                    nonFileBeginning = j + 1
                    Exit For
                End If
            Next
        Next
        If nonFileBeginning <= input.Length - 1 Then
            FindURIsAndText(input, New Span(nonFileBeginning, input.Length - 1), Parse)
        End If
    End Function

    Private Shared Sub FindURIsAndText(text As String, span As Span, ByRef list As List(Of Span))
        Dim l As Integer
        Dim r As Integer
        Dim prevTextBeginning As Integer = span.Left

        For l = span.Left + 1 To span.Right - 1
            If text(l) <> "."c Then
                Continue For
            End If

            r = l + 1
            l = l - 1
            If Not IsAsciiLetterOrDigit(text(l)) OrElse Not IsAsciiLetterOrDigit(text(r)) Then
                l = r
                Continue For
            End If

            l -= 1
            While True
                If l < 0 OrElse IsSpace(text(l)) Then
                    l += 1
                    Exit While
                ElseIf IsAsciiLetterOrDigit(text(l)) Then
                    l -= 1
                ElseIf l - 3 > 0 AndAlso text(l - 2) = ":"c AndAlso text(l - 1) = "/"c AndAlso text(l) = "/"c Then
                    l -= 3
                Else
                    l = r
                    Continue For
                End If
            End While

            r += 1
            While True
                If r > span.Right OrElse IsSpace(text(r)) Then
                    r -= 1
                    Exit While
                ElseIf IsAsciiLetterOrDigit(text(r)) OrElse "-_.~!*'();:@&=+$,/?%#[]".Contains(text(r)) Then
                    r += 1
                Else
                    l = r
                    Continue For
                End If
            End While

            If prevTextBeginning < l Then
                list.Add(New Span(prevTextBeginning, l - 1, SpanType.Text))
            End If
            list.Add(New Span(l, r, SpanType.URL))
            l = r
            prevTextBeginning = r + 1
            Exit For
        Next
        If prevTextBeginning <= span.Right Then
            list.Add(New Span(prevTextBeginning, span.Right, SpanType.Text))
        End If
    End Sub

    Public Shared Function IsSpace(c As Char) As Boolean
        Return (" "c & vbCr & vbLf & vbTab).Contains(c)
    End Function

    Public Shared Function IsAsciiLetterOrDigit(c As Char) As Boolean
        Return (c >= "A"c AndAlso c <= "Z"c) OrElse (c >= "a"c AndAlso c <= "z"c) OrElse (c >= "0"c AndAlso c <= "9"c)
    End Function

End Class
