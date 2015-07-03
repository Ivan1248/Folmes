Imports System.IO

Module Common

    Public Const MaxPath As Integer = 512
    Public Const MaxImageHeight As Integer = 200

#Region "Korisnici i imena"

    Public Function UserExists(name As String) As Boolean
        Return Directory.Exists(Path.Combine(MessagesDir, name))
    End Function

#End Region

End Module
