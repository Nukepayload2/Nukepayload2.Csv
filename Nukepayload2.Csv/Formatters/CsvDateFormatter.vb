Friend Class CsvDateFormatter
    Inherits Singleton(Of CsvDateFormatter)
    Implements ICsvRecordFormatter

    Public Function Parse(text As StringSegment) As Object Implements ICsvRecordFormatter.Parse
        Return ParseDateTime(text)
    End Function

    Public Shared Function ParseDateTime(text As StringSegment) As Date
#If NET8_0_OR_GREATER Then
#Disable Warning BC40000 ' ref struct
        Return Date.Parse(text.AsSpan)
#Enable Warning BC40000 ' ref struct
#Else
        Return Date.Parse(text.CopyToString)
#End If
    End Function

    Public Function GetString(data As Object, format As String) As String Implements ICsvRecordFormatter.GetString
        Return GetDateTimeString(DirectCast(data, Date), format)
    End Function

    Public Shared Function GetDateTimeString(data As Date, format As String) As String
        Return data.ToString(format)
    End Function
End Class
