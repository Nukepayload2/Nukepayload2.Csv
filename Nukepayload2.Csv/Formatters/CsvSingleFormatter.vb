Imports System.Text

Friend Class CsvSingleFormatter
    Inherits Singleton(Of CsvSingleFormatter)
    Implements ICsvDataFormatter

    Public Sub WriteTo(data As Object, format As String, sb As StringBuilder) Implements ICsvDataFormatter.WriteTo
        sb.Append(DirectCast(data, Single).ToString(format))
    End Sub

    Public Function Parse(text As String) As Object Implements ICsvDataFormatter.Parse
        Return Single.Parse(text)
    End Function

    Public Function GetString(data As Object, format As String) As String Implements ICsvDataFormatter.GetString
        Return DirectCast(data, Single).ToString(format)
    End Function
End Class
