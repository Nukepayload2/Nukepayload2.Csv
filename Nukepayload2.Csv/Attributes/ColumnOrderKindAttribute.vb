''' <summary>
''' Overrides the <see cref="CsvSettings.ColumnOrderKind"/> setting for model class.
''' </summary>
<AttributeUsage(AttributeTargets.Class)>
Public Class ColumnOrderKindAttribute
    Inherits Attribute

    Public Sub New(columnOrderKind As CsvColumnOrderKind)
        Me.ColumnOrderKind = columnOrderKind
    End Sub

    Public ReadOnly Property ColumnOrderKind As CsvColumnOrderKind
End Class
