Imports System.IO
Imports System.Text
Imports SharpFolmes

Namespace Classes
    'Struktura datoteke:
    '   1 datoteka(16384) : 16 kiB
    '   1.1 unos(512) × 32
    '   1.1.1 vrijeme(8)
    '   1.1.2 zastavice(2)
    '   1.1.3 duljina(2)
    '   1.1.3 sadržaj(500)

    'Queues:
    'N = new, O = old, P = passed, E = empty
    '1. OOOOOEEE
    '2. OOPPPEEE
    '3. OOPPPNNE
    '4. OOPPPPPE
    '5. NNNPPPPN

    Public Class Message
        Public Sender As String
        Public Type As MessageType
        Public Content As String
        Public Time As Long

        Enum MessageType As Short
            Normal
            Reflexive
            Declaration
        End Enum
    End Class


    ''' <summary>
    '''     OLD - was in the file at the moment of opening
    '''     NEW - appeared in the file after it has been opened 
    ''' </summary>
    Public Class MessageFile
        Implements IDisposable

#Region "Constants"

        Const B As Integer = 512
        Const N As Integer = 8          ' 32
        Const DateInd As Integer = 0       ' DateIndex
        Const TypeInd As Integer = 8       ' TypeIndex
        Const LenInd As Integer = 10      ' LengthIndex
        Const ContInd As Integer = 12      ' ContentIndex

#End Region

#Region "Fields and properties"

        Private ReadOnly _file As FileStream
        Private ReadOnly _memFile(N * B) As Byte     'kopija u memoriji
        Public ReadOnly Path As String
        Public ReadOnly Sender As String
        Public ReadOnly Recipient As String
        ' OldQueue
        Private _oldQueue_Length As Integer
        Private _oldQueue_NextIndex As Integer    ' the newest unread old message
        Private _oldQueue_NextTime As Long
        ' NewQueue
        Private _newQueue_Length As Integer = 0
        Private _newQueue_NextIndex As Integer = 0      ' the oldest unread new message
        Private _newQueue_NextTime As Long
        'Newest message
        Private _newestIndex As Integer = Decr(0)              ' the absolutely newest message
        Private _newestTime As Long = 0
        'Passed Old I New
        Private _passedLength As Integer = 0     ' 0 za novu datoteku

        ReadOnly Property OldQueueLength() As Integer
            Get
                Return _oldQueue_Length
            End Get
        End Property

        ReadOnly Property NextUnreadOldTime() As Long
            Get
                Return _oldQueue_NextTime
            End Get
        End Property

        ReadOnly Property NewQueueLength() As Integer
            Get
                Return _newQueue_Length
            End Get
        End Property

        ReadOnly Property NextUnreadNewTime() As Long
            Get
                Return _newQueue_NextTime
            End Get
        End Property

#End Region

#Region "New + Dispose"

        Public Sub New(path As String, outgoing As Boolean, sender As String, Optional recipient As String = "")
            Me.Path = path
            If outgoing Then
                _file = New FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read, 2 * B,
                                       FileOptions.SequentialScan)
            Else 'ingoing
                _file = New FileStream(path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite, 2 * B,
                                       FileOptions.SequentialScan)
            End If
            _file.Seek(0, SeekOrigin.Begin)
            If _file.Length = N * B Then
                ReadAll()
            Else
                ' _file.SetLength(N * B) - izaziva grešku kod drugih procesa
                For i As Integer = 0 To N * B - 1
                    _file.WriteByte(0)
                Next
                _file.Flush()
            End If
            Me.Sender = sender
            Me.Recipient = recipient
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

#Region "File -> MemFile"

        ''' <summary>
        '''     Učitava sve podatke i stavlja ih u OldQueue
        ''' </summary>
        Private Sub ReadAll()
            Dim I As Integer = 0
            Dim currTime As Long

            _file.Seek(0, SeekOrigin.Begin)
            _file.Read(_memFile, 0, N * B)  ' Load the whole file

            currTime = GetTime(I)

            If currTime = 0 Then Exit Sub

            Do ' Find the newest message
                _oldQueue_NextTime = currTime
                I = Incr(I)
                currTime = GetTime(I)
            Loop While currTime > _oldQueue_NextTime

            _newQueue_NextIndex = I
            _newestIndex = Decr(I)
            _newestTime = GetTime(_newestIndex)
            _oldQueue_NextIndex = _newestIndex

            _oldQueue_Length = If(GetTime(I) = 0, I \ B, N * B)
        End Sub

        ''' <summary>
        '''     Učitava nove poruke i stavlja ih u NewQueue.
        '''     Mijenja OldQueue i Passed ako je potrebno.
        ''' </summary>
        Public Sub ReadNew()
            Dim lastTime As Long = _newestTime
            Dim I As Integer = _newestIndex
            Dim currTime As Long

            I = Incr(I) : ReadBlock(I)
            currTime = GetTime(I)

            If currTime < lastTime Then Exit Sub ' case: no new messages

            If _newQueue_Length = 0 Then _newQueue_NextTime = currTime
            Do
                _newQueue_Length += 1             'amortizacija, nije potrebno kasnije smanjiti
                lastTime = currTime
                I = Incr(I) : ReadBlock(I)
                currTime = GetTime(I)
            Loop While currTime > lastTime

            ' check whether there is more than N new messages
            ReadBlock(_newestIndex)
            If GetTime(_newestIndex) <> _newestTime Then ' all messages new (overwritten)
                _newQueue_Length = N
                _newQueue_NextIndex = Incr(_newestIndex)
                _newQueue_NextTime = GetTime(_newQueue_NextIndex)
                _passedLength = 0
                _oldQueue_Length = 0
            ElseIf _newQueue_Length >= N - _passedLength Then ' oldQueue messages overwritten
                _passedLength = N - _newQueue_Length
                _oldQueue_Length = 0
            ElseIf _newQueue_Length > N - (_passedLength + _oldQueue_Length) Then ' some oldQueue messsages overwritten
                _oldQueue_Length = N - (_passedLength + _newQueue_Length)
            End If

            _newestIndex = Decr(I)
            _newestTime = lastTime
        End Sub

        Sub ConvertOldQueueToNewQueue()
            _newQueue_Length += _oldQueue_Length
            _newQueue_NextIndex = Incr(_oldQueue_NextIndex - B * _oldQueue_Length)
            _newQueue_NextTime = GetTime(_newQueue_NextIndex)

            _newestIndex = Decr(_newQueue_NextIndex + B * _newQueue_Length)
            _newestTime = GetTime(_newestIndex)

            _oldQueue_Length = 0
        End Sub

#End Region

#Region "User -> MemFile -> File"

        ''' <summary>
        '''     Zapisuje novu poruku u sljedeći blok.
        ''' </summary>
        Public Sub StoreEntry(msg As Message)
            _newestIndex = Incr(_newestIndex)
            _file.Seek(_newestIndex, SeekOrigin.Begin)
            ByteConverter.GetBytes(msg.Time, _memFile, _newestIndex)
            ByteConverter.GetBytes(msg.Type, _memFile, _newestIndex + TypeInd)
            Dim contLen As Integer =
                    Encoding.UTF8.GetBytes(msg.Content, 0, msg.Content.Length, _memFile, _newestIndex + ContInd)
            'TU JE GREŠKA
            ByteConverter.GetBytes(CShort(contLen), _memFile, _newestIndex + LenInd)
            _memFile(_newestIndex + ContInd + contLen) = 0
            _file.Write(_memFile, _newestIndex, contLen + ContInd + 1)
            _file.Flush(True)
        End Sub

#End Region

#Region "MemFile -> User"

        ''' <summary>
        '''     Vraća najstariju novu poruku (prvu poruku iz NewQueue).
        ''' </summary>
        Public Function GetNextOlder() As Message
            GetNextOlder = New Message With {
                .Sender = Sender,
                .Content = GetString(_oldQueue_NextIndex),
                .Time = _oldQueue_NextTime,
                .Type = GetMessageType(_oldQueue_NextIndex)
                }
            _oldQueue_Length -= 1
            _oldQueue_NextIndex = Decr(_oldQueue_NextIndex)
            If _oldQueue_Length > 0 Then _oldQueue_NextTime = GetTime(_oldQueue_NextIndex)
            _passedLength += 1
        End Function

        ''' <summary>
        '''     Vraća najnoviju staru poruku (prvu poruku iz oldQueue).
        ''' </summary>
        Public Function GetNextNewer() As Message
            GetNextNewer = New Message With {
                .Sender = Sender,
                .Type = GetMessageType(_newQueue_NextIndex),
                .Content = GetString(_newQueue_NextIndex),
                .Time = _newQueue_NextTime
                }
            _newQueue_Length -= 1
            _newQueue_NextIndex = Incr(_newQueue_NextIndex)
            If _newQueue_Length > 0 Then _newQueue_NextTime = GetTime(_newQueue_NextIndex)
            _passedLength += 1
        End Function

#End Region

#Region "Interne pomoćne funkcije"

        Private Shared Function Incr(ByRef I As Integer) As Integer
            Return (I + B) And (N * B - 1)
        End Function

        Private Shared Function Decr(ByRef I As Integer) As Integer
            Return (I - B) And (N * B - 1)
        End Function

        Private Function GetString(index As Integer) As String
            Return Encoding.UTF8.GetString(_memFile, index + ContInd, ByteConverter.ToInt16(_memFile, index + LenInd))
        End Function

        Private Function GetTime(index As Integer) As Long
            Return ByteConverter.ToInt64(_memFile, index + DateInd)
        End Function

        Private Function GetMessageType(index As Integer) As Message.MessageType
            Return DirectCast(ByteConverter.ToInt16(_memFile, index + TypeInd), Message.MessageType)
        End Function

        Private Sub ReadBlock(I As Integer)
            _file.Seek(I, SeekOrigin.Begin)
            _file.Read(_memFile, I, B)
        End Sub

#End Region
    End Class
End Namespace