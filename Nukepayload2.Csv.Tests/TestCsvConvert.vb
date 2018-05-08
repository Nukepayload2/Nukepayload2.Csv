Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass>
Public Class TestCsvConvert
    <TestMethod>
    Public Sub TestSerializeRaw()
        Dim dat = {
            New RawModel With {
                .BooleanValue = True,
                .DateValue = Date.Now,
                .DoubleValue = 12345.123,
                .IntegerValue = -233,
                .StringValue = "test string"
            },
            New RawModel With {
                .BooleanValue = False,
                .DateValue = Date.Now.AddDays(123),
                .DoubleValue = -12345.123,
                .IntegerValue = 0,
                .StringValue = Nothing
            }
        }
        TestSerialize(dat)
    End Sub

    <TestMethod>
    Public Sub TestSerializeDecorated()
        Dim dat = {
            New DecoratedModel With {
                .BooleanValue = True,
                .DateValue = Date.Now,
                .DoubleValue = 12345.123,
                .IntegerValue = -233,
                .StringValue = "test string"
            },
            New DecoratedModel With {
                .BooleanValue = False,
                .DateValue = Date.Now.AddDays(123),
                .DoubleValue = -12345.123,
                .IntegerValue = 0,
                .StringValue = Nothing
            }
        }
        TestSerialize(dat)
    End Sub

    Private Sub TestSerialize(Of T As {New, Class})(data As T())
        Dim csv = CsvConvert.SerializeObject(data)
        Dim obj = CsvConvert.DeserializeObject(Of T)(csv)
        Dim csv2 = CsvConvert.SerializeObject(obj)
        Assert.AreEqual(csv, csv2)
    End Sub
End Class
