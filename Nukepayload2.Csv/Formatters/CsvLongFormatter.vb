Imports System.Text

Friend Class CsvLongFormatter
    Inherits Singleton(Of CsvLongFormatter)
    Implements ICsvDataFormatter

    Public Sub WriteTo(data As Object, format As String, sb As StringBuilder) Implements ICsvDataFormatter.WriteTo
        sb.Append(DirectCast(data, Long).ToString(format))
    End Sub

    Public Function Parse(text As String) As Object Implements ICsvDataFormatter.Parse
        Return Long.Parse(text)
    End Function

    Public Function GetString(data As Object, format As String) As String Implements ICsvDataFormatter.GetString
        Return DirectCast(data, Long).ToString(format)
    End Function
End Class
