Namespace Numbers

    Friend Structure Range
        Public Start As Integer
        Public Length As Integer

        Public ReadOnly Property NextIndex As Integer
            Get
                Return Start + Length
            End Get
        End Property

        Public ReadOnly Property EndIndex As Integer
            Get
                Return Start + Length - 1
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{Start} To {EndIndex}"
        End Function
    End Structure
End Namespace