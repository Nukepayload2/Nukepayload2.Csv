''' <summary>
''' The exception that is thrown when the format of the csv text is invalid.
''' </summary>
Public Class CsvFormatException
    Inherits FormatException

    Sub New(modelColumnNames As IEnumerable(Of String), columnNames As String())
        MyBase.New($"Csv model binding failure: Actual columns: {String.Join(", ", columnNames)}, expected columns: {String.Join(", ", modelColumnNames)}.")
    End Sub
End Class
