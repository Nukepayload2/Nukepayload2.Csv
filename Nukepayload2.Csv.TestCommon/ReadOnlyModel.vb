Public Class ReadOnlyModel
    Public Sub New(id As Integer, content As String, isChecked As Boolean)
        Me.Id = id
        Me.Content = content
        Me.IsChecked = isChecked
    End Sub

    Public ReadOnly Property Id As Integer
    Public ReadOnly Property Content As String
    Public ReadOnly Property IsChecked As Boolean
End Class