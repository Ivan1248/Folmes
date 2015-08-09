Imports System.IO

Public Class Settings

    ReadOnly _usernameColor As Color = ColorTranslator.FromHtml(My.Settings.UsernameColor)
    Dim _notifChkBoxes() As CheckBox
    ReadOnly _startupRegKey As Microsoft.Win32.RegistryKey = My.Computer.Registry.CurrentUser.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Run", True)

    Public Sub Form1_Load(sender As System.Object, e As EventArgs) Handles MyBase.Load
        Me.Owner = MainGUI

        'General
        Username.Text = My.Settings.Username
        Username.ForeColor = _usernameColor
        palette = New Bitmap(My.Resources.Paleta, ColorPickingPictureBox.Width, ColorPickingPictureBox.Height)

        MinimizeToNotificationArea.Checked = My.Settings.MinimizeToTray
        StartMinimized.Checked = My.Settings.StartMinimized
        LaunchOnStartup.Checked = Not _startupRegKey.GetValue(Application.ProductName) Is Nothing
        'MsgBox(StartupRegKey.GetValue(Application.ProductName))
        HideFolderCB.Checked = IsFolderHidden()

        'Interface
        FontSize.Value = My.Settings.FontSize
        FontSizeLabel.Text = My.Settings.FontSize & " px"

        ThumbnailHeight.Value = My.Settings.ThumbnailHeight \ 20
        ImageHeightLabel.Text = My.Settings.ThumbnailHeight & " px"

        nOfMsgs.Value = My.Settings.NofMsgs \ 5
        nOfMsgsLabel.Text = CStr(My.Settings.NofMsgs)

        'Notifications
        _notifChkBoxes = {ncb1, ncb2, ncb3, ncb4, ncb5, ncb6, ncb7, ncb8}
        Dim bits As New BitArray(New Integer() {My.Settings.Notifications})
        For i As Integer = 0 To 7
            _notifChkBoxes(i).Checked = bits(i)
        Next
    End Sub
    Public Function IsFolderHidden() As Boolean
        Return (My.Computer.FileSystem.GetDirectoryInfo(Dirs.Folmes).Attributes And FileAttributes.Hidden) <> 0
    End Function

#Region "General"

    'Username
    Private palette As Bitmap
    Private Sub Color_MouseDown(sender As Object, e As MouseEventArgs) Handles ColorPickingPictureBox.MouseDown, ColorPickingPictureBox.MouseMove
        If e.Button <> MouseButtons.Left Then Exit Sub
        Dim x As Integer = e.X
        If x < 0 Then
            x = 0
        ElseIf x > palette.Width - 1 Then
            x = palette.Width - 1
        End If
        Username.ForeColor = palette.GetPixel(x, 0)
    End Sub

    'General
    Public Sub RunOnStartup(ByVal enable As Boolean)
        With _startupRegKey
            If enable Then
                .SetValue(Application.ProductName, Application.ExecutablePath)
            ElseIf Not _startupRegKey.GetValue(Application.ProductName) Is Nothing Then
                .DeleteValue(Application.ProductName)
            End If
        End With
    End Sub

#End Region

#Region "Interface"

    'Font size
    Private Sub FontSize_Scroll(sender As Object, e As EventArgs) Handles FontSize.Scroll
        FontSizeLabel.Text = CStr(FontSize.Value)
    End Sub
    Private Sub FontSize_Reset(sender As Object, e As EventArgs) Handles FontSize.MouseDoubleClick
        FontSize.Value = 12
        FontSizeLabel.Text = "12"
    End Sub

    'Maximum image height
    Private Sub ImageHeight_Scroll(sender As Object, e As EventArgs) Handles ThumbnailHeight.Scroll
        ImageHeightLabel.Text = CStr(ThumbnailHeight.Value * 20)
    End Sub

    'Number of messages to load
    Private Sub nOfMsgs_Scroll(sender As Object, e As EventArgs) Handles nOfMsgs.Scroll
        nOfMsgsLabel.Text = CStr(nOfMsgs.Value * 5)
    End Sub

    'Reset all
    Private Sub DefaultAppearance_Click(sender As Object, e As EventArgs) Handles DefaultAppearance.Click
        FontSize.Value = 12
        FontSizeLabel.Text = "12"
        ThumbnailHeight.Value = 3
        ImageHeightLabel.Text = "80"
        nOfMsgs.Value = 6
        nOfMsgsLabel.Text = "30"
    End Sub

#End Region

#Region "Notifications"
    Private Function NotificationCheckboxesValue() As Integer
        Dim result As Integer = _notifChkBoxes(0).CheckState
        Dim b As Integer = 1
        For i As Integer = 1 To _notifChkBoxes.Length - 1
            b *= 2
            result = result Or _notifChkBoxes(i).CheckState * b
        Next
        Return result
    End Function
#End Region

    Public Sub Apply()
        With My.Settings
            'General
            Users.MyUser.Color = ColorTranslator.ToHtml(Username.ForeColor)
            Users.MyUser.SaveInfo()
            .MinimizeToTray = MinimizeToNotificationArea.Checked
            .StartMinimized = StartMinimized.Checked
            RunOnStartup(LaunchOnStartup.Checked)
            Dirs.HideFolmesFolder(HideFolderCB.Checked)

            'Interface
            .FontSize = FontSize.Value
            MainGUI.Input.Font = New Font("Arial", FontSize.Value, FontStyle.Regular, GraphicsUnit.Pixel)
            .ThumbnailHeight = ThumbnailHeight.Value * 20
            .NofMsgs = nOfMsgs.Value * 5

            'Notifications
            .Notifications = NotificationCheckboxesValue()
        End With
    End Sub

#Region "Donja dugmad"

    Private Sub OKButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OKButton.Click
        Apply()
        Me.Close()
    End Sub

    Private Sub CancelButton_Click(sender As Object, e As EventArgs) Handles CancButton.Click
        Me.Close()
    End Sub

    Private Sub ApplyButton_Click(sender As Object, e As EventArgs) Handles ApplyButton.Click
        Apply()
    End Sub

#End Region

    Private Sub ResetButton_Click(sender As Object, e As EventArgs) Handles ResetButton.Click
        My.Settings.Reset()
        Users.MyUser.SetAndSaveStatus(UserStatus.Offline)
        RunOnStartup(False)
        Application.Exit()
    End Sub


End Class
