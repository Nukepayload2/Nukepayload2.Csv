Imports System.Text

Friend Class CsvIntegerFormatter
    Inherits Singleton(Of CsvIntegerFormatter)
    Implements ICsvDataFormatter

    Public Sub WriteTo(data As Object, format As String, sb As StringBuilder) Implements ICsvDataFormatter.WriteTo
        sb.Append(DirectCast(data, Integer).ToString(format))
    End Sub

    Public Function Parse(text As String) As Object Implements ICsvDataFormatter.Parse
        Return Integer.Parse(text)
    End Function

    Public Function GetString(data As Object, format As String) As String Implements ICsvDataFormatter.GetString
        Return DirectCast(data, Integer).ToString(format)
    End Function
End Class
