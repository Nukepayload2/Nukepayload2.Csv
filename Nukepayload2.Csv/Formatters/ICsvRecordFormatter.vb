''' <summary>
''' [Preview] Formats or parses specific field of each object.
''' This API is in preview. It will be changed in future releases.
''' </summary>
Public Interface ICsvRecordFormatter
    ''' <summary>
    ''' Formats the specified object to string.
    ''' </summary>
    ''' <param name="data">Data to be converted to string</param>
    ''' <param name="format">Format string</param>
    Function GetString(data As Object, format As String) As String
    ''' <summary>
    ''' Converts the given text to object.
    ''' </summary>
    ''' <param name="text">The string which to be converted to object.</param>
    Function Parse(text As StringSegment) As Object
End Interface
