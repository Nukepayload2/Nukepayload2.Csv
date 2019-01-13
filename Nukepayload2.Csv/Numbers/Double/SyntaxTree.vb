Imports Nukepayload2.Buffers.Fixed

Namespace Numbers.Double
    Friend Structure SyntaxTree
        Public Children As FixedList32(Of SyntaxNode)
        Public Text As StringSegment
        Public Kind As NumberKind
        Public HasParen As Boolean
        Public DigitNodeCount As Integer
        Public HasComma As Boolean
        Public Sub New(children As FixedList32(Of SyntaxNode), text As StringSegment, kind As NumberKind, hasParen As Boolean, digitNodeCount As Integer, hasComma As Boolean)
            Me.Children = children
            Me.Text = text
            Me.Kind = kind
            Me.HasParen = hasParen
            Me.DigitNodeCount = digitNodeCount
            Me.HasComma = hasComma
        End Sub
    End Structure
End Namespace