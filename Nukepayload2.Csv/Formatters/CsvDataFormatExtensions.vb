Imports System.Runtime.CompilerServices

Module CsvDataFormatExtensions
    <Extension>
    Function GetFormatter(tp As Type, formatterPool As ICsvRecordFormatterCache) As ICsvRecordFormatter
        Return formatterPool.GetFormatter(tp)
    End Function
End Module
