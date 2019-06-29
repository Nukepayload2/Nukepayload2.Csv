Public Class DefaultFormatterCache
    Implements ICsvRecordFormatterCache

    Private ReadOnly _useExcelLikeDoubleParser As Boolean

    Sub New()
        Dim culture = Globalization.CultureInfo.CurrentCulture

        _useExcelLikeDoubleParser =
            culture.Name = "en-US" OrElse culture.Name = "zh-CN" OrElse culture.Name = "ja-JP"
    End Sub

    Public Function GetFormatter(tp As Type) As ICsvRecordFormatter Implements ICsvRecordFormatterCache.GetFormatter
        Select Case tp
            Case GetType(Double)
                If _useExcelLikeDoubleParser Then
                    Return CsvDoubleFormatter.Instance
                Else
                    Return CsvDoubleClrFormatter.Instance
                End If
            Case GetType(Double?)
                If _useExcelLikeDoubleParser Then
                    Return CsvDoubleNullableFormatter.Instance
                Else
                    Return CsvDoubleClrNullableFormatter.Instance
                End If
            Case GetType(Integer)
                Return CsvIntegerFormatter.Instance
            Case GetType(Integer?)
                Return CsvIntegerNullableFormatter.Instance
            Case GetType(Long)
                Return CsvLongFormatter.Instance
            Case GetType(Long?)
                Return CsvLongNullableFormatter.Instance
            Case GetType(Single)
                Return CsvSingleFormatter.Instance
            Case GetType(Single?)
                Return CsvSingleNullableFormatter.Instance
            Case GetType(Date)
                Return CsvDateFormatter.Instance
            Case GetType(Date?)
                Return CsvDateNullableFormatter.Instance
            Case GetType(Boolean)
                Return CsvBooleanFormatter.Instance
            Case GetType(Boolean?)
                Return CsvBooleanNullableFormatter.Instance
            Case GetType(String)
                Return CsvTextFormatter.Instance
            Case Else
                Throw New NotSupportedException($"A property type in the given model class is not supported. {tp.FullName} is not supported.")
        End Select
    End Function
End Class
