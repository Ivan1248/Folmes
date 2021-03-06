﻿#Region "Imports"
Imports System.Collections.Specialized
Imports System.IO
Imports System.Reflection
Imports Folmes.GUI.Controls
#End Region

Public NotInheritable Class MainGUI
    'public = common (synonyms)

    Dim WithEvents sfcm As New SharedFolderCM
    Dim WithEvents irccm As New IrcCm
    Dim CommunicationModules As New List(Of ICommunicationModule)({sfcm, irccm})

    '//////// Učitavanje i zatvaranje, prijava i odjava /////////////////////////////////////

    Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Window settings
        LoadWindowDimensionsSettings()
        Me.Icon = New Icon(Assembly.GetExecutingAssembly.GetManifestResourceStream("Folmes.DBM.ico"))
        TrayIcon.Icon = Me.Icon        'druge ikone

        ' First run? and window text
        If My.Settings.Username = Nothing AndAlso FirstRun.ShowDialog() <> DialogResult.OK Then
            Application.Exit()
            End
        End If
        Me.Text &= " - " & My.Settings.Username
    End Sub

    Private Sub Box_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
            Dirs.AssureMainDirectories()

            ' UsersWatcher initialization
            UsersWatcher.Start()
            AddHandler UsersWatcher.Deleted, AddressOf UsersWatcher_Deleted

            ' Communication module initialization
            InitializeCommunicationModules()

            ' Učitavanje datoteka i poruka
            Users.Initialize()
            Users.MyUser.SetAndSaveStatus(UserStatus.Online_Folder)
            Dim messagesLoad As MessagesDisplay.InitializedEventHandler =
                    Sub()
                        RemoveHandler Output.Initialized, messagesLoad
                        sfcm.LoadOldMessages(Channels.Common, My.Settings.MessageQueueCapacity, AddressOf Output.AddMessage)
                    End Sub
            AddHandler Output.Initialized, messagesLoad
            Output.Initialize({})
        Catch ex As Exception
            Dim errorMessage As String = "Folmes failed to load completely." & vbNewLine & " Message: " & ex.Message
            MessageBox.Show(errorMessage, "Loading error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Try
                Me.Output.AddMessage(
                    New Message With {.Flags = MessageFlags.FolmesSystemMessage,
                                        .HtmlContent = errorMessage & vbNewLine & vbNewLine & Environment.StackTrace})
                Input.Enabled = False
            Catch
            End Try
        End Try
    End Sub

#Region "Communication modules"
    Sub InitializeCommunicationModules()
        For Each cm As ICommunicationModule In CommunicationModules
            cm.Initialize(Me)
            AddHandler cm.MessageReceived, AddressOf MessageReceived
        Next
        AddHandler sfcm.PongReceived, AddressOf sfci_PongReceived
        AddHandler irccm.Connected, AddressOf ircci_Connected
        AddHandler irccm.LostConnection, AddressOf ircci_LostConnection
    End Sub

    Sub MessageReceived(message As Message)
        If (message.Flags And MessageFlags.Privat) = 0 Then
            MessageQueues.AddCommon(message)
            Notify(NotificationType.PublicMessage, Nothing)
        Else
            MessageQueues.AddPrivate(message.Sender, message)
            Notify(NotificationType.PrivateMessage, message.Sender)
        End If
    End Sub

    Sub sfci_PongReceived(rtt_in_ms As Long)
        Output.AddMessage("Ping-pong: File_RTT = " & rtt_in_ms & "ms")
    End Sub

    Sub ircci_Connected(ircNick As String)
        Users.MyUser.IrcNick = ircNick
        Users.MyUser.Status = Users.MyUser.Status Or UserStatus.Online_IRC
        Users.MyUser.SaveInfo()
    End Sub

    Sub ircci_LostConnection()
        Users.MyUser.Status = Users.MyUser.Status And Not UserStatus.Online_IRC
        Users.MyUser.SaveInfo()
    End Sub

#End Region

    Private Sub LoadWindowDimensionsSettings()
        If My.Settings.StartMinimized Then
            Me.WindowState = FormWindowState.Minimized
            If My.Settings.MinimizeToTray Then Me.ShowInTaskbar = False
            FlashIcon()
        End If
        Me.Size = My.Settings.WindowSize
        InputBGPanel.Height = My.Settings.InputHeight
    End Sub

    Private Sub Skin() Handles Me.Load
        TS.Renderer = New ToolStripProfessionalRenderer(New ToolstripColorTable)
        CType(TS.Renderer, ToolStripProfessionalRenderer).RoundedEdges = False

        InputContMenu.Renderer = TS.Renderer
        OutputContMenu.Renderer = TS.Renderer
    End Sub

    Private Sub UsersWatcher_Deleted(username As String) ' Handles UsersWatcher.Deleted
        Users.Remove(username)
        If Channels.Current = username Then SwitchChannel(Channels.Common)
    End Sub

    Private Sub SwitchChannel(channel As String)
        If channel <> Channels.Current Then
            Me.Output.CacheChannelHtml(Channels.Current)
            Channels.Switch(channel)
            If Output.LoadCachedChannelHtml(channel) Then
                MessageQueues.LoadMessages(channel)
            Else
                sfcm.LoadOldMessages(channel, My.Settings.MessageQueueCapacity, AddressOf Output.AddMessage)
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
                Else
                    Input.SelectAll()
                End If
            End If
        End If
    End Sub

    Friend Function ProcessInput() As Boolean
        Dim command As String = Nothing
        If Input.Text(0) = "/"c Then
            Dim I As Integer = Input.Text.IndexOf(" "c)
            command = Input.Text.Substring(1, If(I <> -1, I, Input.Text.Length) - 1)
        Else
            Return SendMessage(MessageFlags.None)
        End If

        Select Case command.ToLower
            Case "me"
                Return SendMessage(MessageFlags.MeIs)
            Case "ping"
                If Input.Text.Length > 6 Then
                    Dim username As String = Input.Text.Substring(6).TrimEnd()
                    If Users.IsOnline(username) OrElse username = My.Settings.Username Then
                        sfcm.Ping(Input.Text.Substring(6).TrimEnd())
                        Return True
                    Else
                        Output.AddMessage("Cannot ping " & username & ". User is not online.")
                        Return False
                    End If
                End If
            Case "status"
                Dim message As String = String.Empty
                If Users.MyUser.IrcNick IsNot Nothing Then
                    message &= "IRC connected: " & Boolean.TrueString
                    message &= "<br>IRC nickname: " & Users.MyUser.IrcNick
                Else
                    message &= "<br>IRC Connected: " & Boolean.FalseString
                    message &= "<br>IRC Nickname: " & "-"
                End If
                message &= "<br>System NetworkTime: " & Date.UtcNow.ToString()
                message &= "<br>Network NetworkTime: " & NetworkTime.UtcNow.ToString()
                Output.AddMessage(message)
            Case "userinfo"
                If Input.Text.Length > 9 Then
                    Dim username As String = Input.Text.Substring(9).TrimEnd()
                    Dim usr As User = Users.GetByName(username)
                    Dim message As String = "Username: " & username
                    message &= "<br>Online: " & usr.IsOnline.ToString
                    If usr.IrcNick IsNot Nothing Then
                        message &= "<br>IRC Connected: " & Boolean.TrueString
                        message &= "<br>IRC Nickname: " & usr.IrcNick
                    Else
                        message &= "<br>IRC Connected: " & Boolean.FalseString
                        message &= "<br>IRC Nickname: " & "-"
                    End If
                End If
            Case "ircpm"
                If Input.Text.Length > 14 Then
                    Dim i As Integer = Input.Text.IndexOf(" ", 13)
                    If i = -1 Then Return False
                    Dim recipientnick As String = Input.Text.Substring(12 + 1, i - 12 - 1)
                    SendIrcMessage(recipientnick, Input.Text.Substring(i + 1))
                End If
            Case "exit", "close"
                Me.Close()
            Case Else
                Return False
        End Select
        Return True
    End Function

    Public Delegate Sub MessageLoadingSub(msg As Message)

    Friend Function SendMessage(messageType As MessageFlags) As Boolean
        Dim msg As New Message()
        Dim attachedFiles As New List(Of String)
        msg.Sender = My.Settings.Username
        msg.Flags = messageType
        msg.Time = NetworkTime.UtcNow.ToBinary()

        If (messageType And MessageFlags.MeIs) = 0 Then
            msg.HtmlContent = HtmlConverter.HtmlizeInputAndGetFiles(Input.Text, attachedFiles)
        Else
            msg.HtmlContent = HtmlConverter.HtmlizeInputAndGetFiles(My.Settings.Username & Input.Text.Substring(3), attachedFiles)
        End If

        sfcm.SendMessage(Channels.Current, msg)
        irccm.SendMessage(Channels.Current, msg)

        For Each af As String In attachedFiles
            Files.SendFile(af)
        Next
        Me.Output.AddMessage(msg)
        Return True
    End Function

    Friend Function SendIrcMessage(recipientNick As String, message As String) As Boolean
        irccm.SendMessage(recipientNick, message)
        Return True
    End Function

#End Region

#Region "Resizing and minimizing"

    Private Sub Box_ResizeEnd(sender As Object, e As EventArgs) Handles MyBase.ResizeEnd
        My.Settings.WindowSize = Me.Size
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
        My.Settings.InputHeight = InputBGPanel.Height
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

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        irccm.Initialize(Me)
    End Sub

#End Region

End Class