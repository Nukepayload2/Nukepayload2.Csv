''' <summary>
''' Specify the format of data in csv.
''' </summary>
<AttributeUsage(AttributeTargets.Property)>
Public Class ColumnFormatAttribute
    Inherits Attribute

    Sub New()

    End Sub

    Public Sub New(columnName As String)
        Name = columnName
    End Sub

    ''' <summary>
    ''' Specify the name of column in csv text.
    ''' </summary>
    Public Property Name As String
    ''' <summary>
    ''' Specify the format for converting numbers or dates to string.
    ''' </summary>
    Public Property FormatString As String
    ''' <summary>
    ''' If <see cref="CsvSettings.ColumnOrderKind"/> is <see cref="CsvColumnOrderKind.Explicit"/>, specify the index of the csv column.
    ''' </summary>
    Public Property ColumnIndex As Integer
End Class
