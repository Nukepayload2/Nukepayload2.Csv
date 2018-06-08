Imports System.Runtime.InteropServices
''' <summary>
''' A union type that is used to prevent box and unbox when dealing with numbers.
''' </summary>
<StructLayout(LayoutKind.Explicit)>
Public Structure CsvBlittablePrimitive
    <FieldOffset(0)>
    Public Int32Value As Integer
    <FieldOffset(0)>
    Public Int64Value As Long
    <FieldOffset(0)>
    Public SingleValue As Single
    <FieldOffset(0)>
    Public DoubleValue As Double
End Structure
