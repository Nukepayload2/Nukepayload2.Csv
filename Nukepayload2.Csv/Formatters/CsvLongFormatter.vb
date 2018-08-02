Friend Class CsvLongFormatter
    Inherits Singleton(Of CsvLongFormatter)
    Implements ICsvRecordFormatter

    Public Function Parse(text As String) As Object Implements ICsvRecordFormatter.Parse
        Return Long.Parse(text)
    End Function

    Public Function GetString(data As Object, format As String) As String Implements ICsvRecordFormatter.GetString
        Return DirectCast(data, Long).ToString(format)
    End Function

    Public Function ParseBlittablePrimitive(text As String, ByRef primitive As CsvBlittablePrimitive) As Boolean Implements ICsvRecordFormatter.ParseBlittablePrimitive
        primitive.Int64Value = Long.Parse(text)
        Return True
    End Function
End Class
