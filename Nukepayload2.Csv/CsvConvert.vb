Imports System.Reflection
Imports System.Text

''' <summary>
''' Convert collection to csv, or convert them back.
''' </summary>
Public Class CsvConvert
    Private Shared s_cachedColumns As New Dictionary(Of Type, (Name As String, FormatString As String, Formatter As ITextTableDataFormatter, Metadata As PropertyInfo)())
    Private Shared s_cachedHeaderOrder As New Dictionary(Of Type, Integer())

    ''' <summary>
    ''' Deserialize the specified csv text to .NET collection.
    ''' </summary>
    ''' <typeparam name="T">The sub type of the collection.</typeparam>
    ''' <param name="text">Csv text.</param>
    ''' <returns>The converted .NET collection</returns>
    ''' <exception cref="CsvFormatException"/>
    ''' <exception cref="FormatException"/>
    Public Shared Function DeserializeObject(Of T As {New, Class})(text As String) As IReadOnlyList(Of T)
        Return DeserializeObject(Of T)(text, CsvSettings.Default)
    End Function

    ''' <summary>
    ''' Deserialize the specified csv text to .NET collection with settings.
    ''' </summary>
    ''' <typeparam name="T">The sub type of the collection.</typeparam>
    ''' <param name="text">Csv text.</param>
    ''' <param name="settings">Custom csv format. If this parameter is null (Nothing in Visual Basic), the default settings will be used.</param>
    ''' <returns>The converted .NET collection</returns>
    ''' <exception cref="CsvFormatException"/>
    ''' <exception cref="FormatException"/>
    Public Shared Function DeserializeObject(Of T As {New, Class})(text As String, settings As CsvSettings) As IReadOnlyList(Of T)
        If settings Is Nothing Then
            settings = CsvSettings.Default
        End If
        Dim columnType As Type = GetType(T)
        Dim columns = GetColumns(columnType)
        If columns.Length = 0 Then
            Return Array.Empty(Of T)
        End If
        Dim lines = text.Split({vbCrLf}, StringSplitOptions.RemoveEmptyEntries)
        If lines.Length < 2 Then
            Return Array.Empty(Of T)
        End If
        Dim head = lines(0)
        Dim separator = settings.Separator
        Dim entities As New List(Of T)
        Select Case settings.ColumnOrderKind
            Case CsvColumnOrderKind.Auto
                Dim order = GetHeadOrder(columns, head, separator)
                For i = 1 To lines.Length - 1
                    entities.Add(InputEntity(Of T)(lines(i), order, columns, separator))
                Next
            Case CsvColumnOrderKind.Sequential
                For i = 1 To lines.Length - 1
                    entities.Add(InputEntity(Of T)(lines(i), columns, separator))
                Next
            Case CsvColumnOrderKind.Explicit
                Dim order = GetHeadOrderFor(Of T)()
                For i = 1 To lines.Length - 1
                    entities.Add(InputEntity(Of T)(lines(i), order, columns, separator))
                Next
        End Select
        entities.TrimExcess()
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

    Private Shared Function InputEntity(Of T)(line As String, columns() As (Name As String, FormatString As String, Formatter As ITextTableDataFormatter, Metadata As PropertyInfo), separator As Char) As T
        Dim lineContent = line.Split({separator}, StringSplitOptions.None)
        Dim entity = Activator.CreateInstance(Of T)
        For i = 0 To lineContent.Length - 1
            Dim col = columns(i)
            Dim data = lineContent(i)
            col.Metadata.SetValue(entity, col.Formatter.Parse(data))
        Next
        Return entity
    End Function

    Private Shared Function InputEntity(Of T As {New, Class})(line As String, order() As Integer, columns() As (Name As String, FormatString As String, Formatter As ITextTableDataFormatter, Metadata As PropertyInfo), separator As Char) As T
        Dim lineContent = line.Split({separator}, StringSplitOptions.None)
        Dim entity As New T
        For i = 0 To lineContent.Length - 1
            Dim col = columns(i)
            Dim data = lineContent(order(i))
            col.Metadata.SetValue(entity, col.Formatter.Parse(data))
        Next
        Return entity
    End Function

    Private Shared Function GetHeadOrder(columns() As (Name As String, FormatString As String, Formatter As ITextTableDataFormatter, Metadata As PropertyInfo), head As String, separator As Char) As Integer()
        Dim lineNames = head.Split({separator, " "}, StringSplitOptions.None)
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

    Private Shared Sub ThrowForColumnNotFound(columns() As (Name As String, FormatString As String, Formatter As ITextTableDataFormatter, Metadata As PropertyInfo), lineNames() As String)
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
    Public Shared Function SerializeObject(Of T As Class)(objs As IEnumerable(Of T)) As String
        Return SerializeObject(objs, CsvSettings.Default)
    End Function

    ''' <summary>
    ''' Serialize the specified collection to csv text.
    ''' </summary>
    ''' <typeparam name="T">The sub type of the collection.</typeparam>
    ''' <param name="objs">The collection that you want to serialize.</param>
    ''' <param name="settings">Custom csv format. If this parameter is null (Nothing in Visual Basic), the default settings will be used.</param>
    ''' <returns>Csv text</returns>
    ''' <exception cref="NotSupportedException"/>
    Public Shared Function SerializeObject(Of T As Class)(objs As IEnumerable(Of T), settings As CsvSettings) As String
        If settings Is Nothing Then
            settings = CsvSettings.Default
        End If
        Dim columnType As Type = GetType(T)
        Dim columns = GetColumns(columnType)
        If columns.Length = 0 Then
            Return String.Empty
        End If
        Dim separator = settings.Separator
        Dim sb As New StringBuilder
        Select Case settings.ColumnOrderKind
            Case CsvColumnOrderKind.Explicit
                Dim order = GetHeadOrderFor(Of T)()
                sb.AppendLine(String.Join(separator, OrderSelect((Aggregate col In columns
                                                                  Select col.Name Into ToArray), order)))
                For Each obj In objs
                    sb.AppendLine(String.Join(separator, OrderSelect((Aggregate col In columns
                                                         Let value = col.Metadata.GetValue(obj)
                                                         Select col.Formatter.GetString(value, col.FormatString)
                                                         Into ToArray), order)))
                Next
            Case Else
                sb.AppendLine(String.Join(separator, From col In columns Select col.Name))
                For Each obj In objs
                    sb.AppendLine(String.Join(separator, From col In columns
                                                         Let value = col.Metadata.GetValue(obj)
                                                         Select col.Formatter.GetString(value, col.FormatString)))
                Next
        End Select
        Return sb.ToString
    End Function

    Private Shared Iterator Function OrderSelect(Of T)(collection As T(), order As Integer()) As IEnumerable(Of T)
        For i = 0 To order.Length - 1
            Yield collection(order(i))
        Next
    End Function

    Private Shared Function GetColumns(columnType As Type) As (Name As String, FormatString As String, Formatter As ITextTableDataFormatter, Metadata As PropertyInfo)()
        Dim value As (Name As String, FormatString As String, Formatter As ITextTableDataFormatter, Metadata As PropertyInfo)() = Nothing
        If Not s_cachedColumns.TryGetValue(columnType, value) Then
            value = Aggregate prop In columnType.GetRuntimeProperties
                    Where prop.CanRead AndAlso prop.GetMethod.IsPublic
                    Let formatInfo = prop.GetCustomAttribute(Of ColumnFormatAttribute)
                    Select (Name:=If(formatInfo?.Name, prop.Name),
                           formatInfo?.FormatString,
                           Formatter:=prop.PropertyType.GetFormatter,
                           Metadata:=prop) Into ToArray
            s_cachedColumns.Add(columnType, value)
        End If
        Return value
    End Function
End Class
