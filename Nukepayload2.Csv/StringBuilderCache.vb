Imports System.Text

Friend Class StringBuilderCache
    ' StringBuilder should be pooled for each thread.
    <ThreadStatic>
    Private Shared t_stringBuilder As StringBuilder

    Public Shared ReadOnly Property Instance As StringBuilder
        Get
            If t_stringBuilder Is Nothing Then
                t_stringBuilder = New StringBuilder
            Else
                t_stringBuilder.Clear()
            End If
            Return t_stringBuilder
        End Get
    End Property

End Class
