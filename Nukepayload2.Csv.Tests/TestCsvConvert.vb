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
    Public Sub TestAutoOrdered_QuoteNewLine()
        Const Csv = "Rate,Id,Name,Date,IsUsed
12345.12,-233,""
"",2018-05-30,True
-12345.12,0,,2018-10-18,False
"
        Const CsvInMemory = "Id,Rate,Name,Date,IsUsed
-233,12345.12,""
"",2018-05-30,True
0,-12345.12,,2018-10-18,False
"
        Dim obj = CsvConvert.DeserializeObject(Of DecoratedModel)(Csv)
        Dim csv2 = CsvConvert.SerializeObject(obj)
        Assert.AreEqual(CsvInMemory, csv2)
    End Sub

    <TestMethod>
    Public Sub TestAutoOrdered_ExcelLikeEscape()
        Const Csv = "Rate,Id,Name,Date,IsUsed
""$ 12,345.12"",-233,""test """"string"""""",2018-05-30,True
-1.234512E+04,0,,2018-10-18,False
"
        Const CsvInMemory = "Id,Rate,Name,Date,IsUsed
-233,12345.12,""test """"string"""""",2018-05-30,True
0,-12345.12,,2018-10-18,False
"
        Dim obj = CsvConvert.DeserializeObject(Of DecoratedModel)(Csv)
        Dim csv2 = CsvConvert.SerializeObject(obj)
        Assert.AreEqual(CsvInMemory, csv2)
    End Sub

    <TestMethod>
    Public Sub TestAutoOrdered_MultiCharSeparator()
        Const vbTab2 As String = vbTab + vbTab
        Dim Csv = $"Rate{vbTab2}Id{vbTab2}Name{vbTab2}Date{vbTab2}IsUsed
""$ 12,345.12""{vbTab2}-233{vbTab2}""test """"string""""""{vbTab2}2018-05-30{vbTab2}True
-1.234512E+04{vbTab2}0{vbTab2}{vbTab2}2018-10-18{vbTab2}False
"
        Dim CsvInMemory = $"Id{vbTab2}Rate{vbTab2}Name{vbTab2}Date{vbTab2}IsUsed
-233{vbTab2}12345.12{vbTab2}""test """"string""""""{vbTab2}2018-05-30{vbTab2}True
0{vbTab2}-12345.12{vbTab2}{vbTab2}2018-10-18{vbTab2}False
"
        Dim settings As New CsvSettings With {.Separator = vbTab2}
        Dim obj = CsvConvert.DeserializeObject(Of DecoratedModel)(Csv, settings)
        Dim csv2 = CsvConvert.SerializeObject(obj, settings)
        Assert.AreEqual(CsvInMemory, csv2)
    End Sub

    <TestMethod>
    Public Sub TestAutoOrdered_ExcelFormats()
        Const Csv = "Value
$0
$0.5
-$0.5
$-0.5
(12.345)
$(12.345)
($12.345)
$ 12.345
1.2E+9
(4.25E-05)
8E7
"
        Const CsvInMemory = "Value
0
0.5
-0.5
-0.5
-12.345
-12.345
-12.345
12.345
1200000000
-4.25E-05
80000000
"
        Dim obj = CsvConvert.DeserializeObject(Of SingleValueModel)(Csv)
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

    <TestMethod>
    Public Sub TestExplicitOrdered2()
        Const Csv = "Id,Name,Rate,Date,IsUsed
-233,test string,12345.12,2018-05-30,True
0,,-12345.12,2018-10-18,False
"
        Dim obj = CsvConvert.DeserializeObject(Of ExplicitModel)(Csv)
        Dim csv2 = CsvConvert.SerializeObject(obj)
        Assert.AreEqual(Csv, csv2)
    End Sub

    <TestMethod>
    Public Sub TestReadOnlySerialize()
        Dim values = {New ReadOnlyModel(1, "内容1", True), New ReadOnlyModel(233, "内容2", False)}
        Dim settings As New CsvSettings
        Dim csv = CsvConvert.SerializeObject(values, settings)
        Const expected As String = "Id,Content,IsChecked
1,内容1,True
233,内容2,False
"
        Assert.AreEqual(expected, csv)
    End Sub

    <TestMethod>
    Public Sub TestOptInOptOutSerialize()
        Dim values = {
            New OptInSerializationModel(1, "内容1", True),
            New OptInSerializationModel(233, "内容2", False)
        }
        Dim settings As New CsvSettings
        Dim csv = CsvConvert.SerializeObject(values, settings)
        Const expected As String = "Id,Content,IsChecked
1,内容1,True
233,内容2,False
"
        Assert.AreEqual(expected, csv)
        Dim values2 = {
            New OptOutSerializationModel(1, "内容1", True),
            New OptOutSerializationModel(233, "内容2", False)
        }
        Dim csv2 = CsvConvert.SerializeObject(values2, settings)
        Assert.AreEqual(expected, csv2)
        Dim optOuts = CsvConvert.DeserializeObject(Of OptOutSerializationModel)(csv, settings)
        Assert.AreEqual(expected, CsvConvert.SerializeObject(optOuts))
        Assert.AreEqual(2, optOuts.Count)
        Assert.IsNull(optOuts(0).Garbage)
        Dim optIns = CsvConvert.DeserializeObject(Of OptInSerializationModel)(csv, settings)
        Assert.AreEqual(expected, CsvConvert.SerializeObject(optIns))
        Assert.IsNull(optIns(0).Garbage)
    End Sub

End Class
