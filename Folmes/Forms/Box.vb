#Region "Imports"



Imports System.IO
Imports System.Reflection
Imports Folmes.Classes

#End Region

Public NotInheritable Class Box
    'public = common (synonyms)

    '//////// Učitavanje i zatvaranje, prijava i odjava /////////////////////////////////////

#Region "Učitavanje"

    Private Sub Box_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
            'Postavke i još neke sitnice
            LoadSettings()
            Me.Icon = New Icon(Assembly.GetExecutingAssembly.GetManifestResourceStream("Folmes.DBM.ico"))
            NotifyIcon.Icon = Me.Icon        'druge ikone
            CMOpenFolder.Text = "Open """ & Path.GetFileName(RootPath) & """"

            'Stvaranje direktorija
            AssureDirectories()
            LoadFSWatchers()

            'Prvo pokretenje? i učitavanje kanala u izbornik
            If My.Settings.Username = Nothing AndAlso FirstRun.ShowDialog() <> DialogResult.OK Then
                Me.Close()
            End If
            Me.Text &= " - " & My.Settings.Username
            CleanPing()
            MakeDir(Path.Combine(MessagesDir, My.Settings.Username))

            'Učitavanje datoteka i poruka
            With Output
                Dim messagesLoad As WebBrowserDocumentCompletedEventHandler =
                        Sub()
                            OutputHtmlMessages.LoadInitial_Once()
                            RemoveHandler Output.DocumentCompleted, messagesLoad
                        End Sub
                AddHandler .DocumentCompleted, messagesLoad
                .Document.Write(DefaultHtml)
                .Visible = True
                '.BringToFront() 'da ne bude iza gornje trake
                LoadBaseHtml(Me.Output, {})
            End With
            MessageFiles.GetCommon()
            MessageFiles.SelectedIngoing = MessageFiles.IngoingCommon
            MessageFiles.SelectedOutgoing = MessageFiles.OutgoingCommon
            MessageFiles.GetIngoingPrivate() 'potrebno za pronalazak novih poruka
            UserInfoFiles.GetAll()
            UserInfoFiles.Mine.SetOnlineStatus(True)

            'Sučelje
            AddHandler Panel2.MouseUp, AddressOf Panel2_MouseUp
            'Povezivanje
            'IM.connect(PORT)
            'InternetBGW.RunWorkerAsync() 'SetOnlineStatus(True) uključeno
        Catch ex As Exception
            Dim errorMessage As String = "Folmes failed to load completely." & vbNewLine & " Message: " & ex.Message
            MessageBox.Show(errorMessage, "Loading error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Try
                OutputHtmlMessages.LoadMessageToOutput(
                    New Message With {.Type = Message.MessageType.Declaration, .Content = errorMessage & vbNewLine & vbNewLine & Environment.StackTrace})
                Input.Enabled = False
            Catch
            End Try
        End Try
    End Sub

    Private Sub LoadSettings()
        If My.Settings.StartMinimized Then
            Me.WindowState = FormWindowState.Minimized
            If My.Settings.MinimizeToTray Then Me.ShowInTaskbar = False
            FlashIcon()
        End If
        Me.Size = My.Settings.WindowSize
        InputBGPanel.Height = My.Settings.InputHeight
    End Sub

    Private Sub Skin() Handles Me.Load
        'Skin() za CCPContMenu i CopyContMenu u bazi
        TS.Renderer = New ToolStripProfessionalRenderer(New ToolstripColorTable)
        CType(TS.Renderer, ToolStripProfessionalRenderer).RoundedEdges = False

        CCPContMenu.Renderer = New ToolStripProfessionalRenderer(New ToolstripColorTable)
        CopyContMenu.Renderer = New ToolStripProfessionalRenderer(New ToolstripColorTable)
    End Sub

#End Region

#Region "Zatvaranje"

    Private Sub MainForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        Try
            If My.Settings.Username <> Nothing Then
                'SetOnlineStatus(False)
                UserInfoFiles.Mine.SetOnlineStatus(False)
                SetLastRead()
            End If
        Catch ex As Exception
        End Try
    End Sub

#End Region

    '//////// Učitavanje datoteka,  poruka, slanje i FSW ///////////////////////////////////////////////

    ' Učitavanje i postavljanje datoteka:
    '    Box.MessageFiles
    ' Učitavanje poruka:
    '    Box.Messages

#Region "Slanje poruka"

    Friend Sub ProcessInput()
        If Input.Text Is String.Empty Then Exit Sub
        Dim command As String = Nothing
        If Input.Text(0) = "/"c Then
            Dim I As Integer = Input.Text.IndexOf(" "c)
            command = Input.Text.Substring(1, If(I <> -1, I, Input.Text.Length) - 1)
        End If
        Select Case command
            Case Nothing : If SendMessage(Message.MessageType.Normal) Then Input.Clear()
            Case "me" : If SendMessage(Message.MessageType.Reflexive) Then Input.Clear()
            Case "put" : If SendMessage(Message.MessageType.Declaration) Then Input.Clear()
            Case "ping" : If Ping(Input.Text.Substring(6)) Then Input.Clear()
            Case "exit", "close" : Me.Close()
        End Select
    End Sub

    Friend Function SendMessage(messageType As Message.MessageType) As Boolean
        If Not DetectAndCopyFiles(Input.Text) Then Return False
        Dim msg As _
                New Message() _
                With {.Sender = My.Settings.Username, .Type = messageType, .Time = DateTime.UtcNow.ToBinary()}
        If messageType = Message.MessageType.Reflexive Then
            msg.Content = HtmlizeMessageContent(My.Settings.Username & Input.Text.Substring(3))
        Else
            msg.Content = HtmlizeMessageContent(Input.Text)
        End If
        If Channels.Current = Channels.PublicChannel Then
            MessageFiles.OutgoingCommon.StoreEntry(msg)
        Else
            For Each msgfile As MessageFile In MessageFiles.OutgoingPrivate
                If msgfile.Recipient = Channels.Current Then
                    msgfile.StoreEntry(msg)
                    Exit For
                End If
            Next
        End If
        OutputHtmlMessages.LoadMessageToOutput(msg)
        Return True
    End Function

#End Region

    '//////// Promjena veličine, NotifyIcon i Toolstrip //////////////////////////////////

#Region "Resizing and minimizing + Notifyicon and context menu"

    Private Sub Box_ResizeEnd(sender As Object, e As EventArgs) Handles MyBase.ResizeEnd
        My.Settings.WindowSize = Me.Size '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        If Me.WindowState <> FormWindowState.Minimized Then
            Panel2_MouseUp(Nothing, Nothing)
        End If
    End Sub

    Private Sub Box_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        If Me.WindowState = FormWindowState.Minimized Then
            If My.Settings.MinimizeToTray Then Me.ShowInTaskbar = False
        Else
            If InputBGPanel.Height + 80 > Me.ClientSize.Height Then
                InputBGPanel.Height = Me.ClientSize.Height - 80
            End If
            RefreshScroller()
        End If
    End Sub

    Private Sub Panel2_MouseUp(sender As Object, e As Object) 'Handles Panel2.MouseUp
        Dim modul As Integer = 8 - InputBGPanel.Height Mod 15
        InputBGPanel.Height += modul
        Cursor.Position = New Point(Cursor.Position.X, Cursor.Position.Y - modul)
        RefreshScroller()
        My.Settings.InputHeight = InputBGPanel.Height '!!!!!!!!!!!!!!!!!!!!!!
    End Sub

    Private Sub NotifyIcon1_Click(sender As Object, e As MouseEventArgs) Handles NotifyIcon.MouseClick
        If e.Button = MouseButtons.Left Then : Deminimize()
        Else : NotifyIcon.ContextMenuStrip.Show()
        End If
    End Sub

    Private Sub NotifyIcon1Baloon_CMShow(sender As Object, e As EventArgs) _
        Handles NotifyIcon.BalloonTipClicked, CMShow.Click
        Deminimize()
    End Sub

    Private Sub CMExit_Click(sender As Object, e As EventArgs) Handles CMExit.Click
        Me.Close()
    End Sub

    Private Shared Sub CMOpenFolder_Click(sender As Object, e As EventArgs) Handles CMOpenFolder.Click
        Process.Start("explorer.exe", AppDomain.CurrentDomain.BaseDirectory())
    End Sub

    Private Sub Deminimize()
        Me.Activate()
        Me.ShowInTaskbar = True
        Me.WindowState = FormWindowState.Normal
    End Sub

#End Region

#Region "Klikovi na linkove u HTML elementu 'click' + ScrollPos"

    Private Sub Output_DocumentCompleted(sender As Object,
                                         e As WebBrowserDocumentCompletedEventArgs) _
        Handles Output.DocumentCompleted
        With Output.Document
            .AttachEventHandler("onclick", AddressOf ClickO)
            .AttachEventHandler("oncontextmenu", AddressOf ClickOCm)
        End With
        'WebBrowser.ObjectForScripting Property 
    End Sub

    Private Sub ClickO(sender As Object, e As EventArgs) 'ClickO za otvaranje datoteka, dinamičko dodavanje handlera
        Dim data As String = Output.Document.Body.GetAttribute("data-click")
        If Not String.IsNullOrEmpty(data) Then Process.Start(data)
    End Sub

    Private Sub ClickOCm(sender As Object, e As EventArgs) 'ClickO za kontekstni izbornik, dinamičko dodavanje handlera
        If Output.Document.Body.GetAttribute("data-sel") <> String.Empty Then
            CopyContMenu.Show(Me, Me.PointToClient(MousePosition))
        End If
    End Sub

    '    Protected Function GetScrollPos() As Integer
    '        Try
    '            With Output
    '                Return _
    '                    .Document.Body.ScrollRectangle.Height - .Height -
    '                    .Document.GetElementsByTagName("HTML")(0).ScrollTop
    '            End With
    '        Catch
    '            Return 0
    '        End Try
    '    End Function

#End Region

#Region "Resizing and minimizing + Notifyicon and context menu"

    Private Sub Panel2_MouseMove(sender As Object, e As MouseEventArgs) Handles Panel2.MouseMove
        If e.Button = MouseButtons.Left Then
            Dim h As Integer = InputBGPanel.Height - e.Y
            Select Case h
                Case Is < 23 : InputBGPanel.Height = 23
                Case Is < Me.ClientSize.Height - 80 : InputBGPanel.Height = h
            End Select
            RefreshScroller()
        End If
    End Sub

#End Region

#Region "Kontekstni izbornici inputa i outputa"
    'input
    Private Sub Cut_Click(sender As Object, e As EventArgs) Handles CutBtn.Click
        Input.Cut()
    End Sub

    Private Sub Copy_Click(sender As Object, e As EventArgs) Handles CopyBtn.Click
        Input.Copy()
    End Sub

    Private Sub Paste_Click(sender As Object, e As EventArgs) Handles PasteBtn.Click
        Input.Paste()
    End Sub

    Private Sub SelectAll_Click(sender As Object, e As EventArgs) Handles SelectAllBtn.Click
        Input.SelectAll()
    End Sub

    'output
    Private Sub CopyO_Click(sender As Object, e As EventArgs) Handles CopyO.Click
        Output.Document.ExecCommand("Copy", False, vbNull)
    End Sub

#End Region

#Region "Tipke"

    Private Sub input_keydown(sender As Object, e As KeyEventArgs) Handles Input.KeyDown
        If e.KeyCode = Keys.Enter AndAlso Not e.Shift Then
            ProcessInput()
            e.SuppressKeyPress = True
        End If
    End Sub

#End Region

    Private Sub TSCMOptions_Click(sender As Object, e As EventArgs) Handles TSOptions.Click, CMOptions.Click
        Config.Show()
    End Sub

    Private Sub TSCleaner_Click(sender As Object, e As EventArgs) Handles TSCleaner.Click
        Cleaner.Show()
    End Sub

    Private Sub TSHelp_Click(sender As Object, e As EventArgs) Handles TSHelp.Click
        Help.Show()
    End Sub

    Private Sub TSAbout_Click(sender As Object, e As EventArgs) Handles TSAbout.Click
        AboutBox.Show()
    End Sub

    'Private Sub TSChat_Click(sender As Object, e As EventArgs) Handles TSChat.Click
    'For Each chatbox As BoxIM In IMBoxes
    'If chatbox.Channel = Name Then
    'chatbox.Focus()
    'Exit Sub
    'End If
    'Next
    'OpenNewIM(CurrentChannel)
    'End Sub

    '///////////////////////////


#Region "DragDrop"

    Private Shared Sub Input_DragEnter(sender As Object, e As DragEventArgs) Handles Input.DragEnter
        e.Effect = DragDropEffects.All
    End Sub

    Private Sub Input_DragDrop(sender As Object, e As DragEventArgs) Handles Input.DragDrop
        If (e.Data.GetDataPresent(DataFormats.Text)) Then
            Input.AppendText(CStr(e.Data.GetData(DataFormats.Text)))
        ElseIf e.Data.GetDataPresent(DataFormats.FileDrop) Then
            Dim filePath As String = CType(e.Data.GetData(DataFormats.FileDrop), String())(0)
            If File.Exists(filePath) Then
                Input.AppendText("[file:" & filePath & "]")
            Else
                MsgBox("I don't accept folders.")
            End If
        End If
        Input.Focus()
    End Sub

#End Region

#Region "Troklik"

    Dim _inTripleClickInterval As Boolean = False
    Dim WithEvents _clickTimer As New Timer With {.Interval = 240}

    Private Sub Input_DoubleClick(sender As Object, e As EventArgs) Handles Input.DoubleClick
        _inTripleClickInterval = True
        _clickTimer.Start()
    End Sub

    Private Sub Input_Click(sender As Object, e As EventArgs) Handles Input.Click
        If _inTripleClickInterval Then Input.SelectAll()
    End Sub

    Private Sub Timertick() Handles _clickTimer.Tick
        _inTripleClickInterval = False
        _clickTimer.Stop()
    End Sub

#End Region

End Class