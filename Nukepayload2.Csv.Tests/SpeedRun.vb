Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass>
Public Class SpeedRun
    Const LoopCount = 10_0000
    Dim dat As DecoratedModel() = {
        New DecoratedModel With {
            .BooleanValue = True,
            .DateValue = Date.Now,
            .DoubleValue = 12345.123,
            .IntegerValue = -233,
            .StringValue = "test string"
        },
        New DecoratedModel With {
            .BooleanValue = False,
            .DateValue = Date.Now.AddDays(123),
            .DoubleValue = -12345.123,
            .IntegerValue = 0,
            .StringValue = Nothing
        }
    }

    <TestMethod>
    Public Sub TestSameTypeExplicitCombo()
        Dim settings = CsvSettings.Default
        settings.ColumnOrderKind = CsvColumnOrderKind.Explicit
        For i = 1 To LoopCount
            TestSerialize(dat, settings)
        Next
    End Sub

    <TestMethod>
    Public Sub TestSameTypeAutoCombo()
        Dim settings = CsvSettings.Default
        settings.ColumnOrderKind = CsvColumnOrderKind.Auto
        For i = 1 To LoopCount
            TestSerialize(dat, settings)
        Next
    End Sub

    <TestMethod>
    Public Sub TestSameTypeSequentialCombo()
        Dim settings = CsvSettings.Default
        settings.ColumnOrderKind = CsvColumnOrderKind.Sequential
        For i = 1 To LoopCount
            TestSerialize(dat, settings)
        Next
    End Sub

    Private Sub TestSerialize(Of T As {New, Class})(data As T(), settings As CsvSettings)
        Dim csv = CsvConvert.SerializeObject(data, settings)
        Dim obj = CsvConvert.DeserializeObject(Of T)(csv, settings)
    End Sub
End Class
