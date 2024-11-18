Imports System.Reflection

Friend MustInherit Class CsvColumnInfo
    Protected Sub New(name As String)
        Me.Name = name
    End Sub

    Public ReadOnly Property Name As String

    Public MustOverride ReadOnly Property CanRead As Boolean

    Public MustOverride ReadOnly Property CanWrite As Boolean

End Class

Friend MustInherit Class CsvColumnInfo(Of TEntity)
    Inherits CsvColumnInfo

    Protected Sub New(name As String)
        MyBase.New(name)
    End Sub

    Public MustOverride Function GetTextValue(entity As TEntity) As String
    Public MustOverride Sub SetParsedValue(text As StringSegment, entity As TEntity)
End Class

Friend Class CsvColumnInfo(Of TEntity, TProperty)
    Inherits CsvColumnInfo(Of TEntity)

    Private ReadOnly FormatString As String
    Private ReadOnly Formatter As ICsvRecordFormatter
    Private ReadOnly GetMethod As Func(Of TEntity, TProperty)
    Private ReadOnly SetMethod As Action(Of TEntity, TProperty)
    Private ReadOnly GetTextValueImpl As GetTextValueDelegate
    Private ReadOnly SetParsedValueImpl As SetParsedValueDelegate

    Public Sub New(name As String, formatString As String, formatter As ICsvRecordFormatter, getMethod As MethodInfo, setMethod As MethodInfo)
        MyBase.New(name)
        Me.FormatString = formatString
        Me.Formatter = formatter
        If getMethod IsNot Nothing Then
            Me.GetMethod = CompileGetMethod(getMethod)
            GetTextValueImpl = CompileGetTextValue()
        End If
        If setMethod IsNot Nothing Then
            Me.SetMethod = CompileSetMethod(setMethod)
            SetParsedValueImpl = CompileSetParsedValue()
        End If
    End Sub

    Private Function CompileGetMethod(getMethod As MethodInfo) As Func(Of TEntity, TProperty)
        Return DirectCast(getMethod.CreateDelegate(GetType(Func(Of TEntity, TProperty))), Func(Of TEntity, TProperty))
    End Function

    Private Function CompileSetMethod(setMethod As MethodInfo) As Action(Of TEntity, TProperty)
        Return DirectCast(setMethod.CreateDelegate(GetType(Action(Of TEntity, TProperty))), Action(Of TEntity, TProperty))
    End Function

    Public Overrides ReadOnly Property CanRead As Boolean
        Get
            Return GetMethod IsNot Nothing
        End Get
    End Property

    Public Overrides ReadOnly Property CanWrite As Boolean
        Get
            Return SetMethod IsNot Nothing
        End Get
    End Property

    Private Delegate Sub SetParsedValueDelegate(text As StringSegment, entity As TEntity)
    Private Delegate Function GetTextValueDelegate(entity As TEntity) As String

    Public Overrides Sub SetParsedValue(text As StringSegment, entity As TEntity)
        SetParsedValueImpl(text, entity)
    End Sub

    Private Sub SetParsedValueFallback(text As StringSegment, entity As TEntity)
        SetMethod(entity, CType(Formatter.Parse(text), TProperty))
    End Sub

    Public Overrides Function GetTextValue(entity As TEntity) As String
        Return GetTextValueImpl(entity)
    End Function

    Private Function GetTextValueFallback(entity As TEntity) As String
        Return Formatter.GetString(GetMethod(entity), FormatString)
    End Function

    Private Function CompileGetTextValue() As GetTextValueDelegate
        If TypeOf Formatter Is CsvBooleanFormatter Then
            Dim getValue = DirectCast(CObj(GetMethod), Func(Of TEntity, Boolean))
            Return Function(entity) CsvBooleanFormatter.GetBooleanString(getValue(entity))
        ElseIf TypeOf Formatter Is CsvBooleanNullableFormatter Then
            Dim getValue = DirectCast(CObj(GetMethod), Func(Of TEntity, Boolean?))
            Return Function(entity) CsvBooleanNullableFormatter.GetBooleanString(getValue(entity))
        ElseIf TypeOf Formatter Is CsvDateFormatter Then
            Dim getValue = DirectCast(CObj(GetMethod), Func(Of TEntity, Date))
            Return Function(entity) CsvDateFormatter.GetDateTimeString(getValue(entity), FormatString)
        ElseIf TypeOf Formatter Is CsvDateNullableFormatter Then
            Dim getValue = DirectCast(CObj(GetMethod), Func(Of TEntity, Date?))
            Return Function(entity) CsvDateNullableFormatter.GetDateTimeString(getValue(entity), FormatString)
        ElseIf TypeOf Formatter Is CsvDoubleClrFormatter Then
            Dim getValue = DirectCast(CObj(GetMethod), Func(Of TEntity, Double))
            Return Function(entity) CsvDoubleClrFormatter.GetDoubleString(getValue(entity), FormatString)
        ElseIf TypeOf Formatter Is CsvDoubleClrNullableFormatter Then
            Dim getValue = DirectCast(CObj(GetMethod), Func(Of TEntity, Double?))
            Return Function(entity) CsvDoubleClrNullableFormatter.GetDoubleString(getValue(entity), FormatString)
        ElseIf TypeOf Formatter Is CsvDoubleFormatter Then
            Dim getValue = DirectCast(CObj(GetMethod), Func(Of TEntity, Double))
            Return Function(entity) CsvDoubleFormatter.GetDoubleString(getValue(entity), FormatString)
        ElseIf TypeOf Formatter Is CsvDoubleNullableFormatter Then
            Dim getValue = DirectCast(CObj(GetMethod), Func(Of TEntity, Double?))
            Return Function(entity) CsvDoubleNullableFormatter.GetDoubleString(getValue(entity), FormatString)
        ElseIf TypeOf Formatter Is CsvIntegerFormatter Then
            Dim getValue = DirectCast(CObj(GetMethod), Func(Of TEntity, Integer))
            Return Function(entity) CsvIntegerFormatter.GetInt32String(getValue(entity), FormatString)
        ElseIf TypeOf Formatter Is CsvIntegerNullableFormatter Then
            Dim getValue = DirectCast(CObj(GetMethod), Func(Of TEntity, Integer?))
            Return Function(entity) CsvIntegerNullableFormatter.GetInt32String(getValue(entity), FormatString)
        ElseIf TypeOf Formatter Is CsvLongFormatter Then
            Dim getValue = DirectCast(CObj(GetMethod), Func(Of TEntity, Long))
            Return Function(entity) CsvLongFormatter.GetInt64String(getValue(entity), FormatString)
        ElseIf TypeOf Formatter Is CsvLongNullableFormatter Then
            Dim getValue = DirectCast(CObj(GetMethod), Func(Of TEntity, Long?))
            Return Function(entity) CsvLongNullableFormatter.GetInt64String(getValue(entity), FormatString)
        ElseIf TypeOf Formatter Is CsvSingleFormatter Then
            Dim getValue = DirectCast(CObj(GetMethod), Func(Of TEntity, Single))
            Return Function(entity) CsvSingleFormatter.GetSingleString(getValue(entity), FormatString)
        ElseIf TypeOf Formatter Is CsvSingleNullableFormatter Then
            Dim getValue = DirectCast(CObj(GetMethod), Func(Of TEntity, Single?))
            Return Function(entity) CsvSingleNullableFormatter.GetSingleString(getValue(entity), FormatString)
        Else
            Return AddressOf GetTextValueFallback
        End If
    End Function

    Private Function CompileSetParsedValue() As SetParsedValueDelegate
        If TypeOf Formatter Is CsvBooleanFormatter Then
            Dim setValue = DirectCast(CObj(SetMethod), Action(Of TEntity, Boolean))
            Return Sub(text, entity) setValue(entity, CsvBooleanFormatter.ParseBoolean(text))
        ElseIf TypeOf Formatter Is CsvBooleanNullableFormatter Then
            Dim setValue = DirectCast(CObj(SetMethod), Action(Of TEntity, Boolean?))
            Return Sub(text, entity) setValue(entity, CsvBooleanNullableFormatter.ParseBoolean(text))
        ElseIf TypeOf Formatter Is CsvDateFormatter Then
            Dim setValue = DirectCast(CObj(SetMethod), Action(Of TEntity, Date))
            Return Sub(text, entity) setValue(entity, CsvDateFormatter.ParseDateTime(text))
        ElseIf TypeOf Formatter Is CsvDateNullableFormatter Then
            Dim setValue = DirectCast(CObj(SetMethod), Action(Of TEntity, Date?))
            Return Sub(text, entity) setValue(entity, CsvDateNullableFormatter.ParseDateTime(text))
        ElseIf TypeOf Formatter Is CsvDoubleClrFormatter Then
            Dim setValue = DirectCast(CObj(SetMethod), Action(Of TEntity, Double))
            Return Sub(text, entity) setValue(entity, CsvDoubleClrFormatter.ParseDouble(text))
        ElseIf TypeOf Formatter Is CsvDoubleClrNullableFormatter Then
            Dim setValue = DirectCast(CObj(SetMethod), Action(Of TEntity, Double?))
            Return Sub(text, entity) setValue(entity, CsvDoubleClrNullableFormatter.ParseDouble(text))
        ElseIf TypeOf Formatter Is CsvDoubleFormatter Then
            Dim setValue = DirectCast(CObj(SetMethod), Action(Of TEntity, Double))
            Return Sub(text, entity) setValue(entity, CsvDoubleFormatter.ParseDouble(text))
        ElseIf TypeOf Formatter Is CsvDoubleNullableFormatter Then
            Dim setValue = DirectCast(CObj(SetMethod), Action(Of TEntity, Double?))
            Return Sub(text, entity) setValue(entity, CsvDoubleNullableFormatter.ParseDouble(text))
        ElseIf TypeOf Formatter Is CsvIntegerFormatter Then
            Dim setValue = DirectCast(CObj(SetMethod), Action(Of TEntity, Integer))
            Return Sub(text, entity) setValue(entity, CsvIntegerFormatter.ParseInt32(text))
        ElseIf TypeOf Formatter Is CsvIntegerNullableFormatter Then
            Dim setValue = DirectCast(CObj(SetMethod), Action(Of TEntity, Integer?))
            Return Sub(text, entity) setValue(entity, CsvIntegerNullableFormatter.ParseInt32(text))
        ElseIf TypeOf Formatter Is CsvLongFormatter Then
            Dim setValue = DirectCast(CObj(SetMethod), Action(Of TEntity, Long))
            Return Sub(text, entity) setValue(entity, CsvLongFormatter.ParseInt64(text))
        ElseIf TypeOf Formatter Is CsvLongNullableFormatter Then
            Dim setValue = DirectCast(CObj(SetMethod), Action(Of TEntity, Long?))
            Return Sub(text, entity) setValue(entity, CsvLongNullableFormatter.ParseInt64(text))
        ElseIf TypeOf Formatter Is CsvSingleFormatter Then
            Dim setValue = DirectCast(CObj(SetMethod), Action(Of TEntity, Single))
            Return Sub(text, entity) setValue(entity, CsvSingleFormatter.ParseSingle(text))
        ElseIf TypeOf Formatter Is CsvSingleNullableFormatter Then
            Dim setValue = DirectCast(CObj(SetMethod), Action(Of TEntity, Single?))
            Return Sub(text, entity) setValue(entity, CsvSingleNullableFormatter.ParseSingle(text))
        Else
            Return AddressOf SetParsedValueFallback
        End If
    End Function

End Class
