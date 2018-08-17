''' <summary>
''' Overrides the <see cref="CsvSettings.ColumnSelectionMode"/> setting for model class.
''' </summary>
<AttributeUsage(AttributeTargets.Class)>
Public Class ColumnSelectionAttribute
    Inherits Attribute

    Public Sub New(selectionMode As CsvColumnSelectionMode)
        Me.SelectionMode = selectionMode
    End Sub

    Public ReadOnly Property SelectionMode As CsvColumnSelectionMode
End Class