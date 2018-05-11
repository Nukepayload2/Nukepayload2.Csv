Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass>
Public Class SpeedRun
    Private Const LoopCount = 2_000
    Private Const Model1Count = 100
    Private Const Model2Count = 100
    Private ReadOnly testModel1 As New DecoratedModel With {
        .BooleanValue = True,
        .DateValue = Date.Now,
        .DoubleValue = 12345.123,
        .IntegerValue = -233,
        .StringValue = "test string"
    }
    Private ReadOnly testModel2 As New DecoratedModel With {
        .BooleanValue = False,
        .DateValue = Date.Now.AddDays(123),
        .DoubleValue = -12345.123,
        .IntegerValue = 0,
        .StringValue = Nothing
    }
    Private ReadOnly testData As DecoratedModel() = Enumerable.Repeat(testModel1, Model1Count).
        Concat(Enumerable.Repeat(testModel2, Model2Count)).ToArray

    <TestMethod>
    Public Sub TestSameTypeExplicitCombo()
        Dim settings = CsvSettings.Default
        settings.ColumnOrderKind = CsvColumnOrderKind.Explicit
        For i = 1 To LoopCount
            TestSerialize(testData, settings)
        Next
    End Sub

    <TestMethod>
    Public Sub TestSameTypeAutoCombo()
        Dim settings = CsvSettings.Default
        settings.ColumnOrderKind = CsvColumnOrderKind.Auto
        For i = 1 To LoopCount
            TestSerialize(testData, settings)
        Next
    End Sub

    <TestMethod>
    Public Sub TestSameTypeSequentialCombo()
        Dim settings = CsvSettings.Default
        settings.ColumnOrderKind = CsvColumnOrderKind.Sequential
        For i = 1 To LoopCount
            TestSerialize(testData, settings)
        Next
    End Sub

    Private Sub TestSerialize(Of T As {New, Class})(data As T(), settings As CsvSettings)
        Dim csv = CsvConvert.SerializeObject(data, settings)
        Dim obj = CsvConvert.DeserializeObject(Of T)(csv, settings)
    End Sub
End Class
