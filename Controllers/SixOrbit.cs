using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using MySql.Data.MySqlClient;


namespace LSSyncApp.Controllers
{
    public class SixOrbit
    {
        public static ODBCSyncParam _OdbcSyncParam = new ODBCSyncParam();
        public static string isReturn, isParam, isFinColName, isStatus, isSqlQuery, isLicId;
        public static int rCnt, iiCmpCnt, iiSyncType, iiCmd, iiSuccess, iiDateListCount = 0;
        private bool disposedValue;
        public static string isTable, isDateList, isBrList;
        public static JsonElement ijeDateList;

        public AuditLogVar init(ODBCSyncParam osp, IProgress<int> progress, string asParam, int aiSyncType,
            string asTable = "", string asDateList = "", string asBrList = "")
        {
            _OdbcSyncParam = osp;
            //_OdbcSyncParam.odbcGlobalVar.compCode = _OdbcSyncParam.path;
            isParam = asParam;
            iiSyncType = aiSyncType;
            iiCmd = _OdbcSyncParam.odbcGlobalVar.giCmd;
            isTable = asTable;
            isDateList = asDateList;
            isBrList = asBrList;
            _OdbcSyncParam.odbcGlobalVar.giApi = 0;
            if (isBrList != "" && isBrList != null && isDateList != "" && isDateList != null)
            {
                try
                {
                    ijeDateList = _OdbcSyncParam.odbcGlobalVar.createJsonElement(isDateList);
                    iiDateListCount = ijeDateList.EnumerateArray().Count();
                }
                catch
                {

                }
            }
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
                //_OdbcSyncParam.ip = "64.227.155.190";
                /*string lsQ = "Select * From (Select invid, inviid, isvid, iid, quantity, meaid, price, price_before_charge, price_after_charge "
                                + " From invoice_item_inventory "
                                + " Where company_id = 18) as allRecords";//inv_temp
                int rc;
                
                _OdbcSyncParam.dbConnect("mysql");
                MySqlDataReader _lsdrData1 = _OdbcSyncParam.odbcGlobalVar.odbcConn.mySqlExecRetMultiple(lsQ);

                DataTable _ldtData1 = new DataTable();
                try
                {
                    _ldtData1.Load(_lsdrData1);
                }
                catch (Exception Load)
                {
                    _OdbcSyncParam.setStatusLog("status", "Failed to Load - " + Load.Message, 1);
                    _OdbcSyncParam.auditLogReset(4);
                    return "";
                }

                rc = _ldtData1.Rows.Count;

                _OdbcSyncParam.odbcGlobalVar.odbcConn.mySqlDBClose();
                
                if (rc > 0)
                {
                    isReturn = _OdbcSyncParam.BulkInsert("inv_temp", _ldtData1);
                }
                return "";*/
                /////////////////////////////
                //Fetch Table List
                /////////////////////////////
                _OdbcSyncParam.odbcGlobalVar._MasterConfig.GetFetchTableList(_OdbcSyncParam.odbcGlobalVar, iiSyncType, isParam, out iiSuccess, out isReturn);
                if (iiSuccess == 0)
                {
                    return isReturn;
                }
                JsonElement ljeTableList = _OdbcSyncParam.odbcGlobalVar.createJsonElement(isReturn);
                rCnt = ljeTableList.EnumerateArray().Count();
                
                /////////////////////////////
                //Fetch Company List
                /////////////////////////////
                _OdbcSyncParam.odbcGlobalVar._MasterConfig.GetSourceDataList(_OdbcSyncParam.odbcGlobalVar, 
                    int.Parse(_OdbcSyncParam.isSeq), out iiSuccess, out isReturn);
                if (iiSuccess == 0)
                {
                    return isReturn;
                }
                JsonElement ljeCompList = _OdbcSyncParam.odbcGlobalVar.createJsonElement(isReturn);
                iiCmpCnt = ljeCompList.EnumerateArray().Count();

                for (int cmp = 0; cmp < iiCmpCnt; cmp++)
                {
                    _OdbcSyncParam.odbcGlobalVar.compCode = ljeCompList[cmp].GetProperty("d1").ToString();
                    isLicId = ljeCompList[cmp].GetProperty("d2").ToString();
                    //if (_OdbcSyncParam.odbcGlobalVar.compCode != "18") continue;
                    _OdbcSyncParam.setStatusLog("comp", _OdbcSyncParam.odbcGlobalVar.compCode, 1);

                    if (rCnt > 0)
                    {
                        int liInsType, liTableType, liCreateTable = 0, liBrLoop, liBrCount = 0, liLoop, llAddDays, iiProgress1, iiProgress2, liFIrstIns = 0;
                        long liTableRowCount, llMaxRowCount, liTranCount;
                        string lsTableName, lsTableCode, lsBrColName, lsTimeColName, lsTimeColNameTmp, lsBr, lsTranWiseColName, lsDelColName, lsTran, lsTranTime = "", lsTranTimeTemp = "";
                        string lsPkeyCols, lsSetUpdateQuery, lsColList, lsColsForInsert, lsJsonConList, lsWhereCondition, lsBrList = "", lsSelJsonColList;
                        string lsFetchTable, lsFetchTableTemp, lsTranitem, lsInsertTran, lsGroup = "";
                        string lsFromTime = "", lsToTime;
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
                            if (lsTableName == "LS_Purchase_Details") lsTranWiseColName = "Tran_Type";
                            _OdbcSyncParam.setStatusLog("tblName", lsTableName, 1);
                            //if (lsTableName != "LS_Sale_Detail") continue;
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
                            if (isStatus.Contains("Failed") || isStatus.Contains("Fetch Query not found")
                                || lsFetchTable == "" || lsFetchTable == null || isReturn.Contains("No Rows"))
                            {
                                GC.Collect();
                                _OdbcSyncParam.setStatusLog("status", isReturn, 1);
                                _OdbcSyncParam.auditLogReset(2);
                                continue;
                            }

                            int liCompCode = (lsFetchTable.Contains("company_code") ? 1 : 0);
                            lsTimeColNameTmp = (lsFetchTable.ToLower().Contains(lsTimeColName.Replace("[", "").Replace("]", "").ToLower()) ? lsTimeColName : "");
                            
                            lsFetchTableTemp = lsFetchTable.Replace("@finYear", _OdbcSyncParam.isFinYear)
                                .Replace("@lsCode", _OdbcSyncParam.odbcGlobalVar.custCode)
                                .Replace("@firmCode", _OdbcSyncParam.odbcGlobalVar.firmCode)
                                .Replace("@companyCode", _OdbcSyncParam.odbcGlobalVar.compCode)
                                .Replace("@licenceId", (isLicId == "-" ? "" : "_" + isLicId));

                            //////////////////////////////////////////////////////////////////////////////
                            //Column List Fetch (Customer Source Database)
                            //////////////////////////////////////////////////////////////////////////////
                            _OdbcSyncParam.odbcGlobalVar._DestinationConfig.ExecuteRawQuery(
                                _OdbcSyncParam.odbcGlobalVar, "", 2, out iiSuccess, out isReturn);
                            if (iiSuccess == 0)
                            {
                                GC.Collect();
                                _OdbcSyncParam.setStatusLog("status", "Failed to Connect Customer Server on Checking Table Exists", 1);
                                _OdbcSyncParam.auditLogReset(2);
                                continue;
                            }
                            _OdbcSyncParam.getColList(lsTableName, _OdbcSyncParam.isCustCode, _OdbcSyncParam.isFirmCode, out lsPkeyCols, out lsColList,
                                                out lsJsonConList, out lsColsForInsert, out lsSetUpdateQuery, out lsSelJsonColList, out isReturn, out liInsType, out _);
                            if (isReturn.Contains("Failed"))
                            {
                                GC.Collect();
                                _OdbcSyncParam.setStatusLog("status", isReturn, 1);
                                _OdbcSyncParam.auditLogReset(2);
                                continue;
                            }

                            //////////////////////////////////////////////////////////////////////////////
                            //Primary Key Columns Check
                            //////////////////////////////////////////////////////////////////////////////
                            /*if (lsPkeyCols == "" || lsPkeyCols == null || liTableType == 3)
                            {

                                if (liCompCode == 1)
                                {
                                    isSqlQuery = $"Delete From {lsTableName} Where company_code = '{_OdbcSyncParam.odbcGlobalVar.compCode}'";
                                    _OdbcSyncParam.setStatusLog("status", "Start Full Delete", 1);
                                }
                                else
                                {
                                    isSqlQuery = "Truncate Table " + lsTableName;
                                    _OdbcSyncParam.setStatusLog("status", "Start Truncate", 1);
                                }
                                _OdbcSyncParam.odbcGlobalVar._DestinationConfig.ExecuteRawQuery(_OdbcSyncParam.odbcGlobalVar,
                                                       isSqlQuery, 1, out iiSuccess, out isReturn);
                                if (iiSuccess == 0)
                                {
                                    _OdbcSyncParam.setStatusLog("status", isReturn, 1);
                                    continue;
                                }
                                _OdbcSyncParam.setStatusLog("status", "Done", 1);

                                _OdbcSyncParam.setStatusLog("status", "MaxTime", 1);
                                isSqlQuery = $"Delete From LS_MaxTime Where [Table Name] = '{lsTableName}'" + (liCompCode == 1 ? " And br_code = " + _OdbcSyncParam.odbcGlobalVar.compCode : "");
                                _OdbcSyncParam.odbcGlobalVar._DestinationConfig.ExecuteRawQuery(_OdbcSyncParam.odbcGlobalVar,
                                                       isSqlQuery, 1, out iiSuccess, out isReturn);
                                if (iiSuccess == 0)
                                {
                                    _OdbcSyncParam.setStatusLog("status", isReturn, 1);
                                    continue;
                                }
                            }*/

                            if (!lsFetchTableTemp.Contains("@brCode"))
                            {
                                lsBrColName = "";
                            }
                            if (!lsFetchTableTemp.Contains("@fromTime"))
                            {
                                lsTimeColName = "";
                            }
                            //////////////////////////////////////////////////////////////////////////////
                            //Branch Fetch (Customer Source Database)
                            //////////////////////////////////////////////////////////////////////////////
                            if (liBrCount <= 1)
                            {
                                if (isBrList != "" && isBrList != null)
                                {
                                    for (int br = 0; br < iiDateListCount; br++)
                                    {
                                        string isFromDateSep = ijeDateList[br].GetProperty("fromDate").ToString(), lsBrSep = ijeDateList[br].GetProperty("brCode").ToString();
                                        _OdbcSyncParam.UpdateMaxTimeBr("", lsTableName, _OdbcSyncParam.isFinYear,
                                            isFromDateSep, out iiSuccess, out isReturn,
                                            "'" + (liCompCode == 1 ? _OdbcSyncParam.odbcGlobalVar.compCode : "-") + "'");

                                        _OdbcSyncParam.setStatusLog("status", "Update Branches - "
                                            + (iiSuccess == 1 ? "Success" : "Failed - " + isReturn), 1);
                                    }

                                    ljeBrList = _OdbcSyncParam.odbcGlobalVar.createJsonElement(lsBrList);
                                    liBrCount = ljeBrList.EnumerateArray().Count();
                                }
                                /*else
                                {
                                    if (lsBrList == null || lsBrList == "")
                                    {
                                        lsBrList = brList("cust", "odbc");

                                        if (lsBrList != null && lsBrList != "" && !lsBrList.Contains("No Rows Found"))
                                        {
                                            ljeBrList = _OdbcSyncParam.odbcGlobalVar.createJsonElement(lsBrList);
                                            liBrCount = ljeBrList.EnumerateArray().Count();
                                        }
                                    }
                                }*/
                            }

                            liBrLoop = 1;
                            if (lsBrColName != null && lsBrColName != "")
                            {
                                liBrLoop = liBrCount;
                            }
                            iiProgress2 = iiProgress1 / liBrLoop;
                            for (int br = 0; br < liBrLoop; br++)
                            {
                                liFIrstIns = 0;
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
                                    //_OdbcSyncParam.setStatusLog("br", lsBr, 1);
                                }
                                else
                                {
                                    lsBr = "";
                                }

                                if (liCompCode == 1)
                                {
                                    lsWhereCondition += " and br_code = '" + (liCompCode == 1 ? _OdbcSyncParam.odbcGlobalVar.compCode : "-") + "'";
                                }
                                if ((isFinColName != null && isFinColName != "") || (liTableType == 1))
                                {
                                    lsWhereCondition += " and " + ((isFinColName == null || isFinColName == "") ? "[Fin Year]" : isFinColName) + " = " + _OdbcSyncParam.isFinYear;
                                }
                                //////////////////////////////////////////////////////////////////////////////
                                //Fetch lTime (Customer Database Custom Tables)
                                //////////////////////////////////////////////////////////////////////////////
                                if (_OdbcSyncParam.CheckTranType(lsTranWiseColName, lsFetchTable) <= 0)
                                {
                                    lsTranWiseColName = null;
                                }

                                if (lsTimeColName != "" && lsTimeColName != null)
                                {
                                    if (liTableType == 1 || liTableType == 2 || liTableType == 3)
                                    {
                                        if (lsTranWiseColName != null && lsTranWiseColName != "")
                                        {
                                            isSqlQuery = _OdbcSyncParam.odbcGlobalVar._fun.tranTypeFetchMax(_OdbcSyncParam.odbcGlobalVar,
                                                _OdbcSyncParam.isFinYear, lsTableName, lsTimeColName, lsTranWiseColName, lsFetchTable,
                                                lsBrColName, (liCompCode == 1 ? _OdbcSyncParam.odbcGlobalVar.compCode : "-")
                                                , lsWhereCondition, out lsInsertTran);
                                            if (lsInsertTran != "" && lsInsertTran != null)
                                            {
                                                _OdbcSyncParam.odbcGlobalVar._DestinationConfig.ExecuteRawQuery(_OdbcSyncParam.odbcGlobalVar,
                                                        lsInsertTran, 1, out iiSuccess, out isReturn);
                                                if (iiSuccess == 0)
                                                {
                                                    GC.Collect();
                                                    _OdbcSyncParam.setStatusLog("status", isReturn, 1);
                                                    _OdbcSyncParam.auditLogReset(3);
                                                    continue;
                                                }
                                            }
                                            _OdbcSyncParam.odbcGlobalVar._DestinationConfig.ExecuteRawQuery(_OdbcSyncParam.odbcGlobalVar,
                                                        isSqlQuery, 0, out iiSuccess, out lsTranTime, 0);
                                            if (iiSuccess == 0)
                                            {
                                                GC.Collect();
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
                                                        GC.Collect();
                                                        _OdbcSyncParam.setStatusLog("status", lsFromTime, 1);
                                                        _OdbcSyncParam.auditLogReset(3);
                                                        continue;
                                                    }
                                                }
                                                if (lsFromTime == "" || lsFromTime == null)
                                                {
                                                    lsFromTime = _OdbcSyncParam.odbcGlobalVar.defTime;
                                                    liFIrstIns = 1;
                                                }
                                            }
                                            _OdbcSyncParam.setStatusLog("t", lsFromTime, 1);
                                        }
                                    }
                                }

                                //////////////////////////////////////////////////////////////////////////////
                                //Execute the Select Statement (Customer Source Database)
                                //////////////////////////////////////////////////////////////////////////////
                                isReturn = _OdbcSyncParam.dbConnect("mysql");
                                if (isReturn != "Success")
                                {
                                    GC.Collect();
                                    _OdbcSyncParam.setStatusLog("status", "Failed to Connect Customer Source Server on Checking Table Exists - " + isReturn, 1);
                                    _OdbcSyncParam.auditLogReset(3);
                                    continue;
                                }

                                liLoop = 1;
                                while (liLoop >= 1)
                                {
                                    GC.Collect();
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
                                    //lsCount = "0";

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

                                    liTableRowCount = 1;
                                ZeroRows:
                                    if (liTableRowCount > 0)
                                    {
                                        liLoop = 0;

                                        _OdbcSyncParam.setStatusLog("status", "Fetch From Source Table", 1);

                                        isSqlQuery = "Select * From (" + lsFetchTable + ") as allRecords";

                                        MySqlDataReader _lsdrData = _OdbcSyncParam.odbcGlobalVar.odbcConn.mySqlExecRetMultiple(isSqlQuery);

                                        DataTable _ldtData = new DataTable();
                                        try
                                        {
                                            _ldtData.Load(_lsdrData);
                                        }
                                        catch (Exception Load)
                                        {
                                            _OdbcSyncParam.setStatusLog("status", "Failed to Load - " + Load.Message, 1);
                                            _OdbcSyncParam.auditLogReset(4);
                                            continue;
                                        }

                                        liTableRowCount = _ldtData.Rows.Count;
                                        _OdbcSyncParam.setStatusLog("s", liTableRowCount.ToString(), 1);

                                        _OdbcSyncParam.odbcGlobalVar.odbcConn.mySqlDBClose();
                                        
                                        if (liTableRowCount < 0) goto ZeroRows;

                                        //////////////////////////////////////////////////////////////////////////////
                                        //Primary Key Columns / Table Type Check & Truncate
                                        //////////////////////////////////////////////////////////////////////////////
                                        if (lsPkeyCols == "" || lsPkeyCols == null || liTableType == 3)
                                        {
                                            if (br == 0)
                                            {
                                                isReturn = _OdbcSyncParam.TruncateTable(lsTableName,
                                                    (liCompCode == 1 ? $"Delete From {lsTableName} Where company_code = '{_OdbcSyncParam.odbcGlobalVar.compCode}'" : ""),
                                                    $"Delete From LS_MaxTime Where [Table Name] = '{lsTableName}'" + (liCompCode == 1 ? " And br_code = " + _OdbcSyncParam.odbcGlobalVar.compCode : ""));

                                                if (isReturn != "") continue;
                                            }
                                        }

                                        /********************************/
                                        if (liTableType == 3)
                                        {
                                            liLoop = 0;

                                            isReturn = _OdbcSyncParam.BulkInsert(lsTableName, _ldtData);
                                            if (isReturn == "Failed") continue;
                                        }
                                        /********************************/
                                        else
                                        {
                                            
                                            _OdbcSyncParam.BulkDeleteInsert(_ldtData, liFIrstIns, lsTableName, lsDelColName, lsPkeyCols,
                                                lsTimeColName, lsBrColName, lsTranWiseColName, lsBr, isFinColName, liLoop,
                                                lsSelJsonColList, lsJsonConList, out iiSuccess);
                                            if (iiSuccess == 0)
                                            {
                                                continue;
                                            }
                                            GC.Collect();
                                        }
                                        //////////////////////////////////////////////////////////////////////////////
                                        //Insert Max Time Table (Customer Destination Database)
                                        //////////////////////////////////////////////////////////////////////////////

                                        if (liLoop == 0)
                                        {
                                            lsWhereCondition = "";
                                            if (lsBr == "")
                                            {
                                                lsBr = "-";
                                            }
                                            lsWhereCondition = " a.[Fin Year] = " + _OdbcSyncParam.isFinYear + " and a.[Table Name] = '" + lsTableName + "' ";
                                            if (liCompCode == 1)
                                            {
                                                lsWhereCondition += " and a.br_code = '" + _OdbcSyncParam.odbcGlobalVar.compCode + "'";
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

                                            if (lsTimeColNameTmp != null && lsTimeColNameTmp != "")
                                            {
                                                string lsUpateMax = "";
                                                if (lsTranWiseColName != null && lsTranWiseColName != "")
                                                {
                                                    liTranCount = ljeTranList.EnumerateArray().Count();
                                                    for (int tran = 0; tran < liTranCount; tran++)
                                                    {
                                                        lsTran = ljeTranList[tran].GetProperty("tranType").ToString();
                                                        List<DateTime> dates = _ldtData.AsEnumerable()
                                                            .Where(ax => ax.Field<string>(lsTranWiseColName.Replace("[", "").Replace("]", "")) == lsTran)
                                                            .Select(al => al.Field<DateTime>(lsTimeColNameTmp.Replace("[", "").Replace("]", "")))
                                                            .Distinct().ToList();

                                                        if (dates.Count > 0)
                                                        {
                                                            string maxDt = dates.Max().ToString("yyyy-MM-dd HH:mm:ss");
                                                            lsUpateMax += (lsUpateMax == "" ? "" : " Union All ")
                                                                + $"Select '{lsTran}' as {lsTranWiseColName}, '{lsTableName}' as tblName,'{maxDt}' as maxTime, '"
                                                                + (liCompCode == 1 ? _OdbcSyncParam.odbcGlobalVar.compCode : "-") + "' as br";
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    List<DateTime> dates = _ldtData.AsEnumerable()
                                                        .Select(al => al.Field<DateTime>(lsTimeColNameTmp.Replace("[", "").Replace("]", ""))).Distinct().ToList();
                                                    if (dates.Count > 0)
                                                    {
                                                        string maxDt = dates.Max().ToString("yyyy-MM-dd HH:mm:ss");
                                                        lsUpateMax = $"Select '{lsTableName}' as tblName,'{maxDt}' as maxTime, '"
                                                            + (liCompCode == 1 ? _OdbcSyncParam.odbcGlobalVar.compCode : "-") + "' as br";
                                                    }
                                                }

                                                if (lsUpateMax != "")
                                                {
                                                    isSqlQuery = "MERGE INTO LS_MaxTime WITH(HOLDLOCK) AS a USING("
                                                                + lsUpateMax + ") as b ON a.[Table Name] = b.tblName "
                                                                + (lsTranWiseColName != null && lsTranWiseColName != "" ? " and a.Tran_Type = b." + lsTranWiseColName : "")
                                                                + " And a.br_code = b.br "
                                                                + " WHEN MATCHED THEN UPDATE SET a.[Max Time] = b.maxTime"
                                                                + " WHEN NOT MATCHED THEN INSERT([Fin Year],[Table Name],br_code,Tran_Type,[Max Time])"
                                                                + " VALUES(" + _OdbcSyncParam.isFinYear + ", b.tblName,b.br,"
                                                                + lsTranitem + ",b.maxTime);";
                                                    _OdbcSyncParam.odbcGlobalVar._DestinationConfig.ExecuteRawQuery(_OdbcSyncParam.odbcGlobalVar,
                                                            isSqlQuery, 1, out iiSuccess, out isReturn);

                                                    if (isReturn.Contains("Failed"))
                                                    {
                                                        GC.Collect();
                                                        liLoop = 0;
                                                        _OdbcSyncParam.setStatusLog("status", isReturn, 1);
                                                        _OdbcSyncParam.auditLogReset(4);
                                                        continue;
                                                    }
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
                                                    _OdbcSyncParam.isFinYear, ljeTranList[tran].GetProperty("tranType").ToString(),
                                                    "'" + (liCompCode == 1 ? _OdbcSyncParam.odbcGlobalVar.compCode : "-") + "'");
                                            }
                                            _OdbcSyncParam.setStatusLog("t", lsTranTimeList, 1);
                                        }
                                        else if (lsTimeColName != "" && lsTimeColName != null)
                                        {
                                            _OdbcSyncParam.UpdateMaxTime(lsToTime, lsTableName, _OdbcSyncParam.isFinYear, "-",
                                                "'" + (liCompCode == 1 ? _OdbcSyncParam.odbcGlobalVar.compCode : "-") + "'");
                                        }

                                        _OdbcSyncParam.setStatusLog("status", "No Row Found", 1);
                                        _OdbcSyncParam.setStatusLog("eT", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 1);
                                        _OdbcSyncParam.setStatusLog("stat", "Success", 1);
                                        _OdbcSyncParam.setStatusLog("", "", 2);
                                        GC.Collect();
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
                                    GC.Collect();
                                }
                            }
                            _OdbcSyncParam.auditLogReset(1);
                        }
                    }
                }
                _OdbcSyncParam.setStatusLog("tblName", "Success", 1);
                _OdbcSyncParam.setStatusLog("status", "Successfully ", 1);
                GC.Collect();
                return "Success";
            }
            catch (Exception e)
            {
                GC.Collect();
                _OdbcSyncParam.setStatusLog("status", e.Message, 1);
                return "Failed " + e.Message;
            }
        }


        public static string getMinTransDate(string asSqlQuery, string asTimeColName, string asColList, string asTranColName)
        {
            string lsReturn;
            try
            {
                asSqlQuery = "SELECT '[' + GROUP_CONCAT('{\"tranType\": \"' + tc + '\",\"maxTime\": \"' + mtc + '\"}' , ',') + ']' FROM("
                    + " SELECT " + asTranColName + " as tc, String(MIN(" + asTimeColName + ")) as mtc FROM(" + asSqlQuery
                    + ") as a(" + asColList + ") Group By " + asTranColName + ") as b";

                lsReturn = _OdbcSyncParam.odbcGlobalVar.odbcConn.mySqlExecQueryRetOne(asSqlQuery,
                    _OdbcSyncParam.ip, _OdbcSyncParam.port, _OdbcSyncParam.isODBCDBName, _OdbcSyncParam.isODBCUID, _OdbcSyncParam.isODBCPwd);

                if (!lsReturn.Contains("|Failed") && lsReturn.Contains("maxTime"))
                {
                    JsonElement ljeTranList = _OdbcSyncParam.odbcGlobalVar.createJsonElement(lsReturn);
                    int liTranCount = ljeTranList.EnumerateArray().Count();
                    DateTime[] ldTime = new DateTime[liTranCount];
                    DateTime ldMinTime = DateTime.Now;
                    for (int tran = 0; tran < liTranCount; tran++)
                    {
                        ldTime[tran] = DateTime.Parse(ljeTranList[tran].GetProperty("maxTime").ToString());
                        ldMinTime = ldMinTime > ldTime[tran] ? ldTime[tran] : ldMinTime;
                    }
                    lsReturn = "";
                    for (int tran = 0; tran < liTranCount; tran++)
                    {
                        lsReturn += (tran == 0 ? "" : ",") + ("{\"tranType\": \"" + ljeTranList[tran].GetProperty("tranType").ToString()
                            + "\",\"maxTime\": \"" + ldMinTime.ToString("yyyy-MM-dd HH:mm") + "\"}");
                    }
                    lsReturn = "[" + lsReturn + "]";
                }
            }
            catch (Exception Ex)
            {
                lsReturn = "Failed - " + Ex.Message;
            }
            return lsReturn;
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