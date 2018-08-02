Friend Class CsvIntegerFormatter
    Inherits Singleton(Of CsvIntegerFormatter)
    Implements ICsvRecordFormatter

    Public Function Parse(text As String) As Object Implements ICsvRecordFormatter.Parse
        Return Integer.Parse(text)
    End Function

    Public Function GetString(data As Object, format As String) As String Implements ICsvRecordFormatter.GetString
        Return DirectCast(data, Integer).ToString(format)
    End Function

    Public Function ParseBlittablePrimitive(text As String, ByRef primitive As CsvBlittablePrimitive) As Boolean Implements ICsvRecordFormatter.ParseBlittablePrimitive
        primitive.Int32Value = Integer.Parse(text)
        Return True
    End Function
End Class
