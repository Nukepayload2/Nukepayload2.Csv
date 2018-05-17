''' <summary>
''' Custom csv format.
''' </summary>
Public Class CsvSettings
    Private _NewLine As String = Environment.NewLine
    Private _Separator As String = ","

    ''' <summary>
    ''' The separator of csv. The default value is ",".
    ''' </summary>
    Public Property Separator As String
        Get
            Return _Separator
        End Get
        Set
            _Separator = Value
            _Separators = {Value}
        End Set
    End Property

    ''' <summary>
    ''' The separator of csv. The default value is ",".
    ''' </summary>
    Friend ReadOnly Property Separators As String() = {","}
    ''' <summary>
    ''' The new line of csv. The default value is <see cref="Environment.NewLine"/>.
    ''' </summary>
    Public Property NewLine As String
        Get
            Return _NewLine
        End Get
        Set(value As String)
            _NewLine = value
            _NewLineSeparators = {value}
        End Set
    End Property
    ''' <summary>
    ''' Work around for missing Stirng.Split(String, StringSplitOptions) in non .net core 2.0 runtime.
    ''' </summary>
    Friend ReadOnly Property NewLineSeparators As String() = {NewLine}
    ''' <summary>
    ''' The column order of csv. The default value is <see cref="CsvColumnOrderKind.Auto"/>.
    ''' </summary>
    Public Property ColumnOrderKind As CsvColumnOrderKind
    ''' <summary>
    ''' Get the default instance of CsvSettings. Any modifications to this object will affect default behaviors of <see cref="CsvConvert.DeserializeObject(Of T)(String)"/> and <see cref="CsvConvert.SerializeObject(Of T)(IEnumerable(Of T))"/> .
    ''' </summary>
    Public Shared ReadOnly Property [Default] As New CsvSettings
End Class

''' <summary>
''' Represents the column order of csv.
''' </summary>
Public Enum CsvColumnOrderKind
    ''' <summary>
    ''' The column order of csv must be inferred at runtime. (Slowest when columns have large quantity, use this mode when csv text is less than )
    ''' </summary>
    Auto
    ''' <summary>
    ''' The column order of csv is the same as the model class. (If csv strings are generated with the same model class, use this mode)
    ''' </summary>
    Sequential
    ''' <summary>
    ''' The column order of csv is specified with <see cref="ColumnFormatAttribute.ColumnIndex"/>. (Recommended when columns are many and unordered)
    ''' </summary>
    Explicit
End Enum
