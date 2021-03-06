﻿Imports Nukepayload2.Csv.Numbers.Double

Friend Class CsvDoubleFormatter
    Inherits Singleton(Of CsvDoubleFormatter)
    Implements ICsvRecordFormatter

    Private ReadOnly _doubleView As New DoubleSemanticView

    Public Function Parse(text As StringSegment) As Object Implements ICsvRecordFormatter.Parse
        Dim value As Double = Nothing
        If _doubleView.TryParse(text, value) Then
            Return value
        End If
        Throw New FormatException
    End Function

    Public Function GetString(data As Object, format As String) As String Implements ICsvRecordFormatter.GetString
        Return DirectCast(data, Double).ToString(format)
    End Function

End Class
