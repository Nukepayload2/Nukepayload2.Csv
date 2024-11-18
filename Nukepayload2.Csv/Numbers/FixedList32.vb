Imports System.ComponentModel
Imports System.Diagnostics.CodeAnalysis
Imports System.Runtime.CompilerServices

Namespace Numbers
    ''' <summary>
    ''' Stack allocated list of <typeparamref name="T"/> * 32.
    ''' </summary>
    ''' <typeparam name="T">Must satisfy constraint <see langword="modreq"/>(<see cref="System.Runtime.InteropServices.UnmanagedType"/>). 
    ''' See <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/unmanaged-types"/></typeparam>
    <SuppressMessage("", "IDE0044")>
    <SuppressMessage("", "IDE0051")>
    <UnsafeValueType>
    Friend Structure FixedList32(Of T As Structure)
        Private _0 As T
        Private _1 As T
        Private _2 As T
        Private _3 As T
        Private _4 As T
        Private _5 As T
        Private _6 As T
        Private _7 As T
        Private _8 As T
        Private _9 As T
        Private _10 As T
        Private _11 As T
        Private _12 As T
        Private _13 As T
        Private _14 As T
        Private _15 As T
        Private _16 As T
        Private _17 As T
        Private _18 As T
        Private _19 As T
        Private _20 As T
        Private _21 As T
        Private _22 As T
        Private _23 As T
        Private _24 As T
        Private _25 As T
        Private _26 As T
        Private _27 As T
        Private _28 As T
        Private _29 As T
        Private _30 As T
        Private _31 As T
        ''' <summary>
        ''' The length of the current buffer. It is between 0 (inclusive) and <see cref="MaxCount"/> (exclusive).
        ''' </summary>
        Public Count As Integer
        ''' <summary>
        ''' The maximum length of the current buffer.
        ''' </summary>
        Public Const MaxCount = 32

        ''' <summary>
        ''' Gets or sets the specified item at the specified index.
        ''' </summary>
        ''' <param name="index">The index of the element.</param>
        ''' <value>The element at the specified index.</value>
        Default Public Property Item(index As Integer) As T
            Get
                If index < 0 OrElse index >= Count Then Throw New ArgumentOutOfRangeException()
                Return Unsafe.Add(_0, index)
            End Get
            Set(value As T)
                If index < 0 OrElse index >= Count Then Throw New ArgumentOutOfRangeException()
                Unsafe.Add(_0, index) = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the specified item at the specified index without boundary check.
        ''' </summary>
        ''' <param name="index">The index of the element.</param>
        ''' <value>The element at the specified index.</value>
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        Public Property UnsafeItem(index As Integer) As T
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return Unsafe.Add(_0, index)
            End Get
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Set(value As T)
                Unsafe.Add(_0, index) = value
            End Set
        End Property

        ''' <summary>
        ''' Gets a new value at the specified index. Returns whether the element was got.
        ''' </summary>
        ''' <param name="index">The index of the new element.</param>
        ''' <param name="value">The new value to be set.</param>
        ''' <returns>If returns <see langword="True"/>, the item was set successfully. Otherwise, the item can't be acquired.</returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function TryGetItem(index As Integer, ByRef value As T) As Boolean
            If index < 0 OrElse index >= Count Then Return False
            value = Unsafe.Add(_0, index)
            Return True
        End Function

        ''' <summary>
        ''' Sets a new value at the specified index. Returns whether the element was set.
        ''' </summary>
        ''' <param name="index">The index of the new element.</param>
        ''' <param name="value">The new value to be set.</param>
        ''' <returns>If returns <see langword="True"/>, the item was set successfully. Otherwise, the item can't be set.</returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function TrySetItem(index As Integer, value As T) As Boolean
            If index < 0 OrElse index >= Count Then Return False
            Unsafe.Add(_0, index) = value
            Return True
        End Function

        ''' <summary>
        ''' Adds a new value to the list. Returns whether the element was added.
        ''' </summary>
        ''' <param name="value">The new value to be added.</param>
        ''' <returns>If returns <see langword="True"/>, the item was added successfully. Otherwise, the item can't be added.</returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function TryAdd(value As T) As Boolean
            If Count >= MaxCount Then Return False
            Unsafe.Add(_0, Count) = value
            Count += 1
            Return True
        End Function

        ''' <summary>
        ''' Adds a new value to the list.
        ''' </summary>
        ''' <param name="value">The new value to be added.</param>
        ''' <exception cref="OverflowException"/>
        Public Sub Add(value As T)
            If Count >= MaxCount Then Throw New OverflowException
            Unsafe.Add(_0, Count) = value
            Count += 1
        End Sub

        ''' <summary>
        ''' Adds a new value to the list without boundary check.
        ''' </summary>
        ''' <param name="value">The new value to be added.</param>
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub UnsafeAdd(value As T)
            Unsafe.Add(_0, Count) = value
            Count += 1
        End Sub

        ''' <summary>
        ''' Removes an item from the specified index.
        ''' </summary>
        ''' <param name="index">The index of the item.</param>
        ''' <exception cref="IndexOutOfRangeException"/>
        Public Sub RemoveAt(index As Integer)
            If index >= Count OrElse index < 0 Then Throw New IndexOutOfRangeException
            For i = index To Count - 2
                Unsafe.Add(_0, i) = Unsafe.Add(_0, i + 1)
            Next
            Count -= 1
        End Sub

        ''' <summary>
        ''' Removes an item from the specified index without boundary check.
        ''' </summary>
        ''' <param name="index">The index of the item.</param>
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub UnsafeRemoveAt(index As Integer)
            For i = index To Count - 2
                Unsafe.Add(_0, i) = Unsafe.Add(_0, i + 1)
            Next
            Count -= 1
        End Sub

        ''' <summary>
        ''' Removes an item from the specified index.
        ''' </summary>
        ''' <param name="index">The index of the item.</param>
        ''' <returns>Whether the operation succeed.</returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function TryRemoveAt(index As Integer) As Boolean
            If index >= Count OrElse index < 0 Then Return False
            For i = index To Count - 2
                Unsafe.Add(_0, i) = Unsafe.Add(_0, i + 1)
            Next
            Count -= 1
            Return True
        End Function

        ''' <summary>
        ''' Inserts an item into the specified index.
        ''' </summary>
        ''' <param name="index">The index of the item.</param>
        ''' <param name="value">The new value.</param>
        ''' <exception cref="IndexOutOfRangeException"/>
        Public Sub Insert(index As Integer, value As T)
            If index > Count OrElse index >= MaxCount OrElse index < 0 Then Throw New IndexOutOfRangeException
            For i = Count - 1 To index Step -1
                Unsafe.Add(_0, i + 1) = Unsafe.Add(_0, i)
            Next
            Unsafe.Add(_0, index) = value
            Count += 1
        End Sub

        ''' <summary>
        ''' Inserts an item into the specified index without boundary check.
        ''' </summary>
        ''' <param name="index">The index of the item.</param>
        ''' <param name="value">The new value.</param>
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub UnsafeInsert(index As Integer, value As T)
            For i = Count - 1 To index Step -1
                Unsafe.Add(_0, i + 1) = Unsafe.Add(_0, i)
            Next
            Unsafe.Add(_0, index) = value
            Count += 1
        End Sub

        ''' <summary>
        ''' Inserts an item into the specified index.
        ''' </summary>
        ''' <param name="index">The index of the new item.</param>
        ''' <param name="value">The new value.</param>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function TryInsert(index As Integer, value As T) As Boolean
            If index > Count OrElse index >= MaxCount OrElse index < 0 Then Return False
            For i = Count - 1 To index Step -1
                Unsafe.Add(_0, i + 1) = Unsafe.Add(_0, i)
            Next
            Unsafe.Add(_0, index) = value
            Count += 1
            Return True
        End Function
    End Structure

End Namespace