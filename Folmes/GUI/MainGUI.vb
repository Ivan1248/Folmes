#Region "Imports"
Imports System.IO
Imports System.Reflection
Imports Folmes.GUI.Controls
#End Region

Public NotInheritable Class MainGUI
    'public = common (synonyms)

    '//////// Učitavanje i zatvaranje, prijava i odjava /////////////////////////////////////

#Region "Učitavanje"

    Private Sub Box_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
            'Postavke i još neke sitnice
            LoadSettings()
            Me.Icon = New Icon(Assembly.GetExecutingAssembly.GetManifestResourceStream("Folmes.DBM.ico"))
            TrayIcon.Icon = Me.Icon        'druge ikone

            'Stvaranje direktorija i učitavanje FSW
            Directories.AssureMainDirectories()
            LoadFSWatchers()

            'Prvo pokretenje? i učitavanje kanala u izbornik
            If My.Settings.Username = Nothing AndAlso FirstRun.ShowDialog() <> DialogResult.OK Then
                Application.Exit()
                End
            End If
            Me.Text &= " - " & My.Settings.Username
            PingPong.CleanPing()
            MakeDir(Path.Combine(PrivateMessagesDir, My.Settings.Username))

            'Učitavanje datoteka i poruka
            UserInfoFiles.GetAll()
            UserInfoFiles.Mine.SetOnlineStatus(True)
            With Output
                Dim messagesLoad As MessagesDisplay.InitializedEventHandler =
                        Sub()
                            RemoveHandler Output.Initialized, messagesLoad
                            MessagesManager.LoadInitial(Channels.Common, AddressOf Me.Output.AddMessage)
                        End Sub
                AddHandler .Initialized, messagesLoad
                .Initialize({})
            End With
            AddHandler InputPaddingPanel.MouseUp, AddressOf Panel2_MouseUp
        Catch ex As Exception
            Dim errorMessage As String = "Folmes failed to load completely." & vbNewLine & " Message: " & ex.Message
            MessageBox.Show(errorMessage, "Loading error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Try
                Me.Output.AddMessage(
                    New Message With {.Type = MessageType.FolmesDeclaration,
                                        .Content = errorMessage & vbNewLine & vbNewLine & Environment.StackTrace})
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

        InputContMenu.Renderer = New ToolStripProfessionalRenderer(New ToolstripColorTable)
        OutputContMenu.Renderer = New ToolStripProfessionalRenderer(New ToolstripColorTable)
    End Sub

#End Region

#Region "Closing"

    Private Shared Sub MainForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        Try
            If My.Settings.Username <> Nothing Then
                'SetOnlineStatus(False)
                UserInfoFiles.Mine.SetOnlineStatus(False)
                Channels.SetLastRead()
            End If
        Catch ex As Exception
        End Try
    End Sub

#End Region

#Region "Input + Input processing"

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

    Private Sub Input_KeyDown(sender As Object, e As KeyEventArgs) Handles Input.KeyDown
        If e.KeyCode = Keys.Enter AndAlso Not e.Shift Then
            e.SuppressKeyPress = True
            If Input.Text IsNot String.Empty Then
                If Not ProcessInput() Then
                    'TODO: zvuk greške
                End If
            End If
        End If
    End Sub

    Friend Function ProcessInput() As Boolean
        Dim command As String = Nothing
        If Input.Text(0) = "/"c Then
            Dim I As Integer = Input.Text.IndexOf(" "c)
            command = Input.Text.Substring(1, If(I <> -1, I, Input.Text.Length) - 1)
        End If
        Select Case command
            Case Nothing : Return SendMessage(MessageType.Normal)
            Case "me" : Return SendMessage(MessageType.Reflexive)
            Case "ping"
                If Input.Text.Length > 6 AndAlso PingPong.PingFile(Input.Text.Substring(6).TrimEnd(), False) Then
                    Input.Clear()
                End If
            Case "exit", "close" : Me.Close()
            Case Else : Return False
        End Select
        Return True
    End Function

    Friend Function SendMessage(messageType As MessageType) As Boolean
        If Not DetectAndCopyFiles(Input.Text) Then Return False
        Dim msg As New Message() _
                With {.Sender = My.Settings.Username, .Type = messageType, .Time = DateTime.UtcNow.ToBinary()}
        Select Case messageType
            Case MessageType.Normal, MessageType.FolmesDeclaration
                msg.Content = Html.HtmlizeMessageContent(Input.Text)
            Case MessageType.Reflexive
                msg.Content = Html.HtmlizeMessageContent(My.Settings.Username & Input.Text.Substring(3))
        End Select
        MessageFile.Create(Channels.Current, msg)
        Me.Output.AddMessage(msg)
        Input.Clear()
        Return True
    End Function

#End Region

#Region "Resizing and minimizing"

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
        End If
    End Sub

    Private Sub Panel2_MouseUp(sender As Object, e As Object) 'Handles Panel2.MouseUp
        Dim modul As Integer = 8 - InputBGPanel.Height Mod 15
        InputBGPanel.Height += modul
        Cursor.Position = New Point(Cursor.Position.X, Cursor.Position.Y - modul)
        My.Settings.InputHeight = InputBGPanel.Height '!!!!!!!!!!!!!!!!!!!!!!
    End Sub

#End Region

#Region "Called from Script.js"

    Public Sub ProcessStart_OutputProcessStartClick(data As String) Handles Output.ProcessStartClick
        Process.Start(data)
    End Sub

    Public Sub ContextMenu_OutputContextMenu() Handles Output.ContextMenu
        OutputContMenu.Show(Me, Me.PointToClient(MousePosition))
    End Sub

#End Region

#Region "Resizing and minimizing + Notifyicon and context menu"

    Private Sub InputPaddingPanel_MouseMove(sender As Object, e As MouseEventArgs) Handles InputPaddingPanel.MouseMove
        If e.Button = MouseButtons.Left Then
            Dim h As Integer = InputBGPanel.Height - e.Y
            Select Case h
                Case Is < 23 : InputBGPanel.Height = 23
                Case Is < Me.ClientSize.Height - 80 : InputBGPanel.Height = h
            End Select
        End If
    End Sub

#End Region

#Region "Menus, context menus and NotifyIcon"

    Sub AddMainMenuHandlers() Handles Me.Load
        AddHandler TSSettings.Click, Sub() Settings.Show()
        AddHandler TSCleaner.Click, Sub() Cleaner.Show()
        AddHandler TSHelp.Click, Sub() Help.Show()
        AddHandler TSAbout.Click, Sub() AboutBox.Show()
    End Sub

    Sub AddContextMenuHandlers() Handles Me.Load
        ' Input
        AddHandler CutBtn.Click, Sub() Input.Cut()
        AddHandler CopyBtn.Click, Sub() Input.Copy()
        AddHandler PasteBtn.Click, Sub() Input.Paste()
        AddHandler SelectAllBtn.Click, Sub() Input.SelectAll()
        ' Output
        AddHandler CopyO.Click, Sub() Output.Document.ExecCommand("Copy", False, vbNull)
    End Sub

    Sub AddTrayIconHandlers() Handles Me.Load
        ' Icon
        AddHandler TrayIcon.BalloonTipClicked, Sub() Deminimize()
        ' Context menu
        AddHandler CMSettings.Click, Sub() Settings.Show()
        AddHandler CMOpenFolder.Click, Sub() Process.Start("explorer.exe", AppDomain.CurrentDomain.BaseDirectory())
        AddHandler CMExit.Click, Sub() Me.Close()
    End Sub

    Private Sub TrayIcon_Click(sender As Object, e As MouseEventArgs) Handles TrayIcon.MouseClick
        If e.Button = MouseButtons.Left Then : Deminimize()
        Else : TrayIcon.ContextMenuStrip.Show()
        End If
    End Sub

    Private Sub Deminimize()
        Me.Activate()
        Me.ShowInTaskbar = True
        Me.WindowState = FormWindowState.Normal
    End Sub

#End Region

    'Private Sub TSChat_Click(sender As Object, e As EventArgs) Handles TSChat.Click
    'For Each chatbox As BoxIM In IMBoxes
    'If chatbox.Channel = Name Then
    'chatbox.Focus()
    'Exit Sub
    'End If
    'Next
    'OpenNewIM(CurrentChannel)
    'End Sub
End Class