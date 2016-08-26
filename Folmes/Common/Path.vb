Namespace Common
    Module Path
        Public Function IsImageFile(fpath As String) As Boolean
            Return {".jpg", ".png", ".bmp", ".jpeg", ".gif", ".tiff"}.Contains(IO.Path.GetExtension(fpath).ToLower())
        End Function
    End Module
End NameSpace