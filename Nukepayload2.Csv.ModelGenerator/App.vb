Option Compare Text

Imports System.ComponentModel.DataAnnotations
Imports System.IO
Imports Nukepayload2.CodeAnalysis
Imports Nukepayload2.ConsoleFramework

Public Class App
    Dim lang As SupportedLanguages

    <EntryMethod>
    Public Sub StartUp(<Display(ShortName:="p", Description:="The path of csv file.")>
                       csvFilePath As String,
                       <Display(ShortName:="o", Description:="The output file path. If it ends with .vb or .vbx, a Visual Basic source file will be generated. If it ends with .cs or .csx, a C# source file will be generated.")>
                       output As String,
                       <Display(ShortName:="e", Description:="The encoding of csv file. The default value is UTF-8.")>
                       Optional encoding As String = "UTF-8",
                       <Display(ShortName:="s", Description:="The separator in csv file. The default value is ','. Escape rules: {Space} for space, {Tab} for tab.")>
                       Optional separator As String = ",",
                       <Display(ShortName:="n", Description:="The new line characters in csv file. The default value is Environment.NewLine. Escape rules: {Cr} for Mac default new line, {Lf} for Linux default new line, {CrLf} for Windows default new line.")>
            Optional newLine As String = Nothing)
        Console.WriteLine("Csv model class file generator")
        Console.WriteLine("Copyright 2018 Nukepayload2")
        Console.WriteLine("For more information, see https://github.com/Nukepayload2/Nukepayload2.Csv")
        If Not File.Exists(csvFilePath) Then
            Console.WriteLine("Csv file does not exist.")
            Return
        End If
        Dim enc As Text.Encoding
        Try
            enc = System.Text.Encoding.GetEncoding(encoding)
        Catch ex As Exception
            enc = System.Text.CodePagesEncodingProvider.Instance.GetEncoding(encoding)
        End Try
        If String.IsNullOrEmpty(newLine) Then
            newLine = Environment.NewLine
        Else
            newLine = newLine.Replace("{CrLf}", vbCrLf).Replace("{Cr}", vbCr).Replace("{Lf}", vbLf)
        End If
        Dim sourceText As String = File.ReadAllText(csvFilePath, enc)
        Dim csv = CsvLineSplitter.SplitLines(sourceText, newLine).ToArray
        If csv.Count < 2 Then
            Console.WriteLine("Csv file is broken or encoding is not correct.")
            Return
        End If
        Dim ext = Path.GetExtension(output)
        Select Case ext
            Case ".vb", ".vbx"
                lang = SupportedLanguages.VisualBasic
            Case ".cs", ".csx"
                lang = SupportedLanguages.CSharp
            Case Else
                Console.WriteLine("Unable to infer output language by file extension.")
                Return
        End Select
        Dim code = CodeGenerator.Create(lang)
        separator = Escape(separator)
        Dim className = Path.GetFileNameWithoutExtension(output)
        Dim codeText = WriteCode(code, csv, separator, className)
        File.WriteAllText(output, codeText)
        Console.WriteLine("Class generated.")
    End Sub

    Private Function Escape(separator As String) As String
        Return separator.Replace("{Tab}", vbTab).Replace("{Space}", " ")
    End Function

    Private Shared ReadOnly _keywords As New HashSet(Of String)(From k In VBKeyWordTranslator.KeywordTable.Keys Select k.ToLowerInvariant)

    Private Function RenameForClrName(name As String) As String
        If Not String.IsNullOrEmpty(name) Then
            If Not Char.IsLetter(name(0)) Then
                name = "_" + name
            Else
                Dim newName = name.ToCharArray
                newName(0) = Char.ToUpperInvariant(newName(0))
                For i = 1 To newName.Length - 1
                    newName(i) = Char.ToLowerInvariant(newName(i))
                Next
                name = New String(newName)
            End If
            name = name.Replace("-"c, "_"c).Replace(" "c, "_").Replace(","c, "_").Replace("$"c, "_").Replace("#"c, "_")
        End If
        If _keywords.Contains(name.ToLowerInvariant) Then
            If lang = SupportedLanguages.VisualBasic Then
                name = "[" + name + "]"
            Else
                name = "@" + name
            End If
        End If
        Return name
    End Function

    Private Function WriteCode(code As ICodeGenerator, csv As IReadOnlyList(Of StringSegment), separator As String, className As String) As String
        Dim firstLine = csv(0)
        Dim headers = CsvLineSplitter.SplitLines(firstLine, separator).ToArray
        Dim sb As New IndentStringBuilder
        sb.IndentAppendLine(code.WriteImport("Nukepayload2.Csv")).AppendLine()
        sb.IndentAppendLines(code.WriteClass(className)).IncreaseIndent()
        Dim inferredTypes = InferTypes(csv, separator, headers.Count)
        For i = 0 To headers.Count - 1
            Dim h = headers(i)
            sb.IndentAppendLine(code.WriteAttribute(GetType(ColumnFormatAttribute), $"""{h}"""))
            sb.IndentAppendLine(code.WriteProperty(RenameForClrName(h), inferredTypes(i)))
        Next
        sb.DecreaseIndent.IndentAppendLine(code.WriteEndClass()).IncreaseIndent()
        Return sb.ToString
    End Function

    Private Function InferTypes(csv As IReadOnlyList(Of StringSegment), separator As String, resultLength As Integer) As Type()
        ' decision:
        ' Integer - Long - Single - Double - DateTime - Boolean - String
        Dim defaultDecision As Type = GetType(String)
        Dim selectors = {(Decision:=GetType(Integer), Decide:=Function(str$) str.IsInteger AndAlso Integer.TryParse(str, Nothing)),
            (Decision:=GetType(Long), Decide:=Function(str$) str.IsInteger AndAlso Long.TryParse(str, Nothing)),
            (Decision:=GetType(Single), Decide:=Function(str$) str.IsFraction AndAlso Single.TryParse(str, Nothing)),
            (Decision:=GetType(Double), Decide:=Function(str$) str.IsFraction AndAlso Double.TryParse(str, Nothing)),
            (Decision:=GetType(Date), Decide:=Function(str$) str.IsDateTime),
            (Decision:=GetType(Boolean), Decide:=Function(str$) str.IsBoolean)
        }
        Dim types(resultLength - 1) As Type
        Dim groups(resultLength - 1, csv.Count - 2) As String
        For i = 1 To csv.Count - 1
            Dim ln = csv(i)
            Dim rec = CsvLineSplitter.SplitLines(ln, separator).ToArray
            For j = 0 To rec.Length - 1
                groups(j, i - 1) = rec(j)
            Next
        Next
        For typeIndex = 0 To resultLength - 1
            Dim selectorIndex = 0
            Do While selectorIndex < selectors.Length
                Dim curSelector = selectors(selectorIndex)
                For i = 0 To csv.Count - 2
                    If Not curSelector.Decide(groups(typeIndex, i)) Then
                        selectorIndex += 1
                        Continue Do
                    End If
                Next
                types(typeIndex) = curSelector.Decision
                Continue For
            Loop
            types(typeIndex) = defaultDecision
        Next
        Return types
    End Function
End Class
