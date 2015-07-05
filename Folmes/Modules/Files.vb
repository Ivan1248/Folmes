Imports System.Runtime.InteropServices

Module Files

    Structure Extension
        Const Message As String = ".fmsg"
        Const Ping As String = ".ping"
        Const Pong As String = ".pong"
        Const UserInfo As String = ".info"
    End Structure

    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True, BestFitMapping:=False)>
    Public Function MoveFile(src As String, dest As String) As Boolean
    End Function

End Module
