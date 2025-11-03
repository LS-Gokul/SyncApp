using LSSyncApp.Forms;
using LSSyncApp.Tally.Exceptions;
using LSSyncApp.Tally.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LSSyncApp.Controllers
{
    public class TallyODBC
    {
        public static ODBCSyncParam _OdbcSyncParam = new ODBCSyncParam();
        private readonly HttpClient client = new HttpClient();

        public static string isReturn, isParam, isStatus, isSqlQuery, isXML, isCompanyID, isGroup, isDate;
        public static string isCompFromDate, isCompToDate, isFromDate = "", isToDate = "", isMaxDate;
        public static int rCnt, iiSyncType, iiCmd, iiFullFetch = 1, iiLoop, iiSuccess;

        private string Port, BaseURL;

        public string Status { get; private set; }
        public string ReqStatus { get; private set; }

        public string Company { get; private set; }
        public string FromDate { get; private set; }
        public string ToDate { get; private set; }

        private bool disposedValue;

        //Gets Full Url from Baseurl and Port
        private string FullURL;


        public AuditLogVar init(ODBCSyncParam osp, IProgress<int> progress, string asParam, int aiSyncType,
                            int Timeoutseconds = 900)
        {
            _OdbcSyncParam = osp;
            isParam = asParam;
            iiSyncType = aiSyncType;
            iiCmd = _OdbcSyncParam.odbcGlobalVar.giCmd;
            Port = _OdbcSyncParam.port;
            BaseURL = _OdbcSyncParam.ip;
            FullURL = "http://" + BaseURL + ":" + Port;
            Company = _OdbcSyncParam.isODBCServer;
            isGroup = _OdbcSyncParam.isODBCDBName;
            isCompanyID = _OdbcSyncParam.isCustCode + "-" + _OdbcSyncParam.isFirmCode + "-" + _OdbcSyncParam.isSeq;

            client.Timeout = TimeSpan.FromSeconds(Timeoutseconds);

            _OdbcSyncParam._auditLogVar.LogId = _OdbcSyncParam.isLogTime;
            /*
            c1:
            if (Company == "" || Company == null)
            {
                Company = null;
                if (iiCmd == 0)
                {
                    CompanyList cpy = new CompanyList(this);
                    cpy.ShowDialog();

                    if(cpy.ExternalID == "Failed" || cpy.ExternalID == null || cpy.ExternalID == "")
                    {
                        SetStatusResult(cpy.ExternalName);
                        return _OdbcSyncParam._auditLogVar;
                    }
                    _OdbcSyncParam.odbcGlobalVar.gsExternalID = cpy.ExternalID;
                    _OdbcSyncParam.odbcGlobalVar.gsExternalName = cpy.ExternalName;
                    isSqlQuery = "Update LS_Customer_Firm_Details Set [External ID] = '" + cpy.ExternalID + "',[External Name] = '" + cpy.ExternalName + "'"
                            + " Where LSCode = '" + _OdbcSyncParam.isCustCode + "' And firm_code = '" + _OdbcSyncParam.isFirmCode + "';";
                    isReturn = _OdbcSyncParam.odbcGlobalVar.mstDBConnLogin.destConnSetup(_OdbcSyncParam.odbcGlobalVar.masterServerName,
                                _OdbcSyncParam.odbcGlobalVar.masterdbName, _OdbcSyncParam.odbcGlobalVar.masterUID,
                                _OdbcSyncParam.odbcGlobalVar.masterPwd);

                    if (isReturn.Contains("Failed"))
                    {
                        SetStatusResult(isReturn);
                        return _OdbcSyncParam._auditLogVar;
                    }

                    isReturn = _OdbcSyncParam.odbcGlobalVar.mstDBConnLogin.destExecQuery(isSqlQuery);
                    if(isReturn.Contains("Failed"))
                    {
                        SetStatusResult(isReturn);
                        return _OdbcSyncParam._auditLogVar;
                    }
                    
            Registry.SetValue(_OdbcSyncParam.odbcGlobalVar.regPath, "EID", cpy.ExternalID);
                    Registry.SetValue(_OdbcSyncParam.odbcGlobalVar.regPath, "EN", cpy.ExternalName);

                    Company = cpy.ExternalName;
                    goto c1;
                }
                else
                {
                    SetStatusResult("Company not Selected");
                    return _OdbcSyncParam._auditLogVar;
                }
            }
            */

            Task<string> lsRet = syncData(progress);
            
            if (lsRet.Result.Contains("Failed"))
            {
                SetStatusResult(lsRet.Result);
            }
            return _OdbcSyncParam._auditLogVar;
        }


        public async Task<string> CheckDetails(string asIP, string asPort, string asSql, int Timeoutseconds = 900)
        {
            Port = asPort;
            BaseURL = asIP;
            FullURL = "http://" + BaseURL + ":" + Port;
            client.Timeout = TimeSpan.FromSeconds(Timeoutseconds);
            return await GetSqlResult(asSql);
        }

        public async Task<int> CheckConnection(string asIP, string asPort, int Timeoutseconds = 900)
        {
            Port = asPort;
            BaseURL = asIP;
            FullURL = "http://" + BaseURL + ":" + Port;
            client.Timeout = TimeSpan.FromSeconds(Timeoutseconds);
            await Check();
            if (Status == "Running")
            {
                return 1;
            }
            return 0;
        }

        private async Task<string> syncData(IProgress<int> progress)
        {
            //SqlDataReader isdReader;
            try
            {
                string lsWhereCondition = "";
                lsWhereCondition = " WHERE [Company] = '" + Company + "'";

                if (iiCmd == 0)
                {
                    if (progress != null) progress.Report(1);//Progress Bar
                }
                //Fetch Summary Table list of LS (Master DB)
                _OdbcSyncParam.odbcGlobalVar._MasterConfig.GetFetchTableList(_OdbcSyncParam.odbcGlobalVar, iiSyncType, isParam, out iiSuccess, out isReturn);
                if (iiSuccess == 0)
                {
                    return isReturn;
                }
                JsonElement ljeTableList = _OdbcSyncParam.odbcGlobalVar.createJsonElement(isReturn);
                rCnt = ljeTableList.EnumerateArray().Count();
                if (rCnt > 0)
                {
                    int liTableType, liLoop, iiProgress1, liSQL, liType;
                    string lsTableName, lsTableCode, lsDelColName;
                    string lsPkeyCols, lsSetUpdateQuery, lsColList, lsColsForInsert, lsJsonConList;
                    string lsCollectionName, lsCollectionType, lsXMLFormCol;

                    iiProgress1 = 100 / rCnt;
                    for (int i = 0; i < rCnt; i++)
                    {
                        string lsSqlQuery = "";
                        if (iiCmd == 0)
                        {
                            if (progress != null) progress.Report(i * iiProgress1);
                        }
                        _OdbcSyncParam.setStatusLog("sT", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 1);
                        lsTableCode = ljeTableList[i].GetProperty("tableCode").ToString();
                        lsTableName = ljeTableList[i].GetProperty("tableName").ToString();
                        liTableType = int.Parse(ljeTableList[i].GetProperty("tableType").ToString());
                        lsDelColName = ljeTableList[i].GetProperty("delColName").ToString();

                        _OdbcSyncParam.setStatusLog("tblName", lsTableName, 1);

                        lsPkeyCols = "";
                        lsSetUpdateQuery = "";
                        lsColList = "";
                        lsColsForInsert = "";
                        lsJsonConList = "";

                        //////////////////////////////////////////////////////////////////////////////
                        //Fetch Select Table Script
                        //////////////////////////////////////////////////////////////////////////////
                        _OdbcSyncParam.setStatusLog("status", "Fetch Table S", 1);
                        _OdbcSyncParam.odbcGlobalVar._fun.fetchTable(_OdbcSyncParam.odbcGlobalVar, lsTableCode, out string lsFetchTable, out isStatus, out isReturn);
                        if (isStatus == "Failed")
                        {
                            _OdbcSyncParam.setStatusLog("status", isReturn, 1);
                            _OdbcSyncParam.auditLogReset(2);
                            continue;
                        }

                        if(lsFetchTable.Contains("|"))
                        {
                            liSQL = 1;
                            lsCollectionName = lsFetchTable.Substring(0, lsFetchTable.IndexOf("|"));
                            lsFetchTable = lsFetchTable.Substring(lsFetchTable.IndexOf("|") + 1);

                            lsCollectionType = lsFetchTable.Substring(0, lsFetchTable.IndexOf("|"));
                            lsFetchTable = lsFetchTable.Substring(lsFetchTable.IndexOf("|") + 1);

                            lsXMLFormCol = lsFetchTable.Substring(0, lsFetchTable.IndexOf("|"));
                        }
                        else
                        {
                            liSQL = 3;
                            lsCollectionName = lsFetchTable.Replace("@lsCompanyId", "'" + isCompanyID + "'")
                                .Replace("@grpId", "'" + isGroup + "'");
                            lsCollectionType = "";
                            lsXMLFormCol = "";
                        }


                        //////////////////////////////////////////////////////////////////////////////
                        //Column List Fetch (Customer Source Database)
                        //////////////////////////////////////////////////////////////////////////////
                        _OdbcSyncParam.odbcGlobalVar._DestinationConfig.ExecuteRawQuery(
                            _OdbcSyncParam.odbcGlobalVar, "", 2, out iiSuccess, out isReturn);
                        if (iiSuccess == 0)
                        {
                            _OdbcSyncParam.setStatusLog("status", "Failed to Connect Customer Server on Checking Table Exists", 1);
                            _OdbcSyncParam.auditLogReset(2);
                            continue;
                        }

                        _OdbcSyncParam.getColList(lsTableName, _OdbcSyncParam.isCustCode, _OdbcSyncParam.isFirmCode, out lsPkeyCols, out lsColList,
                                            out lsJsonConList, out lsColsForInsert, out lsSetUpdateQuery, out string lsSelJsonColList, out isReturn, 
                                            out int liInsType, out _, liSQL, 1, lsXMLFormCol);
                        if (isReturn.Contains("Failed"))
                        {
                            _OdbcSyncParam.setStatusLog("status", isReturn, 1);
                            _OdbcSyncParam.auditLogReset(2);
                            continue;
                        }

                        _OdbcSyncParam.setStatusLog("br", isCompanyID, 1);
                        _OdbcSyncParam.setStatusLog("sT", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 1);

                        //////////////////////////////////////////////////////////////////////////////
                        //Check the Connection for Customer Source Database)
                        //////////////////////////////////////////////////////////////////////////////
                        //_OdbcSyncParam.setStatusLog("status", FullURL,1);
                        await CheckStatus().ConfigureAwait(false);
                        //_OdbcSyncParam.setStatusLog("status", Status, 1);
                        if (Status != "Running")
                        {
                            _OdbcSyncParam.setStatusLog("status", "Failed to Connect Customer Source Server on Checking Table Exists", 1);
                            _OdbcSyncParam.auditLogReset(2);
                            continue;
                        }

                        //////////////////////////////////////////////////////////////////////////////
                        //Find Min Max Date
                        //////////////////////////////////////////////////////////////////////////////
                        FromDate = null;
                        ToDate = null;
                        liType = 0;
                        if (liTableType == 2 && !lsTableName.Contains("Outstanding") && iiFullFetch == 0)
                        {
                            liType = 2;
                            isReturn = maxTime(1, lsTableName);
                            if(iiSuccess == 0 || isReturn == "" || isReturn == "Failed")
                            {
                                FromDate = isCompFromDate;
                                ToDate = isCompToDate;
                                isMaxDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            else
                            {
                                if (isFromDate == "" || isFromDate == null || isToDate == "" || isToDate == null)
                                {
                                    findAllVouchers(isReturn);
                                }
                                FromDate = isFromDate;
                                ToDate = isToDate;
                                if (DateTime.TryParse(isFromDate, out _) && DateTime.TryParse(isToDate, out _))
                                {
                                    if (DateTime.Parse(isFromDate) <= DateTime.Parse(isToDate))
                                    {
                                        isDate = DateTime.Parse(isFromDate).ToString("d-MMM-yyyy");
                                        liType = 1;
                                    }
                                }
                            }
                            //MessageBox.Show( + " - " + DateTime.Parse("1-Jan-2022").ToString("yyyy-MM-dd")); 
                        }

                        liLoop = 1;
                        while (liLoop > 0)
                        {
                            liLoop = 1;
                            _OdbcSyncParam._auditLogVar.Sequence = liLoop;
                            _OdbcSyncParam.setStatusLog("status", "Fetch From Source Table", 1);
                            
                            //////////////////////////////////////////////////////////////////////////////
                            //Execute the Select Statement (Customer Source Database)
                            //////////////////////////////////////////////////////////////////////////////
                            isXML = "";
                            liLoop += 1;
                            if (liType == 1)
                            {
                                lsCollectionName += " where $Date = '" + isDate + "'";
                                if(DateTime.Parse(isDate) >= DateTime.Parse(ToDate))
                                {
                                    liLoop = 0;
                                }
                                isDate = DateTime.Parse(isDate).AddDays(1).ToString("d-MMM-yyyy");
                            }
                            else
                            {
                                liLoop = 0;
                            }
                            await GetColRes(lsCollectionName, liSQL);

                            if (isXML.Length <= 0 || isXML == "" || isXML == null || isXML.Substring(0, 8) == "Failed -")
                            {
                                _OdbcSyncParam.setStatusLog("status", "Failed to Fetch Data" + isXML, 1);
                                _OdbcSyncParam.auditLogReset(3);
                                continue;
                            }
                            if (isXML == FullURL)
                            {
                                _OdbcSyncParam.setStatusLog("status", "Data Fetch Timeout -> " + isXML, 1);
                                _OdbcSyncParam.auditLogReset(3);
                                continue;
                            }
                            //_OdbcSyncParam.setStatusLog("status", isXML, 1);
                            //////////////////////////////////////////////////////////////////////////////
                            //Delete from Destination Table (Customer Destination Database)
                            //////////////////////////////////////////////////////////////////////////////
                            isSqlQuery = "DELETE FROM " + lsTableName + " WHERE LSCode = '" + _OdbcSyncParam.isCustCode
                                + "' And LSFirmCode = " + _OdbcSyncParam.isFirmCode + " And [LS Company ID] = '" + isCompanyID + "'"
                                + (liType == 1 ? " AND [Date of Transaction] >= '" + isFromDate + "' AND [Date of Transaction] <= '" + isToDate + "' " : "") + " ;";
                            
                            _OdbcSyncParam.odbcGlobalVar._DestinationConfig.ExecuteRawQuery(_OdbcSyncParam.odbcGlobalVar, isSqlQuery, 1, out iiSuccess, out isReturn, 0);

                            if (iiSuccess == 0)
                            {
                                liLoop = 0;
                                _OdbcSyncParam.setStatusLog("status", isReturn, 1);
                                _OdbcSyncParam.auditLogReset(3);
                                continue;
                            }
                            _OdbcSyncParam.setStatusLog("status", "Deleted Successfully", 1);

                            //////////////////////////////////////////////////////////////////////////////
                            //Form Sql for insert Query (Customer Destination Database)
                            //////////////////////////////////////////////////////////////////////////////
                            _OdbcSyncParam.setStatusLog("status", "Construct Insert", 1);
                            if (liSQL == 1)
                            {
                                isSqlQuery = "DECLARE @idoc INT, @doc VARCHAR(MAX); "
                                    + " SET @doc ='" + isXML + "'; "
                                    + " EXEC sp_xml_preparedocument @idoc OUTPUT, @doc; "
                                    + " INSERT INTO " + lsTableName + "(" + lsColList + ")"
                                    + " SELECT DISTINCT * FROM("
                                    + " SELECT " + lsSelJsonColList
                                    + " FROM OPENXML(@idoc, '/ENVELOPE/BODY/DATA/COLLECTION/" + lsCollectionType + "',1) "
                                    + " WITH(" + lsJsonConList + ")) as a ";
                            }
                            else if (liSQL == 3)
                            {
                                lsSqlQuery = " SELECT " + lsSelJsonColList.Replace("[SequenceT]", "Row_Number() OVER (PARTITION BY " 
                                    + lsPkeyCols + " ORDER BY " + lsPkeyCols + ") as seqT")
                                    + " FROM ("
                                    + " SELECT DISTINCT * FROM("
                                    + "     SELECT " + lsJsonConList
                                    + "     FROM @doc.nodes('/ENVELOPE/BODY/EXPORTDATARESPONSE/RESULTDATA/ROW') Tbl(Col)"
                                    + " ) as a ) as b" + (lsTableName == "LS_Branch_Master" ? lsWhereCondition : "") ;

                                isSqlQuery = "DECLARE @doc xml = '" + isXML + "';"
                                    + " INSERT INTO " + lsTableName + "(" + lsColList + ")"
                                    + lsSqlQuery + ";";
                            }
                            else
                            {
                                _OdbcSyncParam.setStatusLog("status", "Type Not Found", 1);
                                _OdbcSyncParam.auditLogReset(3);
                                continue;
                            }

                            //////////////////////////////////////////////////////////////////////////////
                            //Execute the insert Query (Customer Destination Database)
                             
                            _OdbcSyncParam.setStatusLog("status", "Insert Start", 1);
                            if (lsTableName == "LS_Ledger_Detail")
                                lsTableName = "LS_Ledger_Detail";

                            _OdbcSyncParam.odbcGlobalVar._DestinationConfig.ExecuteRawQuery(_OdbcSyncParam.odbcGlobalVar, isSqlQuery, 1, out iiSuccess, out isReturn, 0);
                            
                            if (iiSuccess == 0)
                            {
                                liLoop = 0;
                                _OdbcSyncParam.setStatusLog("status", isReturn, 1);
                                //_OdbcSyncParam.odbcGlobalVar.logFile(lsTableName,isSqlQuery,2);
                                _OdbcSyncParam.auditLogReset(3);
                                continue;
                            }
                            if (lsTableName == "LS_Branch_Master") findDate(lsSqlQuery);
                            if (liType == 1 || liType == 2)
                            {
                                isReturn = maxTime(2, lsTableName,isMaxDate);
                                _OdbcSyncParam.setStatusLog("status", "Max Time - " + isReturn, 1);
                            }
                            _OdbcSyncParam.setStatusLog("status", "Data Inserted", 1);

                            
                            _OdbcSyncParam.setStatusLog("eT", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 1);
                            _OdbcSyncParam.setStatusLog("stat", "Success", 1);
                            _OdbcSyncParam.setStatusLog("", "", 2);
                        }
                        _OdbcSyncParam.auditLogReset(1);
                    }
                }

                //isSqlQuery = "Update LS_Branch_Mster Set [LS Company ID] = '" + isCompanyID + "'" + lsWhereCondition;
                _OdbcSyncParam.setStatusLog("tblName", "Success", 1);
                _OdbcSyncParam.setStatusLog("status", "Successfully ", 1);

                if (iiCmd == 0)
                {
                    if (progress != null) progress.Report(100);//Progress Bar
                }
                return "Success";
            }
            catch (Exception e)
            {
                return "Failed " + e.Message;
            }
        }

        private async Task<bool> CheckStatus()
        {
            await Check().ConfigureAwait(false);
            return true;
        }

        private async Task<bool> GetColRes(string colName, int aiType)
        {
            if(aiType == 1)
            {
                isXML = await GetCollectionResult(colName);
            }
            else if(aiType == 3)
            {
                isXML = await GetSqlResult(colName);
            }
            return true;
        }

        public async Task<string> GetCollectionResult(string colName, StaticVariables staticVariables = null, List<string> Nativelist = null,
                                                     List<string> Filters = null, List<string> SystemFilters = null)
        {
            try
            {
                staticVariables = new StaticVariables()
                {
                    SVCompany = Company,
                    SVExportFormat = "XML",
                    ViewName = VoucherViewType.CustOutstanding,
                };
                string GrpXml = await GetNativeCollectionXML(rName: colName,colType: null,Sv: staticVariables);
                GrpXml = GetObjfromXml(GrpXml);
                return GrpXml.Replace("\r\n", "");
            }
            catch(Exception e)
            {
                return "Failed - " + e.Message;
            }
        }

        public async Task<string> GetSqlResult(string asSql, int rptType = 5, string repName = "ODBC Report", StaticVariables staticVariables = null)
        {
            try
            {
                await Check();
                if (Status == "Running")
                {
                    staticVariables = new StaticVariables()
                    {
                        SVCompany = Company,
                        SVExportFormat = "XML",
                        //Collection Name / Report Name
                        ViewName = rptType == 5 ? VoucherViewType.CustOutstanding :
                            rptType == 6 ? VoucherViewType.SuppOutstanding : VoucherViewType.AccountingVoucherView,
                    };

                    string GrpXml = await GetNativeCollectionXML(rName: repName, sql: asSql, Sv: staticVariables);
                    GrpXml = GetObjfromXml(GrpXml);
                    return GrpXml.Replace("\r\n", "");
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
                //RestAPI restAPI = new RestAPI();
                //restAPI.getAPICalling(FullURL);
                HttpResponseMessage response = await client.GetAsync(FullURL).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                string res = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                //_OdbcSyncParam.setStatusLog("status", res, 1);

                Status = "Running";
                return true;
            }
            catch (HttpRequestException ex)
            {
                _OdbcSyncParam.setStatusLog("status", ex.Message, 1);
                HttpRequestException e = ex;
                Status = $"Tally is not opened \n or Tally is not running in given port - { Port} )\n or Given URL - {BaseURL} \n" +
                    e.Message;
                //throw new TallyConnectivityException("Tally is not running", FullURL);
            }
            catch
            {
                
            }
            return false;
        }

        //For SQL Select
        public async Task<string> GetNativeCollectionXML(string rName, string sql, StaticVariables Sv = null)
        {
            //LedgersList LedgList = new();
            string Resxml;
            SqlSelectEnvelope sqlSelect = new SqlSelectEnvelope(); //Collection Envelope
            string RName = rName;

            sqlSelect.Header = new Header("Export Data");  //Configuring Header To get Export data

            if (Sv != null)
            {
                sqlSelect.Body.ExpData.ReqDesc.StaticVariables = Sv;
            }
            sqlSelect.Body.ExpData.ReqDesc.ReportName = RName;
            sqlSelect.Body.ExpData.ReqDesc.SqlReq = sql;

            string Reqxml = sqlSelect.GetXML(); //Gets XML from Object
            string Reqxml1 = Reqxml.Substring(Reqxml.IndexOf("<SQLREQUEST") + 11);
            Reqxml = Reqxml.Substring(0, Reqxml.IndexOf("<SQLREQUEST") + 11);
            Reqxml = Reqxml + " TYPE = \"General\" METHOD = \"SQLExecute\" " + Reqxml1;

            Resxml = await SendRequest(Reqxml.Replace("&amp;","&"));

            if(Resxml.Contains("<RESPONSE>Unknown Request, cannot be processed</RESPONSE>"))
                Resxml = await SendRequest(Reqxml);

            return Resxml;
        }

        //For Collection 
        public async Task<string> GetNativeCollectionXML(string rName, string colType, StaticVariables Sv = null, string childof = null,
                                                         List<string> NativeFields = null, List<string> Filters = null,
                                                         List<string> SystemFilters = null, bool isInitialize = false)
        {
            //LedgersList LedgList = new();
            string Resxml;
            CusColEnvelope ColEnvelope = new CusColEnvelope(); //Collection Envelope
            string RName = rName;

            ColEnvelope.Header = new Header("Export", "Collection", RName);  //Configuring Header To get Export data
            if (Sv != null)
            {
                ColEnvelope.Body.Desc.StaticVariables = Sv;
            }

            ColEnvelope.Body.Desc.TDL.TDLMessage = new ColTDLMessage(colName: RName, colType: colType, nativeFields: NativeFields, Filters, SystemFilters);
            ColEnvelope.Body.Desc.TDL.TDLMessage.Collection.Childof = childof;
            if (isInitialize)
            {
                ColEnvelope.Body.Desc.TDL.TDLMessage.Collection.SetAttributes(isInitialize: "Yes");
            }

            string Reqxml = ColEnvelope.GetXML(); //Gets XML from Object
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
                SXml = SXml.Replace("\t", "&#09;");
                StringContent TXML = new StringContent(SXml, Encoding.UTF8, "application/xml");
                HttpResponseMessage Res = await client.PostAsync(FullURL, TXML);
                Res.EnsureSuccessStatusCode();
                var byteArray = await Res.Content.ReadAsByteArrayAsync();
                Resxml = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
                Resxml = ReplaceXMLText(Resxml);
                return Resxml;
            }
            catch (Exception e)
            {
                ReqStatus = e.Message;
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

        public string GetObjfromXml(string Xml)
        {
            try
            {
                string re = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
                return System.Text.RegularExpressions.Regex.Replace(Xml, re, "");
            }
            catch (Exception e)
            {
                return e.Message;
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

        //////////////////////////////////////////////////////////////////////////////
        //Audit Log
        //////////////////////////////////////////////////////////////////////////////
        private void SetStatusResult(string asText)
        {
            _OdbcSyncParam._auditLogVar.Object = "Sync";//Set Audit Log Var
            _OdbcSyncParam._auditLogVar.ChildObject = "Tally-Sync";//Set Audit Log Var
            _OdbcSyncParam._auditLogVar.Sequence = 1;//Set Audit Log Var
            _OdbcSyncParam._auditLogVar.ObjectFromTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//Set Audit Log Var
            _OdbcSyncParam._auditLogVar.EndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//Set Audit Log Var
            _OdbcSyncParam._auditLogVar.LogDetails = asText;//Set Audit Log Var
            _OdbcSyncParam._auditLogVar.Status = "Failed";//Set Audit Log Var
            _OdbcSyncParam.setStatusLog("", "", 2);
        }

        //////////////////////////////////////////////////////////////////////////////
        //Findi & update Max time from Customer Destination Database
        //////////////////////////////////////////////////////////////////////////////
        private string maxTime(int aiType, string asTableName, string asTime = null)
        {
            string lsRet = "Failed";

            _OdbcSyncParam.odbcGlobalVar._DestinationConfig.ExecuteRawQuery(
                  _OdbcSyncParam.odbcGlobalVar, "", 2, out iiSuccess, out isReturn);
            if (iiSuccess == 0)
            {
                _OdbcSyncParam.setStatusLog("status", "Failed to Connect Customer Server on Max Time Fetch", 1);
                _OdbcSyncParam.auditLogReset(3);
            }
            else
            {
                if (aiType == 1)
                {
                    isSqlQuery = "Select [Max Time] From LS_MaxTime Where LSCode = '" + _OdbcSyncParam.isCustCode + "' And "
                        + "LSFirmCode = " + _OdbcSyncParam.isFirmCode + " And [LS Company ID] = '" + isCompanyID + "' And "
                        + "[Table Name] = '" + asTableName + "';";
                    _OdbcSyncParam.odbcGlobalVar._DestinationConfig.ExecuteRawQuery(_OdbcSyncParam.odbcGlobalVar, isSqlQuery, 0, out iiSuccess, out lsRet, 0);
                }
                else if (aiType == 2)
                {
                    isSqlQuery = "MERGE INTO LS_MaxTime WITH(HOLDLOCK) AS a USING("
                        + "     Select '" + _OdbcSyncParam.isCustCode + "' as  LSCode," + _OdbcSyncParam.isFirmCode + " AS LSFirmCode,"
                        + "         '" + isCompanyID + "' AS [LS Company ID],'" + asTableName + "' AS [Table Name], "
                        + "         '" + asTime + "' AS maxTime"
                        + "     ) as newA "
                        + " ON a.LSCode = newA.LSCode AND a.LSFirmCode = newA.LSFirmCode AND "
                        + " a.[LS Company ID] = newA.[LS Company ID] AND a.[Table Name] = newA.[Table Name] "
                        + " WHEN MATCHED THEN "
                        + "     UPDATE SET a.[Max Time] = newA.maxTime "
                        + " WHEN NOT MATCHED THEN "
                        + "     INSERT (LSCode,LSFirmCode,[LS Company ID],[Table Name],[Max Time]) "
                        + "     VALUES(newA.LSCode,newA.LSFirmCode,newA.[LS Company ID],newA.[Table Name],newA.maxTime);";
                    try
                    {
                        _OdbcSyncParam.odbcGlobalVar._DestinationConfig.ExecuteRawQuery(_OdbcSyncParam.odbcGlobalVar, isSqlQuery, 1, out iiSuccess, out lsRet, 0);
                    }
                    catch
                    {
                        lsRet = "Failed";
                    }
                }
            }
            return lsRet;
        }

        //////////////////////////////////////////////////////////////////////////////
        //Find Branch From To Date
        //////////////////////////////////////////////////////////////////////////////
        private void findDate(string asSql)
        {
            string lsSql = "DECLARE @doc xml = '" + isXML + "';"
                + "Select Cast(Cast([Date Of Starting From] as date) as VarCHar(10)) + '|' + Cast(Cast([Date Of Audited Upto] as  date) as VarCHar(10)) "
                + "From (" + asSql + ") as a";

            _OdbcSyncParam.odbcGlobalVar._DestinationConfig.ExecuteRawQuery(_OdbcSyncParam.odbcGlobalVar, lsSql, 0, out iiSuccess, out isReturn, 0);
            
            if (iiSuccess == 0)
            {
                _OdbcSyncParam.setStatusLog("status", isReturn + " - On Company date fetch",  1);
                _OdbcSyncParam.auditLogReset(3);
                return;
            }
            isCompFromDate = isReturn.Substring(0, isReturn.IndexOf("|"));
            isCompToDate = isReturn.Substring(isReturn.IndexOf("|") + 1);
        }

        //////////////////////////////////////////////////////////////////////////////
        //Find the Vouchers From to date
        //////////////////////////////////////////////////////////////////////////////
        private async void findAllVouchers(string minDate)
        {
            string lsSql;
            isFromDate = null;
            isToDate = null;
            FromDate = null;
            ToDate = null;
            isXML = "";

            try
            {
                /*lsSql = "select $Guid,$Date,$VoucherNumber,$VoucherTypeName,$LS_Vch_Amount,$LS_Created_Modified_Date,$LS_User,"
                    + "$LS_Vch_Status,$LS_Modified_Time,$Iscancelled from CollDB_VchHistory where $mDate != $$Date:\"\"";*/
                lsSql = "select $Date,$LS_Created_Modified_Date from CollDB_VchHistory where $mDate != $$Date:\"\"";
                isXML = await GetSqlResult(lsSql);
                if(isXML != "" && isXML != null)
                {
                    lsSql = "DECLARE @doc xml = '" + isXML + "';"
                        + "Select IsNull(Cast(Min(Cast(bd as date)) as VarChar(10)) + '|' + "
                        + "       Cast(Max(Cast(bd as date)) as VarChar(10)) + '|' + Cast(Max(Cast(md as date)) as VarChar(10)),'') "
                        + "From ("
                        + "     Select Tbl.Col.value('COL[1]', 'VARCHAR(50)') AS [bd],Tbl.Col.value('COL[2]', 'VARCHAR(50)') AS [md] "
                        + "     From @doc.nodes('/ENVELOPE/BODY/EXPORTDATARESPONSE/RESULTDATA/ROW') Tbl(Col)"
                        + ") as a "
                        + "Where Cast(md as date) >= '" + minDate + "'";
                    _OdbcSyncParam.odbcGlobalVar._DestinationConfig.ExecuteRawQuery(_OdbcSyncParam.odbcGlobalVar, lsSql, 0, out iiSuccess, out isReturn, 0);
                    
                    if(iiSuccess == 0)
                    {
                        _OdbcSyncParam.setStatusLog("status", isReturn + " - On Voucher Altered Fetch", 1);
                        _OdbcSyncParam.auditLogReset(3);
                        return;
                    }
                    isFromDate = isReturn.Substring(0, isReturn.IndexOf("|"));
                    isReturn = isReturn.Substring(isReturn.IndexOf("|") +  1);

                    isToDate = isReturn.Substring(0, isReturn.IndexOf("|"));
                    isMaxDate = isReturn.Substring(isReturn.IndexOf("|") + 1);
                }
            }
            catch
            {
                isFromDate = null;
                isToDate = null;
            }
            
            return;

        }
    }
}
