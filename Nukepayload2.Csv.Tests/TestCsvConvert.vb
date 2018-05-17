Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass>
Public Class TestCsvConvert
    Private ReadOnly _decoratedTestData As DecoratedModel() = {
        New DecoratedModel With {
            .BooleanValue = True,
            .DateValue = #2018-05-30#,
            .DoubleValue = 12345.123,
            .IntegerValue = -233,
            .StringValue = "test string"
        },
        New DecoratedModel With {
            .BooleanValue = False,
            .DateValue = #2018-10-18#,
            .DoubleValue = -12345.123,
            .IntegerValue = 0,
            .StringValue = Nothing
        }
    }

    Private ReadOnly _rawTestData As RawModel() = {
        New RawModel With {
            .BooleanValue = True,
            .DateValue = Date.Now,
            .DoubleValue = 12345.123,
            .IntegerValue = -233,
            .StringValue = "test string",
            .SingleValue = 123.13F,
            .LongValue = 12345678901234567L
        },
        New RawModel With {
            .BooleanValue = False,
            .DateValue = Date.Now.AddDays(123),
            .DoubleValue = -12345.123,
            .IntegerValue = 0,
            .StringValue = Nothing,
            .SingleValue = -123.13F,
            .LongValue = -12345678901234567L
        }
    }

    <TestMethod>
    Public Sub TestSerializeRaw()
        TestSerialize(_rawTestData)
    End Sub

    <TestMethod>
    Public Sub TestSerializeDecorated()
        TestSerialize(_decoratedTestData)
    End Sub

    <TestMethod>
    Public Sub TestAutoOrdered()
        Const Csv = "Rate,Id,Name,Date,IsUsed
12345.12,-233,test string,2018-05-30,True
-12345.12,0,,2018-10-18,False
"
        Const CsvInMemory = "Id,Rate,Name,Date,IsUsed
-233,12345.12,test string,2018-05-30,True
0,-12345.12,,2018-10-18,False
"
        Dim obj = CsvConvert.DeserializeObject(Of DecoratedModel)(Csv)
        Dim csv2 = CsvConvert.SerializeObject(obj)
        Assert.AreEqual(CsvInMemory, csv2)
    End Sub

    <TestMethod>
    Public Sub TestSequentialOrdered()
        Const Csv = "Id,Rate,Name,Date,IsUsed
-233,12345.12,test string,2018-05-30,True
0,-12345.12,,2018-10-18,False
"
        Dim settings As New CsvSettings With {.ColumnOrderKind = CsvColumnOrderKind.Sequential}
        Dim obj = CsvConvert.DeserializeObject(Of DecoratedModel)(Csv, settings)
        Dim csv2 = CsvConvert.SerializeObject(obj, settings)
        Assert.AreEqual(Csv, csv2)
    End Sub

    <TestMethod>
    Public Sub TestExplicitOrdered()
        Const Csv = "Id,Name,Rate,Date,IsUsed
-233,test string,12345.12,2018-05-30,True
0,,-12345.12,2018-10-18,False
"
        Dim settings As New CsvSettings With {.ColumnOrderKind = CsvColumnOrderKind.Explicit}
        Dim obj = CsvConvert.DeserializeObject(Of DecoratedModel)(Csv, settings)
        Dim csv2 = CsvConvert.SerializeObject(obj, settings)
        Assert.AreEqual(Csv, csv2)
    End Sub

    Private Sub TestSerialize(Of T As {New, Class})(data As T())
        Dim csv = CsvConvert.SerializeObject(data)
        Dim obj = CsvConvert.DeserializeObject(Of T)(csv)
        Dim csv2 = CsvConvert.SerializeObject(obj)
        Assert.AreEqual(csv, csv2)
    End Sub
End Class
