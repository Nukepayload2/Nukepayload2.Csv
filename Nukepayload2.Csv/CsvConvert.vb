Imports System.Reflection

''' <summary>
''' Convert collection to csv, or convert them back.
''' </summary>
Public Class CsvConvert
    Private Shared s_cachedColumns As New Dictionary(Of Type, CsvColumnInfo())
    Private Shared s_cachedHeaderOrder As New Dictionary(Of Type, Integer())

    ''' <summary>
    ''' Release cached column information of processed model types. If you don't know the consequences, please do not call it.
    ''' </summary>
    Public Shared Sub ReleaseCache()
        s_cachedColumns.Clear()
        s_cachedHeaderOrder.Clear()
    End Sub

    ''' <summary>
    ''' Deserialize the specified csv text to .NET collection.
    ''' </summary>
    ''' <typeparam name="T">The sub type of the collection.</typeparam>
    ''' <param name="text">Csv text.</param>
    ''' <returns>The converted .NET collection</returns>
    ''' <exception cref="CsvFormatException"/>
    ''' <exception cref="FormatException"/>
    ''' <exception cref="NullReferenceException"/>
    Public Shared Function DeserializeObject(Of T As New)(text As String) As IReadOnlyList(Of T)
        Return DeserializeObject(Of T)(text, CsvSettings.Default)
    End Function

    ''' <summary>
    ''' Deserialize the specified csv text to .NET collection with settings.
    ''' </summary>
    ''' <typeparam name="T">The sub type of the collection.</typeparam>
    ''' <param name="text">Csv text.</param>
    ''' <param name="settings">Custom csv format. If this parameter is <see langword="null"/> (<see langword="Nothing"/> in Visual Basic), the default settings will be used.</param>
    ''' <returns>The converted .NET collection</returns>
    ''' <exception cref="CsvFormatException"/>
    ''' <exception cref="FormatException"/>
    ''' <exception cref="NullReferenceException"/>
    Public Shared Function DeserializeObject(Of T As New)(text As String, settings As CsvSettings) As IReadOnlyList(Of T)
        If settings Is Nothing Then
            settings = CsvSettings.Default
        End If
        Dim columnType As Type = GetType(T)
        Dim columns = GetColumns(columnType, settings.RecordFormatterCache)
        If columns.Length = 0 OrElse String.IsNullOrEmpty(text) Then
            Return Array.Empty(Of T)
        End If
        ' TODO: Allocation can be reduced.
        Dim lines = text.Split(settings.NewLineSeparators, StringSplitOptions.RemoveEmptyEntries)
        If lines.Length < 2 Then
            Return Array.Empty(Of T)
        End If
        Dim head = lines(0)
        Dim separator = settings.Separators
        Dim entities(lines.Length - 2) As T
        Select Case settings.ColumnOrderKind
            Case CsvColumnOrderKind.Auto
                Dim order = GetHeadOrder(columns, head, separator)
                For i = 1 To lines.Length - 1
                    entities(i - 1) = InputEntity(Of T)(lines(i), order, columns, separator)
                Next
            Case CsvColumnOrderKind.Sequential
                For i = 1 To lines.Length - 1
                    entities(i - 1) = InputEntity(Of T)(lines(i), columns, separator)
                Next
            Case CsvColumnOrderKind.Explicit
                Dim order = GetHeadOrderFor(Of T)()
                For i = 1 To lines.Length - 1
                    entities(i - 1) = InputEntity(Of T)(lines(i), order, columns, separator)
                Next
        End Select
        Return entities
    End Function

    Private Shared Function GetHeadOrderFor(Of T)() As Integer()
        Dim columnType = GetType(T)
        Dim value As Integer() = Nothing
        If Not s_cachedHeaderOrder.TryGetValue(columnType, value) Then
            value = Aggregate prop In columnType.GetRuntimeProperties
                    Where prop.CanRead AndAlso prop.GetMethod.IsPublic
                    Let formatInfo = prop.GetCustomAttribute(Of ColumnFormatAttribute)
                    Select formatInfo.ColumnIndex Into ToArray
            s_cachedHeaderOrder.Add(columnType, value)
        End If
        Return value
    End Function

    Private Shared Function InputEntity(Of T As New)(line As String, columns() As CsvColumnInfo, separator As String()) As T
        ' TODO: Allocation can be reduced.
        Dim lineContent = line.Split(separator, StringSplitOptions.None)
        Dim entity As New T
        For i = 0 To columns.Length - 1
            Dim col = columns(i)
            Dim data = lineContent(i)
            col(entity) = col.Formatter.Parse(data)
        Next
        Return entity
    End Function

    Private Shared Function InputEntity(Of T As New)(line As String, order As Integer(), columns() As CsvColumnInfo, separator As String()) As T
        ' TODO: Allocation can be reduced.
        Dim lineContent = line.Split(separator, StringSplitOptions.None)
        Dim entity As New T
        For i = 0 To columns.Length - 1
            Dim col = columns(i)
            Dim data = lineContent(order(i))
            ' The slowest part.
            ' TODO: Numbers should be parsed in a more effective way
            ' since we do not support NumberStyles and IFormatProvider.
            col(entity) = col.Formatter.Parse(data)
        Next
        Return entity
    End Function

    Private Shared Function GetHeadOrder(columns() As CsvColumnInfo, head As String, separator As String()) As Integer()
        ' TODO: Allocation can be reduced.
        Dim lineNames = head.Split(separator, StringSplitOptions.RemoveEmptyEntries)
        If lineNames.Length <> columns.Length Then
            ThrowForColumnNotFound(columns, lineNames)
        End If
        Dim headOrder = Aggregate col In columns
                        Let name = col.Name
                        Select IndexOf(name, lineNames) Into ToArray
        If Aggregate n In headOrder Where n = -1 Into Any Then
            ThrowForColumnNotFound(columns, lineNames)
        End If
        Return headOrder
    End Function

    Private Shared Sub ThrowForColumnNotFound(columns() As CsvColumnInfo, lineNames() As String)
        Throw New CsvFormatException((From c In columns Select c.Name), lineNames)
    End Sub

    Private Shared Function IndexOf(value As String, data As String()) As Integer
        For i = 0 To data.Length - 1
            If value = data(i) Then
                Return i
            End If
        Next
        Return -1
    End Function

    ''' <summary>
    ''' Serialize the specified collection to csv text.
    ''' </summary>
    ''' <typeparam name="T">The sub type of the collection.</typeparam>
    ''' <param name="objs">The collection that you want to serialize.</param>
    ''' <returns>Csv text</returns>
    ''' <exception cref="NotSupportedException"/>
    ''' <exception cref="NullReferenceException"/>
    Public Shared Function SerializeObject(Of T)(objs As IEnumerable(Of T)) As String
        Return SerializeObject(objs, CsvSettings.Default)
    End Function

    ''' <summary>
    ''' Serialize the specified collection to csv text.
    ''' </summary>
    ''' <typeparam name="T">The sub type of the collection.</typeparam>
    ''' <param name="objs">The collection that you want to serialize.</param>
    ''' <param name="settings">Custom csv format. If this parameter is <see langword="null"/> (<see langword="Nothing"/> in Visual Basic), the default settings will be used.</param>
    ''' <returns>Csv text</returns>
    ''' <exception cref="NotSupportedException"/>
    ''' <exception cref="NullReferenceException"/>
    Public Shared Function SerializeObject(Of T)(objs As IEnumerable(Of T), settings As CsvSettings) As String
        If settings Is Nothing Then
            settings = CsvSettings.Default
        End If
        Dim columnType As Type = GetType(T)
        Dim columns = GetColumns(columnType, settings.RecordFormatterCache)
        Dim columnLength As Integer = columns.Length
        If columnLength = 0 OrElse objs Is Nothing Then
            Return String.Empty
        End If
        Dim separator = settings.Separator
        ' Enable StringBuilder pooling.
        Dim sb = StringBuilderCache.Instance
        Dim newline = settings.NewLine
        Select Case settings.ColumnOrderKind
            Case CsvColumnOrderKind.Explicit
                Dim order = GetHeadOrderFor(Of T)()
                ' Convert to non-linq code on hot paths.
                Dim i As Integer
                For i = 0 To columnLength - 2
                    Dim col = columns(order(i))
                    sb.Append(col.Name).Append(separator)
                Next
                Dim lastCol = columns(order(i))
                sb.Append(lastCol.Name).Append(newline)
                ' IReadOnlyList(Of T) special case
                Dim roList As IReadOnlyList(Of T) = TryCast(objs, IReadOnlyList(Of T))
                If roList IsNot Nothing Then
                    For i = 0 To roList.Count - 1
                        For j = 0 To columnLength - 2
                            Dim obj = roList(i)
                            Dim col = columns(order(j))
                            Dim value = col(obj)
                            ' The slowest part.
                            ' TODO: If FormatString = Nothing, numbers should be written in a optimized way.
                            col.Formatter.WriteTo(value, col.FormatString, sb)
                            sb.Append(separator)
                        Next
                        Dim lastValue = lastCol(roList(i))
                        lastCol.Formatter.WriteTo(lastValue, lastCol.FormatString, sb)
                        sb.Append(newline)
                    Next
                Else
                    ' Use non IReadOnlyList(Of T) means high-performance is not required.
                    ' We will not optimize for this path.
                    For Each obj In objs
                        sb.Append(String.Join(separator, OrderSelect((Aggregate col In columns
                                                             Let value = col(obj)
                                                             Select col.Formatter.GetString(value, col.FormatString)
                                                             Into ToArray), order))).Append(newline)
                    Next
                End If
            Case Else
                ' Convert to non-linq code on hot paths.
                Dim i As Integer
                For i = 0 To columnLength - 2
                    Dim col = columns(i)
                    sb.Append(col.Name).Append(separator)
                Next
                Dim lastCol = columns(i)
                sb.Append(lastCol.Name).Append(newline)
                ' IReadOnlyList(Of T) special case
                Dim roList As IReadOnlyList(Of T) = TryCast(objs, IReadOnlyList(Of T))
                If roList IsNot Nothing Then
                    For i = 0 To roList.Count - 1
                        For j = 0 To columnLength - 2
                            Dim obj = roList(i)
                            Dim col = columns(j)
                            Dim value = col(obj)
                            col.Formatter.WriteTo(value, col.FormatString, sb)
                            sb.Append(separator)
                        Next
                        Dim lastValue = lastCol(roList(i))
                        lastCol.Formatter.WriteTo(lastValue, lastCol.FormatString, sb)
                        sb.Append(newline)
                    Next
                Else
                    ' Use non IReadOnlyList(Of T) means high-performance is not required.
                    ' We will not optimize for this path.
                    For Each obj In objs
                        sb.Append(String.Join(separator, From col In columns
                                                         Let value = col(obj)
                                                         Select col.Formatter.GetString(value, col.FormatString))).Append(newline)
                    Next
                End If
        End Select
        Return sb.ToString
    End Function

    Private Shared Iterator Function OrderSelect(Of T)(collection As T(), order As IReadOnlyList(Of Integer)) As IEnumerable(Of T)
        For i = 0 To order.Count - 1
            Yield collection(order(i))
        Next
    End Function

    Private Shared Function GetColumns(modelType As Type, formatterCache As ICsvRecordFormatterCache) As CsvColumnInfo()
        Dim value As CsvColumnInfo() = Nothing
        If Not s_cachedColumns.TryGetValue(modelType, value) Then
            value = Aggregate prop In modelType.GetRuntimeProperties
                    Where prop.CanRead AndAlso prop.GetMethod.IsPublic
                    Let formatInfo = prop.GetCustomAttribute(Of ColumnFormatAttribute)
                    Select CreateCsvColumnInfo(prop, formatInfo, modelType, formatterCache) Into ToArray
            s_cachedColumns.Add(modelType, value)
        End If
        Return value
    End Function

    Private Shared Function CreateCsvColumnInfo(prop As PropertyInfo, formatInfo As ColumnFormatAttribute, modelType As Type, formatterCache As ICsvRecordFormatterCache) As CsvColumnInfo
        Dim instance = Activator.CreateInstance(GetType(CsvColumnInfo(Of)).MakeGenericType(modelType),
                                   If(formatInfo?.Name, prop.Name),
                                   formatInfo?.FormatString,
                                   prop.PropertyType.GetFormatter(formatterCache),
                                   prop.GetMethod, prop.SetMethod)
        Return DirectCast(instance, CsvColumnInfo)
    End Function
End Class
