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
                       Optional separator As String = ",")
        Console.WriteLine("Csv model class file generator")
        Console.WriteLine("Copyright 2018 Nukepayload2")
        Console.WriteLine("For more information, see https://github.com/Nukepayload2/Nukepayload2.Csv")
        If Not File.Exists(csvFilePath) Then
            Console.WriteLine("Csv file does not exist.")
            Return
        End If
        Dim csv = File.ReadAllText(csvFilePath, System.Text.Encoding.GetEncoding(encoding)).Replace(vbCr, "").Split(vbLf, StringSplitOptions.RemoveEmptyEntries)
        If csv.Length < 2 Then
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
        Dim codeText = WriteCode(code, csv, separator)
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

    Private Function WriteCode(code As ICodeGenerator, csv() As String, separator As String) As String
        Dim firstLine = csv(0)
        Dim headers = firstLine.Split({separator}, StringSplitOptions.RemoveEmptyEntries)
        Dim sb As New IndentStringBuilder
        sb.IndentAppendLines(code.WriteClass("Model")).IncreaseIndent()
        Dim inferredTypes = InferTypes(csv, separator, headers.Length)
        For i = 0 To headers.Length - 1
            Dim h = headers(i)
            sb.IndentAppendLine(code.WriteAttribute(GetType(ColumnFormatAttribute), $"""{h}"""))
            sb.IndentAppendLine(code.WriteProperty(RenameForClrName(h), inferredTypes(i)))
        Next
        sb.DecreaseIndent.IndentAppendLine(code.WriteEndClass()).IncreaseIndent()
        Return sb.ToString
    End Function

    Private Function InferTypes(csv() As String, separator As String, resultLength As Integer) As Type()
        ' decision:
        ' Integer - Double - DateTime - Boolean - String
        Dim defaultDecision As Type = GetType(String)
        Dim selectors = {(Decision:=GetType(Integer), Decide:=Function(str$) str.IsInteger),
            (Decision:=GetType(Double), Decide:=Function(str$) str.IsFraction),
            (Decision:=GetType(Date), Decide:=Function(str$) str.IsDateTime),
            (Decision:=GetType(Boolean), Decide:=Function(str$) str.IsBoolean)
        }
        Dim types(resultLength - 1) As Type
        Dim groups(resultLength - 1, csv.Length - 2) As String
        For i = 1 To csv.Length - 1
            Dim ln = csv(i)
            Dim rec = ln.Split(separator)
            For j = 0 To resultLength - 1
                groups(j, i - 1) = rec(j)
            Next
        Next
        For typeIndex = 0 To resultLength - 1
            Dim selectorIndex = 0
            Do While selectorIndex < selectors.Length
                Dim curSelector = selectors(selectorIndex)
                For i = 0 To csv.Length - 2
                    If Not curSelector.Decide(groups(typeIndex, selectorIndex)) Then
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
