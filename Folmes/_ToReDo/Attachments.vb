Imports System.Drawing.Imaging
Imports System.IO
Imports System.Text

Module Attachments

    Public Const MaxPath As Integer = 512
    Public Const MaxImageHeight As Integer = 2

    Public Function GetFilesWithDates() As List(Of String()) 'zastarjelo
        GetFilesWithDates = New List(Of String())
        For Each fl As FileInfo In New DirectoryInfo(Dirs.Attachments).GetFiles()
            GetFilesWithDates.Add({fl.Name, FormatDate(fl.LastWriteTime.ToLocalTime)})
        Next
    End Function

    Private Function FormatDate(theDate As Date) As String
        Dim hourFormat As String = theDate.ToShortTimeString
        Return If(theDate.Date <> Date.Today, theDate.ToShortDateString & " " & hourFormat, hourFormat)
    End Function

    Public Sub MakeFileThumbnail(imgPath As String, thumbName As String)
        Dim callback As New Image.GetThumbnailImageAbort(Function() True)
        Dim img As Image = New Bitmap(imgPath)
        Dirs.Create(Dirs.Thumbnails)
        If img.Height > MaxImageHeight Then
            img = img.GetThumbnailImage(MaxImageHeight * img.Width \ img.Height, MaxImageHeight, callback, New IntPtr())
        End If
        img.Save(Path.Combine(Dirs.Thumbnails, thumbName), StringToImageFormat(Path.GetExtension(imgPath)))
    End Sub

    Public Function DetectAndCopyFiles(fileObj As String) As Boolean
        Dim a As Integer
        Dim filename, filepath As String
        Dim sb As New StringBuilder(MaxPath, MaxPath)
        Using sr As New StringReader(fileObj)
            While True
                a = sr.Read
                If _
                    a = Asc("[") AndAlso sr.Read = Asc("f") AndAlso sr.Read = Asc("i") AndAlso sr.Read = Asc("l") AndAlso
                    sr.Read = Asc("e") AndAlso sr.Read = Asc(":") Then
                    While True
                        a = sr.Read
                        If a = Asc("]") Then
                            filepath = sb.ToString
                            filename = Path.GetFileName(filepath)
                            Try
                                Directory.CreateDirectory(Dirs.Attachments)
                                File.Copy(filepath, Path.Combine(Dirs.Attachments, filename), True)
                                If IsImageFile(filepath) Then
                                    MakeFileThumbnail(filepath, filename)
                                End If
                            Catch
                                MsgBox("The file """ & filepath & """ doesn't seem to exist.")
                                Return False
                            End Try
                            Exit While
                        ElseIf a = -1 Then
                            Return True
                        Else
                            sb.Append(Chr(a))
                        End If
                    End While
                ElseIf a = -1 Then
                    Exit While
                End If
            End While
        End Using
        Return True
    End Function

    Public Function IsImageFile(fpath As String) As Boolean
        Return {".jpg", ".png", ".bmp", ".jpeg", ".gif", ".tiff"}.Contains(Path.GetExtension(fpath).ToLower())
    End Function

    Public Function StringToImageFormat(ext As String) As ImageFormat
        Select Case ext.ToLower()
            Case "jpg", "jpeg" : Return ImageFormat.Jpeg
            Case "png" : Return ImageFormat.Png
            Case "gif" : Return ImageFormat.Gif
            Case "tiff" : Return ImageFormat.Tiff
            Case Else : Return ImageFormat.Bmp
        End Select
    End Function
End Module
