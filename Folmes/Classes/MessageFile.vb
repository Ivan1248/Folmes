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
    ''' </summary
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
        Private _oldQueueLength As Integer
        Private _nextNewestUnreadOldIndex As Integer    ' the newest unread old message
        Private _nextNewestUnreadOldTime As Long
        ' NewQueue
        Private _newQueueLength As Integer = 0
        Private _nextUnreadNewIndex As Integer = 0      ' the oldest unread new message
        Private _nextUnreadNewTime As Long
        'Newest message
        Private _newestIndex As Integer                 ' the absolutely newest message
        'Passed Old I New
        Private _passedLength As Integer = 0     ' 0 za novu datoteku

        ReadOnly Property OldQueueLength() As Integer
            Get
                Return _oldQueueLength
            End Get
        End Property

        ReadOnly Property NextUnreadOldTime() As Long
            Get
                Return _nextNewestUnreadOldTime
            End Get
        End Property

        ReadOnly Property NewQueueLength() As Integer
            Get
                Return _newQueueLength
            End Get
        End Property

        ReadOnly Property NextUnreadNewTime() As Long
            Get
                Return _nextUnreadNewTime
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
            If _file.Length <> N * B Then
                ' _file.SetLength(N * B) - izaziva grešku kod drugih procesa
                For i As Integer = 0 To N * B - 1
                    _file.WriteByte(0)
                Next
                _file.Flush()
            Else
                ReadAll()
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
            Dim currTime As Long = 0

            _file.Seek(0, SeekOrigin.Begin)
            _file.Read(_memFile, 0, N * B)  ' Load the whole file

            currTime = GetTime(I)
            If currTime = 0 Then
                _newestIndex = -B
                Exit Sub
            End If
            Do ' Find the newest message
                _nextNewestUnreadOldTime = currTime
                I = Incr(I)
                currTime = GetTime(I)
            Loop While currTime > _nextNewestUnreadOldTime

            _nextUnreadNewIndex = I '
            _newestIndex = Decr(I)
            _nextNewestUnreadOldIndex = _newestIndex

            _oldQueueLength = If(GetTime(I) = 0, I \ B, N * B)
        End Sub

        ''' <summary>
        '''     Učitava nove poruke i stavlja ih u NewQueue.
        '''     Mijenja OldQueue i Passed ako je potrebno.
        ''' </summary>
        Public Sub ReadNew()
            Static newestTime As Long = 0
            Dim I As Integer = _newestIndex
            Dim currTime As Long

            I = Incr(I)
            ReadBlock(I)
            currTime = GetTime(I)

            If currTime < newestTime Then Exit Sub

            If _newQueueLength = 0 Then _nextUnreadNewTime = currTime 'ako je NewQueueLength dosad bilo jednako 0

            Do
                _newQueueLength += 1             'amortizacija, nije potrebno kasnije smanjiti
                newestTime = currTime
                I = Incr(I)
                ReadBlock(I)
                currTime = GetTime(I)
            Loop While currTime > newestTime

            ReadBlock(_newestIndex) 'prošli NewestIndex
            If GetTime(_newestIndex) <> newestTime Then 'sve poruke su nove
                _newQueueLength = N
                _nextUnreadNewIndex = Incr(_newestIndex)
                _nextUnreadNewTime = GetTime(_nextUnreadNewIndex)
                _passedLength = 0
                _oldQueueLength = 0
            ElseIf _newQueueLength >= N - _passedLength Then 'nema starih, ali nije sve puno
                _passedLength = N - _newQueueLength
                _oldQueueLength = 0
            ElseIf _newQueueLength > N - (_passedLength + _oldQueueLength) Then _
                'neke nove su zapisane na mjesto nesvih startih
                _oldQueueLength = N - (_passedLength + _newQueueLength)
            End If

            _newestIndex = Decr(I)
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
                .Content = GetString(_nextNewestUnreadOldIndex),
                .Time = _nextNewestUnreadOldTime,
                .Type = GetMessageType(_nextNewestUnreadOldIndex)
                }
            _oldQueueLength -= 1
            _nextNewestUnreadOldIndex = Decr(_nextNewestUnreadOldIndex)
            If _oldQueueLength > 0 Then _nextNewestUnreadOldTime = GetTime(_nextNewestUnreadOldIndex)
            _passedLength += 1
        End Function

        ''' <summary>
        '''     Vraća najnoviju staru poruku (prvu poruku iz oldQueue).
        ''' </summary>
        Public Function GetNextNewer() As Message
            GetNextNewer = New Message With {
                .Sender = Sender,
                .Type = GetMessageType(_nextUnreadNewIndex),
                .Content = GetString(_nextUnreadNewIndex),
                .Time = _nextUnreadNewTime
                }
            _newQueueLength -= 1
            _nextUnreadNewIndex = Incr(_nextUnreadNewIndex)
            If _newQueueLength > 0 Then _nextUnreadNewTime = GetTime(_nextUnreadNewIndex)
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