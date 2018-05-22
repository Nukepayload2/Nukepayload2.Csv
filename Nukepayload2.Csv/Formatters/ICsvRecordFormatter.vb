Imports System.Text

Public Interface ICsvRecordFormatter
    Sub WriteTo(data As Object, format As String, sb As StringBuilder)
    Function GetString(data As Object, format As String) As String
    Function Parse(text As String) As Object
End Interface
