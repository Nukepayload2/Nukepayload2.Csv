Imports Microsoft.VisualStudio.TestTools.UnitTesting

Module TestHelper

    Sub TestSerialize(Of T As {New, Class})(data As T())
        Dim csv = CsvConvert.SerializeObject(data)
        Dim obj = CsvConvert.DeserializeObject(Of T)(csv)
        Dim csv2 = CsvConvert.SerializeObject(obj)
        Assert.AreEqual(csv, csv2)
    End Sub
End Module
