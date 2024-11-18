Imports System.Runtime.CompilerServices
''' <summary>
''' [Preview] Represents a span of read-only memory of <see cref="Char"/>.
''' </summary>
Public Structure StringSegment
    Private ReadOnly _reference As String
    Private ReadOnly _start As Integer
    Public ReadOnly Length As Integer

    Public Sub New(reference As String, start As Integer, length As Integer)
        _reference = reference
        _start = start
        Me.Length = length
    End Sub

    Default Public ReadOnly Property Item(index As Integer) As Char
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return _reference(_start + index)
        End Get
    End Property

    Public Shared Function Slice(str As String, start As Integer, length As Integer) As StringSegment
        Dim seg As New StringSegment(str, start, length)
        Return seg
    End Function

    Public Function Slice(start As Integer, length As Integer) As StringSegment
        Dim seg As New StringSegment(_reference, start + _start, length)
        Return seg
    End Function

    Public Function Slice(start As Integer) As StringSegment
        Dim seg As New StringSegment(_reference, start + _start, Length - start)
        Return seg
    End Function

    ''' <summary>
    ''' Devirtualized version of <see cref="ToString()"/>.
    ''' </summary>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function CopyToString() As String
        If _reference Is Nothing Then
            Return Nothing
        End If
        Return _reference.Substring(_start, Length)
    End Function

    Public Shared Narrowing Operator CType(str As String) As StringSegment
        Return New StringSegment(str, 0, str.Length)
    End Operator

    Public Shared Narrowing Operator CType(str As StringSegment) As String
        Return str.CopyToString()
    End Operator

    Public ReadOnly Property IsNull As Boolean
        Get
            Return _reference Is Nothing
        End Get
    End Property

    Public ReadOnly Property IsNullOrEmpty As Boolean
        Get
            Return Length = 0
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return CopyToString()
    End Function

#If NET8_0_OR_GREATER Then
    <Obsolete("ref struct")>
    Public Function AsSpan() As ReadOnlySpan(Of Char)
        Return _reference.AsSpan(_start, Length)
    End Function
#End If

    Public Shared ReadOnly Empty As New StringSegment(String.Empty, 0, 0)
End Structure
