Friend Class CsvSingleFormatter
    Inherits Singleton(Of CsvSingleFormatter)
    Implements ICsvRecordFormatter

    Public Function Parse(text As String) As Object Implements ICsvRecordFormatter.Parse
        Return Single.Parse(text)
    End Function

    Public Function GetString(data As Object, format As String) As String Implements ICsvRecordFormatter.GetString
        Return DirectCast(data, Single).ToString(format)
    End Function

    Public Function ParseBlittablePrimitive(text As StringSegment, ByRef primitive As CsvBlittablePrimitive) As Boolean Implements ICsvRecordFormatter.ParseBlittablePrimitive
        primitive.SingleValue = Single.Parse(text.GetString)
        Return True
    End Function
End Class
