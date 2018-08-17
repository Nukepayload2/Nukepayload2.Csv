<ColumnSelection(CsvColumnSelectionMode.OptIn)>
Public Class OptInSerializationModel
    Sub New()

    End Sub

    Public Sub New(id As Integer, content As String, isChecked As Boolean)
        Me.Id = id
        Me.Content = content
        Me.IsChecked = isChecked
    End Sub

    <CsvProperty>
    Public Property Id As Integer
    <CsvProperty>
    Public Property Content As String
    <CsvProperty>
    Public Property IsChecked As Boolean
    Public Property Garbage As Object
End Class

<ColumnSelection(CsvColumnSelectionMode.OptOut)>
Public Class OptOutSerializationModel
    Sub New()

    End Sub

    Public Sub New(id As Integer, content As String, isChecked As Boolean)
        Me.Id = id
        Me.Content = content
        Me.IsChecked = isChecked
    End Sub

    Public Property Id As Integer
    Public Property Content As String
    Public Property IsChecked As Boolean
    <CsvIgnore>
    Public Property Garbage As Object
End Class

<ColumnOrderKind(CsvColumnOrderKind.Explicit)>
Public Class ExplicitModel
    <ColumnFormat(Name:="Id", ColumnIndex:=0)>
    Public Property IntegerValue As Integer
    <ColumnFormat(Name:="Rate", FormatString:="0.00", ColumnIndex:=2)>
    Public Property DoubleValue As Double
    <ColumnFormat(Name:="Name", ColumnIndex:=1)>
    Public Property StringValue As String
    <ColumnFormat(Name:="Date", FormatString:="yyyy-MM-dd", ColumnIndex:=3)>
    Public Property DateValue As Date
    <ColumnFormat(Name:="IsUsed", ColumnIndex:=4)>
    Public Property BooleanValue As Boolean
End Class
