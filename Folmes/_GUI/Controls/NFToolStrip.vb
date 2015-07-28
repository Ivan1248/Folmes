Namespace GUI.Controls
    Public Class NFToolStrip : Inherits ToolStrip
        'Doesn't require focus un window to accept clicks

        Const WM_LBUTTONDOWN As Integer = &H201
        Const WM_LBUTTONUP As Integer = &H202

        Private Shared down As Boolean = False

        Protected Overrides Sub WndProc(ByRef m As Windows.Forms.Message)
            If m.Msg = WM_LBUTTONUP AndAlso Not down Then
                m.Msg = WM_LBUTTONDOWN
                MyBase.WndProc(m)
                m.Msg = WM_LBUTTONUP
            End If

            If m.Msg = WM_LBUTTONDOWN Then down = True
            If m.Msg = WM_LBUTTONUP Then down = False

            MyBase.WndProc(m)
        End Sub

        Sub New()
            MyBase.New()
        End Sub
    End Class
End Namespace