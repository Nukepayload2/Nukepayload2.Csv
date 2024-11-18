Friend Class CsvLongNullableFormatter
    Inherits Singleton(Of CsvLongNullableFormatter)
    Implements ICsvRecordFormatter

    Public Function Parse(text As StringSegment) As Object Implements ICsvRecordFormatter.Parse
        Return ParseInt64(text)
    End Function

    Public Shared Function ParseInt64(text As StringSegment) As Long?
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
        Return Long.Parse(textSpan)
    End Function

    Public Function GetString(data As Object, format As String) As String Implements ICsvRecordFormatter.GetString
        Return GetInt64String(DirectCast(data, Long?), format)
    End Function

    Public Shared Function GetInt64String(data As Long?, format As String) As String
        If data Is Nothing Then
            Return Nothing
        End If
        Return data.Value.ToString(format)
    End Function
End Class
