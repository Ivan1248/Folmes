Imports System.Drawing.Imaging
Imports System.IO
Imports System.Text

Public MustInherit Class Attachments

    Public Const MaxPath As Integer = 512
    Public Const MaxImageHeight As Integer = 2

    Public Shared Function GetFilesWithDates() As List(Of String()) 'zastarjelo
        GetFilesWithDates = New List(Of String())
        For Each fl As FileInfo In New DirectoryInfo(Dirs.Attachments).GetFiles()
            GetFilesWithDates.Add({fl.Name, FormatDate(fl.LastWriteTime.ToLocalTime)})
        Next
    End Function

    Private Shared Function FormatDate(theDate As Date) As String
        Dim hourFormat As String = theDate.ToShortTimeString
        Return If(theDate.Date <> Date.Today, theDate.ToShortDateString & " " & hourFormat, hourFormat)
    End Function

    Public Shared Sub MakeFileThumbnail(imgPath As String, thumbName As String)
        Dim callback As New Image.GetThumbnailImageAbort(Function() True)
        Dim img As Image = New Bitmap(imgPath)
        Dirs.Create(Dirs.Thumbnails)
        If img.Height > MaxImageHeight Then
            img = img.GetThumbnailImage(MaxImageHeight * img.Width \ img.Height, MaxImageHeight, callback, New IntPtr())
        End If
        img.Save(Path.Combine(Dirs.Thumbnails, thumbName), StringToImageFormat(Path.GetExtension(imgPath)))
    End Sub

    Public Shared Sub CopyFile(filePath As String)
        Try
            Dim fileName As String = Path.GetFileName(filePath)
            Directory.CreateDirectory(Dirs.Attachments)

            File.Copy(filePath, Path.Combine(Dirs.Attachments, fileName), True)
            If IsImageFile(filePath) Then
                MakeFileThumbnail(filePath, fileName)
            End If
            File.SetLastAccessTime(filePath, Date.UtcNow)
        Catch
            MsgBox("It seems like the file """ & filePath & """ doesn't exist.")
        End Try
    End Sub

    Public Shared Function IsImageFile(fpath As String) As Boolean
        Return {".jpg", ".png", ".bmp", ".jpeg", ".gif", ".tiff"}.Contains(Path.GetExtension(fpath).ToLower())
    End Function

    Public Shared Function StringToImageFormat(ext As String) As ImageFormat
        Select Case ext.ToLower()
            Case "jpg", "jpeg" : Return ImageFormat.Jpeg
            Case "png" : Return ImageFormat.Png
            Case "gif" : Return ImageFormat.Gif
            Case "tiff" : Return ImageFormat.Tiff
            Case Else : Return ImageFormat.Bmp
        End Select
    End Function
End Class
