Imports System.IO

Public Class Config

    Dim usernameIsValid As Boolean = True
    Dim UsernameColor As Color = System.Drawing.ColorTranslator.FromHtml(My.Settings.UsernameColor)
    Dim NotifChkBoxes() As CheckBox
    Dim StartupRegKey As Microsoft.Win32.RegistryKey = My.Computer.Registry.CurrentUser.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Run", True)

    Public Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Owner = Box

        'General
        Username.Text = My.Settings.Username
        Username.ForeColor = UsernameColor

        MinimizeToNotificationArea.Checked = My.Settings.MinimizeToTray
        StartMinimized.Checked = My.Settings.StartMinimized
        LaunchOnStartup.Checked = Not StartupRegKey.GetValue(Application.ProductName) Is Nothing
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
        NotifChkBoxes = {ncb1, ncb2, ncb3, ncb4, ncb5, ncb6, ncb7, ncb8}
        Dim bits As New BitArray(New Integer() {My.Settings.Notifications})
        For i As Integer = 0 To 7
            NotifChkBoxes(i).Checked = bits(i)
        Next
    End Sub
    Public Function IsFolderHidden() As Boolean
        Return (My.Computer.FileSystem.GetDirectoryInfo(FolmesDir).Attributes And FileAttributes.Hidden) <> 0
    End Function

#Region "General"

    'Username
    Private Sub Color_Click(sender As Object, e As EventArgs) Handles C0.Click, C1.Click, C2.Click, C3.Click, C4.Click, C5.Click, C6.Click, C7.Click, C8.Click, C9.Click
        Username.ForeColor = CType(sender, Button).BackColor
    End Sub

    'General
    Public Sub RunOnStartup(ByVal enable As Boolean)
        With StartupRegKey
            If enable Then
                .SetValue(Application.ProductName, Application.ExecutablePath)
            ElseIf Not StartupRegKey.GetValue(Application.ProductName) Is Nothing Then
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
        ThumbnailHeight.Value = 5
        ImageHeightLabel.Text = "120"
        nOfMsgs.Value = 4
        nOfMsgsLabel.Text = "20"
    End Sub

#End Region

#Region "Notifications"
    Private Function NotificationCheckboxesValue() As Integer
        Dim result As Integer = NotifChkBoxes(0).CheckState
        Dim b As Integer = 1
        For i As Integer = 1 To NotifChkBoxes.Length - 1
            b *= 2
            result = result Or NotifChkBoxes(i).CheckState * b
        Next
        Return result
    End Function
#End Region

    Public Sub Apply()
        With My.Settings
            'General
            .UsernameColor = System.Drawing.ColorTranslator.ToHtml(Username.ForeColor)
            .MinimizeToTray = MinimizeToNotificationArea.Checked
            .StartMinimized = StartMinimized.Checked
            RunOnStartup(LaunchOnStartup.Checked)
            HideFolmesFolder(HideFolderCB.Checked)

            'Interface
            .FontSize = FontSize.Value
            Box.Input.Font = New Font("Arial", FontSize.Value, FontStyle.Regular, GraphicsUnit.Pixel)
            .ThumbnailHeight = ThumbnailHeight.Value * 20
            .NofMsgs = nOfMsgs.Value * 5

            'Notifications
            .Notifications = NotificationCheckboxesValue()

            'Box.ReloadTimer.Start()
            ''Omogućuje učitavanje poruka odmah
            'TREBA PONOVO UČITATI PORUKE
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
        Box.UserInfoFiles.Mine.SetOnlineStatus(False)
        RunOnStartup(False)
        Box.Close()
    End Sub


End Class
