#Region "Imports"
Imports System.IO
Imports System.Reflection
Imports Folmes.GUI.Controls
#End Region

Public NotInheritable Class MainGUI
    'public = common (synonyms)

    '//////// Učitavanje i zatvaranje, prijava i odjava /////////////////////////////////////

#Region "Učitavanje"

    Dim WithEvents sfci As New SharedFolderCI

    Sub fsi_NewCommonMessage(message As Message) Handles sfci.NewCommonMessage
        NewMessageQueues.AddCommon(message)
        Notify(NotificationType.PublicMessage, Nothing)
    End Sub

    Sub fsi_NewPrivateMessage(message As Message) Handles sfci.NewPrivateMessage
        NewMessageQueues.AddPrivate(message.Sender.Name, message)
        Notify(NotificationType.PrivateMessage, message.Sender.Name)
    End Sub

    Sub fsi_PongReceived(rtt_in_ms As Long) Handles sfci.PongReceived
        Output.AddMessage("Ping-pong: File_RTT = " & rtt_in_ms & "ms")
    End Sub

    Private Sub Box_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.BeginInvoke(DirectCast(AddressOf InitializeEverything, MethodInvoker))
    End Sub

    Private Sub InitializeEverything()
        Try
            'Postavke i još neke sitnice
            LoadWindowDimensionsSettings()
            Me.Icon = New Icon(Assembly.GetExecutingAssembly.GetManifestResourceStream("Folmes.DBM.ico"))
            TrayIcon.Icon = Me.Icon        'druge ikone

            'Stvaranje direktorija i učitavanje FSW
            Dirs.AssureMainDirectories()
            sfci.Start(Me)
            AddHandler UsersWatcher.Deleted, AddressOf UsersWatcher_Deleted

            'Prvo pokretenje? i učitavanje kanala u izbornik
            If My.Settings.Username = Nothing AndAlso FirstRun.ShowDialog() <> DialogResult.OK Then
                Application.Exit()
                End
            End If
            Me.Text &= " - " & My.Settings.Username
            Dirs.Create(Path.Combine(Dirs.PrivateMessages, My.Settings.Username))

            'Učitavanje datoteka i poruka
            Users.Initialize()
            Users.MyUser.SetAndSaveStatus(UserStatus.Online)
            With Output
                Dim messagesLoad As MessagesDisplay.InitializedEventHandler =
                        Sub()
                            RemoveHandler Output.Initialized, messagesLoad
                            sfci.GetOldMessages(Channels.Common, My.Settings.NofMsgs, AddressOf Output.AddMessage)
                            'MessagesManager.LoadInitialAndDeleteOld(Channels.Common, AddressOf Output.AddMessage)
                        End Sub
                AddHandler .Initialized, messagesLoad
                .Initialize({})
            End With
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

    Private Sub LoadWindowDimensionsSettings()
        If My.Settings.StartMinimized Then
            Me.WindowState = FormWindowState.Minimized
            If My.Settings.MinimizeToTray Then Me.ShowInTaskbar = False
            FlashIcon()
        End If
        'Me.Size = My.Settings.WindowSize
        InputBGPanel.Height = My.Settings.InputHeight
    End Sub

    Private Sub Skin() Handles Me.Load
        TS.Renderer = New ToolStripProfessionalRenderer(New ToolstripColorTable)
        CType(TS.Renderer, ToolStripProfessionalRenderer).RoundedEdges = False

        InputContMenu.Renderer = TS.Renderer
        OutputContMenu.Renderer = TS.Renderer
    End Sub

#End Region

    Private Sub UsersWatcher_Deleted(username As String) ' Handles UsersWatcher.Deleted
        Users.Remove(username)
        If Channels.Current = username Then SwitchChannel(Channels.Common)
    End Sub

    Private Sub SwitchChannel(channel As String)
        If channel <> Channels.Current Then
            Me.Output.CacheChannelHtml(Channels.Current)
            Channels.Switch(channel)
            If Output.LoadCachedChannelHtml(channel) Then
                NewMessageQueues.LoadMessages(channel)
            Else
                sfci.GetOldMessages(channel, My.Settings.NofMsgs, AddressOf Output.AddMessage)
            End If
            TSChannels.Text = channel
        End If
    End Sub

#Region "Closing"

    Private Shared Sub MainForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        Try
            If My.Settings.Username <> Nothing Then
                Users.MyUser.SetAndSaveStatus(UserStatus.Offline)
                Channels.SetLastRead()
            End If
        Catch ex As Exception
        End Try
    End Sub

#End Region

#Region "Input + Input processing"

    Private Sub Input_KeyDown(sender As Object, e As KeyEventArgs) Handles Input.KeyDown
        If e.KeyCode = Keys.Enter AndAlso Not e.Shift Then
            e.SuppressKeyPress = True
            If Input.Text IsNot String.Empty Then
                If ProcessInput() Then
                    Input.Clear()
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
            Case Nothing
                Return SendMessage(MessageType.Normal)
            Case "me"
                Return SendMessage(MessageType.Reflexive)
            Case "ping"
                Return Input.Text.Length > 6 AndAlso sfci.Ping(Input.Text.Substring(6).TrimEnd())
            Case "exit", "close"
                Me.Close()
            Case Else
                Return False
        End Select
        Return True
    End Function

    Public Delegate Sub MessageLoadingSub(msg As Message)

    Friend Function SendMessage(messageType As MessageType) As Boolean
        Dim msg As New Message()
        Dim attachedFiles As New List(Of String)
        msg.Sender = Users.MyUser
        msg.Type = messageType
        msg.Time = Date.UtcNow.ToBinary()
        Select Case messageType
            Case MessageType.Normal, MessageType.FolmesDeclaration
                msg.Content = HtmlConverter.HtmlizeInputAndGetFiles(Input.Text, attachedFiles)
            Case MessageType.Reflexive
                msg.Content = HtmlConverter.HtmlizeInputAndGetFiles(My.Settings.Username & Input.Text.Substring(3), attachedFiles)
        End Select

        sfci.SendMessage(Channels.Current, msg)
        For Each af As String In attachedFiles
            Files.SendFile(af)
        Next
        Me.Output.AddMessage(msg)
        Return True
    End Function

#End Region

#Region "Resizing and minimizing"

    Private Sub Box_ResizeEnd(sender As Object, e As EventArgs) Handles MyBase.ResizeEnd
        My.Settings.WindowSize = Me.Size '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        If Me.WindowState <> FormWindowState.Minimized Then
            InputPaddingPanel_MouseUp(Nothing, Nothing)
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

    Private Sub InputPaddingPanel_MouseUp(sender As Object, e As Object) Handles InputPaddingPanel.MouseUp
        Dim modul As Integer = 8 - InputBGPanel.Height Mod 15
        InputBGPanel.Height += modul
        Cursor.Position = New Point(Cursor.Position.X, Cursor.Position.Y - modul)
        My.Settings.InputHeight = InputBGPanel.Height '!!!!!!!!!!!!!!!!!!!!!!
    End Sub

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

#Region "Output context menu"

    Public Sub ContextMenu_OutputContextMenu() Handles Output.ContextMenu
        OutputContMenu.Show(Me, Me.PointToClient(MousePosition))
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

End Class