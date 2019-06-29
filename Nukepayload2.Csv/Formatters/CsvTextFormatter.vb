Friend Class CsvTextFormatter
    Inherits Singleton(Of CsvTextFormatter)
    Implements ICsvRecordFormatter

    Public Function Parse(text As StringSegment) As Object Implements ICsvRecordFormatter.Parse
        Return text.CopyToString
    End Function

    Public Function GetString(data As Object, format As String) As String Implements ICsvRecordFormatter.GetString
        Return DirectCast(data, String)
    End Function
End Class
