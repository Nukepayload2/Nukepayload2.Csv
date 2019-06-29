Friend Class CsvDoubleClrNullableFormatter
    Inherits Singleton(Of CsvDoubleClrNullableFormatter)
    Implements ICsvRecordFormatter

    Public Function Parse(text As StringSegment) As Object Implements ICsvRecordFormatter.Parse
        If text.IsNullOrEmpty Then
            Return Nothing
        End If
        Dim value As Double = Nothing
        If Double.TryParse(text.CopyToString, Globalization.NumberStyles.Any,
                           Globalization.CultureInfo.CurrentCulture, value) Then
            Return value
        End If
        Throw New FormatException
    End Function

    Public Function GetString(data As Object, format As String) As String Implements ICsvRecordFormatter.GetString
        If data Is Nothing Then
            Return Nothing
        End If
        Return DirectCast(data, Double).ToString(format)
    End Function

End Class
