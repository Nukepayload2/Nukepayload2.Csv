Imports System.Linq.Expressions
Imports System.Reflection

Friend MustInherit Class CsvColumnInfo
    Public Name As String, FormatString As String, Formatter As ICsvRecordFormatter

    Public Sub New(name As String, formatString As String, formatter As ICsvRecordFormatter)
        Me.Name = name
        Me.FormatString = formatString
        Me.Formatter = formatter
    End Sub

    Public MustOverride ReadOnly Property CanRead As Boolean
    Public MustOverride ReadOnly Property CanWrite As Boolean

    Default Public MustOverride Property ColumnValue(entity As Object) As Object
End Class

Friend Class CsvColumnInfo(Of T)
    Inherits CsvColumnInfo

    Dim GetMethod As Func(Of T, Object), SetMethod As Action(Of T, Object)

    Public Sub New(name As String, formatString As String, formatter As ICsvRecordFormatter, getMethod As MethodInfo, setMethod As MethodInfo)
        MyBase.New(name, formatString, formatter)
        If getMethod IsNot Nothing Then
            Me.GetMethod = CompileGetMethod(getMethod)
        End If
        If setMethod IsNot Nothing Then
            Me.SetMethod = CompileSetMethod(setMethod)
        End If
    End Sub

    Private Function CompileGetMethod(getMethod As MethodInfo) As Func(Of T, Object)
        Dim instanceType As Type = getMethod.DeclaringType
        Dim paramExpr = Expression.Parameter(instanceType)
        Return Expression.Lambda(Of Func(Of T, Object))(
            Expression.Convert(Expression.Call(Expression.Convert(paramExpr, instanceType), getMethod), GetType(Object)),
            {paramExpr}
        ).Compile
    End Function

    Private Function CompileSetMethod(setMethod As MethodInfo) As Action(Of T, Object)
        Dim instanceType As Type = setMethod.DeclaringType
        Dim propValueType As Type = setMethod.GetParameters(0).ParameterType
        Dim param1 = Expression.Parameter(instanceType)
        Dim param2 = Expression.Parameter(GetType(Object))
        Return Expression.Lambda(Of Action(Of T, Object))(
            Expression.Call(Expression.Convert(param1, instanceType), setMethod,
                Expression.Convert(param2, propValueType)
            ), {param1, param2}
        ).Compile
    End Function

    Default Public Overrides Property ColumnValue(entity As Object) As Object
        Get
            Return GetMethod.Invoke(DirectCast(entity, T))
        End Get
        Set(value As Object)
            SetMethod.Invoke(DirectCast(entity, T), value)
        End Set
    End Property

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
End Class