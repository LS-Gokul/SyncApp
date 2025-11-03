using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json; 
using System.Xml;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LSSyncApp
{
    public class APIQuery
    {
        public static ODBCSyncParam _apiSync = new ODBCSyncParam();
        public static string isReturn, isParam, isFinColName, isStatus, isSqlQuery;
        public static int rCnt, iiSyncType, iiCmd, iiResType = 0, iiSuccess;
        private bool disposedValue;

        public void init(ODBCSyncParam sync, IProgress<int> progress, string asParam, int aiSyncType)
        {
            _apiSync = sync;
            isParam = asParam;
            iiSyncType = aiSyncType;
            iiCmd = _apiSync.odbcGlobalVar.giCmd;

            //Audit Log
            _apiSync._auditLogVar.LogId = _apiSync.isLogTime;//Set Audit Log Var
            _apiSync._auditLogVar.StartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//Set Audit Log Var
            if (iiCmd == 0)
            {
                if (progress != null) progress.Report(1);//Progress Bar
            }
            Task<string> lsRet = syncSybaseDB(progress);
            if (iiCmd == 0)
            {
                if (progress != null) progress.Report(100);//Progress Bar
            }
            if (lsRet.Result.Contains("Failed"))
            {
                _apiSync._auditLogVar.Object = "Sync";//Set Audit Log Var
                _apiSync._auditLogVar.ChildObject = "API-Sync";//Set Audit Log Var
                _apiSync._auditLogVar.Sequence = 1;//Set Audit Log Var
                _apiSync._auditLogVar.ObjectFromTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//Set Audit Log Var
                _apiSync._auditLogVar.EndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//Set Audit Log Var
                _apiSync._auditLogVar.LogDetails = lsRet.Result;//Set Audit Log Var
                _apiSync._auditLogVar.Status = "Failed";//Set Audit Log Var
                _apiSync.setStatusLog("", "", 2);
            }
        }

        //////////////////////////////////////////////////////////////////////////////
        //Calling Webservice for getting Table List in JSON Format
        //////////////////////////////////////////////////////////////////////////////
        private async Task<string> syncSybaseDB(IProgress<int> progress)
        {
            try
            {
                _apiSync.odbcGlobalVar._MasterConfig.GetFetchTableList(_apiSync.odbcGlobalVar, iiSyncType, isParam, out iiSuccess, out isReturn);
                if(iiSuccess == 0)
                {
                    return isReturn;
                }
                JsonElement ljeTableList = _apiSync.odbcGlobalVar.createJsonElement(isReturn);
                rCnt = ljeTableList.EnumerateArray().Count();
                if (rCnt > 0)
                {
                    int liInsType, liTableType, liCreateTable = 0, liBrLoop, liBrCount = 0, liLoop, llAddDays, iiProgress1, iiProgress2;
                    long liTableRowCount, llMaxRowCount, liTranCount;
                    string lsTableName, lsTableCode, lsBrColName, lsTimeColName, lsBr, lsTranWiseColName, lsDelColName, lsTran, lsTranTime = "";
                    string lsPkeyCols, lsSetUpdateQuery, lsColList, lsColsForInsert, lsJsonConList, lsWhereCondition, lsBrList = "", lsSelJsonColList;
                    string lsFetchTable, lsJson, lsSelqctQuery, lsFetchTableTemp, lsTranitem, lsInsertTran, lsGroup = "";
                    string lsCount, lsFromTime = "", lsToTime;
                    JsonElement ljeBrList = new JsonElement();
                    JsonElement ljeTranList = new JsonElement();

                    llAddDays = _apiSync.odbcGlobalVar.maxDaysToSync;
                    llMaxRowCount = _apiSync.odbcGlobalVar.maxRowCount;

                    iiProgress1 = 100 / rCnt;
                    for (int i = 0; i < rCnt; i++)
                    {
                        if (iiCmd == 0)
                        {
                            if (progress != null) progress.Report(i * iiProgress1);
                        }
                        _apiSync.setStatusLog("sT", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 1);
                        lsTableCode = ljeTableList[i].GetProperty("tableCode").ToString();
                        lsTableName = ljeTableList[i].GetProperty("tableName").ToString();

                        liTableType = int.Parse(ljeTableList[i].GetProperty("tableType").ToString());

                        lsBrColName = ljeTableList[i].GetProperty("brColName").ToString();
                        lsTimeColName = ljeTableList[i].GetProperty("timeColName").ToString();
                        lsTranWiseColName = ljeTableList[i].GetProperty("tranColName").ToString();
                        isFinColName = ljeTableList[i].GetProperty("finColName").ToString();
                        lsDelColName = ljeTableList[i].GetProperty("delColName").ToString();

                        _apiSync.setStatusLog("tblName", lsTableName, 1);

                        lsPkeyCols = "";
                        lsSetUpdateQuery = "";
                        lsColList = "";
                        lsColsForInsert = "";
                        lsJsonConList = "";

                        //////////////////////////////////////////////////////////////////////////////
                        //Fetch Select & Create Table Script
                        //////////////////////////////////////////////////////////////////////////////
                        _apiSync.setStatusLog("status", "Fetch Table S", 1);
                        _apiSync.odbcGlobalVar._fun.fetchTable(_apiSync.odbcGlobalVar, lsTableCode, out lsFetchTable, out isStatus, out isReturn);
                        if (isStatus == "Failed")
                        {
                            _apiSync.setStatusLog("status", isReturn, 1);
                            _apiSync.auditLogReset(2);
                            continue;
                        }
                        lsFetchTableTemp = lsFetchTable.Replace("string_agg(", "List(").Replace("@finYear", _apiSync.isFinYear);

                        //////////////////////////////////////////////////////////////////////////////
                        //Column List Fetch (Customer Source Database)
                        //////////////////////////////////////////////////////////////////////////////
                        _apiSync.odbcGlobalVar._DestinationConfig.ExecuteRawQuery(_apiSync.odbcGlobalVar, "", 2, out iiSuccess, out isReturn);
                        if (iiSuccess == 0)
                        {
                            _apiSync.setStatusLog("status", "Failed to Connect Customer Server on Checking Table Exists", 1);
                            _apiSync.auditLogReset(2);
                            continue;
                        }
                        _apiSync.getColList(lsTableName, _apiSync.isCustCode, _apiSync.isFirmCode, out lsPkeyCols, out lsColList,
                                            out lsJsonConList, out lsColsForInsert, out lsSetUpdateQuery, out lsSelJsonColList, out isReturn, out liInsType, out _);
                        if (isReturn.Contains("Failed"))
                        {
                            _apiSync.setStatusLog("status", isReturn, 1);
                            _apiSync.auditLogReset(2);
                            continue;
                        }

                        //////////////////////////////////////////////////////////////////////////////
                        //Branch Fetch (Customer Source Database)
                        //////////////////////////////////////////////////////////////////////////////
                        if (liBrCount <= 1)
                        {
                            if (lsBrList == null || lsBrList == "")
                            {
                                lsBrList = brList("cust");
                                if (lsBrList != null && lsBrList != "" && !lsBrList.Contains("Failed"))
                                {
                                    ljeBrList = _apiSync.odbcGlobalVar.createJsonElement(lsBrList);
                                    liBrCount = ljeBrList.EnumerateArray().Count();
                                }
                            }
                        }
                        liBrLoop = 1;
                        if (lsBrColName != null && lsBrColName != "")
                        {
                            liBrLoop = liBrCount;
                        }
                        iiProgress2 = iiProgress1 / liBrLoop;
                        for (int br = 0; br < liBrLoop; br++)
                        {
                            if (iiCmd == 0)
                            {
                                if (progress != null) progress.Report((i * iiProgress1) + (br * iiProgress2));
                            }
                            _apiSync.setStatusLog("sT", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 1);
                            lsBr = "";
                            lsWhereCondition = " Where [Table Name] = '" + lsTableName + "' ";
                            if (lsBrColName != null && lsBrColName != "")
                            {
                                lsBr = ljeBrList[br].GetProperty("brCode").ToString();
                            }
                            else
                            {
                                lsBr = "-";
                            }
                            _apiSync.setStatusLog("br", lsBr, 1);
                            if (lsBr == "-")
                            {
                                lsBr = "";
                            }
                            if (lsBr != "" && lsBr != null && lsBrColName != null && lsBrColName != "")
                            {
                                lsWhereCondition += " and " + lsBrColName + " = '" + lsBr + "'";
                            }
                            if ((isFinColName != null && isFinColName != "") || (liTableType == 1))
                            {
                                lsWhereCondition += " and " + ((isFinColName == null || isFinColName == "") ? "[Fin Year]" : isFinColName) + " = " + _apiSync.isFinYear;
                            }
                            //////////////////////////////////////////////////////////////////////////////
                            //Fetch lTime (Customer Database Custom Tables)
                            //////////////////////////////////////////////////////////////////////////////
                            if (lsTimeColName != "" && lsTimeColName != null)
                            {
                                if (liTableType == 1 || liTableType == 2)
                                {
                                    if (lsTranWiseColName != null && lsTranWiseColName != "")
                                    {
                                        isSqlQuery = tranTypeFetchMax(lsTableName, lsTimeColName, lsTranWiseColName, lsFetchTable, lsBrColName, lsBr, lsWhereCondition, out lsInsertTran);
                                        if (lsInsertTran != "" && lsInsertTran != null)
                                        {
                                            _apiSync.odbcGlobalVar._DestinationConfig.ExecuteRawQuery(_apiSync.odbcGlobalVar,
                                                lsInsertTran, 1, out iiSuccess, out isReturn);
                                            if (iiSuccess == 0)
                                            {
                                                _apiSync.setStatusLog("status", isReturn, 1);
                                                _apiSync.auditLogReset(3);
                                                continue;
                                            }
                                        }

                                        _apiSync.odbcGlobalVar._DestinationConfig.ExecuteRawQuery(_apiSync.odbcGlobalVar,
                                                isSqlQuery, 0, out iiSuccess, out lsTranTime, 0);
                                        if (iiSuccess == 0)
                                        {
                                            _apiSync.setStatusLog("status", lsTranTime, 1);
                                            _apiSync.auditLogReset(3);
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        if (liCreateTable == 1)
                                        {
                                            lsFromTime = _apiSync.odbcGlobalVar.defTime;
                                        }
                                        else
                                        {
                                            _apiSync.odbcGlobalVar._DestinationConfig.CheckMaxTime(_apiSync.odbcGlobalVar,
                                                lsWhereCondition, out iiSuccess, out lsFromTime);
                                            if (iiSuccess == 0)
                                            {
                                                if (lsFromTime.Contains("No Rows Found"))
                                                {
                                                    lsFromTime = "";
                                                }
                                                else
                                                {
                                                    _apiSync.setStatusLog("status", lsFromTime, 1);
                                                    _apiSync.auditLogReset(3);
                                                    continue;
                                                }
                                            }
                                            if (lsFromTime == "" || lsFromTime == null)
                                            {
                                                lsFromTime = _apiSync.odbcGlobalVar.defTime;
                                            }
                                        }
                                        _apiSync.setStatusLog("t", lsFromTime, 1);
                                    }
                                }
                            }

                            //////////////////////////////////////////////////////////////////////////////
                            //Execute the Select Statement (Customer Source Database)
                            //////////////////////////////////////////////////////////////////////////////
                            isReturn = await PostUrlAsync("http://" + _apiSync.ip + ":" + _apiSync.port + "/ws_leapsurge_service", "valueType=checkStatus");
                            if (isReturn != "Success")
                            {
                                _apiSync.setStatusLog("status", "Failed to Connect Customer Source Server", 1);
                                _apiSync.auditLogReset(3);
                                continue;
                            }

                            liLoop = 1;
                            while (liLoop >= 1)
                            {
                                lsToTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                _apiSync.setStatusLog("sT", lsToTime, 1);
                                _apiSync._auditLogVar.Sequence = liLoop;
                                //Row Count (Customer Source Database)
                                if (lsTranWiseColName != null && lsTranWiseColName != "" && lsTimeColName != "" && lsTimeColName != null)
                                {
                                    string lsTranTimeList = "";
                                    ljeTranList = _apiSync.odbcGlobalVar.createJsonElement(lsTranTime);
                                    liTranCount = ljeTranList.EnumerateArray().Count();
                                    lsFetchTable = lsFetchTableTemp;
                                    for (int tran = 0; tran < liTranCount; tran++)
                                    {
                                        lsTran = ljeTranList[tran].GetProperty("tranType").ToString();
                                        lsFromTime = ljeTranList[tran].GetProperty("maxTime").ToString();
                                        lsTranTimeList += lsTran + " -> " + lsFromTime + " | ";
                                        lsFetchTable = lsFetchTable.Replace(lsTran + "@fromTime", lsFromTime).Replace(lsTran + "@toTime", lsToTime).Replace("@brCode", lsBr);
                                    }
                                    _apiSync.setStatusLog("t", lsTranTimeList, 1);
                                }
                                else
                                {
                                    lsFetchTable = lsFetchTableTemp.Replace("@fromTime", lsFromTime).Replace("@toTime", lsToTime).Replace("@brCode", lsBr);
                                }
                                lsCount = "0";
                                
                                isSqlQuery = "SELECT COUNT(*) AS cnt FROM (" + lsFetchTable + ") as a";
                                lsCount = await PostUrlAsync("http://" + _apiSync.ip + ":" + _apiSync.port + "/ws_leapsurge_service", "execQueryCount=?query=" + isSqlQuery);
                                if (lsCount.Contains("Failed"))
                                {
                                    liLoop = 0;
                                    _apiSync.setStatusLog("status", "Failed to Fetch Number of Rows in Source", 1);
                                    _apiSync.auditLogReset(3);
                                    continue;
                                }
                                
                                if (long.TryParse(lsCount, out _))
                                {
                                    liTableRowCount = long.Parse(lsCount);
                                    _apiSync.setStatusLog("s", liTableRowCount.ToString(), 1);
                                }
                                else
                                {
                                    _apiSync.setStatusLog("status", lsCount, 1);
                                    liTableRowCount = 0;
                                }

                                if (liTableRowCount > 0)
                                {
                                    if (liTableRowCount > llMaxRowCount)
                                    {
                                        if (lsTranWiseColName != null && lsTranWiseColName != "" && lsTimeColName != "" && lsTimeColName != null)
                                        {
                                            ljeTranList = _apiSync.odbcGlobalVar.createJsonElement(lsTranTime);
                                            liTranCount = ljeTranList.EnumerateArray().Count();
                                            lsFetchTable = lsFetchTableTemp;
                                            for (int tran = 0; tran < liTranCount; tran++)
                                            {
                                                lsTran = ljeTranList[tran].GetProperty("tranType").ToString();
                                                lsFromTime = ljeTranList[tran].GetProperty("maxTime").ToString();
                                                lsToTime = Convert.ToDateTime(lsFromTime).AddDays(llAddDays).ToString("yyyy-MM-dd HH:mm:ss");
                                                lsFetchTable = lsFetchTable.Replace(lsTran + "@fromTime", lsFromTime).Replace(lsTran + "@toTime", lsToTime).Replace("@brCode", lsBr);

                                                dynamic jsonObject = JsonConvert.DeserializeObject(lsTranTime);
                                                jsonObject[tran].maxTime = lsToTime;
                                                lsTranTime = JsonConvert.SerializeObject(jsonObject);
                                            }
                                        }
                                        else if (lsTimeColName != "" && lsTimeColName != null)
                                        {
                                            
                                            lsFromTime = Convert.ToDateTime(_apiSync.odbcGlobalVar.odbcConn.srcDBExecQueryRetOne("SELECT MIN(" +
                                                    lsTimeColName + ") FROM (" + lsFetchTable + ") as a(" + lsColList + ")",
                                                    _apiSync.isODBCServer, _apiSync.isODBCUID, _apiSync.isODBCPwd)).ToString("yyyy-MM-dd HH:mm:ss");
                                            
                                            if (lsFromTime.Contains("|Failed"))
                                            {
                                                liLoop = 0;
                                                _apiSync.setStatusLog("status", lsFromTime, 1);
                                                _apiSync.auditLogReset(4);
                                                continue;
                                            }
                                            lsToTime = Convert.ToDateTime(lsFromTime).AddDays(llAddDays).ToString("yyyy-MM-dd HH:mm:ss");
                                            lsFetchTable = lsFetchTableTemp.Replace("@fromTime", lsFromTime).Replace("@toTime", lsToTime).Replace("@brCode", lsBr);
                                            lsFromTime = lsToTime;
                                        }
                                        else
                                        {
                                            liLoop = 0;
                                        }
                                    }
                                    else
                                    {
                                        liLoop = 0;
                                    }
                                    _apiSync.setStatusLog("status", "Fetch From Source Table", 1);
                                    lsSelqctQuery = "";

                                    isSqlQuery = "Select " + lsColList + " From (" + lsFetchTable + ") as a(" + lsColList + ")";
                                    isReturn = await PostUrlAsync("http://" + _apiSync.ip + ":" + _apiSync.port + "/ws_leapsurge_service", "execQuery=?query=" + isSqlQuery);
                                    if (isReturn == "" || isReturn == null || isReturn.Contains("Failed"))
                                    {
                                        if (isReturn.Contains("Failed"))
                                        {
                                            liLoop = 0;
                                            _apiSync.setStatusLog("status", isReturn, 1);
                                            _apiSync.auditLogReset(4);
                                        }
                                        continue;
                                    }
                                    
                                    XmlDocument doc = new XmlDocument();
                                    doc.LoadXml(isReturn);

                                    lsJson = JsonConvert.SerializeXmlNode(doc);//,Formatting.None, true);
                                    lsJson = _apiSync.odbcGlobalVar.replaceSpecialCharacters(lsJson);

                                    lsSelqctQuery = "SELECT " + lsSelJsonColList.ToUpper() + " FROM OPENJSON('" + lsJson.ToUpper() + "','$.ROOT.DATA') WITH(" + lsJsonConList.ToUpper() + ")";

                                    //////////////////////////////////////////////////////////////////////////////
                                    //Delete from Destination Table (Customer Destination Database)
                                    //////////////////////////////////////////////////////////////////////////////
                                    if (lsDelColName != "" && lsDelColName != null)
                                    {
                                        _apiSync.setStatusLog("status", "Delete From Destination Table -- Start", 1);
                                        isSqlQuery = "DELETE FROM " + lsTableName + " WHERE " + lsDelColName
                                                        + " IN (SELECT distinct " + lsDelColName + " FROM(" + lsSelqctQuery + ") a (" + lsColList + "));";

                                        _apiSync.odbcGlobalVar._DestinationConfig.ExecuteRawQuery(_apiSync.odbcGlobalVar,
                                                isSqlQuery, 1, out iiSuccess, out isReturn);
                                        if (iiSuccess == 0)
                                        {
                                            liLoop = 0;
                                            _apiSync.setStatusLog("status", isReturn, 1);
                                            _apiSync.auditLogReset(4);
                                            continue;
                                        }
                                        _apiSync.setStatusLog("status", "Deleted Successfully", 1);
                                    }

                                    //////////////////////////////////////////////////////////////////////////////
                                    //Execute the insert Query (Customer Destination Database)
                                    //////////////////////////////////////////////////////////////////////////////
                                    _apiSync.setStatusLog("status", "Construct Insert", 1);

                                    if (liInsType == 1)
                                    {
                                        isSqlQuery = "MERGE INTO " + lsTableName + " WITH(HOLDLOCK) AS a USING("
                                                        + lsSelqctQuery + ") AS new_a (" + lsColList + ") ON " + lsPkeyCols
                                                        + lsSetUpdateQuery + " WHEN NOT MATCHED THEN INSERT(" + lsColList
                                                        + ")" + " VALUES(" + lsColsForInsert + ");";
                                    }
                                    else
                                    {
                                        isSqlQuery = "INSERT INTO " + lsTableName + "(" + lsColList + ") "
                                                        + " SELECT " + lsColList + " FROM ("
                                                        + lsSelqctQuery + ") AS tbl (" + lsColList + ");";
                                    }

                                    _apiSync.setStatusLog("status", "Insert Start", 1);
                                    
                                    _apiSync.odbcGlobalVar._DestinationConfig.ExecuteRawQuery(_apiSync.odbcGlobalVar,
                                                isSqlQuery, 1, out iiSuccess, out isReturn);
                                    if (isReturn.Contains("Failed"))
                                    {
                                        liLoop = 0;
                                        _apiSync.setStatusLog("status", isReturn, 1);
                                        _apiSync.auditLogReset(4);
                                        continue;
                                    }

                                    //Insert Max Time Table
                                    lsWhereCondition = "";
                                    if (lsBr == "")
                                    {
                                        lsBr = "-";
                                    }
                                    lsWhereCondition = " a.[Fin Year] = " + _apiSync.isFinYear + " and a.[Table Name] = '" + lsTableName + "' ";
                                    if (lsBr != "" && lsBr != null && lsBrColName != null && lsBrColName != "")
                                    {
                                        lsWhereCondition += " and a.br_code = '" + lsBr + "'";
                                    }
                                    else
                                    {
                                        lsWhereCondition += " and a.br_code = '-'";
                                    }

                                    if (lsTranWiseColName != null && lsTranWiseColName != "")
                                    {
                                        lsWhereCondition += " and a.Tran_Type = b." + lsTranWiseColName;
                                        lsTranitem = "b." + lsTranWiseColName;
                                        lsGroup = "group by " + lsTranWiseColName;
                                    }
                                    else
                                    {
                                        lsWhereCondition += " and a.Tran_Type = '-'";
                                        lsTranitem = "'-'";
                                        lsGroup = "";
                                    }

                                    if (liLoop == 0)
                                    {
                                        if (lsTimeColName != "" && lsTimeColName != null)
                                        {

                                            isSqlQuery = "MERGE INTO LS_MaxTime WITH(HOLDLOCK) AS a USING("
                                                            + "Select '" + lsBr + "'," + lsTranitem.Replace("b.", "") + ",Max(" + lsTimeColName + ") as maxTime From("
                                                            + lsSelqctQuery + ") as maxTbl " + lsGroup + " ) AS b (br_code,Tran_Type,maxTime) ON " + lsWhereCondition
                                                            + " WHEN MATCHED THEN UPDATE SET a.[Max Time] = b.maxTime"
                                                            + " WHEN NOT MATCHED THEN INSERT("
                                                            + "[Fin Year],[Table Name],br_code,Tran_Type,[Max Time])"
                                                            + " VALUES(" + _apiSync.isFinYear + ",'" + lsTableName + "','" + lsBr + "',"
                                                            + lsTranitem + ",b.maxTime);";
                                            
                                            _apiSync.odbcGlobalVar._DestinationConfig.ExecuteRawQuery(_apiSync.odbcGlobalVar,
                                                isSqlQuery, 1, out iiSuccess, out isReturn);
                                            if (isReturn.Contains("Failed"))
                                            {
                                                liLoop = 0;
                                                _apiSync.setStatusLog("status", isReturn, 1);
                                                _apiSync.auditLogReset(4);
                                                continue;
                                            }
                                        }
                                    }
                                    _apiSync.setStatusLog("status", "Data Inserted", 1);
                                }
                                else
                                {
                                    liLoop = 0;
                                    _apiSync.setStatusLog("status", "No Row Found", 1);
                                    _apiSync.setStatusLog("eT", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 1);
                                    _apiSync.setStatusLog("stat", "Success", 1);
                                    _apiSync.setStatusLog("", "", 2);
                                    continue;
                                }
                                if (lsFromTime != "" && lsFromTime != null)
                                {
                                    if (Convert.ToDateTime(lsFromTime) > DateTime.Now || Convert.ToDateTime(lsToTime) > DateTime.Now)
                                    {
                                        liLoop = 0;
                                    }
                                }
                                else
                                {
                                    liLoop = 0;
                                }
                                if (liLoop > 0)
                                {
                                    liLoop += 1;
                                }
                                _apiSync.setStatusLog("eT", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 1);
                                _apiSync.setStatusLog("stat", "Success", 1);
                                _apiSync.setStatusLog("", "", 2);
                            }
                        }
                        _apiSync.auditLogReset(1);
                    }
                }

                _apiSync.setStatusLog("tblName", "Success", 1);
                _apiSync.setStatusLog("status", "Successfully ", 1);
                return "Success";
            }
            catch (Exception e)
            {
                return "Failed " + e.Message;
            }
        }

        /////////////////////////////////////////////////////////////////////
        //Branch List Fetch
        /////////////////////////////////////////////////////////////////////
        public static string brList(string fetchDB)
        {
            string lsBrList;
            int licnt;
            _apiSync.odbcGlobalVar._DestinationConfig.ExecuteRawQuery(_apiSync.odbcGlobalVar, "", 2, out iiSuccess, out lsBrList);
            if (iiSuccess == 0)
            {
                _apiSync.setStatusLog("status", "Failed to Connect Customer Server on Checking Table Exists", 1);
                lsBrList = "";
            }
            else
            {
                isSqlQuery = "SELECT COUNT(*) FROM sys.tables WHERE name = 'LS_Branch_Master'";
                _apiSync.odbcGlobalVar._DestinationConfig.ExecuteRawQuery(_apiSync.odbcGlobalVar, isSqlQuery, 0, out iiSuccess, out lsBrList, 0);
                if (int.TryParse(lsBrList, out _))
                {
                    licnt = int.Parse(lsBrList);
                    if (licnt > 0)
                    {
                        _apiSync.odbcGlobalVar._DestinationConfig.GetBranchList(_apiSync.odbcGlobalVar, out iiSuccess, out lsBrList);
                    }
                    else
                    {
                        lsBrList = "";
                    }
                }
                else
                {
                    lsBrList = "";
                }
            }
            return lsBrList;
        }

        /////////////////////////////////////////////////////////////////////
        //Trans Type Column List and Max time Fetch
        /////////////////////////////////////////////////////////////////////
        public static string tranTypeFetchMax(string tableName, string dateColName, string tranColName, string fetchSql,
            string brColName, string br, string whereCondition, out string tranInsert)
        {
            string lsReturn = "", lsFetchSql, lsTranColName, lsTransList, lsWhere = whereCondition, lsTranCol, lsInsertTran = "", lsBr;
            int i = 0;
            lsBr = br == "" ? ('-').ToString() : br;
            if (dateColName != "" && dateColName != null && tranColName != "" && tranColName != null)
            {
                lsTranColName = _apiSync.odbcGlobalVar.reverseString(tranColName.Replace("]", "").Replace("[", ""));
                lsFetchSql = _apiSync.odbcGlobalVar.reverseString(fetchSql.Replace("]", "").Replace("[", ""));

                while (lsFetchSql.Contains(lsTranColName))
                {
                    lsFetchSql = lsFetchSql.Substring(lsFetchSql.IndexOf(lsTranColName));
                    lsTransList = lsFetchSql.Substring(0, lsFetchSql.IndexOf(','));
                    lsFetchSql = lsFetchSql.Substring(lsFetchSql.IndexOf(','));
                    if (lsTransList.Contains("'"))
                    {
                        if (!lsReturn.Contains(_apiSync.odbcGlobalVar.reverseString(lsTransList)))
                        {
                            lsTranCol = _apiSync.odbcGlobalVar.reverseString(lsTransList);
                            lsInsertTran += "Select " + _apiSync.isFinYear + " as finYear,'" + tableName + "' as tblName,'" + lsBr
                                    + "' as brcode," + lsTranCol + ",'" + _apiSync.odbcGlobalVar.defTime + "' as maxTime {Union}";
                            lsReturn += "Select IsNull(CONVERT(Varchar,[Max Time],120),'" + _apiSync.odbcGlobalVar.defTime + "') as maxTime, "
                                + lsTranCol + " From LS_MaxTime " + lsWhere + " and " + lsTranCol.Replace(" as ", " = ") + " {Union}";
                        }
                    }
                    i += 1;
                }
                if (i > 0)
                {
                    lsInsertTran = lsInsertTran.Replace("{Union}Select", " Union All Select").Replace("{Union}", "");

                    lsInsertTran = "MERGE INTO LS_MaxTime WITH(HOLDLOCK) AS a USING(" + lsInsertTran + ") as b(finYear,tblName,br_code,Tran_Type,maxTime)"
                            + " ON a.[Fin Year] = b.finYear And a.[Table Name] = b.tblName And a.br_code = b.br_code and a.Tran_Type = b.Tran_Type "
                            + " WHEN NOT MATCHED THEN INSERT([Fin Year],[Table Name],br_code,Tran_Type,[Max Time])"
                            + " Values(b.finYear,b.tblName,b.br_code,b.Tran_Type,b.maxTime);";

                    lsReturn = lsReturn.Replace("{Union}Select", " Union All Select").Replace("{Union}", "");
                    lsReturn = "Select '[' + String_Agg('{\"tranType\": \"' + " + _apiSync.odbcGlobalVar.reverseString(lsTranColName) + " + '\",\"maxTime\": \"' + maxTime + '\"}' , ',') + ']' From(" + lsReturn + ") a";
                }
            }
            tranInsert = lsInsertTran;
            return lsReturn;
        }

        //////////////////////////////////////////////////////////////////////////////
        //Posting API.
        //////////////////////////////////////////////////////////////////////////////
        private async Task<string> PostUrlAsync(string aURL, string asbody)
        {
            iiResType = 0;
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(900);
            HttpResponseMessage Res;
            string lsResult, lsFile = "c:\\WriteText.txt";
            try
            {
                asbody = asbody.Replace("\t", "&#09;");
                StringContent TXML = new StringContent(asbody, Encoding.UTF8, "application/x-www-form-urlencoded");
                Res = await client.PostAsync(aURL, TXML);
                Res.EnsureSuccessStatusCode();
                var byteArray = await Res.Content.ReadAsByteArrayAsync();
                try
                {
                    lsResult = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
                    iiResType = 1;
                }
                catch
                {
                    try
                    {
                        File.WriteAllText(lsFile, Encoding.UTF8.GetString(byteArray, 0, byteArray.Length));
                        iiResType = 2;
                        lsResult = lsFile;
                    }
                    catch (Exception Ex)
                    {
                        iiResType = 0;
                        lsResult = "Failed on File Creation - " + Ex.Message;
                    }
                }
            }
            catch (Exception E)
            {
                iiResType = 0;
                lsResult = "Failed on API Request - " + E.Message;
            }
            return lsResult;
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
                    // TODO: dispose managed state (managed objects)
                }

                disposedValue = true;
            }
        }
    }
}
