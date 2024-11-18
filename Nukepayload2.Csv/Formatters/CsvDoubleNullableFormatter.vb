﻿Imports Nukepayload2.Csv.Numbers.Double

Friend Class CsvDoubleNullableFormatter
    Inherits Singleton(Of CsvDoubleNullableFormatter)
    Implements ICsvRecordFormatter

    Public Function Parse(text As StringSegment) As Object Implements ICsvRecordFormatter.Parse
        Return ParseDouble(text)
    End Function

    Public Shared Function ParseDouble(text As StringSegment) As Double?
        If text.IsNullOrEmpty Then
            Return Nothing
        End If
        Dim value As Double = Nothing
        If DoubleSemanticView.TryParse(text, value) Then
            Return value
        End If
        Throw New FormatException
    End Function

    Public Function GetString(data As Object, format As String) As String Implements ICsvRecordFormatter.GetString
        If data Is Nothing Then
            Return Nothing
        End If
        Return DirectCast(data, Double).ToString(format)
    End Function
End Class
