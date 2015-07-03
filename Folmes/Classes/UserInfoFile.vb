Imports System.IO

Namespace Classes
    'File structure:
    '   1 file(1) : 1B
    '   1.1 onlineStatus(1)

    Public Class UserInfoFile
        Implements IDisposable

#Region "Constants"

        Const FileSize As Integer = 1

#End Region

#Region "Variables"

        Private _file As FileStream
        Public Path As String
        Public Name As String
        Public Online As Boolean

#End Region

#Region "New + Dispose"

        Public Sub New(path As String)
            Me.Path = path
            Refresh()
        End Sub

        Dim _disposed As Boolean = False

        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

        Protected Overridable Sub Dispose(disposing As Boolean)
            If _disposed Then Return
            If disposing Then _file.Close()
            _disposed = True
        End Sub

#End Region

#Region "Private"

        Private Sub OpenFile()
            _file = New FileStream(Path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, FileSize)
        End Sub

#End Region

#Region "Read"

        Public Sub Refresh()
            OpenFile()
            Using _file
                _file.Seek(0, SeekOrigin.Begin)
                Online = _file.CanRead AndAlso CBool(_file.ReadByte())
            End Using
        End Sub

#End Region

#Region "Write"

        Public Sub SetOnlineStatus(status As Boolean)
            OpenFile()
            Using _file
                _file.Seek(0, SeekOrigin.Begin)
                _file.WriteByte(CByte(status))
                _file.Flush()
            End Using
        End Sub

#End Region
    End Class
End Namespace