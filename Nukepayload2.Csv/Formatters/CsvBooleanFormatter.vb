Imports System.Text

Friend Class CsvBooleanFormatter
    Inherits Singleton(Of CsvBooleanFormatter)
    Implements ICsvRecordFormatter

    Public Sub WriteTo(data As Object, format As String, sb As StringBuilder) Implements ICsvRecordFormatter.WriteTo
        sb.Append(DirectCast(data, Boolean))
    End Sub

    Public Function Parse(text As String) As Object Implements ICsvRecordFormatter.Parse
        Return Boolean.Parse(text)
    End Function

    Public Function GetString(data As Object, format As String) As String Implements ICsvRecordFormatter.GetString
        Return DirectCast(data, Boolean).ToString
    End Function
End Class
