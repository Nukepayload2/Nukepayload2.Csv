Friend Class CsvIntegerNullableFormatter
    Inherits Singleton(Of CsvIntegerNullableFormatter)
    Implements ICsvRecordFormatter

    Public Function Parse(text As StringSegment) As Object Implements ICsvRecordFormatter.Parse
        Return ParseInt32(text)
    End Function

    Public Shared Function ParseInt32(text As StringSegment) As Integer?
        If text.IsNullOrEmpty Then
            Return Nothing
        End If
#If NET8_0_OR_GREATER Then
#Disable Warning BC40000 ' ref struct
        Dim textSpan = text.AsSpan
#Enable Warning BC40000 ' ref struct
#Else
        Dim textSpan = text.CopyToString
#End If
        Return Integer.Parse(textSpan)
    End Function

    Public Function GetString(data As Object, format As String) As String Implements ICsvRecordFormatter.GetString
        If data Is Nothing Then
            Return Nothing
        End If
        Return DirectCast(data, Integer).ToString(format)
    End Function
End Class
