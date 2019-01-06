Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass>
Public Class CsvLineSplitterTests
    <TestMethod>
    Public Sub TestNoSeparator()
        Const str = "1234"
        Const separator = ","
        Dim resultArr(0) As String
        str.SplitElementsInto(separator, resultArr, StringSplitOptions.None)
        Assert.AreEqual(str, resultArr(0))
    End Sub

    <TestMethod>
    Public Sub Test1Separator()
        Const str = "1234,5678"
        Const separator = ","
        Dim resultArr(1) As String
        str.SplitElementsInto(separator, resultArr, StringSplitOptions.None)
        Assert.AreEqual("1234", resultArr(0))
        Assert.AreEqual("5678", resultArr(1))
    End Sub

    <TestMethod>
    Public Sub Test2Separators()
        Const str = "1,2,3"
        Const separator = ","
        Dim resultArr(2) As String
        str.SplitElementsInto(separator, resultArr, StringSplitOptions.None)
        Assert.AreEqual("1", resultArr(0))
        Assert.AreEqual("2", resultArr(1))
        Assert.AreEqual("3", resultArr(2))
    End Sub

    <TestMethod>
    Public Sub Test1SeparatorWithQuote()
        Const str = """1234"",5678"
        Const separator = ","
        Dim resultArr(1) As String
        str.SplitElementsInto(separator, resultArr, StringSplitOptions.None)
        Assert.AreEqual("1234", resultArr(0))
        Assert.AreEqual("5678", resultArr(1))
    End Sub

    <TestMethod>
    Public Sub Test1SeparatorWithQuoteAndFakeSeparator()
        Const str = """12,34"",5678"
        Const separator = ","
        Dim resultArr(1) As String
        str.SplitElementsInto(separator, resultArr, StringSplitOptions.None)
        Assert.AreEqual("12,34", resultArr(0))
        Assert.AreEqual("5678", resultArr(1))
    End Sub

    <TestMethod>
    Public Sub Test1SeparatorWithQuoteAndEscapeQuote()
        Const str = """1234"",""5""""6""""78"""
        Const separator = ","
        Dim resultArr(1) As String
        str.SplitElementsInto(separator, resultArr, StringSplitOptions.None)
        Assert.AreEqual("1234", resultArr(0))
        Assert.AreEqual("5""6""78", resultArr(1))
    End Sub

    <TestMethod>
    Public Sub TestEmptyTwoStrings()
        Const str = ","
        Const separator = ","
        Dim resultArr(1) As String
        str.SplitElementsInto(separator, resultArr, StringSplitOptions.None)
        Assert.AreEqual("", resultArr(0))
        Assert.AreEqual("", resultArr(1))
    End Sub

    <TestMethod>
    Public Sub TestEmptyStringsRemoveEmpty()
        Const str = ","
        Const separator = ","
        Dim resultArr(0) As String
        str.SplitElementsInto(separator, resultArr, StringSplitOptions.RemoveEmptyEntries)
        Assert.IsNull(resultArr(0))
    End Sub

    <TestMethod>
    Public Sub TestEmptyFirst()
        Const str = ",1"
        Const separator = ","
        Dim resultArr(1) As String
        str.SplitElementsInto(separator, resultArr, StringSplitOptions.None)
        Assert.AreEqual("", resultArr(0))
        Assert.AreEqual("1", resultArr(1))
    End Sub

    <TestMethod>
    Public Sub TestEmptyFirstRemoveEmpty()
        Const str = ",1"
        Const separator = ","
        Dim resultArr(0) As String
        str.SplitElementsInto(separator, resultArr, StringSplitOptions.RemoveEmptyEntries)
        Assert.AreEqual("1", resultArr(0))
    End Sub

    <TestMethod>
    Public Sub TestEmptyLast()
        Const str = "1,"
        Const separator = ","
        Dim resultArr(1) As String
        str.SplitElementsInto(separator, resultArr, StringSplitOptions.None)
        Assert.AreEqual("1", resultArr(0))
        Assert.AreEqual("", resultArr(1))
    End Sub

    <TestMethod>
    Public Sub TestEmptyLastRemoveEmpty()
        Const str = "1,"
        Const separator = ","
        Dim resultArr(0) As String
        str.SplitElementsInto(separator, resultArr, StringSplitOptions.RemoveEmptyEntries)
        Assert.AreEqual("1", resultArr(0))
    End Sub

    <TestMethod>
    Public Sub Test3LongSeparators()
        Const str = "一还有二还有三还有四"
        Const separator = "还有"
        Dim resultArr(3) As String
        str.SplitElementsInto(separator, resultArr, StringSplitOptions.None)
        Assert.AreEqual("一", resultArr(0))
        Assert.AreEqual("二", resultArr(1))
        Assert.AreEqual("三", resultArr(2))
        Assert.AreEqual("四", resultArr(3))
    End Sub

    <TestMethod>
    Public Sub Test3LongSeparatorsEx()
        Const str = "一还还有二还还有三还还有四还"
        Const separator = "还有"
        Dim resultArr(3) As String
        str.SplitElementsInto(separator, resultArr, StringSplitOptions.None)
        Assert.AreEqual("一还", resultArr(0))
        Assert.AreEqual("二还", resultArr(1))
        Assert.AreEqual("三还", resultArr(2))
        Assert.AreEqual("四还", resultArr(3))
    End Sub

    <TestMethod>
    Public Sub TestLongSeparatorWithQuote_1()
        Const str = """一还有二""还有三还有四"
        Const separator = "还有"
        Dim resultArr(2) As String
        str.SplitElementsInto(separator, resultArr, StringSplitOptions.None)
        Assert.AreEqual("一还有二", resultArr(0))
        Assert.AreEqual("三", resultArr(1))
        Assert.AreEqual("四", resultArr(2))
    End Sub

    <TestMethod>
    Public Sub TestLongSeparatorWithQuote_2()
        Const str = """一""还有""二还有三""还有""""""四"""""""
        Const separator = "还有"
        Dim resultArr(2) As String
        str.SplitElementsInto(separator, resultArr, StringSplitOptions.None)
        Assert.AreEqual("一", resultArr(0))
        Assert.AreEqual("二还有三", resultArr(1))
        Assert.AreEqual("""四""", resultArr(2))
    End Sub

    <TestMethod>
    Public Sub TestWindowsLineSplit()
        Const str = "1" & vbCrLf & "2" & vbCrLf & "3" & vbCrLf & "4"
        Const separator = vbCrLf
        Dim result = str.SplitLines(separator)
        Assert.AreEqual("1", result(0))
        Assert.AreEqual("2", result(1))
        Assert.AreEqual("3", result(2))
        Assert.AreEqual("4", result(3))
    End Sub

    <TestMethod>
    Public Sub TestLinuxLineSplit()
        Const str = "1" & vbLf & "2" & vbLf & "3" & vbLf & "4"
        Const separator = vbLf
        Dim result = str.SplitLines(separator)
        Assert.AreEqual("1", result(0))
        Assert.AreEqual("2", result(1))
        Assert.AreEqual("3", result(2))
        Assert.AreEqual("4", result(3))
    End Sub

    <TestMethod>
    Public Sub TestWindowsLineSplitQuoted()
        Const separator = vbCrLf
        Const str = "1" & separator & """2" & separator & "3""" & separator & "4"
        Dim result = str.SplitLines(separator)
        Assert.AreEqual("1", result(0))
        Assert.AreEqual("""2" & separator & "3""", result(1))
        Assert.AreEqual("4", result(2))
    End Sub

    <TestMethod>
    Public Sub TestLinuxLineSplitQuoted()
        Const separator = vbLf
        Const str = "1" & separator & """2" & separator & "3""" & separator & "4"
        Dim result = str.SplitLines(separator)
        Assert.AreEqual("1", result(0))
        Assert.AreEqual("""2" & separator & "3""", result(1))
        Assert.AreEqual("4", result(2))
    End Sub

    <TestMethod>
    Public Sub TestCustomLineSplit()
        Const str = "一还还有二还还有三还还有四还"
        Const separator = "还有"
        Dim result = str.SplitLines(separator)
        Assert.AreEqual("一还", result(0))
        Assert.AreEqual("二还", result(1))
        Assert.AreEqual("三还", result(2))
        Assert.AreEqual("四还", result(3))
    End Sub

End Class
