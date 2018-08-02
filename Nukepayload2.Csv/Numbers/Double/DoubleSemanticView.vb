Namespace Numbers.Double
    Friend Class DoubleSemanticView
        Private Const ZeroChar = AscW("0"c)

        Friend Function TryParse(text As String, ByRef result As Double, currencySymbol As Char) As Boolean
            If Not String.IsNullOrEmpty(text) Then
                Dim syntaxTree As New SyntaxTree
                If TryGetSyntaxTree(text, currencySymbol, syntaxTree) Then
                    Return TryParseExact(syntaxTree, result)
                End If
            End If
            result = 0.0
            Return False
        End Function

        Private Function TryGetSyntaxTree(text As String, currencySymbol As Char, ByRef syntaxTree As SyntaxTree) As Boolean
            Dim strLen As Integer = text.Length
            Dim hasLeftParen As Boolean = False
            Dim hasRightParen As Boolean = False
            Dim leadingWhitespace As Boolean = False
            Dim hasCurrencySymbol As Boolean = False
            Dim hasPercentage As Boolean = False
            Dim hasSign As Boolean = False
            Dim hasComma As Boolean = False
            Dim foundDigits As Boolean = False
            Dim digitNodeCount As Integer = 0
            Dim integerPartEndIndex As Integer = -1
            Dim curRange As New Range With {
                .Length = 1
            }
            Dim numberKind As NumberKind = NumberKind.Integer
            Dim curSyntax As SyntaxKind = SyntaxKind.Unknown
            Dim i As Integer = 0
            While i < strLen
                Dim current As Char = text(i)
                Select Case curSyntax
                    Case SyntaxKind.Unknown
                        Select Case current
                            Case "0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c
                                foundDigits = True
                                curSyntax = SyntaxKind.Digits
                                Continue While
                            Case "("c
                                If hasLeftParen Then
                                    Return False
                                End If
                                hasLeftParen = True
                                curSyntax = SyntaxKind.LeftParenthesis
                                Continue While
                            Case ")"c
                                If Not hasLeftParen OrElse hasRightParen Then
                                    Return False
                                End If
                                hasRightParen = True
                                curSyntax = SyntaxKind.RightParenthesis
                                Continue While
                            Case "+"c
                                If numberKind <> NumberKind.Scientific Then
                                    If hasSign Then
                                        Return False
                                    End If
                                    hasSign = True
                                End If
                                curSyntax = SyntaxKind.Plus
                                Continue While
                            Case "-"c
                                If numberKind <> NumberKind.Scientific Then
                                    If hasSign Then
                                        Return False
                                    End If
                                    hasSign = True
                                End If
                                curSyntax = SyntaxKind.Negation
                                Continue While
                            Case "."c
                                If numberKind = NumberKind.Integer Then
                                    If integerPartEndIndex < 0 Then
                                        integerPartEndIndex = syntaxTree.Children.Count
                                    End If
                                    numberKind = NumberKind.Decimal
                                Else
                                    Return False
                                End If
                                curSyntax = SyntaxKind.Dot
                                Continue While
                            Case ","c
                                If numberKind <> NumberKind.Integer Then
                                    Return False
                                End If
                                digitNodeCount -= 1
                                hasComma = True
                                curSyntax = SyntaxKind.Comma
                                Continue While
                            Case "/"c
                                If numberKind = NumberKind.Integer Then
                                    numberKind = NumberKind.Fraction
                                Else
                                    Return False
                                End If
                                curSyntax = SyntaxKind.Slash
                                Continue While
                            Case "e"c, "E"c
                                If numberKind < NumberKind.PlainNumber AndAlso foundDigits Then
                                    If integerPartEndIndex < 0 Then
                                        integerPartEndIndex = syntaxTree.Children.Count
                                    End If
                                    numberKind = NumberKind.Scientific
                                Else
                                    Return False
                                End If
                                curSyntax = SyntaxKind.Exponent
                                Continue While
                            Case " "c
                                leadingWhitespace = i = 0
                                curSyntax = SyntaxKind.Space
                                If foundDigits Then
                                    If integerPartEndIndex < 0 Then
                                        integerPartEndIndex = syntaxTree.Children.Count
                                    End If
                                End If
                                Continue While
                            Case "%"c
                                If Not hasPercentage Then
                                    hasPercentage = True
                                Else
                                    Return False
                                End If
                                curSyntax = SyntaxKind.Percentage
                                Continue While
                            Case Else
                                If current = currencySymbol Then
                                    If hasCurrencySymbol Then
                                        Return False
                                    End If
                                    curSyntax = SyntaxKind.CurrencySymbol
                                    hasCurrencySymbol = True
                                    Continue While
                                Else
                                    Return False
                                End If
                        End Select
                    Case SyntaxKind.Digits
                        digitNodeCount += 1
                        For j As Integer = i + 1 To strLen - 1
                            Dim ch As Char = text(j)
                            If ch >= "0"c AndAlso ch <= "9"c Then
                                curRange.Length += 1
                            Else
                                Exit For
                            End If
                        Next
                        GoTo CaseElse
                    Case SyntaxKind.Space
                        If leadingWhitespace Then
                            For j As Integer = i + 1 To strLen - 1
                                Dim ch As Char = text(j)
                                If ch = " "c Then
                                    curRange.Length += 1
                                Else
                                    Exit For
                                End If
                            Next
                            Exit Select
                        End If
                        GoTo CaseElse
                    Case Else
CaseElse:               Dim success = syntaxTree.Children.TryAdd(New SyntaxNode(curSyntax, curRange))
                        If Not success Then
                            Return False
                        End If
                End Select
                curSyntax = SyntaxKind.Unknown
                i += curRange.Length
                curRange.Start = i
                curRange.Length = 1
            End While
            If Not foundDigits Then
                Return False
            End If
            If integerPartEndIndex = -1 Then
                integerPartEndIndex = syntaxTree.Children.Count
            End If
            If hasLeftParen AndAlso Not hasRightParen Then
                Return False
            End If
            If hasSign AndAlso hasLeftParen Then
                Return False
            End If
            Dim firstIndexOfDigits As Integer = -1
            For j = 0 To integerPartEndIndex - 1
                If syntaxTree.Children.UnsafeItem(j).Kind = SyntaxKind.Comma Then
                    Return False
                ElseIf syntaxTree.Children.UnsafeItem(j).Kind = SyntaxKind.Digits Then
                    firstIndexOfDigits = j
                    Exit For
                End If
            Next
            If hasComma Then
                If integerPartEndIndex = -1 Then
                    integerPartEndIndex = syntaxTree.Children.Count - 1
                End If
                If integerPartEndIndex = 0 Then
                    Return False
                End If
                Dim expectedKind = SyntaxKind.Comma
                For j = firstIndexOfDigits + 1 To integerPartEndIndex - 1
                    If syntaxTree.Children.UnsafeItem(j).Kind <> expectedKind Then
                        Return False
                    End If
                    If syntaxTree.Children.UnsafeItem(j).Kind = SyntaxKind.Digits Then
                        If syntaxTree.Children.UnsafeItem(j).Range.Length < 3 Then
                            Return False
                        Else
                            expectedKind = SyntaxKind.Comma
                        End If
                    Else
                        expectedKind = SyntaxKind.Digits
                    End If
                Next
                If expectedKind = SyntaxKind.Digits Then
                    Return False
                End If
            End If
            For j = 0 To firstIndexOfDigits - 1
                If syntaxTree.Children.UnsafeItem(j).Kind = SyntaxKind.Space Then
                    syntaxTree.Children.TryRemoveAt(j)
                Else
                    Exit For
                End If
            Next
            For j = syntaxTree.Children.Count - 1 To 0
                Dim kind As SyntaxKind = syntaxTree.Children.UnsafeItem(j).Kind
                If kind = SyntaxKind.Space Then
                    syntaxTree.Children.TryRemoveAt(j)
                ElseIf kind = SyntaxKind.Digits OrElse kind = SyntaxKind.Dot Then
                    Exit For
                End If
            Next
            With syntaxTree
                .Text = text
                .Kind = numberKind
                .HasParen = hasRightParen
                .DigitNodeCount = digitNodeCount
                .HasComma = hasComma
            End With
            Return True
        End Function

        Private Function TryParseExact(ByRef syntax As SyntaxTree, ByRef result As Double) As Boolean
            Dim number As Double = 0.0
            Dim multipler As Double = 1.0
            Dim isNegative As Boolean = False
            Select Case syntax.Kind
                Case NumberKind.Integer
                    If syntax.DigitNodeCount <> 1 Then
                        Return False
                    End If
                    GoTo PlainNumber
                Case NumberKind.Decimal
                    If syntax.DigitNodeCount > 2 OrElse syntax.DigitNodeCount < 1 Then
                        Return False
                    End If
                    GoTo PlainNumber
                Case NumberKind.PlainNumber
PlainNumber:        If syntax.HasParen Then
                        If syntax.Children.Count < 3 Then
                            Return False
                        End If
                        Dim child0 As SyntaxNode = syntax.Children.UnsafeItem(0)
                        If child0.Kind = SyntaxKind.LeftParenthesis Then
                            isNegative = True
                        ElseIf child0.Kind = SyntaxKind.CurrencySymbol AndAlso syntax.Children.UnsafeItem(1).Kind = SyntaxKind.LeftParenthesis Then
                            isNegative = True
                        End If
                    Else
                        If syntax.Children.Count < 1 Then
                            Return False
                        End If
                    End If
                    Dim foundNumbers As Boolean = False
                    Dim foundDot As Boolean = False
                    For i = 0 To syntax.Children.Count - 1
                        Dim child As SyntaxNode = syntax.Children.UnsafeItem(i)
                        Select Case child.Kind
                            Case SyntaxKind.Unknown
                                Throw New InvalidOperationException("Bad syntax tree.")
                            Case SyntaxKind.Plus
                            Case SyntaxKind.Negation
                                If foundNumbers Then
                                    Return False
                                Else
                                    isNegative = True
                                End If
                            Case SyntaxKind.CurrencySymbol
                            Case SyntaxKind.Digits
                                foundNumbers = True
                                If foundDot Then
                                    ParseDecimal(number, child.Range, syntax.Text)
                                Else
                                    If syntax.HasComma Then
                                        ParseCurrencyInteger(number, child.Range, syntax.Text)
                                    Else
                                        ParseInteger(number, child.Range, syntax.Text)
                                    End If
                                End If
                            Case SyntaxKind.Comma
                                If foundDot Then
                                    Return False
                                End If
                            Case SyntaxKind.Dot
                                foundDot = True
                            Case SyntaxKind.LeftParenthesis
                                If foundNumbers OrElse foundDot Then
                                    Return False
                                End If
                            Case SyntaxKind.RightParenthesis
                                If Not foundNumbers Then
                                    Return False
                                End If
                            Case SyntaxKind.Slash
                                Throw New InvalidOperationException("Bad NumberKind.")
                            Case SyntaxKind.Exponent
                                Throw New InvalidOperationException("Bad NumberKind.")
                            Case SyntaxKind.Space
                                If foundNumbers Then
                                    Return False
                                End If
                            Case SyntaxKind.Percentage
                                multipler /= 100
                            Case Else
                                Throw New InvalidOperationException("Bad syntax tree.")
                        End Select
                    Next
                Case NumberKind.Scientific
                    Dim isEMinus As Boolean = False
                    If syntax.Children.Count < 3 Then
                        Return False
                    End If
                    Dim index As Integer = 0
                    If syntax.Children.UnsafeItem(index).Kind = SyntaxKind.Plus OrElse syntax.Children.UnsafeItem(index).Kind = SyntaxKind.Negation Then
                        If syntax.Children.UnsafeItem(index).Kind = SyntaxKind.Negation Then
                            isNegative = True
                        End If
                        index += 1
                    ElseIf syntax.Children.UnsafeItem(index).Kind = SyntaxKind.LeftParenthesis Then
                        isNegative = True
                        index += 1
                    End If
                    If syntax.Children.UnsafeItem(index).Kind <> SyntaxKind.Digits Then
                        Return False
                    End If
                    If syntax.HasComma Then
                        ParseCurrencyInteger(number, syntax.Children.UnsafeItem(index).Range, syntax.Text)
                    Else
                        ParseInteger(number, syntax.Children.UnsafeItem(index).Range, syntax.Text)
                    End If
                    index += 1
                    While syntax.Children.UnsafeItem(index).Kind = SyntaxKind.Comma
                        index += 1
                        If index >= syntax.Children.Count Then
                            Return False
                        End If
                        If syntax.Children.UnsafeItem(index).Kind = SyntaxKind.Digits Then
                            ParseCurrencyInteger(number, syntax.Children.UnsafeItem(index).Range, syntax.Text)
                        End If
                        index += 1
                        If index >= syntax.Children.Count Then
                            Return False
                        End If
                    End While
                    If syntax.Children.UnsafeItem(index).Kind = SyntaxKind.Dot Then
                        index += 1
                        If index >= syntax.Children.Count Then
                            Return False
                        End If
                        If syntax.Children.UnsafeItem(index).Kind = SyntaxKind.Digits Then
                            ParseDecimal(number, syntax.Children.UnsafeItem(index).Range, syntax.Text)
                            index += 1
                            If index >= syntax.Children.Count Then
                                Return False
                            End If
                        End If
                    End If
                    If syntax.Children.UnsafeItem(index).Kind <> SyntaxKind.Exponent Then
                        Return False
                    End If
                    index += 1
                    If index >= syntax.Children.Count Then
                        Return False
                    End If
                    If syntax.Children.UnsafeItem(index).Kind = SyntaxKind.Plus OrElse syntax.Children.UnsafeItem(index).Kind = SyntaxKind.Negation Then
                        If syntax.Children.UnsafeItem(index).Kind = SyntaxKind.Negation Then
                            isEMinus = True
                        End If
                        index += 1
                        If index >= syntax.Children.Count Then
                            Return False
                        End If
                    End If
                    If syntax.Children.UnsafeItem(index).Kind <> SyntaxKind.Digits Then
                        Return False
                    End If
                    Dim exp As Integer = 0
                    If Not TryParseExponent(exp, syntax.Children.UnsafeItem(index).Range, syntax.Text, isEMinus) Then
                        Return False
                    End If
                    index += 1
                    If index = syntax.Children.Count Then
                    ElseIf index + 1 = syntax.Children.Count Then
                        If syntax.Children.UnsafeItem(index).Kind <> SyntaxKind.RightParenthesis Then
                            Return False
                        End If
                    Else
                        Return False
                    End If
                    If isEMinus Then
                        exp = -exp
                    End If
                    multipler = Math.Pow(10, exp)
                Case NumberKind.Fraction
                    Return False
            End Select
            If isNegative Then
                multipler = -multipler
            End If
            result = number * multipler
            Return True
        End Function

        Private Sub ParseDecimal(ByRef number As Double, range As Range, text As String)
            Dim dValue As Double = 0.0
            Dim endIndex As Integer = range.Length + range.Start
            For i = endIndex - 1 To range.Start Step -1
                Dim ch As Char = text(i)
                Dim cur As Integer = AscW(ch) - ZeroChar
                dValue /= 10
                dValue += cur / 10
            Next
            number += dValue
        End Sub

        Private Sub ParseInteger(ByRef number As Double, range As Range, text As String)
            Dim endIndex As Integer = range.Length + range.Start
            If range.Length <= 8 Then
                Dim int32Value As Integer = 0
                For i = range.Start To endIndex - 1
                    Dim ch As Char = text(i)
                    Dim cur As Integer = AscW(ch) - ZeroChar
                    int32Value *= 10
                    int32Value += cur
                Next
                number += int32Value
            ElseIf range.Length <= 18 Then
                Dim int64Value As Long = 0
                For i = range.Start To endIndex - 1
                    Dim ch As Char = text(i)
                    Dim cur As Integer = AscW(ch) - ZeroChar
                    int64Value *= 10
                    int64Value += cur
                Next
                number += int64Value
            Else
                Dim dValue As Double = 0
                For i = range.Start To endIndex - 1
                    Dim ch As Char = text(i)
                    Dim cur As Integer = AscW(ch) - ZeroChar
                    dValue *= 10
                    dValue += cur
                Next
                number += dValue
            End If
        End Sub

        Private Sub ParseCurrencyInteger(ByRef number As Double, range As Range, text As String)
            Dim endIndex As Integer = range.Length + range.Start
            For i = range.Start To endIndex - 1
                Dim ch As Char = text(i)
                Dim cur As Integer = AscW(ch) - ZeroChar
                number *= 10
                number += cur
            Next
        End Sub

        Private Function TryParseExponent(ByRef number As Integer, range As Range, text As String, isEMinus As Boolean) As Boolean
            If range.Length > 3 Then
                Return False
            End If
            Dim value As Integer = 0
            Dim endIndex As Integer = range.Length + range.Start
            For i = range.Start To endIndex - 1
                Dim ch As Char = text(i)
                Dim cur As Integer = AscW(ch) - ZeroChar
                value *= 10
                value += cur
            Next
            If value > (If(isEMinus, 309, 308)) OrElse value < 0 Then
                Return False
            End If
            number = value
            Return True
        End Function
    End Class
End Namespace