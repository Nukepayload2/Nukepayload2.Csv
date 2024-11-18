Friend Class CsvDateNullableFormatter
    Inherits Singleton(Of CsvDateNullableFormatter)
    Implements ICsvRecordFormatter

    Public Function Parse(text As StringSegment) As Object Implements ICsvRecordFormatter.Parse
        Return ParseDateTime(text)
    End Function

    Public Shared Function ParseDateTime(text As StringSegment) As Date?
        If text.IsNullOrEmpty Then
            Return Nothing
        End If
#If NET8_0_OR_GREATER Then
#Disable Warning BC40000 ' ref struct
        Return Date.Parse(text.AsSpan)
#Enable Warning BC40000 ' ref struct
#Else
        Return Date.Parse(text.CopyToString)
#End If
    End Function

    Public Function GetString(data As Object, format As String) As String Implements ICsvRecordFormatter.GetString
        If data Is Nothing Then
            Return Nothing
        End If
        Return DirectCast(data, Date).ToString(format)
    End Function
End Class
