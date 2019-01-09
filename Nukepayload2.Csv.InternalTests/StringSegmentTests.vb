Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass>
Public Class StringSegmentTests
    <TestMethod>
    Public Sub TestSlice()
        Dim str = "123456"
        Dim segment = StringSegment.Slice(str, 0, 2).GetString
        Assert.AreEqual("12", segment)
        segment = StringSegment.Slice(str, 1, 2).GetString
        Assert.AreEqual("23", segment)
        segment = StringSegment.Slice(str, 0, 6).Slice(1).GetString
        Assert.AreEqual("23456", segment)
        segment = StringSegment.Slice(str, 0, 6).Slice(1, 4).GetString
        Assert.AreEqual("2345", segment)
    End Sub

    <TestMethod>
    Public Sub TestDoubleSlice()
        Dim str = "123456"
        Dim segment = StringSegment.Slice(str, 2, 3).Slice(1).GetString
        Assert.AreEqual("45", segment)
        segment = StringSegment.Slice(str, 2, 3).Slice(1, 1).GetString
        Assert.AreEqual("4", segment)
    End Sub
End Class
