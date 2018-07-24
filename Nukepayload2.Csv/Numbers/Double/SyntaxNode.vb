Namespace Numbers.Double
    Friend Structure SyntaxNode
        Public Kind As SyntaxKind
        Public Range As Range
        Public Sub New(kind As SyntaxKind, range As Range)
            Me.Kind = kind
            Me.Range = range
        End Sub
        Public Overrides Function ToString() As String
            If Kind = SyntaxKind.Unknown Then
                Return "Unknown"
            End If
            Return $"{Kind.ToString} ({Range.ToString})"
        End Function
    End Structure

End Namespace