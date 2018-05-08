Public Class RawModel
    Public Property IntegerValue As Integer
    Public Property DoubleValue As Double
    Public Property StringValue As String
    Public Property DateValue As Date
    Public Property BooleanValue As Boolean
End Class

Public Class DecoratedModel
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
