# Nukepayload2.Csv
Nukepayload2.Csv is a cross-platform Csv String &lt;==&gt; .NET Object converter.

Supported types in csv columns:
- System.String
- System.DateTime
- System.Int32
- System.Int64
- System.Single
- System.Double

__Get on Nuget__

https://www.nuget.org/packages/Nukepayload2.Csv

如果你需要中文介绍，请向下滚动页面。

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
- [Testing] VB and C# model class generator will be added.
- [Pending] Performance improvements (waiting for `ref struct` support in VB) https://github.com/dotnet/vblang/issues/297 .

# Nukepayload2.Csv 中文介绍

Nukepayload2.Csv 是一个跨平台的 Csv 字符串 &lt;==&gt; .NET 对象转换器。

在 csv 每一列中支持的数据类型:
- System.String
- System.DateTime
- System.Int32
- System.Int64
- System.Single
- System.Double

__在 Nuget 下载__

https://www.nuget.org/packages/Nukepayload2.Csv

## 示例代码
### 将 Csv 字符串转换为 .NET 对象
目标: 把下面的 csv 字符串转换为 .NET 对象
```csv
Id,Rate,Name,Date,IsUsed
-233,12345.12,test string,2018-05-09,True
0,-12345.12,,2018-09-09,False
```
#### 1. 创建模型类

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
#### 2. 将 csv 转换为 `MyCsvModel` 的 `IReadOnlyList`
假定 `csv` 是包含 csv 的 `System.String`。

__VB__
```vb
Dim converted = CsvConvert.DeserializeObject(Of MyCsvModel)(csv)
```

__C#__
```csharp
var converted = CsvConvert.DeserializeObject<MyCsvModel>(csv);
```

### 将 .NET 对象转换为 Csv 字符串

__VB__
```vb
csv = CsvConvert.SerializeObject(converted)
```

__C#__
```csharp
csv = CsvConvert.SerializeObject(converted);
```

## 下一步的计划?
- [测试中] 将添加 VB 和 C# 的模型类生成器。
- [挂起] 提升性能 （等 `ref struct` 在 VB 能用了再说） https://github.com/dotnet/vblang/issues/297 。
