Imports System.Collections.Concurrent
Imports System.Reflection
Imports System.Text
Imports Nukepayload2.Csv

''' <summary>
''' Convert collection to csv, or convert them back.
''' </summary>
Public Class CsvConvert
    Private Shared ReadOnly s_cachedColumns As New ConcurrentDictionary(Of Type, CsvSheetInfo)
    Private Shared ReadOnly s_cachedHeaderOrder As New ConcurrentDictionary(Of Type, Integer())

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
        Dim sheetInfo = GetColumns(columnType, settings.RecordFormatterCache, settings.ColumnSelectionMode)
        Dim columns = sheetInfo.ColumnInfo.Cast(Of CsvColumnInfo(Of T)).ToArray
        If columns.Length = 0 OrElse String.IsNullOrEmpty(text) Then
            Return Array.Empty(Of T)
        End If
        CheckColumnWriteAccess(columns)

        Dim lines = text.SplitLines(settings.NewLine)
        If lines.Count < 2 Then
            Return Array.Empty(Of T)
        End If
        Dim head = lines(0)
        Dim separator = settings.Separator
        Dim entities(lines.Count - 2) As T
        Select Case GetOrderKind(settings, sheetInfo.ColumnOrderKind)
            Case CsvColumnOrderKind.Auto
                Dim order = GetHeadOrder(columns, head, separator)
                For i = 1 To lines.Count - 1
                    entities(i - 1) = InputEntity(Of T)(lines(i), order, columns, separator)
                Next
            Case CsvColumnOrderKind.Sequential
                For i = 1 To lines.Count - 1
                    entities(i - 1) = InputEntity(Of T)(lines(i), columns, separator)
                Next
            Case CsvColumnOrderKind.Explicit
                Dim order = GetHeadOrderFor(Of T)()
                For i = 1 To lines.Count - 1
                    entities(i - 1) = InputEntity(Of T)(lines(i), order, columns, separator)
                Next
        End Select
        Return entities
    End Function

    Private Shared Sub CheckColumnWriteAccess(columns As CsvColumnInfo())
        For Each c In columns
            If Not c.CanWrite Then
                Throw New ArgumentException("Attempt to write to a read-only property.")
            End If
        Next
    End Sub

    Private Shared Sub CheckColumnReadAccess(columns As CsvColumnInfo())
        For Each c In columns
            If Not c.CanRead Then
                Throw New ArgumentException("Attempt to read from a write-only property.")
            End If
        Next
    End Sub

    Private Shared Function GetHeadOrderFor(Of T)() As Integer()
        Dim columnType = GetType(T)
        Dim value As Integer() = Nothing
        If Not s_cachedHeaderOrder.TryGetValue(columnType, value) Then
            value = Aggregate prop In columnType.GetRuntimeProperties
                    Where prop.CanRead AndAlso prop.GetMethod.IsPublic
                    Let formatInfo = prop.GetCustomAttribute(Of ColumnFormatAttribute)
                    Select formatInfo.ColumnIndex Into ToArray
            s_cachedHeaderOrder.GetOrAdd(columnType, value)
        End If
        Return value
    End Function

    Private Shared Function InputEntity(Of T As New)(line As StringSegment, columns() As CsvColumnInfo(Of T), separator As String) As T
        ' TODO: Allocation can be reduced.
        Dim lineContent(columns.Length - 1) As StringSegment
        line.SplitElementsInto(separator, lineContent, StringSplitOptions.None)
        Dim entity As New T
        For i = 0 To columns.Length - 1
            Dim col = columns(i)
            Dim data = lineContent(i)
            col.SetParsedValue(data, entity)
        Next
        Return entity
    End Function

    Private Shared Function InputEntity(Of T As New)(line As StringSegment, order As Integer(), columns() As CsvColumnInfo(Of T), separator As String) As T
        ' TODO: Allocation can be reduced.
        Dim lineContent(columns.Length - 1) As StringSegment
        line.SplitElementsInto(separator, lineContent, StringSplitOptions.None)
        Dim entity As New T
        For i = 0 To columns.Length - 1
            Dim col = columns(i)
            Dim data = lineContent(order(i))
            col.SetParsedValue(data, entity)
        Next
        Return entity
    End Function

    Private Shared Function GetHeadOrder(columns() As CsvColumnInfo, head As StringSegment, separator As String) As Integer()
        ' TODO: Allocation can be reduced.
        Dim lineNames(columns.Length - 1) As String
        Dim lineNamesSeg(columns.Length - 1) As StringSegment
        head.SplitElementsInto(separator, lineNamesSeg, StringSplitOptions.RemoveEmptyEntries)
        For i = 0 To lineNamesSeg.Length - 1
            Dim seg = lineNamesSeg(i)
            lineNames(i) = seg.CopyToString
        Next
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
        Dim sheetInfo = GetColumns(columnType, settings.RecordFormatterCache, settings.ColumnSelectionMode)
        Dim columns = sheetInfo.ColumnInfo.Cast(Of CsvColumnInfo(Of T)).ToArray
        Dim columnLength As Integer = columns.Length
        If columnLength = 0 OrElse objs Is Nothing Then
            Return String.Empty
        End If
        CheckColumnReadAccess(columns)
        Dim separator = settings.Separator
        ' Enable StringBuilder pooling.
        Dim sb = StringBuilderCache.Instance
        Dim newline = settings.NewLine
        Select Case GetOrderKind(settings, sheetInfo.ColumnOrderKind)
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
                            Dim value = col.GetTextValue(obj)
                            ' The slowest part.
                            EscapeAppend(value, separator, newline, sb)
                            sb.Append(separator)
                        Next
                        Dim lastValue = lastCol.GetTextValue(roList(i))
                        EscapeAppend(lastValue, separator, newline, sb)
                        sb.Append(newline)
                    Next
                Else
                    ' Use non IReadOnlyList(Of T) means high-performance is not required.
                    ' We will not optimize for this path.
                    For Each obj In objs
                        sb.Append(String.Join(separator, OrderSelect((Aggregate col In columns
                                                             Select col.GetTextValue(obj)
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
                            Dim value = col.GetTextValue(obj)
                            EscapeAppend(value, separator, newline, sb)
                            sb.Append(separator)
                        Next
                        Dim lastValue = lastCol.GetTextValue(roList(i))
                        EscapeAppend(lastValue, separator, newline, sb)
                        sb.Append(newline)
                    Next
                Else
                    ' TODO: optimize it with array pooled row buffers.
                    For Each obj In objs
                        sb.Append(String.Join(separator, From col In columns
                                                         Let value = col.GetTextValue(obj)
                                                         Select Escape(value, separator))).Append(newline)
                    Next
                End If
        End Select
        Return sb.ToString
    End Function

    Private Shared Function GetOrderKind(settings As CsvSettings, fallback As CsvColumnOrderKind?) As CsvColumnOrderKind
        If fallback Is Nothing Then
            Return settings.ColumnOrderKind
        Else
            Return fallback.Value
        End If
    End Function

    Private Shared ReadOnly Quote1 As String = """"
    Private Shared ReadOnly Quote2 As String = """"""

    Private Shared Sub EscapeAppend(value As String, separator As String, newLine As String, sb As StringBuilder)
        If value Is Nothing Then
            Return
        End If
        If value.Contains(Quote1) Then
            sb.Append(Quote1)
            ReplaceAppend(value, Quote1, Quote2, sb)
            sb.Append(Quote1)
        ElseIf value.Contains(separator) Then
            sb.Append(Quote1).Append(value).Append(Quote1)
        ElseIf value.Contains(newLine) Then
            sb.Append(Quote1).Append(value).Append(Quote1)
        Else
            sb.Append(value)
        End If
    End Sub

    Private Shared Sub ReplaceAppend(value As String, find As String, replacement As String, sb As StringBuilder)
        Dim expLength = value.Length
        Dim findLength = find.Length
        Dim spanStart = 0
        Do While spanStart < expLength
            Dim spanNext As Integer = value.IndexOf(find, spanStart)
            If spanNext >= 0 Then
                sb.Append(value.Substring(spanStart, spanNext - spanStart)).Append(replacement)
                spanStart = spanNext + findLength
            Else
                sb.Append(value.Substring(spanStart))
                Return
            End If
        Loop
    End Sub

    Private Shared Function Escape(value As String, separator As String) As String
        If value Is Nothing Then
            Return Nothing
        End If
        If value.Contains(Quote1) Then
            Return Quote1 + value.Replace(Quote1, Quote2) + Quote1
        ElseIf value.Contains(separator) Then
            Return Quote1 + value + Quote1
        Else
            Return value
        End If
    End Function

    Private Shared Iterator Function OrderSelect(Of T)(collection As T(), order As IReadOnlyList(Of Integer)) As IEnumerable(Of T)
        For i = 0 To order.Count - 1
            Yield collection(order(i))
        Next
    End Function

    Private Shared Function GetColumns(modelType As Type, formatterCache As ICsvRecordFormatterCache, defaultSelectionMode As CsvColumnSelectionMode) As CsvSheetInfo
        Dim value As CsvSheetInfo = Nothing
        If Not s_cachedColumns.TryGetValue(modelType, value) Then
            Dim selectionModeOverride = modelType.GetCustomAttribute(Of ColumnSelectionAttribute)
            If selectionModeOverride IsNot Nothing Then
                defaultSelectionMode = selectionModeOverride.SelectionMode
            End If
            Dim propPredict As Predicate(Of PropertyInfo)
            If defaultSelectionMode = CsvColumnSelectionMode.OptOut Then
                propPredict = Function(prop) prop.GetCustomAttribute(Of CsvIgnoreAttribute) Is Nothing
            Else
                propPredict = Function(prop) prop.GetCustomAttribute(Of CsvPropertyAttribute) IsNot Nothing
            End If
            Dim orderKindOverride = modelType.GetCustomAttribute(Of ColumnOrderKindAttribute)
            If orderKindOverride IsNot Nothing Then
                value.ColumnOrderKind = orderKindOverride.ColumnOrderKind
            End If
            value.ColumnInfo =
                Aggregate prop In modelType.GetRuntimeProperties
                Where prop.CanRead AndAlso prop.GetMethod.IsPublic
                Let formatInfo = prop.GetCustomAttribute(Of ColumnFormatAttribute)
                Where propPredict(prop)
                Select CreateCsvColumnInfo(prop, formatInfo, modelType, formatterCache) Into ToArray
            s_cachedColumns.GetOrAdd(modelType, value)
        End If
        Return value
    End Function

    Private Shared Function CreateCsvColumnInfo(prop As PropertyInfo, formatInfo As ColumnFormatAttribute, modelType As Type, formatterCache As ICsvRecordFormatterCache) As CsvColumnInfo
        Dim instance = Activator.CreateInstance(GetType(CsvColumnInfo(Of,)).MakeGenericType(modelType, prop.PropertyType),
                                   If(formatInfo?.Name, prop.Name),
                                   formatInfo?.FormatString,
                                   prop.PropertyType.GetFormatter(formatterCache),
                                   prop.GetMethod, prop.SetMethod)
        Return DirectCast(instance, CsvColumnInfo)
    End Function
End Class
