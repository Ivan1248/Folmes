Imports System.IO
Imports System.Runtime.InteropServices

Module Directories

    Public RootPath As String = Application.StartupPath
    Public FolmesDir As String = RootPath & "\.Folmes"
    Public MessagesDir As String = FolmesDir & "\Messages"
    Public FilesDir As String = FolmesDir & "\Files"
    Public ThumbnailDir As String = FilesDir & "\thumbnails"

    Public Sub AssureDirectories()
        If Not Directory.Exists(FolmesDir) Then
            MakeDir(FolmesDir)
            HideFolmesFolder(True)
        End If
        MakeDir(MessagesDir)
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

    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True, BestFitMapping:=False)>
    Public Function MoveFile(src As String, dest As String) As Boolean
    End Function

End Module
