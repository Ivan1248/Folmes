Imports System.Runtime.InteropServices


Namespace GUI.Controls

    Public Class InputTextBox : Inherits TextBox

#Region "Mousewheel scrolling"
        Private Const WM_VSCROLL As Integer = &H115
        Private Const SB_LINEUP As Integer = 0
        Private Const SB_LINEDOWN As Integer = 1

        <DllImport("user32.dll", CharSet:=CharSet.Auto)>
        Private Shared Function SendMessage(hwnd As IntPtr, wMsg As Integer, wParam As IntPtr, lParam As IntPtr) As Integer
        End Function

        Sub MouseScroll(sender As Object, e As MouseEventArgs) Handles Me.MouseWheel
            If e.Delta > 0 Then
                SendMessage(Me.Handle, WM_VSCROLL, CType(SB_LINEUP, IntPtr), IntPtr.Zero)
            Else
                SendMessage(Me.Handle, WM_VSCROLL, CType(SB_LINEDOWN, IntPtr), IntPtr.Zero)
            End If
        End Sub
#End Region

#Region "Tripleclick"

        Dim _inTripleClickInterval As Boolean = False
        Dim WithEvents _clickTimer As New Timer With {.Interval = 240}

        Private Sub Input_DoubleClick(sender As Object, e As EventArgs) Handles Me.DoubleClick
            _inTripleClickInterval = True
            _clickTimer.Start()
        End Sub

        Private Sub Input_Click(sender As Object, e As EventArgs) Handles Me.Click
            If _inTripleClickInterval Then Me.SelectAll()
        End Sub

        Private Sub ClickTimerTick() Handles _clickTimer.Tick
            _inTripleClickInterval = False
            _clickTimer.Stop()
        End Sub

#End Region

#Region "Select All"
        Private Sub SelectAll_Input_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Me.KeyPress
            If e.KeyChar = Chr(1) Then
                DirectCast(sender, TextBox).SelectAll()
                e.Handled = True
            End If
        End Sub
#End Region

#Region "DragDrop"

        Private Shared Sub Input_DragEnter(sender As Object, e As DragEventArgs) Handles Me.DragEnter
            e.Effect = DragDropEffects.All
        End Sub

        Private Sub Input_DragDrop(sender As Object, e As DragEventArgs) Handles Me.DragDrop
            If (e.Data.GetDataPresent(DataFormats.Text)) Then
                Me.AppendText(CStr(e.Data.GetData(DataFormats.Text)))
            ElseIf e.Data.GetDataPresent(DataFormats.FileDrop) Then
                Dim filePath As String = CType(e.Data.GetData(DataFormats.FileDrop), String())(0)
                Me.AppendText("[file:" & filePath & "]")
            End If
            Me.Focus()
        End Sub

#End Region

    End Class
End Namespace