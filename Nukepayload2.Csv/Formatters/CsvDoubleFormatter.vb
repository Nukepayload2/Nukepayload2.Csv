Imports Nukepayload2.Csv.Numbers.Double

Friend Class CsvDoubleFormatter
    Inherits Singleton(Of CsvDoubleFormatter)
    Implements ICsvRecordFormatter

    Public Function Parse(text As StringSegment) As Object Implements ICsvRecordFormatter.Parse
        Return ParseDouble(text)
    End Function

    Public Shared Function ParseDouble(text As StringSegment) As Double
        Dim value As Double = Nothing
        If DoubleSemanticView.TryParse(text, value) Then
            Return value
        End If
        Throw New FormatException
    End Function

    Public Function GetString(data As Object, format As String) As String Implements ICsvRecordFormatter.GetString
        Return GetDoubleString(DirectCast(data, Double), format)
    End Function

    Public Shared Function GetDoubleString(data As Double, format As String) As String
        Return data.ToString(format)
    End Function
End Class
