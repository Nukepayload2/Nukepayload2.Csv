Friend Class CsvBooleanFormatter
    Inherits Singleton(Of CsvBooleanFormatter)
    Implements ICsvRecordFormatter

    Private Const AscIIToUpperMask As Integer = &HFFDF

    Public Function Parse(text As StringSegment) As Object Implements ICsvRecordFormatter.Parse
        If text.Length = 4 Then
            If (Convert.ToInt32(text(0)) And AscIIToUpperMask) = Convert.ToInt32("T"c) AndAlso
               (Convert.ToInt32(text(1)) And AscIIToUpperMask) = Convert.ToInt32("R"c) AndAlso
               (Convert.ToInt32(text(2)) And AscIIToUpperMask) = Convert.ToInt32("U"c) AndAlso
               (Convert.ToInt32(text(3)) And AscIIToUpperMask) = Convert.ToInt32("E"c) Then
                Return True
            End If
        ElseIf text.Length = 5 Then
            If (Convert.ToInt32(text(0)) And AscIIToUpperMask) = Convert.ToInt32("F"c) AndAlso
               (Convert.ToInt32(text(1)) And AscIIToUpperMask) = Convert.ToInt32("A"c) AndAlso
               (Convert.ToInt32(text(2)) And AscIIToUpperMask) = Convert.ToInt32("L"c) AndAlso
               (Convert.ToInt32(text(3)) And AscIIToUpperMask) = Convert.ToInt32("S"c) AndAlso
               (Convert.ToInt32(text(4)) And AscIIToUpperMask) = Convert.ToInt32("E"c) Then
                Return False
            End If
        End If
        Throw New FormatException("Boolean expected.")
    End Function

    Public Function GetString(data As Object, format As String) As String Implements ICsvRecordFormatter.GetString
        Return DirectCast(data, Boolean).ToString
    End Function
End Class

Friend Class CsvBooleanNullableFormatter
    Inherits Singleton(Of CsvBooleanNullableFormatter)
    Implements ICsvRecordFormatter

    Private Const AscIIToUpperMask As Integer = &HFFDF

    Public Function Parse(text As StringSegment) As Object Implements ICsvRecordFormatter.Parse
        If text.IsNullOrEmpty Then
            Return Nothing
        End If
        If text.Length = 4 Then
            If (Convert.ToInt32(text(0)) And AscIIToUpperMask) = Convert.ToInt32("T"c) AndAlso
               (Convert.ToInt32(text(1)) And AscIIToUpperMask) = Convert.ToInt32("R"c) AndAlso
               (Convert.ToInt32(text(2)) And AscIIToUpperMask) = Convert.ToInt32("U"c) AndAlso
               (Convert.ToInt32(text(3)) And AscIIToUpperMask) = Convert.ToInt32("E"c) Then
                Return True
            End If
        ElseIf text.Length = 5 Then
            If (Convert.ToInt32(text(0)) And AscIIToUpperMask) = Convert.ToInt32("F"c) AndAlso
               (Convert.ToInt32(text(1)) And AscIIToUpperMask) = Convert.ToInt32("A"c) AndAlso
               (Convert.ToInt32(text(2)) And AscIIToUpperMask) = Convert.ToInt32("L"c) AndAlso
               (Convert.ToInt32(text(3)) And AscIIToUpperMask) = Convert.ToInt32("S"c) AndAlso
               (Convert.ToInt32(text(4)) And AscIIToUpperMask) = Convert.ToInt32("E"c) Then
                Return False
            End If
        End If
        Throw New FormatException("Boolean expected.")
    End Function

    Public Function GetString(data As Object, format As String) As String Implements ICsvRecordFormatter.GetString
        If data Is Nothing Then
            Return Nothing
        End If
        Return DirectCast(data, Boolean).ToString
    End Function
End Class
