using System.Xml.Serialization;

namespace LSSyncApp.Tally.Models
{
    class SqlSelect
    {

    }

    [XmlRoot(ElementName = "ENVELOPE")]
    public class SqlSelectEnvelope : TallyXmlJson
    {
        [XmlElement(ElementName = "HEADER")]
        public Header Header { get; set; }

        [XmlElement(ElementName = "BODY")]
        public SqlSelectBody Body { get; set; } = new SqlSelectBody();
    }

    [XmlRoot(ElementName = "BODY")]
    public class SqlSelectBody
    {
        [XmlElement(ElementName = "EXPORTDATA")]
        public SqlSelectExportData ExpData { get; set; } = new SqlSelectExportData();

        //[XmlElement(ElementName = "DATA")]
        //public LData Data { get; set; } = new LData();
    }

    [XmlRoot(ElementName = "EXPORTDATA")]
    public class SqlSelectExportData
    {
        [XmlElement(ElementName = "REQUESTDESC")]
        public SqlSelectRequestDesc ReqDesc { get; set; } = new SqlSelectRequestDesc();
    }

    [XmlRoot(ElementName = "REQUESTDESC")]
    public class SqlSelectRequestDesc
    {
        [XmlElement(ElementName = "STATICVARIABLES")]
        public StaticVariables StaticVariables { get; set; } = new StaticVariables();

        [XmlElement(ElementName = "REPORTNAME")]
        public string ReportName { get; set; }

        [XmlElement(ElementName = "SQLREQUEST")]
        public string SqlReq { get; set; }
    }

    [XmlRoot(ElementName = "SQLREQUEST")]
    public class SqlRequest
    {
        [XmlAttribute(AttributeName = "TYPE")]
        public string Type { get; set; }

        [XmlAttribute(AttributeName = "METHOD")]
        public string Method { get; set; }

        public void SetAttributes(string asType = "General", string asMethod = "SQLExecute")
        {
            Type = asType;
            Method = asMethod;
        }
    }
}