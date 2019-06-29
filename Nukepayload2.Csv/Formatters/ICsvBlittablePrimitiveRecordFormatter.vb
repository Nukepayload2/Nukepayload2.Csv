Imports System.Text

''' <summary>
''' [Preview] Formats or parses specific field of blittable type of each object.
''' This API is in preview. It will be changed in future releases.
''' </summary>
Public Interface ICsvBlittablePrimitiveRecordFormatter
    Inherits ICsvRecordFormatter
    ''' <summary>
    ''' A shortcut for blittable primitive types. To avoid box and unbox when formatting.
    ''' Formats the specified object to string.
    ''' </summary>
    ''' <param name="data">Data to be converted to string</param>
    ''' <param name="format">Format string</param>
    Function TryWriteStringForBlittablePrimitive(data As CsvBlittablePrimitive, format As String, output As StringBuilder) As Boolean
    ''' <summary>
    ''' A shortcut for blittable primitive types. To avoid box and unbox when parsing.
    ''' Returns whether the converter supports primitive type.
    ''' </summary>
    ''' <param name="text">The string which to be converted to object.</param>
    ''' <param name="primitive">The result of parsed primitives</param>
    ''' <returns>Whether the converter supports primitive type.</returns>
    Function TryParseBlittablePrimitive(text As StringSegment, ByRef primitive As CsvBlittablePrimitive) As Boolean
End Interface
