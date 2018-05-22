Imports System.Text

Friend Class CsvIntegerFormatter
    Inherits Singleton(Of CsvIntegerFormatter)
    Implements ICsvRecordFormatter

    Public Sub WriteTo(data As Object, format As String, sb As StringBuilder) Implements ICsvRecordFormatter.WriteTo
        sb.Append(DirectCast(data, Integer).ToString(format))
    End Sub

    Public Function Parse(text As String) As Object Implements ICsvRecordFormatter.Parse
        Return Integer.Parse(text)
    End Function

    Public Function GetString(data As Object, format As String) As String Implements ICsvRecordFormatter.GetString
        Return DirectCast(data, Integer).ToString(format)
    End Function
End Class
