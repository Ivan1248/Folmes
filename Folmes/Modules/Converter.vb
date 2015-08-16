Imports System.Text

Public MustInherit Class Converter

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