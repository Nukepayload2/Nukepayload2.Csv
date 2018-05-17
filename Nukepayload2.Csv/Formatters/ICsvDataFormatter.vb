Imports System.Text

Friend Interface ICsvDataFormatter
    Sub WriteTo(data As Object, format As String, sb As StringBuilder)
    Function GetString(data As Object, format As String) As String
    Function Parse(text As String) As Object
End Interface
