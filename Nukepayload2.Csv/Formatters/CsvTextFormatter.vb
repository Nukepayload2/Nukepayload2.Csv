Imports System.Text

Friend Class CsvTextFormatter
    Inherits Singleton(Of CsvTextFormatter)
    Implements ICsvRecordFormatter

    Public Sub WriteTo(data As Object, format As String, sb As StringBuilder) Implements ICsvRecordFormatter.WriteTo
        sb.Append(DirectCast(data, String))
    End Sub

    Public Function Parse(text As String) As Object Implements ICsvRecordFormatter.Parse
        Return text
    End Function

    Public Function GetString(data As Object, format As String) As String Implements ICsvRecordFormatter.GetString
        Return DirectCast(data, String)
    End Function
End Class
