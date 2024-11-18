Friend Class CsvSingleFormatter
    Inherits Singleton(Of CsvSingleFormatter)
    Implements ICsvRecordFormatter

    Public Function Parse(text As StringSegment) As Object Implements ICsvRecordFormatter.Parse
        Return ParseSingle(text)
    End Function

    Public Shared Function ParseSingle(text As StringSegment) As Single
#If NET8_0_OR_GREATER Then
#Disable Warning BC40000 ' ref struct
        Dim textSpan = text.AsSpan
#Enable Warning BC40000 ' ref struct
#Else
        Dim textSpan = text.CopyToString
#End If
        Return Single.Parse(textSpan)
    End Function

    Public Function GetString(data As Object, format As String) As String Implements ICsvRecordFormatter.GetString
        Return GetSingleString(DirectCast(data, Single), format)
    End Function

    Public Shared Function GetSingleString(data As Single, format As String) As String
        Return data.ToString(format)
    End Function
End Class
