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
        Set(value As String)
            If String.IsNullOrEmpty(value) OrElse value.Contains("""") Then
                Throw New InvalidOperationException("Separator can't be set to empty or contains quote.")
            End If
            _Separator = value
        End Set
    End Property

    ''' <summary>
    ''' The new line of csv. The default value is <see cref="Environment.NewLine"/>.
    ''' </summary>
    Public Property NewLine As String
        Get
            Return _NewLine
        End Get
        Set(value As String)
            If String.IsNullOrEmpty(value) Then
                Throw New InvalidOperationException("NewLine can't be set to empty.")
            End If
            If Aggregate s In value Where s <> ChrW(10) AndAlso s <> ChrW(13) Into Any Then
                Throw New InvalidOperationException("NewLine can't contain characters other than CR and LF.")
            End If
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
    ''' <summary>
    ''' Get or set the record formatter cache which is used to get and cache instance of record formatters.
    ''' </summary>
    Public Property RecordFormatterCache As ICsvRecordFormatterCache = New DefaultFormatterCache
End Class

''' <summary>
''' Represents the column order of csv.
''' </summary>
Public Enum CsvColumnOrderKind
    ''' <summary>
    ''' The column order of csv must be inferred at runtime. (Slowest when columns have large quantity)
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
