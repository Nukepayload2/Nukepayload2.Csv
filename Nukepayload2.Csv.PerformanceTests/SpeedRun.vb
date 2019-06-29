Imports System.IO
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass>
Public Class SpeedRun
    Private Const LoopCount = 5000
    Private Const Model1Count = 150
    Private Const Model2Count = 150
    Private ReadOnly _testModel1 As New DecoratedModel With {
        .BooleanValue = True,
        .DateValue = #2018-05-30#,
        .DoubleValue = 12345.123,
        .IntegerValue = -233,
        .StringValue = "test string"
    }
    Private ReadOnly _testModel2 As New DecoratedModel With {
        .BooleanValue = False,
        .DateValue = #2018-10-18#,
        .DoubleValue = -12345.123,
        .IntegerValue = 0,
        .StringValue = Nothing
    }
    Private ReadOnly testData As DecoratedModel() = Enumerable.Repeat(_testModel1, Model1Count).
        Concat(Enumerable.Repeat(_testModel2, Model2Count)).ToArray

    <TestMethod>
    Public Sub TestOptimal()
        For i = 1 To LoopCount
            TestSerializeOptimal(testData)
        Next
    End Sub

    Private ReadOnly _pooledText As New StringBuilder
    Private Function TestSerializeOptimal(testData() As DecoratedModel) As List(Of DecoratedModel)
        For Each test In testData
            _pooledText.Append(test.BooleanValue).Append(",")
            _pooledText.Append(test.DateValue.ToString).Append(",")
            _pooledText.Append(test.DoubleValue).Append(",")
            _pooledText.Append(test.IntegerValue).Append(",")
            _pooledText.Append(test.StringValue).Append(vbCrLf)
        Next
        Dim csv = _pooledText.ToString
        _pooledText.Clear()
        Dim newItems As New List(Of DecoratedModel)
        Dim length = 0
        Dim remaining = csv.AsSpan

#Disable Warning BC40000
        Do While SelectLine(remaining, length)
#Enable Warning BC40000
            Dim curLine = remaining.Slice(0, length)
            Dim commaIndex = curLine.IndexOf(","c)
            Dim obj As New DecoratedModel
            obj.BooleanValue = Boolean.Parse(curLine.Slice(0, commaIndex))

            Dim restPart = curLine.Slice(commaIndex + 1)
            commaIndex = restPart.IndexOf(","c)
            obj.DateValue = Date.Parse(restPart.Slice(0, commaIndex))

            restPart = restPart.Slice(commaIndex + 1)
            commaIndex = restPart.IndexOf(","c)
            obj.DoubleValue = Double.Parse(restPart.Slice(0, commaIndex))

            restPart = restPart.Slice(commaIndex + 1)
            commaIndex = restPart.IndexOf(","c)
            obj.IntegerValue = Integer.Parse(restPart.Slice(0, commaIndex))

            restPart = restPart.Slice(commaIndex + 1)
            obj.StringValue = New String(restPart.Slice(0, restPart.Length - 2))

            newItems.Add(obj)
            If length < remaining.Length Then
                remaining = remaining.Slice(length)
            Else
                Exit Do
            End If
        Loop
        Return newItems
    End Function

    <Obsolete("This function contains the use of ref struct. Needs additional code review.")>
    Private Function SelectLine(csv As ReadOnlySpan(Of Char), ByRef length As Integer) As Boolean
        Dim idx = csv.IndexOf(vbCrLf)
        If idx < 0 Then
            Return False
        Else
            length = idx + 2
            Return True
        End If
    End Function

    <TestMethod>
    Public Sub TestSameTypeExplicitCombo()
        Dim settings As New CsvSettings With {
            .ColumnOrderKind = CsvColumnOrderKind.Explicit
        }
        For i = 1 To LoopCount
            TestSerialize(testData, settings)
        Next
    End Sub

    <TestMethod>
    Public Sub TestSameTypeAutoCombo()
        Dim settings As New CsvSettings With {
            .ColumnOrderKind = CsvColumnOrderKind.Auto
        }
        For i = 1 To LoopCount
            TestSerialize(testData, settings)
        Next
    End Sub

    <TestMethod>
    Public Sub TestSameTypeSequentialCombo()
        Dim settings As New CsvSettings With {
            .ColumnOrderKind = CsvColumnOrderKind.Sequential
        }
        For i = 1 To LoopCount
            TestSerialize(testData, settings)
        Next
    End Sub

    Private Sub TestSerialize(Of T As New)(data As T(), settings As CsvSettings)
        Dim csv = CsvConvert.SerializeObject(data, settings)
        Dim obj = CsvConvert.DeserializeObject(Of T)(csv, settings)
    End Sub
End Class
