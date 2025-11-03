using System;
using System.Linq;
using System.Text.Json; 

namespace LSSyncApp.Controllers
{
    public class Bizom
    {
        public static ODBCSyncParam _OdbcSyncParam = new ODBCSyncParam();
        public static RestAPI _RestAPI = new RestAPI();
        public static string isReturn, isParam, isFinColName, isStatus, isSqlQuery, isAccessToken;
        public static int rCnt, iiSyncType, iiCmd, iiSuccess, iiSeq;
        private bool disposedValue;
        
        public AuditLogVar init(ODBCSyncParam osp, IProgress<int> progress, string asParam, int aiSyncType)
        {
            _OdbcSyncParam = osp;
            isParam = asParam;
            iiSyncType = aiSyncType;
            iiCmd = _OdbcSyncParam.odbcGlobalVar.giCmd;

            _OdbcSyncParam._auditLogVar.LogId = _OdbcSyncParam.isLogTime;
            _OdbcSyncParam._auditLogVar.StartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//Set Audit Log Var
            if (iiCmd == 0)
            {
                if (progress != null) progress.Report(1);//Progress Bar
            }
            isAccessToken = GetAccessToken();
            string lsRet = syncOdbc(progress);
            if (iiCmd == 0)
            {
                if (progress != null) progress.Report(100);//Progress Bar
            }
            if (lsRet.Contains("Failed"))
            {
                _OdbcSyncParam._auditLogVar.Object = "Sync";//Set Audit Log Var
                _OdbcSyncParam._auditLogVar.ChildObject = "ODBC-Sync";//Set Audit Log Var
                _OdbcSyncParam._auditLogVar.Sequence = 1;//Set Audit Log Var
                _OdbcSyncParam._auditLogVar.ObjectFromTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//Set Audit Log Var
                _OdbcSyncParam._auditLogVar.EndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//Set Audit Log Var
                _OdbcSyncParam._auditLogVar.LogDetails = lsRet;//Set Audit Log Var
                _OdbcSyncParam._auditLogVar.Status = "Failed";//Set Audit Log Var
                _OdbcSyncParam.setStatusLog("", "", 2);
            }
            return _OdbcSyncParam._auditLogVar;
        }

        public static string syncOdbc(IProgress<int> progress)
        {
            try
            {
                _OdbcSyncParam.odbcGlobalVar._MasterConfig.GetFetchTableList(_OdbcSyncParam.odbcGlobalVar, 
                    iiSyncType, isParam, out iiSuccess, out isReturn);

                if (iiSuccess == 0)
                {
                    return isReturn;
                }

                JsonElement ljeTableList = _OdbcSyncParam.odbcGlobalVar.createJsonElement(isReturn);
                rCnt = ljeTableList.EnumerateArray().Count();
                if (rCnt > 0)
                {
                    int liTableType, liCustomCount = 0, liLoop, llAddDays, iiProgress1, iiProgress2;
                    long liTableRowCount;
                    string lsTableName, lsTableCode, lsTimeColName, lsBr;
                    string lsPkeyCols, lsSetUpdateQuery, lsColList, lsColsForInsert, lsWhereCondition, lsCustomList = "";
                    string lsFetchTable, lsTranitem, lsGroup = "", lsFromTime = "", lsXML;
                    string lsSql = "DECLARE @idoc INT, @doc VARCHAR(MAX); SET @doc = N'<Data>'; EXEC sp_xml_preparedocument @idoc OUTPUT, @doc;";


                    string lsResponseType, lsFailedResponse, lsSuccessResponse, lsApiType, lsParams, lsQuery, lsUrl;
                    int liStart;

                    JsonElement ljeCustomList = new JsonElement();

                    llAddDays = _OdbcSyncParam.odbcGlobalVar.maxDaysToSync;

                    iiProgress1 = 100 / rCnt;
                    for (int i = 0; i < rCnt; i++)
                    {
                        if (iiCmd == 0)
                        {
                            if (progress != null) progress.Report(i * iiProgress1);
                        }
                        _OdbcSyncParam.setStatusLog("sT", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 1);
                        lsTableCode = ljeTableList[i].GetProperty("tableCode").ToString();
                        lsTableName = ljeTableList[i].GetProperty("tableName").ToString();

                        liTableType = int.Parse(ljeTableList[i].GetProperty("tableType").ToString());
                        lsTimeColName = ljeTableList[i].GetProperty("timeColName").ToString();
                        isFinColName = ljeTableList[i].GetProperty("finColName").ToString();

                        /*lsBrColName = ljeTableList[i].GetProperty("brColName").ToString();
                        lsTranWiseColName = ljeTableList[i].GetProperty("tranColName").ToString();
                        lsDelColName = ljeTableList[i].GetProperty("delColName").ToString();*/

                        _OdbcSyncParam.setStatusLog("tblName", lsTableName, 1);

                        lsPkeyCols = "";
                        lsSetUpdateQuery = "";
                        lsColList = "";
                        lsColsForInsert = "";

                        //////////////////////////////////////////////////////////////////////////////
                        //Fetch Select & Create Table Script
                        //////////////////////////////////////////////////////////////////////////////
                        _OdbcSyncParam.setStatusLog("status", "Fetch Table S", 1);
                        _OdbcSyncParam.odbcGlobalVar._fun.fetchTable(_OdbcSyncParam.odbcGlobalVar, lsTableCode, out lsFetchTable, out isStatus, out isReturn);
                        if (isStatus == "Failed" || lsFetchTable == "" || lsFetchTable == null)
                        {
                            _OdbcSyncParam.setStatusLog("status", isReturn, 1);
                            _OdbcSyncParam.auditLogReset(2);
                            continue;
                        }
                        
                        lsFetchTable = lsFetchTable.Replace("@lsCode", "'" + _OdbcSyncParam.isCustCode + "'")
                            .Replace("@firmCode", _OdbcSyncParam.isFirmCode).Replace("@finYear", _OdbcSyncParam.isFinYear);

                        //////////////////////////////////////////////////////////////////////////////
                        //Split Parameters & Query
                        //////////////////////////////////////////////////////////////////////////////
                        isReturn = lsFetchTable.Substring(0, lsFetchTable.IndexOf("|"));

                        //Sequence Count / Itteration
                        iiSeq = int.TryParse(isReturn,out _) ? int.Parse(isReturn) : 9999;
                        lsFetchTable = lsFetchTable.Substring(lsFetchTable.IndexOf("|") + 1);

                        //Response Type(xml/json)
                        lsResponseType = lsFetchTable.Substring(0, lsFetchTable.IndexOf("|"));
                        lsFetchTable = lsFetchTable.Substring(lsFetchTable.IndexOf("|") + 1);

                        //Failure Response
                        lsFailedResponse = lsFetchTable.Substring(0, lsFetchTable.IndexOf("|"));
                        lsFetchTable = lsFetchTable.Substring(lsFetchTable.IndexOf("|") + 1);

                        //Success Response
                        lsSuccessResponse = lsFetchTable.Substring(0, lsFetchTable.IndexOf("|"));
                        lsFetchTable = lsFetchTable.Substring(lsFetchTable.IndexOf("|") + 1);

                        //API Call Type (fordate, created, modified)
                        lsApiType = lsFetchTable.Substring(0, lsFetchTable.IndexOf("|"));
                        lsFetchTable = lsFetchTable.Substring(lsFetchTable.IndexOf("|") + 1);

                        //Parameters for Replacing values like Company, Warehouse, etc.,
                        lsParams = lsFetchTable.Substring(0, lsFetchTable.IndexOf("|"));
                        lsFetchTable = lsFetchTable.Substring(lsFetchTable.IndexOf("|") + 1);

                        //Query to fetch values for the Parameters
                        lsQuery = lsFetchTable.Substring(0, lsFetchTable.IndexOf("|"));
                        lsFetchTable = lsFetchTable.Substring(lsFetchTable.IndexOf("|") + 1);

                        //URL for Fetch the Data
                        lsUrl = lsFetchTable.Substring(0, lsFetchTable.IndexOf("|"));
                        
                        lsFetchTable = lsFetchTable.Substring(lsFetchTable.IndexOf("|") + 1);

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
                                            out _, out lsColsForInsert, out lsSetUpdateQuery, out _, out isReturn, out _, out _);
                        if (isReturn.Contains("Failed"))
                        {
                            _OdbcSyncParam.setStatusLog("status", isReturn, 1);
                            _OdbcSyncParam.auditLogReset(2);
                            continue;
                        }

                        //////////////////////////////////////////////////////////////////////////////
                        //Fetch Custom Query (Customer Source Database)
                        //////////////////////////////////////////////////////////////////////////////
                        liCustomCount = 1;
                        int isCustLoop = 0;
                        if (lsQuery != "" && lsQuery != null)
                        {
                            lsCustomList = _OdbcSyncParam.brList("cust", "",lsQuery, 1);
                            if (lsCustomList != null && lsCustomList != "")
                            {
                                ljeCustomList = _OdbcSyncParam.odbcGlobalVar.createJsonElement(lsCustomList);
                                liCustomCount = ljeCustomList.EnumerateArray().Count();
                                isCustLoop = 1;
                            }
                        }

                        iiProgress2 = iiProgress1 / liCustomCount;
                        for (int custLoop = 0; custLoop < liCustomCount; custLoop++)
                        {
                            string lsUrlTemp = lsUrl;
                            if (iiCmd == 0)
                            {
                                if (progress != null) progress.Report((i * iiProgress1) + (custLoop * iiProgress2));
                            }
                            _OdbcSyncParam.setStatusLog("sT", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 1);
                            
                            lsBr = "";
                            lsWhereCondition = " Where [Table Name] = '" + lsTableName + "' ";
                            if (isCustLoop == 1)
                            {
                                /////////////////////////////////////////////////////
                                //////Replace URL with Company/Warehouse ERP Id//////
                                /////////////////////////////////////////////////////
                                string lsParamsTemp = lsParams, lsSplitParams;
                                int custLoopCount = 0;
                                while(lsParamsTemp.Length > 0)
                                {
                                    lsSplitParams = lsParamsTemp.IndexOf(',') > 0 ? lsParamsTemp.Substring(0, lsParamsTemp.IndexOf(',')) : lsParamsTemp;
                                    lsParamsTemp = lsParamsTemp.IndexOf(',') > 0 ? lsParamsTemp.Substring(lsParamsTemp.IndexOf(',') + 1) : "";
                                    if (custLoopCount == 0) lsBr = ljeCustomList[custLoop].GetProperty(lsSplitParams).ToString();
                                    lsUrlTemp = lsUrlTemp.Replace("<" + lsSplitParams + ">", ljeCustomList[custLoop].GetProperty(lsSplitParams).ToString());
                                    custLoopCount++;
                                }
                            }
                            else
                            {
                                lsBr = "-";
                            }

                            _OdbcSyncParam.setStatusLog("br", lsBr, 1);
                            if (lsBr == "-")
                            {
                                lsBr = "";
                            }
                            if (lsBr != "" && lsBr != null)
                            {
                                lsWhereCondition += " and br_code = '" + lsBr + "'";
                            }
                            if ((isFinColName != null && isFinColName != "") || (liTableType == 1))
                            {
                                lsWhereCondition += " and " + ((isFinColName == null || isFinColName == "") ? "[Fin Year]" : isFinColName) + " = " + _OdbcSyncParam.isFinYear;
                            }

                            //////////////////////////////////////////////////////////////////////////////
                            //Fetch lTime (Customer Database Custom Tables)
                            //////////////////////////////////////////////////////////////////////////////
                            if (lsUrl.Contains("<FromDate>"))
                            {
                                if (liTableType == 1 || liTableType == 2)
                                {
                                    _OdbcSyncParam.odbcGlobalVar._DestinationConfig.CheckMaxTime(_OdbcSyncParam.odbcGlobalVar,
                                        lsWhereCondition, out iiSuccess, out lsFromTime);
                                    if (iiSuccess == 0)
                                    {
                                        if (lsFromTime.Contains("No Rows Found"))
                                        {
                                            lsFromTime = "";
                                        }
                                        else
                                        {
                                            _OdbcSyncParam.setStatusLog("status", lsFromTime, 1);
                                            _OdbcSyncParam.auditLogReset(3);
                                            continue;
                                        }
                                    }
                                    if (lsFromTime == "" || lsFromTime == null)
                                    {
                                        lsFromTime = _OdbcSyncParam.odbcGlobalVar.defTime;
                                    }
                                    _OdbcSyncParam.setStatusLog("t", lsFromTime, 1);
                                }
                            }
                            else
                            {
                                lsTimeColName = "";
                            }

                            liLoop = 1;
                            liStart = 0;

                            while (liLoop >= 1)
                            {
                                string lsUrlTempMultiLoop = ReplaceURL(lsUrlTemp, liStart, lsFromTime, lsResponseType, isAccessToken);
                                liStart += iiSeq;

                                if (lsUrlTempMultiLoop.Contains("Failed - "))
                                {
                                    liLoop = 0;
                                    _OdbcSyncParam.setStatusLog("status", isReturn, 1);
                                    _OdbcSyncParam.auditLogReset(4);
                                    continue;
                                }
                                _OdbcSyncParam._auditLogVar.Sequence = liLoop;

                                //////////////////////////////////////////////////////////////////////////////
                                //Fetch Data from API
                                //////////////////////////////////////////////////////////////////////////////
                                lsXML = _RestAPI.getAPICalling(lsUrlTempMultiLoop, out _);
                                if(lsXML.Substring(0,9) == "Failed - ")
                                {
                                    liLoop = 0;
                                    _OdbcSyncParam.setStatusLog("status", isReturn, 1);
                                    _OdbcSyncParam.auditLogReset(4);
                                    continue;
                                }
                                lsXML = lsXML.Replace("'", "''");
                                /*
                                //////////////////////////////////////////////////////////////////////////////
                                //Delete from Destination Table (Customer Destination Database)
                                //////////////////////////////////////////////////////////////////////////////
                                if (lsDelColName != "" && lsDelColName != null)
                                {
                                    _OdbcSyncParam.setStatusLog("status", "Delete From Destination Table -- Start", 1);
                                    isSqlQuery = "DELETE FROM " + lsTableName + " WHERE " + lsDelColName
                                                    + " IN (SELECT distinct " + lsDelColName + " FROM(" + lsSelectQuery + ") a (" + lsColList + "));";
                                    
                                    _OdbcSyncParam.odbcGlobalVar._DestinationConfig.ExecuteRawQuery(_OdbcSyncParam.odbcGlobalVar,
                                        isSqlQuery, 1, out iiSuccess, out isReturn); 
                                    if (iiSuccess == 0)
                                    {
                                        liLoop = 0;
                                        _OdbcSyncParam.setStatusLog("status", isReturn, 1);
                                        _OdbcSyncParam.auditLogReset(4);
                                        continue;
                                    }
                                    _OdbcSyncParam.setStatusLog("status", "Deleted Successfully", 1);
                                }*/
                                int liSuccess = 0;
                                liTableRowCount = 0;

                                if(lsSuccessResponse != "" && lsSuccessResponse != null)
                                {
                                    if (lsXML.Contains(lsSuccessResponse) && !lsXML.Contains(lsFailedResponse))
                                    {
                                        liSuccess = 1;
                                    }
                                }
                                else
                                {
                                    isSqlQuery = lsSql.Replace("<Data>", lsXML) + " Select Count(*) From(" + lsFetchTable + ") as a";

                                    _OdbcSyncParam.odbcGlobalVar._DestinationConfig.ExecuteRawQuery(_OdbcSyncParam.odbcGlobalVar,
                                        isSqlQuery, 0, out iiSuccess, out isReturn, 0);

                                    long.TryParse(isReturn, out liTableRowCount);
                                }
                                

                                if (liSuccess == 1 || liTableRowCount > 0)
                                {
                                    //////////////////////////////////////////////////////////////////////////////
                                    //Execute the insert Query (Customer Destination Database)
                                    //////////////////////////////////////////////////////////////////////////////
                                    _OdbcSyncParam.setStatusLog("status", "Construct Insert", 1);

                                    isSqlQuery = lsSql.Replace("<Data>", lsXML)
                                        + "MERGE INTO " + lsTableName + " WITH(HOLDLOCK) AS a USING("
                                        + lsFetchTable + ") AS new_a (" + lsColList + ") ON " + lsPkeyCols
                                        + lsSetUpdateQuery + " WHEN NOT MATCHED THEN INSERT(" + lsColList
                                        + ")" + " VALUES(" + lsColsForInsert + ");";

                                    _OdbcSyncParam.setStatusLog("status", "Insert Start", 1);
                                    _OdbcSyncParam.odbcGlobalVar._DestinationConfig.ExecuteRawQuery(_OdbcSyncParam.odbcGlobalVar,
                                        isSqlQuery, 1, out iiSuccess, out isReturn); 

                                    if (iiSuccess == 0)
                                    {
                                        liLoop = 0;
                                        _OdbcSyncParam.setStatusLog("status", isReturn, 1);
                                        _OdbcSyncParam.auditLogReset(4);
                                        continue;
                                    }

                                    //Insert Max Time Table
                                    lsWhereCondition = "";
                                    if (lsBr == "")
                                    {
                                        lsBr = "-";
                                    }
                                    lsWhereCondition = "a.LSCode = '" + _OdbcSyncParam.isCustCode + "' And a.LSFirmCode = " + _OdbcSyncParam.isFirmCode + " And "
                                        + " a.[Fin Year] = " + _OdbcSyncParam.isFinYear + " and a.[Table Name] = '" + lsTableName + "' ";
                                    if (lsBr != "" && lsBr != null)
                                    {
                                        lsWhereCondition += " and a.br_code = '" + lsBr + "'";
                                    }
                                    else
                                    {
                                        lsWhereCondition += " and a.br_code = '-'";
                                    }
                                    lsWhereCondition += " and a.Tran_Type = '-'";
                                    lsTranitem = "'-'";
                                    lsGroup = "";
                                    
                                    if (lsTimeColName != "" && lsTimeColName != null)
                                    {

                                        isSqlQuery = lsSql.Replace("<Data>", lsXML)
                                            + "MERGE INTO LS_MaxTime WITH(HOLDLOCK) AS a USING("
                                            + "Select '" + lsBr + "'," + lsTranitem.Replace("b.", "") + ",Max(" + lsTimeColName + ") as maxTime From("
                                            + lsFetchTable + ") as maxTbl " + lsGroup + " ) AS b (br_code,Tran_Type,maxTime) ON " + lsWhereCondition
                                            + " WHEN MATCHED THEN UPDATE SET a.[Max Time] = b.maxTime"
                                            + " WHEN NOT MATCHED THEN INSERT("
                                            + "     LSCode,LSFirmCode,[Fin Year],[Table Name],br_code,Tran_Type,[Max Time])"
                                            + " VALUES('" + _OdbcSyncParam.isCustCode + "'," + _OdbcSyncParam.isFirmCode + ","
                                            + _OdbcSyncParam.isFinYear + ",'" + lsTableName + "','" + lsBr + "',"
                                            + lsTranitem + ",b.maxTime);";

                                        _OdbcSyncParam.odbcGlobalVar._DestinationConfig.ExecuteRawQuery(_OdbcSyncParam.odbcGlobalVar,
                                            isSqlQuery, 1, out iiSuccess, out isReturn);

                                        if (iiSuccess == 0)
                                        {
                                            liLoop = 0;
                                            _OdbcSyncParam.setStatusLog("status", isReturn, 1);
                                            _OdbcSyncParam.auditLogReset(4);
                                            continue;
                                        }
                                    }
                                    _OdbcSyncParam.setStatusLog("status", "Data Inserted - " + liLoop.ToString(), 1);
                                }
                                else
                                {
                                    if (lsFromTime != "" && lsFromTime != null)
                                    {
                                        if (Convert.ToDateTime(lsFromTime) > DateTime.Now)
                                        {
                                            liLoop = 0;
                                        }
                                        else
                                        {
                                            liStart = 0;
                                            lsFromTime = ((DateTime.Parse(lsFromTime))
                                                .AddDays(_OdbcSyncParam.odbcGlobalVar.maxDaysToSync)).ToString("yyyy-MM-dd HH:mm:ss");
                                        }
                                    }
                                    else
                                    {
                                        liLoop = 0;
                                        _OdbcSyncParam.setStatusLog("status", "No Row Found", 1);
                                        _OdbcSyncParam.setStatusLog("eT", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 1);
                                        _OdbcSyncParam.setStatusLog("stat", "Success", 1);
                                        _OdbcSyncParam.setStatusLog("", "", 2);
                                        continue;
                                    }
                                }

                                if (liLoop > 0)
                                {
                                    liLoop += 1;
                                }
                                _OdbcSyncParam.setStatusLog("eT", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 1);
                                _OdbcSyncParam.setStatusLog("stat", "Success", 1);
                                _OdbcSyncParam.setStatusLog("", "", 2);
                            }
                        }
                        _OdbcSyncParam.auditLogReset(1);
                    }
                }

                _OdbcSyncParam.setStatusLog("tblName", "Success", 1);
                _OdbcSyncParam.setStatusLog("status", "Successfully ", 1);
                return "Success";
            }
            catch (Exception e)
            {
                return "Failed " + e.Message;
            }
        }


        /////////////////////////////////////////////////////////////////////
        //Replace URL Parameters
        /////////////////////////////////////////////////////////////////////
        public static string ReplaceURL(string asUrl, int aiStart, string asFromDate, string asResType, string asAccessToken)
        {
            try
            {
                if (iiSeq > 0 && iiSeq < 9999)
                {
                    asUrl = asUrl.Replace("<StartSeq>", aiStart.ToString()).Replace("<EndSeq>", (aiStart + iiSeq).ToString());
                }

                if (asFromDate != "" && asFromDate != null && DateTime.TryParse(asFromDate, out _))
                {
                    asUrl = asUrl.Replace("<FromDate>", (DateTime.Parse(asFromDate)).ToString("yyyy-MM-dd"))
                        .Replace("<ToDate>", ((DateTime.Parse(asFromDate)).AddDays(_OdbcSyncParam.odbcGlobalVar.maxDaysToSync)).ToString("yyyy-MM-dd").ToString());
                }

                asUrl = asUrl.Replace("<ResType>", asResType).Replace("<Token>", asAccessToken);
            }
            catch(Exception Ex)
            {
                asUrl = "Failed - " + Ex.Message;
            }
            return asUrl;
        }

        /////////////////////////////////////////////////////////////////////
        //Get Access Token
        /////////////////////////////////////////////////////////////////////
        private string GetAccessToken()
        {
            try
            {
                string lsUrl = "https://api.bizom.in/oauth/directLogin/xml";
                string lsBody = "<User><username>" + _OdbcSyncParam.isODBCUID + "</username><password>" + _OdbcSyncParam.isODBCPwd + "</password></User>";
                if (_OdbcSyncParam.odbcGlobalVar.staging == "_Stage")
                {
                    lsUrl = "https://stagingapi.bizomstaging.in/oauth/directLogin/xml";
                }
                lsUrl = "https://stagingapi.bizomstaging.in/oauth/directLogin/xml";

                isReturn = _RestAPI.postAPICalling(lsUrl, "application/x-www-form-urlencoded", lsBody,out _);

                if (isReturn.Substring(0, 8) != "Failed -")
                {
                    if (isReturn.Contains("<Token>") && isReturn.Contains("</Token>"))
                    {
                        isReturn = isReturn.Substring(isReturn.IndexOf("<Token>") + 7);
                        isReturn = isReturn.Substring(0, isReturn.IndexOf("</Token>"));
                    }
                    else
                    {
                        isReturn = "Failed - No Token Found";
                    }
                }
            }
            catch(Exception Ex)
            {
                isReturn = "Failed - " + Ex.Message;
            }
            return isReturn;
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    GC.Collect();
                }

                disposedValue = true;
            }
        }
    }
}
