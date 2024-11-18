Friend Class CsvLongFormatter
    Inherits Singleton(Of CsvLongFormatter)
    Implements ICsvRecordFormatter

    Public Function Parse(text As StringSegment) As Object Implements ICsvRecordFormatter.Parse
        Return ParseInt64(text)
    End Function

    Public Shared Function ParseInt64(text As StringSegment) As Long
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
        Return DirectCast(data, Long).ToString(format)
    End Function
End Class
