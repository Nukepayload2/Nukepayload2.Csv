Imports System.Runtime.CompilerServices

Friend Module CsvLineSplitter
    Private Const QuoteChar = """"c
    Private Const DoubleQuote As String = """"""
    Private Const SingleQuote As String = """"

    <Extension>
    Public Function SplitLines(text As String, newLine As String) As List(Of String)
        Debug.Assert(text <> Nothing, "Text can't be empty.")
        Debug.Assert(newLine <> Nothing, "Separator can't be empty.")

        Dim result As New List(Of String)
        Dim inQuote = False
        Dim startIndex = 0
        Dim length = 0
        Dim newLineStart = newLine(0)
        Dim i = 0
        Do While i < text.Length
            Dim ch = text(i)
            If ch = QuoteChar Then
                inQuote = Not inQuote
                length += 1
                i += 1
            ElseIf ch = newLineStart Then
                If IsSeparator(text, newLine, i) Then
                    If inQuote Then
                        length += 1
                        i += 1
                    Else
                        If length > 0 Then
                            result.Add(text.Substring(startIndex, length))
                        End If
                        startIndex = i + newLine.Length
                        i += newLine.Length
                        length = 0
                    End If
                Else
                    ' i was increased by IsSeparator
                End If
            Else
                length += 1
                i += 1
            End If
        Loop
        If length > 0 Then
            result.Add(text.Substring(startIndex, length))
        End If
        Return result
    End Function

    <Extension>
    Public Sub SplitElementsInto(text As String, separator As String, result As String(), options As StringSplitOptions)
        Debug.Assert(text <> Nothing, "Text can't be empty.")
        Debug.Assert(separator <> Nothing, "Separator can't be empty.")
        Debug.Assert(result IsNot Nothing AndAlso result.Length > 0, "Result array can't be empty.")

        Dim status = CsvLineSplitStatus.Unknown
        Dim separatorStart = separator(0)
        Dim resultIndex = 0
        Dim i = 0
        Dim recordStart = 0
        While i < text.Length
            Select Case status
                Case CsvLineSplitStatus.Unknown
                    Dim curChr = text(i)
                    Select Case curChr
                        Case separatorStart
                            If separator.Length + i <= text.Length Then
                                If IsSeparator(text, separator, i) Then
                                    status = CsvLineSplitStatus.Separator
                                Else
                                    status = CsvLineSplitStatus.RecordWithoutQuote
                                End If
                            Else
                                i = text.Length
                                Exit While
                            End If
                        Case QuoteChar
                            status = CsvLineSplitStatus.RecordWithQuote
                        Case Else
                            status = CsvLineSplitStatus.RecordWithoutQuote
                    End Select
                Case CsvLineSplitStatus.RecordWithoutQuote
                    ' Record ends with the separator or EOF.
                    Do While i < text.Length
                        Dim curChr = text(i)
                        If curChr = separatorStart Then
                            If separator.Length + i <= text.Length Then
                                If IsSeparator(text, separator, i) Then
                                    result(resultIndex) = text.Substring(recordStart, i - recordStart)
                                    status = CsvLineSplitStatus.Separator
                                    Continue While
                                End If
                            Else
                                i = text.Length
                                Exit Do
                            End If
                        Else
                            i += 1
                        End If
                    Loop
                Case CsvLineSplitStatus.RecordWithQuote
                    ' Separators will be ignored.
                    ' Record ends with a single quote or EOF.
                    ' Quotes will be escaped with the rule of Visual Basic.
                    i += 1
                    recordStart += 1
                    Dim escapeSuspected = False
                    Do While i < text.Length
                        Dim curChr = text(i)
                        If curChr = QuoteChar Then
                            escapeSuspected = Not escapeSuspected
                        ElseIf escapeSuspected Then
                            If curChr = separatorStart Then
                                result(resultIndex) = text.Substring(recordStart, i - 1 - recordStart).Replace(DoubleQuote, SingleQuote)
                                status = CsvLineSplitStatus.Separator
                                Continue While
                            Else
                                Throw New FormatException("Invalid character after quote.")
                            End If
                        End If
                        i += 1
                    Loop
                    If escapeSuspected Then
                        result(resultIndex) = text.Substring(recordStart, i - 1 - recordStart).Replace(DoubleQuote, SingleQuote)
                        recordStart = i
                        Exit While
                    Else
                        Throw New FormatException("Missing the quote character at the end of line.")
                    End If
                Case CsvLineSplitStatus.Separator
                    If options = StringSplitOptions.None Then
                        If result(resultIndex) Is Nothing Then
                            result(resultIndex) = String.Empty
                        End If
                        resultIndex += 1
                    ElseIf Not String.IsNullOrEmpty(result(resultIndex)) Then
                        resultIndex += 1
                    End If
                    i += separator.Length
                    recordStart = i
                    status = CsvLineSplitStatus.Unknown
            End Select
        End While
        If recordStart < i Then
            result(resultIndex) = text.Substring(recordStart)
        End If
        If options = StringSplitOptions.None Then
            For j = resultIndex To result.Length - 1
                If result(j) Is Nothing Then
                    result(j) = String.Empty
                End If
            Next
        End If
    End Sub

    Private Function IsSeparator(text As String, separator As String, ByRef i As Integer) As Boolean
        For j = 1 To separator.Length - 1
            Dim ch = text(i + j)
            Dim sp = separator(j)
            If ch <> sp Then
                i += j
                Return False
            End If
        Next
        Return True
    End Function
End Module

Friend Enum CsvLineSplitStatus
    Unknown
    RecordWithoutQuote
    RecordWithQuote
    Separator
End Enum