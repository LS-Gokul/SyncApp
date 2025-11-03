using Microsoft.VisualBasic;
using System;
using System.Xml;
using System.Linq;
using Newtonsoft.Json;
using System.Text.Json; 
using System.Data;

namespace LSSyncApp.Controllers
{
    public class Busy
    {
        public static ODBCSyncParam _OdbcSyncParam = new ODBCSyncParam();
        public static string isReturn, isParam, isFinColName, isStatus, isSqlQuery;
        public static int rCnt, iiSyncType, iiCmd, iiSuccess;
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
            string lsRet = syncOdbc(progress);
            if (iiCmd == 0)
            {
                if (progress != null) progress.Report(100);//Progress Bar
            }
            if (lsRet.Contains("Failed"))
            {
                _OdbcSyncParam._auditLogVar.Object = "Sync";//Set Audit Log Var
                _OdbcSyncParam._auditLogVar.ChildObject = "Busy-Sync";//Set Audit Log Var
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
                _OdbcSyncParam.odbcGlobalVar._MasterConfig.GetFetchTableList(_OdbcSyncParam.odbcGlobalVar, iiSyncType, isParam, out iiSuccess, out isReturn);

                if (iiSuccess == 0)
                {
                    return isReturn;
                }

                JsonElement ljeTableList = _OdbcSyncParam.odbcGlobalVar.createJsonElement(isReturn);
                rCnt = ljeTableList.EnumerateArray().Count();
                if (rCnt > 0)
                {
                    int liInsType, liTableType, liCreateTable = 0, liBrLoop, liBrCount = 0, liLoop, llAddDays, iiProgress1, iiProgress2;
                    long liTableRowCount, llMaxRowCount, liTranCount;
                    string lsTableName, lsTableCode, lsBrColName, lsTimeColName, lsBr, lsTranWiseColName, lsDelColName, lsTran, lsTranTime = "", lsTranTimeTemp = "";
                    string lsPkeyCols, lsSetUpdateQuery, lsColList, lsColsForInsert, lsJsonConList, lsWhereCondition, lsBrList = "", lsSelJsonColList;
                    string lsFetchTable, lsJson, lsSelqctQuery, lsFetchTableTemp, lsTranitem, lsInsertTran, lsGroup = "";
                    string lsCount, lsFromTime = "", lsToTime;
                    JsonElement ljeBrList = new JsonElement();
                    JsonElement ljeTranList = new JsonElement();
                    JsonElement ljeTranListTemp = new JsonElement();

                    llAddDays = _OdbcSyncParam.odbcGlobalVar.maxDaysToSync;
                    llMaxRowCount = _OdbcSyncParam.odbcGlobalVar.maxRowCount;

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

                        lsBrColName = ljeTableList[i].GetProperty("brColName").ToString();
                        lsTimeColName = ljeTableList[i].GetProperty("timeColName").ToString();
                        lsTranWiseColName = ljeTableList[i].GetProperty("tranColName").ToString();
                        isFinColName = ljeTableList[i].GetProperty("finColName").ToString();
                        lsDelColName = ljeTableList[i].GetProperty("delColName").ToString();

                        _OdbcSyncParam.setStatusLog("tblName", lsTableName, 1);

                        lsPkeyCols = "";
                        lsSetUpdateQuery = "";
                        lsColList = "";
                        lsColsForInsert = "";
                        lsJsonConList = "";

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
                        lsFetchTableTemp = lsFetchTable.Replace("string_agg(", "List(").Replace("@finYear", _OdbcSyncParam.isFinYear);

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
                                            out lsJsonConList, out lsColsForInsert, out lsSetUpdateQuery, out lsSelJsonColList, out isReturn, out liInsType, out _);
                        if (isReturn.Contains("Failed"))
                        {
                            _OdbcSyncParam.setStatusLog("status", isReturn, 1);
                            _OdbcSyncParam.auditLogReset(2);
                            continue;
                        }

                        //////////////////////////////////////////////////////////////////////////////
                        //Branch Fetch (Customer Source Database)
                        //////////////////////////////////////////////////////////////////////////////
                        if (liBrCount <= 1)
                        {
                            if (lsBrList == null || lsBrList == "")
                            {
                                //lsBrList = brList("cust", "odbc");

                                if (lsBrList != null && lsBrList != "" && !lsBrList.Contains("No Rows Found"))
                                {
                                    ljeBrList = _OdbcSyncParam.odbcGlobalVar.createJsonElement(lsBrList);
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
                            _OdbcSyncParam.setStatusLog("sT", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 1);
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

                            _OdbcSyncParam.setStatusLog("br", lsBr, 1);
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
                                lsWhereCondition += " and " + ((isFinColName == null || isFinColName == "") ? "[Fin Year]" : isFinColName) + " = " + _OdbcSyncParam.isFinYear;
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
                                        //isSqlQuery = tranTypeFetchMax(lsTableName, lsTimeColName, lsTranWiseColName, lsFetchTable, lsBrColName, lsBr, lsWhereCondition, out lsInsertTran);
                                        lsInsertTran = "";
                                        if (lsInsertTran != "" && lsInsertTran != null)
                                        {
                                            _OdbcSyncParam.odbcGlobalVar._DestinationConfig.ExecuteRawQuery(_OdbcSyncParam.odbcGlobalVar,
                                                    lsInsertTran, 1, out iiSuccess, out isReturn);
                                            if (iiSuccess == 0)
                                            {
                                                _OdbcSyncParam.setStatusLog("status", isReturn, 1);
                                                _OdbcSyncParam.auditLogReset(3);
                                                continue;
                                            }
                                        }
                                        _OdbcSyncParam.odbcGlobalVar._DestinationConfig.ExecuteRawQuery(_OdbcSyncParam.odbcGlobalVar,
                                                    isSqlQuery, 0, out iiSuccess, out lsTranTime, 0);
                                        if (iiSuccess == 0)
                                        {
                                            _OdbcSyncParam.setStatusLog("status", lsTranTime, 1);
                                            _OdbcSyncParam.auditLogReset(3);
                                            continue;
                                        }
                                        lsTranTimeTemp = lsTranTime;
                                    }
                                    else
                                    {
                                        if (liCreateTable == 1)
                                        {
                                            lsFromTime = _OdbcSyncParam.odbcGlobalVar.defTime;
                                        }
                                        else
                                        {
                                            isSqlQuery = "Select IsNull(CONVERT(Varchar,[Max Time],120),'" + _OdbcSyncParam.odbcGlobalVar.defTime + "') as maxTime"
                                                + " From LS_MaxTime " + lsWhereCondition;
                                            _OdbcSyncParam.odbcGlobalVar._DestinationConfig.ExecuteRawQuery(_OdbcSyncParam.odbcGlobalVar,
                                                    isSqlQuery, 0, out iiSuccess, out lsFromTime, 0, 0);
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
                                        }
                                        _OdbcSyncParam.setStatusLog("t", lsFromTime, 1);
                                    }
                                }
                            }

                            //////////////////////////////////////////////////////////////////////////////
                            //Execute the Select Statement (Customer Source Database)
                            //////////////////////////////////////////////////////////////////////////////
                            isReturn = _OdbcSyncParam.dbConnect("odbc");
                            if (isReturn != "Success")
                            {
                                _OdbcSyncParam.setStatusLog("status", "Failed to Connect Customer Source Server on Checking Table Exists", 1);
                                _OdbcSyncParam.setStatusLog("status", isReturn, 1);
                                _OdbcSyncParam.auditLogReset(3);
                                continue;
                            }

                            liLoop = 1;
                            while (liLoop >= 1)
                            {
                                lsToTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                _OdbcSyncParam.setStatusLog("sT", lsToTime, 1);
                                _OdbcSyncParam._auditLogVar.Sequence = liLoop;
                                //Row Count (Customer Source Database)
                                if (lsTranWiseColName != null && lsTranWiseColName != "" && lsTimeColName != "" && lsTimeColName != null)
                                {
                                    string lsTranTimeList = "";
                                    ljeTranList = _OdbcSyncParam.odbcGlobalVar.createJsonElement(lsTranTime);

                                    liTranCount = ljeTranList.EnumerateArray().Count();
                                    lsFetchTable = lsFetchTableTemp;
                                    for (int tran = 0; tran < liTranCount; tran++)
                                    {
                                        lsTran = ljeTranList[tran].GetProperty("tranType").ToString();
                                        lsFromTime = ljeTranList[tran].GetProperty("maxTime").ToString();
                                        lsTranTimeList += lsTran + " -> " + lsFromTime + " | ";
                                        lsFetchTable = lsFetchTable.Replace(lsTran + "@fromTime", lsFromTime).Replace(lsTran + "@toTime", lsToTime).Replace("@brCode", lsBr);
                                    }
                                    _OdbcSyncParam.setStatusLog("t", lsTranTimeList, 1);
                                }
                                else
                                {
                                    lsFetchTable = lsFetchTableTemp.Replace("@fromTime", lsFromTime).Replace("@toTime", lsToTime).Replace("@brCode", lsBr);
                                }
                                lsCount = "0";


                                if (lsFetchTable.Contains("@fromTime") || lsFetchTable.Contains("@toTime"))
                                {
                                    if (lsTranWiseColName != null && lsTranWiseColName != "" && lsTimeColName != "" && lsTimeColName != null)
                                    {
                                        ljeTranListTemp = _OdbcSyncParam.odbcGlobalVar.createJsonElement(lsTranTimeTemp);
                                        for (int tran = 0; tran < ljeTranListTemp.EnumerateArray().Count(); tran++)
                                        {
                                            string lsTranTemp = ljeTranListTemp[tran].GetProperty("tranType").ToString();
                                            string lsFromTimeTemp = ljeTranListTemp[tran].GetProperty("maxTime").ToString();
                                            string lsToTimeTemp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                            lsFetchTable = lsFetchTable.Replace(lsTranTemp + "@fromTime", lsFromTimeTemp)
                                                .Replace(lsTranTemp + "@toTime", lsToTimeTemp);
                                        }
                                    }
                                }


                                isSqlQuery = "SELECT COUNT(*) FROM (" + lsFetchTable + ") as a";
                                lsCount = _OdbcSyncParam.odbcGlobalVar.odbcConn.srcDBExecQueryRetOne(isSqlQuery, _OdbcSyncParam.isODBCServer, _OdbcSyncParam.isODBCUID, _OdbcSyncParam.isODBCPwd);

                                if (long.TryParse(lsCount, out _))
                                {
                                    liTableRowCount = long.Parse(lsCount);
                                    _OdbcSyncParam.setStatusLog("s", liTableRowCount.ToString(), 1);
                                }
                                else
                                {
                                    _OdbcSyncParam.setStatusLog("status", lsCount, 1);
                                    liTableRowCount = 0;
                                }

                                if (liTableRowCount > 0)
                                {
                                    if (liTableRowCount > llMaxRowCount)
                                    {
                                        if (lsTranWiseColName != null && lsTranWiseColName != "" && lsTimeColName != "" && lsTimeColName != null)
                                        {

                                            //if (liLoop == 1) lsTranTime = getMinTransDate(lsFetchTable, lsTimeColName, lsColList, lsTranWiseColName);
                                            if (lsTranTime.Contains("Failed"))
                                            {
                                                liLoop = 0;
                                                _OdbcSyncParam.setStatusLog("status", lsTranTime, 1);
                                                _OdbcSyncParam.auditLogReset(4);
                                                continue;
                                            }
                                            ljeTranList = _OdbcSyncParam.odbcGlobalVar.createJsonElement(lsTranTime);
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

                                            lsFromTime = Convert.ToDateTime(_OdbcSyncParam.odbcGlobalVar.odbcConn.srcDBExecQueryRetOne("SELECT MIN(" +
                                                    lsTimeColName + ") FROM (" + lsFetchTable + ") as a(" + lsColList + ")",
                                                    _OdbcSyncParam.isODBCServer, _OdbcSyncParam.isODBCUID, _OdbcSyncParam.isODBCPwd)).ToString("yyyy-MM-dd HH:mm:ss");

                                            if (lsFromTime.Contains("|Failed"))
                                            {
                                                liLoop = 0;
                                                _OdbcSyncParam.setStatusLog("status", lsFromTime, 1);
                                                _OdbcSyncParam.auditLogReset(4);
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

                                    if (lsFetchTable.Contains("@fromTime") || lsFetchTable.Contains("@toTime"))
                                    {
                                        if (lsTranWiseColName != null && lsTranWiseColName != "" && lsTimeColName != "" && lsTimeColName != null)
                                        {
                                            ljeTranListTemp = _OdbcSyncParam.odbcGlobalVar.createJsonElement(lsTranTimeTemp);
                                            for (int tran = 0; tran < ljeTranListTemp.EnumerateArray().Count(); tran++)
                                            {
                                                string lsTranTemp = ljeTranListTemp[tran].GetProperty("tranType").ToString();
                                                string lsFromTimeTemp = ljeTranListTemp[tran].GetProperty("maxTime").ToString();
                                                string lsToTimeTemp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                lsFetchTable = lsFetchTable.Replace(lsTranTemp + "@fromTime", lsFromTimeTemp)
                                                    .Replace(lsTranTemp + "@toTime", lsToTimeTemp);
                                            }
                                        }
                                    }

                                    _OdbcSyncParam.setStatusLog("status", "Fetch From Source Table", 1);
                                    lsSelqctQuery = "";

                                    isSqlQuery = "Select " + lsColList + " From (" + lsFetchTable + ") as a(" + lsColList + ") for xml raw, elements;";
                                    isReturn = _OdbcSyncParam.odbcGlobalVar.odbcConn.srcDBExecQueryRetOne(isSqlQuery, _OdbcSyncParam.isODBCServer, _OdbcSyncParam.isODBCUID, _OdbcSyncParam.isODBCPwd);
                                    if (isReturn == "" || isReturn == null || isReturn.Contains("Failed"))
                                    {
                                        if (isReturn.Contains("Failed"))
                                        {
                                            liLoop = 0;
                                            _OdbcSyncParam.setStatusLog("status", isReturn, 1);
                                            _OdbcSyncParam.auditLogReset(4);
                                        }
                                        continue;
                                    }
                                    isReturn = "<root>" + isReturn.Replace("&quot;", "") + "</root>";
                                    XmlDocument doc = new XmlDocument();
                                    doc.LoadXml(isReturn);

                                    lsJson = JsonConvert.SerializeXmlNode(doc);//,Formatting.None, true);
                                    lsJson = _OdbcSyncParam.odbcGlobalVar.replaceSpecialCharacters(lsJson);

                                    lsSelqctQuery = "SELECT " + lsSelJsonColList.ToUpper() + " FROM OPENJSON('" + lsJson.ToUpper() + "','$.ROOT.ROW') WITH(" + lsJsonConList.ToUpper() + ")";

                                    //////////////////////////////////////////////////////////////////////////////
                                    //Delete from Destination Table (Customer Destination Database)
                                    //////////////////////////////////////////////////////////////////////////////
                                    if (lsDelColName != "" && lsDelColName != null)
                                    {
                                        _OdbcSyncParam.setStatusLog("status", "Delete From Destination Table -- Start", 1);
                                        isSqlQuery = "DELETE FROM " + lsTableName + " WHERE " + lsDelColName
                                                        + " IN (SELECT distinct " + lsDelColName + " FROM(" + lsSelqctQuery + ") a (" + lsColList + "));";
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
                                    }

                                    //////////////////////////////////////////////////////////////////////////////
                                    //Execute the insert Query (Customer Destination Database)
                                    //////////////////////////////////////////////////////////////////////////////
                                    _OdbcSyncParam.setStatusLog("status", "Construct Insert", 1);

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
                                    lsWhereCondition = " a.[Fin Year] = " + _OdbcSyncParam.isFinYear + " and a.[Table Name] = '" + lsTableName + "' ";
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
                                    //_OdbcSyncParam.setStatusLog("status", liLoop.ToString(), 1);
                                    if (liLoop == 0)
                                    {
                                        //_OdbcSyncParam.setStatusLog("status", "Inside", 1);
                                        if (lsTimeColName != "" && lsTimeColName != null)
                                        {

                                            isSqlQuery = "MERGE INTO LS_MaxTime WITH(HOLDLOCK) AS a USING("
                                                            + "Select '" + lsBr + "'," + lsTranitem.Replace("b.", "") + ",Max(" + lsTimeColName + ") as maxTime From("
                                                            + lsSelqctQuery + ") as maxTbl " + lsGroup + " ) AS b (br_code,Tran_Type,maxTime) ON " + lsWhereCondition
                                                            + " WHEN MATCHED THEN UPDATE SET a.[Max Time] = b.maxTime"
                                                            + " WHEN NOT MATCHED THEN INSERT("
                                                            + "[Fin Year],[Table Name],br_code,Tran_Type,[Max Time])"
                                                            + " VALUES(" + _OdbcSyncParam.isFinYear + ",'" + lsTableName + "','" + lsBr + "',"
                                                            + lsTranitem + ",b.maxTime);";
                                            //_OdbcSyncParam.setStatusLog("status", isSqlQuery, 1);
                                            _OdbcSyncParam.odbcGlobalVar._DestinationConfig.ExecuteRawQuery(_OdbcSyncParam.odbcGlobalVar,
                                                    isSqlQuery, 1, out iiSuccess, out isReturn);

                                            if (isReturn.Contains("Failed"))
                                            {
                                                liLoop = 0;
                                                _OdbcSyncParam.setStatusLog("status", isReturn, 1);
                                                _OdbcSyncParam.auditLogReset(4);
                                                continue;
                                            }
                                        }
                                    }
                                    _OdbcSyncParam.setStatusLog("status", "Data Inserted - " + liLoop.ToString(), 1);
                                }
                                else
                                {
                                    liLoop = 0;
                                    if (lsTranWiseColName != null && lsTranWiseColName != "" && lsTimeColName != "" && lsTimeColName != null)
                                    {
                                        string lsTranTimeList = "";
                                        ljeTranList = _OdbcSyncParam.odbcGlobalVar.createJsonElement(lsTranTime);

                                        liTranCount = ljeTranList.EnumerateArray().Count();
                                        lsFetchTable = lsFetchTableTemp;
                                        for (int tran = 0; tran < liTranCount; tran++)
                                        {
                                            _OdbcSyncParam.UpdateMaxTime(ljeTranList[tran].GetProperty("maxTime").ToString(), lsTableName,
                                                _OdbcSyncParam.isFinYear, ljeTranList[tran].GetProperty("tranType").ToString(), lsBr);
                                        }
                                        _OdbcSyncParam.setStatusLog("t", lsTranTimeList, 1);
                                    }
                                    else if (lsTimeColName != "" && lsTimeColName != null)
                                    {
                                        _OdbcSyncParam.UpdateMaxTime(lsToTime, lsTableName, _OdbcSyncParam.isFinYear, "-", lsBr);
                                    }

                                    _OdbcSyncParam.setStatusLog("status", "No Row Found", 1);
                                    _OdbcSyncParam.setStatusLog("eT", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 1);
                                    _OdbcSyncParam.setStatusLog("stat", "Success", 1);
                                    _OdbcSyncParam.setStatusLog("", "", 2);
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
                //return "Success";
            }
            catch (Exception e)
            {
                return "Failed " + e.Message;
            }

            //DAO.Recordset rst;
            dynamic FI = Interaction.CreateObject("Busy2L21.CFixedInterface");
            FI.OpenDB("C:\\BusyWin", "C:\\BusyWin\\DATA", "COMP0023");
            
            string Qry, UnitQry, PGQry;
            long RowNo;
            
            PGQry = ",(Select M1.Name from Master1 as M1 where M1.code=Master1.ParentGrp) as ParentGrpName";
            UnitQry = ",(Select M1.Name from Master1 as M1 where M1.code=Master1.CM1) as UnitName";

            Qry = "Select Name,Code,Alias" + PGQry + UnitQry + " from Master1 where Master1.MasterType=6 order by name";

            //rst = FI.GetRecordset(Qry);
            
            //rst.MoveLast();
            //rst.MoveFirst();
            
            DataTable dt = new DataTable();
            //dt = (DataTable)rst;

            /*while (rst.EOF == false)
            {
                MessageBox.Show(rst.Fields["Name"].Value.ToString());
                rst.MoveNext();
            }*/

            RowNo = 10231;
            return "";
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
