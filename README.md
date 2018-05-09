# Nukepayload2.Csv
Nukepayload2.Csv is a cross-platform Csv String &lt;==> .NET Object converter.

## Sample Code
### Convert Csv string to .NET Object
Goal: convert the following csv string to .NET Object.
```csv
Id,Rate,Name,Date,IsUsed
-233,12345.12,test string,2018-05-09,True
0,-12345.12,,2018-09-09,False
```
#### 1. Create a model class

__VB__
```vb
Public Class MyCsvModel
    Public Property Id As Integer
    Public Property Rate As Double
    Public Property Name As String
    Public Property [Date] As Date
    Public Property IsUsed As Boolean
End Class
```

__C#__
```csharp
public class MyCsvModel
{
    public int Id { get; set; }
    public double Rate { get; set; }
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public bool IsUsed { get; set; }
}
```
#### 2. Convert csv to `IReadOnlyList` of `MyCsvModel`
Assume that `csv` is a `System.String` that contains csv.

__VB__
```vb
Dim converted = CsvConvert.DeserializeObject(Of MyCsvModel)(csv)
```

__C#__
```csharp
var converted = CsvConvert.DeserializeObject<MyCsvModel>(csv);
```

### Convert .NET Object back to Csv string

__VB__
```vb
csv = CsvConvert.SerializeObject(converted)
```

__C#__
```csharp
csv = CsvConvert.SerializeObject(converted);
```

## What is next?
- VB and C# model class generator will be added.
