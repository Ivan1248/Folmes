Namespace GUI
    Public Class InputTextBox : Inherits TextBox

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

        ' Ctrl+a
        Private Sub SelectAll_Input_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Me.KeyPress
            If e.KeyChar = Chr(1) Then
                DirectCast(sender, TextBox).SelectAll()
                e.Handled = True
            End If
        End Sub

    End Class
End Namespace