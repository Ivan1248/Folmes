Imports System.IO
Imports System.Text
Imports Microsoft.Win32

Module Dropbox
    Public Function DropBoxPath() As String
        Dim dropBoxHostDb As String =
                My.Computer.Registry.CurrentUser.OpenSubKey("Software\Dropbox").GetValue("InstallPath", "",
                                                                                         RegistryValueOptions.None).
                ToString
        dropBoxHostDb = dropBoxHostDb.Substring(0, dropBoxHostDb.LastIndexOf("\", StringComparison.Ordinal) + 1) &
                        "host.db"

        Dim sr As New StreamReader(dropBoxHostDb)
        sr.ReadLine()
        Dim retval As String = ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(sr.ReadLine))
        If Not retval.EndsWith("\") Then retval &= "\"

        Return retval
    End Function
End Module
