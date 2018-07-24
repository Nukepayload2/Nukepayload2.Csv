Namespace Numbers

    Friend Structure Range
        Public Start As Integer
        Public Length As Integer

        Public Overrides Function ToString() As String
            Return $"{Start} To {Start + Length - 1}"
        End Function
    End Structure
End Namespace