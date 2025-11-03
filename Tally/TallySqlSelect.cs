using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Xml.Xsl;
using LSSyncApp.Tally.Exceptions;
using LSSyncApp.Tally.Models;
using System;
using System.Net.Http;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LSSyncApp.Tally
{
    public class TallySqlSelect : IDisposable
    {
        private readonly HttpClient client = new HttpClient();

        private ILogger Logger { get; }
        private CLogger CLogger { get; }

        private int Port;
        private string BaseURL;

        public string Status { get; private set; }
        public string ReqStatus { get; private set; }

        public string Company { get; private set; }
        public string FromDate { get; private set; }
        public string ToDate { get; private set; }

        private bool disposedValue;

        //Gets Full Url from Baseurl and Port
        private string FullURL => BaseURL + ":" + Port;

        public LicenseInfo LicenseInfo { get; private set; }

        /// <summary>
        /// Intiate Tally with <strong>baseURL</strong> and <strong>port</strong>
        /// </summary>
        /// <param name="baseURL">Url on which Tally is Running</param>
        /// <param name="port">Port on which Tally is Running</param>
        public TallySqlSelect(string baseURL,
                     int port,
                     ILogger<TallySqlSelect> Logger = null,
                     int Timeoutseconds = 60)
        {
            this.Logger = Logger ?? NullLogger<TallySqlSelect>.Instance;
            CLogger = new CLogger(Logger);
            Port = port;
            BaseURL = baseURL;
            client.Timeout = TimeSpan.FromSeconds(Timeoutseconds);
        }

        public async Task<string> GetSqlResult(string asSql, string repName, int rptType, StaticVariables staticVariables = null)
        {
            try
            {
                if (Check().Result)
                {
                    string ReqType = "List of companies in Default Tally path";
                    CLogger.TallyReqStart(ReqType);
                    staticVariables = new StaticVariables()
                    {
                        SVCompany = Company,
                        SVExportFormat = "XML",
                        //Collection Name / Report Name
                        ViewName = rptType == 5 ? VoucherViewType.CustOutstanding :
                            rptType == 6 ? VoucherViewType.SuppOutstanding : VoucherViewType.AccountingVoucherView,
                    };

                    string GrpXml = await GetNativeCollectionXML(rName: repName,
                                                                 sql: asSql,
                                                                 Sv: staticVariables);
                    GrpXml = GrpXml.Replace("\r\n", "");
                    return GrpXml;
                }
                else
                {
                    return "Tally not connected";
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        
        /// <summary>
        /// Checks whether Tally is running in given URL and port
        /// </summary>
        /// <returns>Return true if running,else false</returns>
        public async Task<bool> Check()
        {
            try
            {
                CLogger.TallyCheck(FullURL);
                HttpResponseMessage response = await client.GetAsync(FullURL);
                response.EnsureSuccessStatusCode();
                string res = await response.Content.ReadAsStringAsync();

                Status = "Running";
                CLogger.TallyRunning(FullURL);
                return true;
            }
            catch (HttpRequestException ex)
            {
                HttpRequestException e = ex;
                CLogger.TallyNotRunning(FullURL);
                Status = $"Tally is not opened \n or Tally is not running in given port - { Port} )\n or Given URL - {BaseURL} \n" +
                    e.Message;
                //throw new TallyConnectivityException("Tally is not running", FullURL);
            }
            catch (Exception e)
            {
                CLogger.TallyError(FullURL, e.Message);
            }
            return false;
        }

        /// <summary>
        /// Generates XML for custom collection using TDL report
        /// </summary>
        /// <param name="rName">Custom Report Name to be used</param>
        /// <param name="colType">Specify Name of collection as per Tally</param>
        /// <param name="Sv">instance of Static vairiables</param>
        /// <param name="NativeFields">Filters if any</param>
        /// <param name="Filters">Filters if any</param>
        /// <param name="SystemFilters">Definition for filter</param>
        /// <returns>returns xml as string</returns>
        public async Task<string> GetNativeCollectionXML(string rName,
                                                         string sql,
                                                         StaticVariables Sv = null)
        {
            //LedgersList LedgList = new();
            string Resxml;
            SqlSelectEnvelope sqlSelect = new SqlSelectEnvelope(); //Collection Envelope

            sqlSelect.Header = new Header("Export Data");  //Configuring Header To get Export data

            if (Sv != null)
            {
                sqlSelect.Body.ExpData.ReqDesc.StaticVariables = Sv;
            }
            sqlSelect.Body.ExpData.ReqDesc.ReportName = rName;
            sqlSelect.Body.ExpData.ReqDesc.SqlReq = sql;
            
            string Reqxml = sqlSelect.GetXML(); //Gets XML from Object
            string Reqxml1 = Reqxml.Substring(Reqxml.IndexOf("<SQLREQUEST") + 11);
            Reqxml = Reqxml.Substring( 0 ,Reqxml.IndexOf("<SQLREQUEST") + 11);
            Reqxml = Reqxml + " TYPE = \"General\" METHOD = \"SQLExecute\" " + Reqxml1;

            Resxml = await SendRequest(Reqxml);

            return Resxml;
        }

        /// <summary>
        /// Posts XML to tally
        /// </summary>
        /// <param name="SXml">XML to be posted to tally</param>
        /// <returns>Response received from Tally</returns>
        /// <exception cref="TallyConnectivityException">If tally is not opened or not configured correctly</exception>
        public async Task<string> SendRequest(string SXml)
        {
            string Resxml;

            try
            {
                CLogger.TallyRequest(SXml);
                SXml = SXml.Replace("\t", "&#09;");
                StringContent TXML = new StringContent(SXml, Encoding.UTF8, "application/xml");
                HttpResponseMessage Res = await client.PostAsync(FullURL, TXML);
                Res.EnsureSuccessStatusCode();
                var byteArray = await Res.Content.ReadAsByteArrayAsync();
                Resxml = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length); ;
                Resxml = ReplaceXMLText(Resxml);
                CLogger.TallyResponse(Resxml);
                return Resxml;
            }
            catch (Exception e)
            {
                ReqStatus = e.Message;
                CLogger.TallyReqError(e.Message);
                return FullURL;
                //throw new TallyConnectivityException("Tally is not running", FullURL);
            }

        }

        //Helper method to convert escaped characters to text
        public static string ReplaceXMLText(string strXmlText)
        {
            string result = null;
            if (strXmlText != null)
            {
                result = strXmlText.Replace("&#x4;", "");
                result = result.Replace("&#4;", "");
            }
            return result;
        }

        public dynamic GetObjfromXml<T>(string Xml, XmlAttributeOverrides attrOverrides = null)
        {
            try
            {
                string re = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
                Xml = System.Text.RegularExpressions.Regex.Replace(Xml, re, "");
                XmlSerializer XMLSer = attrOverrides == null ? new XmlSerializer(typeof(T)) : new XmlSerializer(typeof(T), attrOverrides);

                NameTable nt = new NameTable();
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(nt);
                nsmgr.AddNamespace("UDF", "TallyUDF");
                XmlParserContext context = new XmlParserContext(null, nsmgr, null, XmlSpace.None);

                XmlReaderSettings xset = new XmlReaderSettings()
                {
                    CheckCharacters = false,
                    ConformanceLevel = ConformanceLevel.Fragment
                };
                XmlReader rd = XmlReader.Create(new StringReader(Xml), xset, context);
                //StringReader XmlStream = new StringReader(Xml);
                if (typeof(T).Name.Contains("VoucherEnvelope"))
                {
                    XmlReader xslreader = XmlReader.Create(new StringReader("<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\"><xsl:template match=\"@*|node()\">    <xsl:copy>        <xsl:apply-templates select=\"@*|node()\" />    </xsl:copy></xsl:template><xsl:template match=\"/ENVELOPE/BODY/DATA/TALLYMESSAGE/VOUCHER/LEDGERENTRIES.LIST\">		<ALLLEDGERENTRIES.LIST><xsl:apply-templates select=\"@*|node()\" /></ALLLEDGERENTRIES.LIST></xsl:template>   <xsl:template match=\"/ENVELOPE/BODY/DATA/TALLYMESSAGE/VOUCHER/INVENTORYENTRIES.LIST\">		   <ALLINVENTORYENTRIES.LIST><xsl:apply-templates select=\"@*|node()\" /></ALLINVENTORYENTRIES.LIST>	   </xsl:template></xsl:stylesheet>"));
                    XslCompiledTransform xslTransform = new XslCompiledTransform();
                    xslTransform.Load(xslreader);
                    StringWriter textWriter = new StringWriter();
                    XmlWriter xmlwriter = XmlWriter.Create(textWriter, new XmlWriterSettings() { OmitXmlDeclaration = true, Encoding = Encoding.Unicode });
                    xslTransform.Transform(rd, null, xmlwriter);
                    rd = XmlReader.Create(new StringReader(textWriter.ToString()), xset, context);
                }
                dynamic obj = XMLSer.Deserialize(rd);

                return obj;
            }
            catch (Exception e)
            {
                Logger.LogError("Error  - {Msg}", e.Message);
                Logger.LogError("Error occured during de-serialization of - {Xml}", Xml);
                return default;
            }

        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}