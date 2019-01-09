Imports System.Runtime.CompilerServices
''' <summary>
''' Represents a span of read-only memory of <see cref="Char"/>.
''' </summary>
Public Structure StringSegment
    Private _reference As String
    Private _start As Integer
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
    ''' Devirtualize ToString.
    ''' </summary>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetString() As String
        Return _reference.Substring(_start, Length)
    End Function

    Public Shared Widening Operator CType(str As String) As StringSegment
        Return New StringSegment(str, 0, str.Length)
    End Operator

    Public Overrides Function ToString() As String
        Return GetString()
    End Function
End Structure
