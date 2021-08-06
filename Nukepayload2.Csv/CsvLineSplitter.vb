Imports System.Runtime.CompilerServices

Friend Module CsvLineSplitter
    Private Const QuoteChar As Char = """"c
    Private ReadOnly DoubleQuote As String = """"""
    Private ReadOnly SingleQuote As String = """"

    <ThreadStatic>
    Private t_result As New List(Of StringSegment)

    <Extension>
    Public Function SplitLines(text As String, newLine As String) As List(Of StringSegment)
        Debug.Assert(text <> Nothing, "Text can't be empty.")
        Debug.Assert(newLine <> Nothing, "Separator can't be empty.")

        If t_result Is Nothing Then
            t_result = New List(Of StringSegment)
        Else
            t_result.Clear()
        End If
        Dim result = t_result

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
            ElseIf ch = newLineStart AndAlso Not inQuote Then
                If newLine.Length + i <= text.Length Then
                    If IsSeparator(text, i, newLine) Then
                        If length > 0 Then
                            result.Add(StringSegment.Slice(text, startIndex, length))
                        Else
                            result.Add(Nothing)
                        End If
                        startIndex = i + newLine.Length
                        i += newLine.Length
                        length = 0
                    Else
                        length += 1
                        i += 1
                    End If
                Else
                    length += 1
                    Exit Do
                End If
            Else
                length += 1
                i += 1
            End If
        Loop
        If length > 0 Then
            result.Add(StringSegment.Slice(text, startIndex, length))
        End If
        Return result
    End Function

    <Extension>
    Public Sub SplitElementsInto(text As String, separator As String, result As StringSegment(), options As StringSplitOptions)
        SplitElementsInto(CType(text, StringSegment), separator, result, options)
    End Sub

    <Extension>
    Public Sub SplitElementsInto(text As StringSegment, separator As String, result As StringSegment(), options As StringSplitOptions)
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
                    Dim curChr = text.Item(i)
                    Select Case curChr
                        Case separatorStart
                            If separator.Length + i <= text.Length Then
                                If IsSeparator(text, i, separator) Then
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
                                If IsSeparator(text, i, separator) Then
                                    result(resultIndex) = text.Slice(recordStart, i - recordStart)
                                    status = CsvLineSplitStatus.Separator
                                    Continue While
                                Else
                                    i += 1
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
                                result(resultIndex) = CType(text.Slice(recordStart, i - 1 - recordStart).CopyToString.Replace(DoubleQuote, SingleQuote), StringSegment)
                                status = CsvLineSplitStatus.Separator
                                Continue While
                            Else
                                Throw New FormatException("Invalid character after quote.")
                            End If
                        End If
                        i += 1
                    Loop
                    If escapeSuspected Then
                        result(resultIndex) = CType(text.Slice(recordStart, i - 1 - recordStart).CopyToString.Replace(DoubleQuote, SingleQuote), StringSegment)
                        recordStart = i
                        Exit While
                    Else
                        Throw New FormatException("Missing the quote character at the end of line.")
                    End If
                Case CsvLineSplitStatus.Separator
                    If options = StringSplitOptions.None Then
                        If result(resultIndex).IsNull Then
                            result(resultIndex) = StringSegment.Empty
                        End If
                        resultIndex += 1
                    ElseIf Not result(resultIndex).IsNullOrEmpty Then
                        resultIndex += 1
                    End If
                    i += separator.Length
                    recordStart = i
                    status = CsvLineSplitStatus.Unknown
            End Select
        End While
        If recordStart < i Then
            result(resultIndex) = text.Slice(recordStart)
        End If
        If options = StringSplitOptions.None Then
            For j = resultIndex To result.Length - 1
                If result(j).IsNull Then
                    result(j) = StringSegment.Empty
                End If
            Next
        End If
    End Sub

    Private Function IsSeparator(text As String, startIndex As Integer, separator As String) As Boolean
        For j = 1 To separator.Length - 1
            Dim ch = text(startIndex + j)
            Dim sp = separator(j)
            If ch <> sp Then
                Return False
            End If
        Next
        Return True
    End Function

    Private Function IsSeparator(text As StringSegment, startIndex As Integer, separator As String) As Boolean
        For j = 1 To separator.Length - 1
            Dim ch = text(startIndex + j)
            Dim sp = separator(j)
            If ch <> sp Then
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