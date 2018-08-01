Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass>
Public Class CsvLineSplitterTests
    <TestMethod>
    Public Sub TestNoSeparator()
        Const str = "1234"
        Const separator = ","
        Dim resultArr(0) As String
        str.SplitInto(separator, resultArr, StringSplitOptions.None)
        Assert.AreEqual(str, resultArr(0))
    End Sub

    <TestMethod>
    Public Sub Test1Separator()
        Const str = "1234,5678"
        Const separator = ","
        Dim resultArr(1) As String
        str.SplitInto(separator, resultArr, StringSplitOptions.None)
        Assert.AreEqual("1234", resultArr(0))
        Assert.AreEqual("5678", resultArr(1))
    End Sub

    <TestMethod>
    Public Sub Test2Separators()
        Const str = "1,2,3"
        Const separator = ","
        Dim resultArr(2) As String
        str.SplitInto(separator, resultArr, StringSplitOptions.None)
        Assert.AreEqual("1", resultArr(0))
        Assert.AreEqual("2", resultArr(1))
        Assert.AreEqual("3", resultArr(2))
    End Sub

    <TestMethod>
    Public Sub Test1SeparatorWithQuote()
        Const str = """1234"",5678"
        Const separator = ","
        Dim resultArr(1) As String
        str.SplitInto(separator, resultArr, StringSplitOptions.None)
        Assert.AreEqual("1234", resultArr(0))
        Assert.AreEqual("5678", resultArr(1))
    End Sub

    <TestMethod>
    Public Sub Test1SeparatorWithQuoteAndFakeSeparator()
        Const str = """12,34"",5678"
        Const separator = ","
        Dim resultArr(1) As String
        str.SplitInto(separator, resultArr, StringSplitOptions.None)
        Assert.AreEqual("12,34", resultArr(0))
        Assert.AreEqual("5678", resultArr(1))
    End Sub

    <TestMethod>
    Public Sub Test1SeparatorWithQuoteAndEscapeQuote()
        Const str = """1234"",""5""""6""""78"""
        Const separator = ","
        Dim resultArr(1) As String
        str.SplitInto(separator, resultArr, StringSplitOptions.None)
        Assert.AreEqual("1234", resultArr(0))
        Assert.AreEqual("5""6""78", resultArr(1))
    End Sub

    <TestMethod>
    Public Sub TestEmptyTwoStrings()
        Const str = ","
        Const separator = ","
        Dim resultArr(1) As String
        str.SplitInto(separator, resultArr, StringSplitOptions.None)
        Assert.AreEqual("", resultArr(0))
        Assert.AreEqual("", resultArr(1))
    End Sub

    <TestMethod>
    Public Sub TestEmptyStringsRemoveEmpty()
        Const str = ","
        Const separator = ","
        Dim resultArr(0) As String
        str.SplitInto(separator, resultArr, StringSplitOptions.RemoveEmptyEntries)
        Assert.IsNull(resultArr(0))
    End Sub

    <TestMethod>
    Public Sub TestEmptyFirst()
        Const str = ",1"
        Const separator = ","
        Dim resultArr(1) As String
        str.SplitInto(separator, resultArr, StringSplitOptions.None)
        Assert.AreEqual("", resultArr(0))
        Assert.AreEqual("1", resultArr(1))
    End Sub

    <TestMethod>
    Public Sub TestEmptyFirstRemoveEmpty()
        Const str = ",1"
        Const separator = ","
        Dim resultArr(0) As String
        str.SplitInto(separator, resultArr, StringSplitOptions.RemoveEmptyEntries)
        Assert.AreEqual("1", resultArr(0))
    End Sub

    <TestMethod>
    Public Sub TestEmptyLast()
        Const str = "1,"
        Const separator = ","
        Dim resultArr(1) As String
        str.SplitInto(separator, resultArr, StringSplitOptions.None)
        Assert.AreEqual("1", resultArr(0))
        Assert.AreEqual("", resultArr(1))
    End Sub

    <TestMethod>
    Public Sub TestEmptyLastRemoveEmpty()
        Const str = "1,"
        Const separator = ","
        Dim resultArr(0) As String
        str.SplitInto(separator, resultArr, StringSplitOptions.RemoveEmptyEntries)
        Assert.AreEqual("1", resultArr(0))
    End Sub

    <TestMethod>
    Public Sub Test3LongSeparators()
        Const str = "一还有二还有三还有四"
        Const separator = "还有"
        Dim resultArr(3) As String
        str.SplitInto(separator, resultArr, StringSplitOptions.None)
        Assert.AreEqual("一", resultArr(0))
        Assert.AreEqual("二", resultArr(1))
        Assert.AreEqual("三", resultArr(2))
        Assert.AreEqual("四", resultArr(3))
    End Sub

    <TestMethod>
    Public Sub TestLongSeparatorWithQuote_1()
        Const str = """一还有二""还有三还有四"
        Const separator = "还有"
        Dim resultArr(2) As String
        str.SplitInto(separator, resultArr, StringSplitOptions.None)
        Assert.AreEqual("一还有二", resultArr(0))
        Assert.AreEqual("三", resultArr(1))
        Assert.AreEqual("四", resultArr(2))
    End Sub

    <TestMethod>
    Public Sub TestLongSeparatorWithQuote_2()
        Const str = """一""还有""二还有三""还有""""""四"""""""
        Const separator = "还有"
        Dim resultArr(2) As String
        str.SplitInto(separator, resultArr, StringSplitOptions.None)
        Assert.AreEqual("一", resultArr(0))
        Assert.AreEqual("二还有三", resultArr(1))
        Assert.AreEqual("""四""", resultArr(2))
    End Sub

End Class
