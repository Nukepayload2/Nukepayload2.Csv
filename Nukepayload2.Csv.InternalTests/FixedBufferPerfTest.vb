Imports Microsoft.VisualStudio.TestTools.UnitTesting
' Imports Nukepayload2.Buffers.Fixed
Imports Nukepayload2.Csv.Numbers.Double

<TestClass>
Public Class FixedBufferPerfTest
    Const RepeatCount = 1000_0000

    <TestMethod>
    Sub TestSyntaxNodeList()
        For i = 1 To RepeatCount
            Dim items As New List(Of SyntaxNode)
            For j = 1 To 32
                items.Add(New SyntaxNode)
            Next
            For j = 0 To 31
                Dim item = items(j)
            Next
        Next
    End Sub

    <TestMethod>
    Sub TestSyntaxNodeFixed()
        For i = 1 To RepeatCount
            Dim items As New FixedList32(Of SyntaxNode)
            For j = 1 To 32
                If Not items.TryAdd(New SyntaxNode) Then
                    Assert.Fail()
                End If
            Next
            For j = 0 To 31
                Dim item As SyntaxNode
                If Not items.TryGetItem(j, item) Then
                    Assert.Fail()
                End If
            Next
        Next
    End Sub
End Class
