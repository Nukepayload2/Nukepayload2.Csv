Imports System.Text

''' <summary>
''' [Preview] Formats or parses specific field of each object.
''' This API is in preview. It will be changed in future releases.
''' </summary>
Public Interface ICsvRecordFormatter
    ''' <summary>
    ''' Writes data into <see cref="StringBuilder"/> with the specified format string.
    ''' </summary>
    ''' <param name="data">Data to write</param>
    ''' <param name="format">Format string to be applied</param>
    ''' <param name="sb">Write to this string builder</param>
    Sub WriteTo(data As Object, format As String, sb As StringBuilder)
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
    Function Parse(text As String) As Object
    ''' <summary>
    ''' A shortcut for blittable primitive types. To avoid box and unbox when parsing.
    ''' Returns whether the converter supports primitive type.
    ''' </summary>
    ''' <param name="text">The string which to be converted to object.</param>
    ''' <param name="primitive">The result of parsed primitives</param>
    ''' <returns>Whether the converter supports primitive type.</returns>
    Function ParseBlittablePrimitive(text As String, ByRef primitive As CsvBlittablePrimitive) As Boolean
End Interface
