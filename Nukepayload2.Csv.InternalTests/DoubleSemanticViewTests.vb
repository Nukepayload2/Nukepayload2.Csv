Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Nukepayload2.Csv.Numbers.Double

<TestClass>
Public Class DoubleSemanticViewTests
    <TestMethod>
    Public Sub TestParseNumber0()
        AssertNumber("123", 123)
    End Sub

    <TestMethod>
    Public Sub TestParseNumber1()
        AssertNumber("0.123", 0.123)
    End Sub

    Private Shared Sub AssertNumber(text As String, answer As Double)
        Dim sv As New DoubleSemanticView
        Dim result As Double = Nothing
        If sv.TryParse(text, result) Then
            Assert.AreEqual(answer, result)
        Else
            Assert.Fail()
        End If
    End Sub

End Class
