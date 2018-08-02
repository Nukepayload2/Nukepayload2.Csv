Friend Class CsvBooleanFormatter
    Inherits Singleton(Of CsvBooleanFormatter)
    Implements ICsvRecordFormatter

    Public Function Parse(text As String) As Object Implements ICsvRecordFormatter.Parse
        If text Is Nothing Then
            Throw New FormatException("Boolean expected.")
        End If
        Return text.Equals("True", StringComparison.OrdinalIgnoreCase)
    End Function

    Public Function GetString(data As Object, format As String) As String Implements ICsvRecordFormatter.GetString
        Return DirectCast(data, Boolean).ToString
    End Function

    Public Function ParseBlittablePrimitive(text As String, ByRef primitive As CsvBlittablePrimitive) As Boolean Implements ICsvRecordFormatter.ParseBlittablePrimitive
        Return False
    End Function
End Class
