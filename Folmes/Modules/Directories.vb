Imports System.IO

Module Directories

    Public RootPath As String = Application.StartupPath
    Public FolmesDir As String = RootPath & "\.Folmes"
    Public MessagesDir As String = FolmesDir & "\Messages"
    Public UsersDir As String = FolmesDir & "\Users"
    Public FilesDir As String = FolmesDir & "\Files"
    Public ThumbnailDir As String = FilesDir & "\thumbnails"
    Public PingDir As String = FolmesDir & "\RTT"


    Public Sub AssureMainDirectories()
        If Not Directory.Exists(FolmesDir) Then
            MakeDir(FolmesDir)
            HideFolmesFolder(True)
        End If
        MakeDir(MessagesDir)
        MakeDir(UsersDir)
        MakeDir(Path.Combine(PingDir, My.Settings.Username))
    End Sub

    Public Sub HideFolmesFolder(hide As Boolean)
        File.SetAttributes(FolmesDir, If(hide, FileAttributes.Hidden, FileAttributes.Normal))
    End Sub

    Public Function GetUserDirs() As String()
        Return Directory.GetDirectories(MessagesDir)
    End Function

    Public Sub MakeDir(dirpath As String)
        Dim filecheck As New FileInfo(dirpath)
        If filecheck.Exists Then filecheck.Delete()
        Directory.CreateDirectory(dirpath)
    End Sub

End Module
