Imports System.IO
Imports System.Linq.Expressions
Imports System.Net.Sockets
Imports System.Threading
Imports System.Runtime.CompilerServices

Public Class IrcClient : Implements IDisposable
    Dim sendingQueue As New ThreadSafeQueue(Of String)

    Public Event Connected(nickName As String)
    Public Event LostConnection()
    Public Event MessageReceived(message As Message)
    Public Event PongReceived(rtt As Long)

    Dim server As String
    Dim port As Integer = 6667

    Dim sock As TcpClient
    Dim stream As NetworkStream
    Dim input As StreamReader
    Dim output As StreamWriter

    Dim channel As String = "#Folmes"
    Dim username As String

    Dim thr As Thread

    Sub New(username As String, server As String)
        Me.username = username
        Me.server = server
    End Sub

    Public Sub Run()
        thr = New Thread(New ThreadStart(Sub() RunInternal()))
        thr.IsBackground = True
        thr.Start()
    End Sub

    Private Sub RunInternal()
        ' Connect to the irc server and get input and output text streams from TcpClient.
        If Not Connect() Then Exit Sub
        RaiseEvent Connected(username)

        ' Process each line received from the server and measure RTT
        Dim rttStopwatch As New Stopwatch
        While True
            Const sleepTime As Integer = 100 ' 100 ms
            Const pingPeriod As Integer = sleepTime * 100 \ 100 ' 10 s / 100
            Dim pingCounter As Integer = 0

            While Not IsDataReceived()
                SendPending()

                If pingCounter < pingPeriod Then
                    pingCounter += 1
                    Thread.Sleep(sleepTime)
                Else
                    pingCounter = 0
                    output.WriteLine("PING " & server)
                    output.Flush()
                    Debug.WriteLine("PING")
                    rttStopwatch.Restart()
                    While True
                        While Not IsDataReceived()
                            Thread.Sleep(10)
                            If rttStopwatch.ElapsedMilliseconds > 500 Then
                                RaiseEvent LostConnection()
                                Exit Sub
                            End If
                        End While
                        Dim buf As String = input.ReadLine()
                        If IsPongCommand(buf) Then
                            Debug.WriteLine(rttStopwatch.ElapsedMilliseconds)
                            RaiseEvent PongReceived(rttStopwatch.ElapsedMilliseconds)
                            Exit While
                        Else
                            ProcessReceivedCommand(buf)
                        End If
                    End While
                    rttStopwatch.Stop()
                End If
            End While
            ProcessReceivedCommand(input.ReadLine())
        End While
        sock.Close()
    End Sub

    Private Function Connect() As Boolean
conn:   sock = New TcpClient
        Dim buf As String
        Try
            sock.Connect(server, port)
        Catch ex As Exception
            MsgBox(Me.GetType().ToString() + ": " + ex.Message)
        End Try
        If Not sock.Connected Then
            sock.Close()
            Debug.WriteLine(Me.GetType().ToString() + ": " + "Failed to connect.")
            Return False
        End If

        stream = sock.GetStream()
        input = New StreamReader(stream)
        output = New StreamWriter(stream)

        SendUserAndNickCommands()

        While True
            While Not IsDataReceived() ' strange bahaviour when one of conditions used
                Thread.Sleep(1)
            End While
            buf = input.ReadLine()
            Debug.WriteLine(buf)
            Select Case buf.Substring(buf.IndexOf(" "c) + 1, 3)
                Case "001"
                    Debug.WriteLine("Connected.")
                    Exit While
                Case "433"
                    Debug.WriteLine("Nickname in use.")
                    username &= "_"
                    sock.Close()
                    GoTo conn
                Case "437"
                    Debug.WriteLine("Nickname temporarily unavailable.")
                    username &= "_"
                    sock.Close()
                    GoTo conn
            End Select
        End While

        ' Set mode and join channel
        output.WriteLine("MODE " & username & " +C")
        output.WriteLine("JOIN " & channel)
        output.Flush()
        Return True
    End Function

    Private Sub SendPending()
        While sendingQueue.Count > 0
            output.WriteLine(sendingQueue.Dequeue)
            output.Flush()
        End While
    End Sub
    
    Private Shared Function IsPongCommand(command As String) As Boolean
        Return command.Substring(command.IndexOf(" "c) + 1, 4) = "PONG"
    End Function

    Private Function IsDataReceived() As Boolean  ' strange bahaviour when one of conditions used
        If stream.DataAvailable Then Return True
        Try
            Return input.Peek() <> -1
        Catch ex As Exception
            Debug.WriteLine(Me.GetType().ToString() + ": " + ex.Message)
            Return False
        End Try
    End Function

    Private Sub ProcessReceivedCommand(command As String)
        ' Reply to ping messages
        If command.StartsWith("PING ") Then
            output.WriteLine("PONG " & command.Substring(5))
            output.Flush()
            Exit Sub
        End If

        Debug.WriteLine(command)

        If command(0) <> ":"c Then Exit Sub

        Dim m As Message = IrcMessage.GetMessageFromCommand(command)
        If m IsNot Nothing Then
            RaiseEvent MessageReceived(m)
        End If
    End Sub

    Private Sub SendUserAndNickCommands()
        output.WriteLine("USER " & username & " 8 * :" & username) ' 8 - invisible
        output.WriteLine("NICK " & username)
        output.Flush()
    End Sub

    Public Sub SendCommand(command As String)
        If command Is Nothing Then Exit Sub
        sendingQueue.Enqueue(command)
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        sock.Close()
    End Sub
End Class
