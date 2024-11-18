Friend Class CsvDateFormatter
    Inherits Singleton(Of CsvDateFormatter)
    Implements ICsvRecordFormatter

    Public Function Parse(text As StringSegment) As Object Implements ICsvRecordFormatter.Parse
#If NET8_0_OR_GREATER Then
#Disable Warning BC40000 ' ref struct
        Return Date.Parse(text.AsSpan)
#Enable Warning BC40000 ' ref struct
#Else
        Return Date.Parse(text.CopyToString)
#End If
    End Function

    Public Function GetString(data As Object, format As String) As String Implements ICsvRecordFormatter.GetString
        Return DirectCast(data, Date).ToString(format)
    End Function
End Class
