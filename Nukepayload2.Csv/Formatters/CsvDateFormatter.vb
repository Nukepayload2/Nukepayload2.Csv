Imports System.Globalization
Imports System.Text

Friend Class CsvDateFormatter
    Inherits Singleton(Of CsvDateFormatter)
    Implements ICsvDataFormatter

    Public Sub WriteTo(data As Object, format As String, sb As StringBuilder) Implements ICsvDataFormatter.WriteTo
        sb.Append(DirectCast(data, Date).ToString(format))
    End Sub

    Public Function Parse(text As String) As Object Implements ICsvDataFormatter.Parse
        Return Date.Parse(text)
    End Function

    Public Function GetString(data As Object, format As String) As String Implements ICsvDataFormatter.GetString
        Return DirectCast(data, Date).ToString(format)
    End Function
End Class
