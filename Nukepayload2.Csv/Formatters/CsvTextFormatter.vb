Imports System.Text

Friend Class CsvTextFormatter
    Inherits Singleton(Of CsvTextFormatter)
    Implements ICsvDataFormatter

    Public Sub WriteTo(data As Object, format As String, sb As StringBuilder) Implements ICsvDataFormatter.WriteTo
        sb.Append(DirectCast(data, String))
    End Sub

    Public Function Parse(text As String) As Object Implements ICsvDataFormatter.Parse
        Return text
    End Function

    Public Function GetString(data As Object, format As String) As String Implements ICsvDataFormatter.GetString
        Return DirectCast(data, String)
    End Function
End Class
