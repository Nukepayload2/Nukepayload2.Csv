Friend Class CsvBooleanFormatter
    Inherits Singleton(Of CsvBooleanFormatter)
    Implements ICsvRecordFormatter

    Public Function Parse(text As StringSegment) As Object Implements ICsvRecordFormatter.Parse
        If text.IsNullOrEmpty Then
            Throw New FormatException("Boolean expected.")
        End If
        If text.Length = 4 Then
            If (AscW(text(0)) And &HFFDF) = AscW("T"c) AndAlso
               (AscW(text(1)) And &HFFDF) = AscW("R"c) AndAlso
               (AscW(text(2)) And &HFFDF) = AscW("U"c) AndAlso
               (AscW(text(3)) And &HFFDF) = AscW("E"c) Then
                Return True
            End If
        ElseIf text.Length = 5 Then
            If (AscW(text(0)) And &HFFDF) = AscW("F"c) AndAlso
               (AscW(text(1)) And &HFFDF) = AscW("A"c) AndAlso
               (AscW(text(2)) And &HFFDF) = AscW("L"c) AndAlso
               (AscW(text(3)) And &HFFDF) = AscW("S"c) AndAlso
               (AscW(text(4)) And &HFFDF) = AscW("E"c) Then
                Return False
            End If
        End If
        Throw New FormatException("Boolean expected.")
    End Function

    Public Function GetString(data As Object, format As String) As String Implements ICsvRecordFormatter.GetString
        Return DirectCast(data, Boolean).ToString
    End Function

    Public Function ParseBlittablePrimitive(text As StringSegment, ByRef primitive As CsvBlittablePrimitive) As Boolean Implements ICsvRecordFormatter.ParseBlittablePrimitive
        Return False
    End Function
End Class
