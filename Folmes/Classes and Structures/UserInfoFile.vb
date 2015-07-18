Imports System.IO
Imports System.Runtime.CompilerServices

Namespace Classes
    'File structure:
    '   1 file(1) : 1B
    '   1.1 onlineStatus(1)

    Public Class UserInfoFile

#Region "Constants"

        Const FileSize As Integer = 1

#End Region

#Region "Variables"

        Public Path As String
        Public Username As String
        Public Online As Boolean = False

#End Region

#Region "New + Dispose"

        Public Sub New(path As String)
            Me.Path = path
            Refresh()
        End Sub

#End Region

#Region "Private"

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function UserFile() As FileStream
            Return New FileStream(Path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, FileSize)
        End Function

#End Region


#Region "Read"

        Public Sub Refresh()
            Using fs As FileStream = UserFile()
                fs.Seek(0, SeekOrigin.Begin)
                Online = fs.CanRead AndAlso CBool(fs.ReadByte())
            End Using
        End Sub

#End Region

#Region "Write"

        Public Sub SetOnlineStatus(status As Boolean)
            Using fs As FileStream = UserFile()
                fs.Seek(0, SeekOrigin.Begin)
                fs.WriteByte(CByte(status))
                fs.Flush()
            End Using
        End Sub

#End Region
    End Class
End Namespace