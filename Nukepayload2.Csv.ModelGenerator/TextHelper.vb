Imports System.Runtime.CompilerServices

Module TextHelper
    <Extension>
    Function IsDateTime(txt As String) As Boolean
        If txt = Nothing Then
            Return False
        End If
        If Not Char.IsDigit(txt(0)) Then
            Return False
        End If
        Return Date.TryParse(txt, Nothing)
    End Function
    <Extension>
    Function IsInteger(txt As String) As Boolean
        If txt = Nothing Then
            Return False
        End If
        If txt.StartsWith("-") OrElse txt.StartsWith("+") Then txt = txt.Substring(1)
        If txt.Length = 0 Then Return False
        Return txt.Length = Aggregate c In From t In txt.ToCharArray Take While t >= "0"c AndAlso t <= "9"c Into Count
    End Function
    <Extension>
    Function IsBoolean(txt As String) As Boolean
        If txt = Nothing Then
            Return False
        End If
        Return Boolean.TryParse(txt, Nothing)
    End Function
    <Extension>
    Function IsFraction(txt As String) As Boolean
        If txt = Nothing Then
            Return False
        End If
        If txt.StartsWith("-") OrElse txt.StartsWith("+") Then txt = txt.Substring(1)
        If txt.CountOf("."c) <> 1 Then Return False
        Return txt.Length = Aggregate c In From t In txt.ToCharArray Take While t >= "0"c AndAlso t <= "9"c OrElse t = "." Into Count
    End Function
    <Extension>
    Function CountOf(txt As String, chr As Char) As Integer
        Return Aggregate co In From ch In txt.ToCharArray Where ch = chr Into Count
    End Function
End Module
