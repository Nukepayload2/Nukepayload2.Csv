﻿Friend Class CsvIntegerFormatter
    Inherits Singleton(Of CsvIntegerFormatter)
    Implements ICsvRecordFormatter

    Public Function Parse(text As StringSegment) As Object Implements ICsvRecordFormatter.Parse
        Return Integer.Parse(text.CopyToString)
    End Function

    Public Function GetString(data As Object, format As String) As String Implements ICsvRecordFormatter.GetString
        Return DirectCast(data, Integer).ToString(format)
    End Function
End Class
