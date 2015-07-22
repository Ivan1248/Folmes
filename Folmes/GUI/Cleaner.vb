Imports System.IO
Imports System.Text

Public Class Cleaner

    Private Sub Cleaner_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Owner = MainGUI
        Skin()
        AddHandler Output.Initialized, AddressOf LoadList
        Output.Initialize({My.Resources.CleanerScripts})
    End Sub

    Enum CleanerMode
        Files
        Users
    End Enum
    Private _selected As CleanerMode = CleanerMode.Files

    Private Sub Skin()
        Dim t As ToolstripColorTable = New ToolstripColorTable
        ToolStrip1.Renderer = New ToolStripProfessionalRenderer(t)
        CType(ToolStrip1.Renderer, ToolStripProfessionalRenderer).RoundedEdges = False
    End Sub

    Private Sub DeleteBtn_Click(sender As Object, e As EventArgs) Handles DeleteBtn.Click
        Dim list As String = Output.Document.Body.GetAttribute("data-sel")
        If list <> Nothing Then
            Dim Confirmed As Boolean = (MessageBox.Show("Are you sure you want to delete the selected file(s)?", "Confirm deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = 6)
            If Confirmed Then
                Dim FilesOrUsers As String() = list.Split("|"c)
                For Each currentFileOrUser As String In FilesOrUsers
                    If currentFileOrUser <> Nothing Then
                        If _selected = CleanerMode.Files Then  'ako je datoteka, nije korisnik
                            DeleteFile(currentFileOrUser) 'brisanje datoteke
                        Else
                            DeleteUser(currentFileOrUser) 'brisanje korisnika
                        End If
                        LoadList()
                    End If
                Next
            End If
        End If
    End Sub

    Public Sub DeleteFile(ByVal filename As String)
        filename = Path.Combine(FilesDir, filename)
        File.Delete(filename)
    End Sub
    Public Sub DeleteUser(ByVal username As String)
        Dim userPth As String = Path.Combine(MessagesDir, username)
        Directory.Delete(userPth, True)
        File.Delete(userPth & Files.Extension.Message)
        For Each dir As String In GetUserDirs()
            File.Delete(dir + "\" & username & Files.Extension.Message)
        Next
    End Sub

    Private Sub Toggel_Click(sender As Object, e As EventArgs) Handles Toggle.Click
        _selected = Not _selected
        If _selected = CleanerMode.Files Then
            CType(sender, ToolStripButton).Text = "Files"
        Else
            CType(sender, ToolStripButton).Text = "Users"
        End If
        LoadList()
    End Sub
    Private Sub LoadList(sender As Object, e As WebBrowserDocumentCompletedEventArgs)
        LoadList()
        RemoveHandler Output.Initialized, AddressOf LoadList
    End Sub
    Private Sub LoadList()
        With New StringBuilder()    'kapacitet
            If Me._selected = CleanerMode.Files Then
                For Each file_date As String() In GetFilesWithDates()
                    .Append("<div class=""item message""><div class=""time"">")
                    .Append(file_date(1)).Append("</div><span class=""file"" onclick=""fileClick('")
                    .Append(Replace(Path.Combine(FilesDir, file_date(0)), "\", "\\")).Append("')"">")
                    .Append(file_date(0)).Append("</span></div>")
                Next
            Else
                For Each user As String In GetUserDirs()
                    user = Path.GetFileName(user)
                    'If user <> My.Settings.Username Then
                    .Append("<div class=""item message""><div class=""time""></div><span class=""file"">")
                    .Append(user).Append("</span></div>")
                    'End If
                Next
            End If
            Output.Document.GetElementById("container").InnerHtml = .ToString ' = Output.MsgContainer
        End With
    End Sub

#Region "Klikovi na linkove u HTML elementu 'click'"

    Private Sub ClickO(sender As Object, e As EventArgs)
        Dim FilePath As String = Output.Document.Body.GetAttribute("data-click")
        If FilePath <> Nothing Then
            System.Diagnostics.Process.Start(FilePath)
        End If
        'Output.Document.GetElementById("click").SetAttribute("value", " ")
    End Sub
#End Region
End Class
