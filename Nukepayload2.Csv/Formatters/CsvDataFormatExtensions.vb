Imports System.Runtime.CompilerServices

Module CsvDataFormatExtensions
    <Extension>
    Function GetFormatter(tp As Type) As ICsvDataFormatter
        Select Case tp
            Case GetType(Date)
                Return CsvDateFormatter.Instance
            Case GetType(Double)
                Return CsvDoubleFormatter.Instance
            Case GetType(Integer)
                Return CsvIntegerFormatter.Instance
            Case GetType(Long)
                Return CsvLongFormatter.Instance
            Case GetType(Single)
                Return CsvSingleFormatter.Instance
            Case GetType(String)
                Return CsvTextFormatter.Instance
            Case GetType(Boolean)
                Return CsvBooleanFormatter.Instance
            Case Else
                Throw New NotSupportedException($"A property type in the given model class is not supported. {tp.FullName} is not supported.")
        End Select
    End Function
End Module