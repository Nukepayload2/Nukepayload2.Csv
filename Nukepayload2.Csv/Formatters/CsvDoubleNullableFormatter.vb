Imports Nukepayload2.Csv.Numbers.Double

Friend Class CsvDoubleNullableFormatter
    Inherits Singleton(Of CsvDoubleNullableFormatter)
    Implements ICsvRecordFormatter

    Private ReadOnly _doubleView As New DoubleSemanticView

    Public Function Parse(text As StringSegment) As Object Implements ICsvRecordFormatter.Parse
        If text.IsNullOrEmpty Then
            Return Nothing
        End If
        Dim value As Double = Nothing
        If _doubleView.TryParse(text, value) Then
            Return value
        End If
        Throw New FormatException
    End Function

    Public Function GetString(data As Object, format As String) As String Implements ICsvRecordFormatter.GetString
        If data Is Nothing Then
            Return Nothing
        End If
        Return DirectCast(data, Double).ToString(format)
    End Function
End Class
