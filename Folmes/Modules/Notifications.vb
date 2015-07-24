Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic.Devices

Public Module Notifications
    Enum Notifications As Integer
        PublicMessage = 0
        PrivateMessage = 1
        LoggedIn = 2
        LoggedOut = 3
        Joined = 4
    End Enum

    Public Sub Notify(kind As Notifications, msg As String)
        FlashIcon()
        If GetActiveWindow <> MainGUI.Handle Then
            Dim notifs As New BitArray(New Integer() {My.Settings.Notifications})
            Dim sound As New Audio
            Select Case kind
                Case Notifications.PublicMessage 'nova poruka
                    If notifs(0) Then
                        ShowNotification("New message", " " & msg)
                    End If
                    If notifs(1) Then
                        sound.Play("C:\Windows\Media\Speech On.wav")
                    End If
                Case Notifications.PrivateMessage 'privatna poruka 
                    If notifs(2) Then
                        ShowNotification("New private message", " " & msg)
                    End If
                    If notifs(3) Then
                        sound.Play("C:\Windows\Media\Speech On.wav")
                    End If
                Case Notifications.LoggedIn 'prijava
                    If notifs(4) Then
                        ShowNotification("User logged in", " " & msg & " logged in to Dropbox Messenger")
                    End If
                    If notifs(5) Then
                        sound.Play("C:\Windows\Media\Speech On.wav")
                    End If
                Case Notifications.LoggedOut 'odjava   
                    If notifs(6) And MainGUI.WindowState = FormWindowState.Minimized Then
                        ShowNotification("User logged out", " " & msg & " logged out from Dropbox Messenger")
                    End If '
                    If notifs(7) Then
                        sound.Play("C:\Windows\Media\Speech Off.wav")
                    End If
                Case Notifications.Joined _
                    'novi korisnik                                                                               '
                    If notifs(5) Then
                        sound.Play("C:\Windows\Media\Speech On.wav")
                    End If
                    ShowNotification("New user joined", " " & msg & " joined Dropbox Messenger")
            End Select
        End If
    End Sub

    Private Sub ShowNotification(title As String, text As String)
        MainGUI.TrayIcon.ShowBalloonTip(1000, title, text, ToolTipIcon.None)
    End Sub

    '<DllImport("user32.dll", CharSet:=CharSet.Auto, ExactSpelling:=)>
    'Public Function GetActiveWindow() As IntPtr
    'End Function

    Public Declare Function GetActiveWindow Lib "user32.dll" () As IntPtr

    'taskbar icon
    Public Structure FLASHWINFO
        Public CbSize As Int32
        Public Hwnd As IntPtr
        Public DwFlags As Int32
        Public UCount As Int32
        Public DwTimeout As Int32
    End Structure

    Private Declare Function FlashWindowEx Lib "user32.dll" (ByRef pfwi As FLASHWINFO) As Int32

    Public Sub FlashIcon()
        Dim flash As New FLASHWINFO
        With flash
            .CbSize = Marshal.SizeOf(flash) '/// size of structure in bytes
            .Hwnd = MainGUI.Handle '/// Handle to the window to be flashed
            .DwFlags = &H2 '/// to flash both the tray , &H1 for caption (&H2 or &H1) for both
            .UCount = 3 '/// the number of flashes
        End With
        'flash.dwTimeout = 500 '/// speed of flashes in MilliSeconds ( can be left out )
        FlashWindowEx(flash) '/// flash the window you have specified the handle for...
    End Sub
End Module