Imports System.Drawing.Imaging
Imports System.IO
Imports System.Text

Public MustInherit Class Files
    Structure Extension
        Const Message As String = ".fmsg"
        Const Ping As String = ".ping"
        Const Pong As String = ".pong"
        Const UserStatus As String = ".st"
        Const UserInfo As String = ".info"
    End Structure

    Public Const MaxImageHeight As Integer = 64

    Public Shared Sub CreateThumbnail(imgPath As String, thumbName As String)
        Dim callback As New Image.GetThumbnailImageAbort(Function() True)
        Dim img As Image = New Bitmap(imgPath)
        Dirs.Create(Dirs.Thumbnails)
        If img.Height > MaxImageHeight Then
            img = img.GetThumbnailImage(MaxImageHeight * img.Width \ img.Height, MaxImageHeight, callback, New IntPtr())
        End If
        img.Save(Path.Combine(Dirs.Thumbnails, thumbName), StringToImageFormat(Path.GetExtension(imgPath)))
    End Sub

    Public Shared Sub SendFile(filePath As String)
        Try
            Dim fileName As String = Path.GetFileName(filePath)
            Directory.CreateDirectory(Dirs.Attachments)

            File.Copy(filePath, Path.Combine(Dirs.Attachments, fileName), True)
            If IsImageFile(fileName) Then
                CreateThumbnail(filePath, fileName)
            End If
        Catch
            MsgBox("It seems like the file """ & filePath & """ doesn't exist.")
        End Try
    End Sub

    Public Shared Function IsImageFile(fpath As String) As Boolean
        Return {".jpg", ".png", ".bmp", ".jpeg", ".gif", ".tiff"}.Contains(Path.GetExtension(fpath).ToLower())
    End Function

    Public Shared Function StringToImageFormat(ext As String) As ImageFormat
        Select Case ext.ToLower()
            Case ".jpg", ".jpeg" : Return ImageFormat.Jpeg
            Case ".png" : Return ImageFormat.Png
            Case ".gif" : Return ImageFormat.Gif
            Case ".tiff" : Return ImageFormat.Tiff
            Case Else : Return ImageFormat.Bmp
        End Select
    End Function
End Class
