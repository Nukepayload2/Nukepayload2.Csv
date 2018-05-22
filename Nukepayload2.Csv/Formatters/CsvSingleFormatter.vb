Imports System.Text

Friend Class CsvSingleFormatter
    Inherits Singleton(Of CsvSingleFormatter)
    Implements ICsvRecordFormatter

    Public Sub WriteTo(data As Object, format As String, sb As StringBuilder) Implements ICsvRecordFormatter.WriteTo
        sb.Append(DirectCast(data, Single).ToString(format))
    End Sub

    Public Function Parse(text As String) As Object Implements ICsvRecordFormatter.Parse
        Return Single.Parse(text)
    End Function

    Public Function GetString(data As Object, format As String) As String Implements ICsvRecordFormatter.GetString
        Return DirectCast(data, Single).ToString(format)
    End Function
End Class
