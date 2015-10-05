Imports System.IO

Public MustInherit Class Dirs
    Public Shared Root As String = Application.StartupPath
    Public Shared Folmes As String = Root & "\.Folmes"
    Public Shared Messages As String = Folmes & "\Messages"
    Public Shared CommonChannel As String = Messages & "\Common"
    Public Shared PrivateMessages As String = Messages & "\Private"
    Public Shared Users As String = Folmes & "\Users"
    Public Shared Attachments As String = Folmes & "\Attachments"
    Public Shared Thumbnails As String = Attachments & "\thumbnails"
    Public Shared PingPong As String = Folmes & "\PingPong"

    Public Shared Sub AssureMainDirectories()
        If Not Directory.Exists(Folmes) Then
            Create(Folmes)
            HideFolmesFolder(True)
        End If
        Create(Users)
        AssureSfciDirectories()
        Create(Attachments)
        Create(Thumbnails)
    End Sub

    Private Shared Sub AssureSfciDirectories()
        Create(Messages)
        Create(CommonChannel)
        Create(PrivateMessages)
        Create(Path.Combine(PrivateMessages, My.Settings.Username))
        Create(PingPong)
        Create(Path.Combine(PingPong, My.Settings.Username))
    End Sub

    Public Shared Sub HideFolmesFolder(hide As Boolean)
        File.SetAttributes(Folmes, If(hide, FileAttributes.Hidden, FileAttributes.Normal))
    End Sub

    Public Shared Function GetUserDirs() As String()
        Return Directory.GetDirectories(PrivateMessages)
    End Function

    Public Shared Sub Create(dirPath As String)
        Dim filecheck As New FileInfo(dirPath)
        If filecheck.Exists Then filecheck.Delete()
        Directory.CreateDirectory(dirPath)
    End Sub

End Class
