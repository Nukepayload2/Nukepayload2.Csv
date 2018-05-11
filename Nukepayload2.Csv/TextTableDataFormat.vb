﻿Imports System.Runtime.CompilerServices
Imports System.Text
Imports Nukepayload2.Csv

Friend Interface ITextTableDataFormatter
    Sub WriteTo(data As Object, format As String, sb As StringBuilder)
    Function GetString(data As Object, format As String) As String
    Function Parse(text As String) As Object
End Interface

Friend Class TextTableTextFormatter
    Inherits Singleton(Of TextTableTextFormatter)
    Implements ITextTableDataFormatter

    Public Sub WriteTo(data As Object, format As String, sb As StringBuilder) Implements ITextTableDataFormatter.WriteTo
        sb.Append(DirectCast(data, String))
    End Sub

    Public Function Parse(text As String) As Object Implements ITextTableDataFormatter.Parse
        Return text
    End Function

    Public Function GetString(data As Object, format As String) As String Implements ITextTableDataFormatter.GetString
        Return DirectCast(data, String)
    End Function
End Class

Friend Class TextTableDoubleFormatter
    Inherits Singleton(Of TextTableDoubleFormatter)
    Implements ITextTableDataFormatter

    Public Sub WriteTo(data As Object, format As String, sb As StringBuilder) Implements ITextTableDataFormatter.WriteTo
        sb.Append(DirectCast(data, Double).ToString(format))
    End Sub

    Public Function Parse(text As String) As Object Implements ITextTableDataFormatter.Parse
        Return Double.Parse(text)
    End Function

    Public Function GetString(data As Object, format As String) As String Implements ITextTableDataFormatter.GetString
        Return DirectCast(data, Double).ToString(format)
    End Function
End Class

Friend Class TextTableIntegerFormatter
    Inherits Singleton(Of TextTableIntegerFormatter)
    Implements ITextTableDataFormatter

    Public Sub WriteTo(data As Object, format As String, sb As StringBuilder) Implements ITextTableDataFormatter.WriteTo
        sb.Append(DirectCast(data, Integer).ToString(format))
    End Sub

    Public Function Parse(text As String) As Object Implements ITextTableDataFormatter.Parse
        Return Integer.Parse(text)
    End Function

    Public Function GetString(data As Object, format As String) As String Implements ITextTableDataFormatter.GetString
        Return DirectCast(data, Integer).ToString(format)
    End Function
End Class

Friend Class TextTableBooleanFormatter
    Inherits Singleton(Of TextTableBooleanFormatter)
    Implements ITextTableDataFormatter

    Public Sub WriteTo(data As Object, format As String, sb As StringBuilder) Implements ITextTableDataFormatter.WriteTo
        sb.Append(DirectCast(data, Boolean))
    End Sub

    Public Function Parse(text As String) As Object Implements ITextTableDataFormatter.Parse
        Return Boolean.Parse(text)
    End Function

    Public Function GetString(data As Object, format As String) As String Implements ITextTableDataFormatter.GetString
        Return DirectCast(data, Boolean).ToString
    End Function
End Class

Friend Class TextTableDateFormatter
    Inherits Singleton(Of TextTableDateFormatter)
    Implements ITextTableDataFormatter

    Public Sub WriteTo(data As Object, format As String, sb As StringBuilder) Implements ITextTableDataFormatter.WriteTo
        sb.Append(DirectCast(data, Date).ToString(format))
    End Sub

    Public Function Parse(text As String) As Object Implements ITextTableDataFormatter.Parse
        Return Date.Parse(text)
    End Function

    Public Function GetString(data As Object, format As String) As String Implements ITextTableDataFormatter.GetString
        Return DirectCast(data, Date).ToString(format)
    End Function
End Class

Module TextTableDataFormatExtensions
    <Extension>
    Function GetFormatter(tp As Type) As ITextTableDataFormatter
        Select Case tp
            Case GetType(Date)
                Return TextTableDateFormatter.Instance
            Case GetType(Double)
                Return TextTableDoubleFormatter.Instance
            Case GetType(Integer)
                Return TextTableIntegerFormatter.Instance
            Case GetType(String)
                Return TextTableTextFormatter.Instance
            Case GetType(Boolean)
                Return TextTableBooleanFormatter.Instance
            Case Else
                Throw New NotSupportedException($"A property type in the given model class is not supported. {tp.FullName} is not supported.")
        End Select
    End Function
End Module