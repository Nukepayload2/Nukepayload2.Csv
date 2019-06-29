Friend Class CsvDateNullableFormatter
    Inherits Singleton(Of CsvDateNullableFormatter)
    Implements ICsvRecordFormatter

    Public Function Parse(text As StringSegment) As Object Implements ICsvRecordFormatter.Parse
        If text.IsNullOrEmpty Then
            Return Nothing
        End If
        Return Date.Parse(text.CopyToString)
    End Function

    Public Function GetString(data As Object, format As String) As String Implements ICsvRecordFormatter.GetString
        If data Is Nothing Then
            Return Nothing
        End If
        Return DirectCast(data, Date).ToString(format)
    End Function
End Class
