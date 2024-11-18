Friend Class CsvDoubleClrNullableFormatter
    Inherits Singleton(Of CsvDoubleClrNullableFormatter)
    Implements ICsvRecordFormatter

    Public Function Parse(text As StringSegment) As Object Implements ICsvRecordFormatter.Parse
        Return ParseDouble(text)
    End Function

    Public Shared Function ParseDouble(text As StringSegment) As Double?
        If text.IsNullOrEmpty Then
            Return Nothing
        End If
        Dim value As Double = Nothing
#If NET8_0_OR_GREATER Then
#Disable Warning BC40000 ' ref struct
        Dim textSpan = text.AsSpan
#Enable Warning BC40000 ' ref struct
#Else
        Dim textSpan = text.CopyToString
#End If
        If Double.TryParse(textSpan, Globalization.NumberStyles.Any,
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
