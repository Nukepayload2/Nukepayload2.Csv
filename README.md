# Nukepayload2.Csv
Nukepayload2.Csv is a cross-platform Csv String &lt;==&gt; .NET Object converter.

## Supported types in csv columns:
- System.String
- System.DateTime
- System.DateTime?
- System.Int32
- System.Int32?
- System.Int64
- System.Int64?
- System.Single
- System.Single?
- System.Double
- System.Double?
- System.Boolean
- System.Boolean?

## Feature status

|Feature|IsSupported|
|-|-|
|Custom line separator|True|
|Custom column separator|True|
|Custom csv layout|True|
|Csv String to Objects|True|
|Objects to Csv String|True|
|Escape characters|True|
|Custom format string for output|True|
|Read-only properties|True|
|Opt-in and Opt-out columns|True|
|Run in multi-threaded code|True|
|Custom format string for input|False|
|Csv UTF-8 Stream to Objects|False|
|Objects to UTF-8 Stream|False|
|Column oriented and Sparse storage|False|
|Excel number parser compatibility|Partial|
|Excel formula compatibility|False|

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

## VB and C# model Code generator (Preview)
1. Clone this repo.
2. Make sure .NET 9 or higher SDK is installed.
3. Open cmd or bash in cloned code's root directory.
4. Go the ModelGenerator directory `cd Nukepayload2.Csv.ModelGenerator`
5. Build the project `dotnet build Nukepayload2.Csv.ModelGenerator.vbproj`
6. To see help, execute `dotnet run Nukepayload2.Csv.ModelGenerator.dll` (Change \ to / on Linux) .

## What is next?
- AOT compatibility for .NET 8 and later (`PublishAot`)

# Nukepayload2.Csv 中文介绍

Nukepayload2.Csv 是一个跨平台的 Csv 字符串 &lt;==&gt; .NET 对象转换器。

在 csv 每一列中支持的数据类型:
- System.String
- System.DateTime
- System.DateTime?
- System.Int32
- System.Int32?
- System.Int64
- System.Int64?
- System.Single
- System.Single?
- System.Double
- System.Double?
- System.Boolean
- System.Boolean?

## 功能状态

|功能|是否支持|
|-|-|
|自定义行分隔符|是|
|自定义列分隔符|是|
|自定义 CSV 布局|是|
|字符串到对象转换|是|
|对象到字符串转换|是|
|转义字符|是|
|自定义输出格式字符串|是|
|只读属性支持|是|
|选入和排除的 csv 列选择模式|是|
|执行在多线程代码中|是|
|自定义输入格式字符串|否|
|UTF-8 流到对象|否|
|对象 to UTF-8 流|否|
|列式存储和稀疏存储|否|
|Excel 数字转换器兼容|部分|
|Excel 公式兼容|否|

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

## VB 和 C# 模型代码生成器（预览）
1. 克隆此仓库.
2. 确保 .NET 9 SDK 安装了.
3. 在克隆后的代码根目录打开 cmd 或者 bash.
4. 进入生成器的目录 `cd Nukepayload2.Csv.ModelGenerator`
5. 编译 `dotnet build Nukepayload2.Csv.ModelGenerator.vbproj`
6. 执行命令查看查看帮助 `dotnet run Nukepayload2.Csv.ModelGenerator.dll` (Linux 上把 \ 改成 /) .

## 下一步的计划?
- 为 .NET 8 以及后续版本支持提前编译为本机代码 (`PublishAot`)
