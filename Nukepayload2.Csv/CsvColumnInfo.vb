Imports System.Reflection

Friend Structure CsvColumnInfo
    Dim Name As String, FormatString As String, Formatter As ITextTableDataFormatter
    Dim GetMethod, SetMethod As MethodInfo

    Public Sub New(name As String, formatString As String, formatter As ITextTableDataFormatter, getMethod As MethodInfo, setMethod As MethodInfo)
        Me.Name = name
        Me.FormatString = formatString
        Me.Formatter = formatter
        Me.GetMethod = getMethod
        Me.SetMethod = setMethod
    End Sub

    Default Public Property ColumnValue(entity As Object) As Object
        Get
            Return GetMethod.Invoke(entity, Nothing)
        End Get
        Set(value As Object)
            SetMethod.Invoke(entity, {value})
        End Set
    End Property

End Structure
