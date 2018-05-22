Imports System.Text

Friend Class CsvLongFormatter
    Inherits Singleton(Of CsvLongFormatter)
    Implements ICsvRecordFormatter

    Public Sub WriteTo(data As Object, format As String, sb As StringBuilder) Implements ICsvRecordFormatter.WriteTo
        sb.Append(DirectCast(data, Long).ToString(format))
    End Sub

    Public Function Parse(text As String) As Object Implements ICsvRecordFormatter.Parse
        Return Long.Parse(text)
    End Function

    Public Function GetString(data As Object, format As String) As String Implements ICsvRecordFormatter.GetString
        Return DirectCast(data, Long).ToString(format)
    End Function
End Class
