﻿Friend Class CsvSingleFormatter
    Inherits Singleton(Of CsvSingleFormatter)
    Implements ICsvRecordFormatter

    Public Function Parse(text As StringSegment) As Object Implements ICsvRecordFormatter.Parse
        Return Single.Parse(text.CopyToString)
    End Function

    Public Function GetString(data As Object, format As String) As String Implements ICsvRecordFormatter.GetString
        Return DirectCast(data, Single).ToString(format)
    End Function
End Class
