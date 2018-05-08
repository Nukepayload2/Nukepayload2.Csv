﻿Public Class CsvSettings
    Public Property Separator As Char
    Public Property ColumnOrderKind As CsvColumnOrderKind
    Public Shared ReadOnly Property [Default] As New CsvSettings With {
        .Separator = ","c
    }
End Class

''' <summary>
''' Represents the column order of csv text file.
''' </summary>
Public Enum CsvColumnOrderKind
    ''' <summary>
    ''' The csv files' column order must be inferred at runtime. (Slowest when columns have large quantity)
    ''' </summary>
    Auto
    ''' <summary>
    ''' The csv files' column order is the same as the model class. (If csv strings are generated by the same model class, use this mode)
    ''' </summary>
    Sequential
    ''' <summary>
    ''' The csv files' column order is specified with <see cref="ColumnFormatAttribute.ColumnIndex"/>. (Recommended when columns are much and unordered)
    ''' </summary>
    Explicit
End Enum
