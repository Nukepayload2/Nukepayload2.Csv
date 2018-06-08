''' <summary>
''' Manages formatter instances for each type of objects.
''' </summary>
Public Interface ICsvRecordFormatterCache
    ''' <summary>
    ''' Gets formatter for the specified type.
    ''' </summary>
    ''' <param name="tp">The type which will be formatted</param>
    ''' <returns>The formatter for the specified type.</returns>
    Function GetFormatter(tp As Type) As ICsvRecordFormatter
End Interface
