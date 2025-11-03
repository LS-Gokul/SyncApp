using System;
using System.Text.Json;
using System.Linq;

namespace LSSyncApp
{
    public class SummaryTables
    {
        public static ODBCSyncParam odbcSyncParam;

        public static string isReturn, isSqlQuery, isCount, isLogString, isStatus, isRetStatus = "Success";
        public static int iiCmd, iiSuccess;

        //New Sync Function 28/01/2022
        public string summData(IProgress<int> progress, string asLogID, ODBCSyncParam param, string asParam, int aiSyncType)
        {
            odbcSyncParam = param;
            iiCmd = odbcSyncParam.odbcGlobalVar.giCmd;
            odbcSyncParam._auditLogVar.LogId = asLogID;

            if (iiCmd == 0)
            {
                if (progress != null) progress.Report(1);//Progress Bar
            }
            
            string lsRet = commonSummary(progress, asParam, aiSyncType);
            if (iiCmd == 0)
            {
                if (progress != null) progress.Report(100);//Progress Bar
            }
            
            if (lsRet.Contains("Failed"))
            {
                odbcSyncParam._auditLogVar.Object = "Sync";//Set Audit Log Var
                odbcSyncParam._auditLogVar.ChildObject = "Common-Summary";//Set Audit Log Var
                odbcSyncParam._auditLogVar.Sequence = 1;//Set Audit Log Var
                odbcSyncParam._auditLogVar.ObjectFromTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//Set Audit Log Var
                odbcSyncParam._auditLogVar.EndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//Set Audit Log Var
                odbcSyncParam._auditLogVar.LogDetails = lsRet;//Set Audit Log Var
                odbcSyncParam._auditLogVar.Status = "Failed";//Set Audit Log Var
                odbcSyncParam.setStatusLog("", "", 2);
            }
            return lsRet;
        }

        public string commonSummary(IProgress<int> progress, string asParam,int aiSyncType)
        {
            try
            {
                //////////////////////////////////////////////////////////////////////////////
                //Get Table List (Master Database)
                //////////////////////////////////////////////////////////////////////////////
                odbcSyncParam.odbcGlobalVar._MasterConfig.GetFetchTableList(odbcSyncParam.odbcGlobalVar, aiSyncType, asParam, out int liSuccess, out isReturn, " In (0, -1) ");
                
                if (liSuccess == 1 && isReturn != null && isReturn != "")
                {
                    int rCnt, liInsType, liProgress1;
                    string lsTableName, lsTableCode, lsFetchTable, lsTableType;
                    string lsPkeyCols, lsSetUpdateQuery, lsColList, lsColsForInsert, lsJsonConList, lsSelJsonColList;
                    JsonElement ljeTableList = new JsonElement();

                    ljeTableList = odbcSyncParam.odbcGlobalVar.createJsonElement(isReturn);
                    rCnt = ljeTableList.EnumerateArray().Count();

                    liProgress1 = 100 / rCnt;
                    for (int i = 0; i < rCnt; i++)
                    {
                        //tableCode,tableName,tableType,brColName,timeColName,tranColName,finColName
                        if (iiCmd == 0)
                        {
                            if (progress != null) progress.Report(i * liProgress1);
                        }
                        odbcSyncParam.setStatusLog("sT", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 1);
                        lsTableCode = ljeTableList[i].GetProperty("tableCode").ToString();
                        lsTableName = ljeTableList[i].GetProperty("tableName").ToString();
                        lsTableType = ljeTableList[i].GetProperty("tableType").ToString();

                        odbcSyncParam.setStatusLog("tblName", lsTableName,1);

                        lsPkeyCols = "";
                        lsSetUpdateQuery = "";
                        lsColList = "";
                        lsColsForInsert = "";
                        //////////////////////////////////////////////////////////////////////////////
                        //Fetch Select & Create Table Script
                        //////////////////////////////////////////////////////////////////////////////

                        isReturn = "";
                        odbcSyncParam.setStatusLog("status", "Fetch Select & Create Table Script", 1);
                        odbcSyncParam.odbcGlobalVar._fun.fetchTable(odbcSyncParam.odbcGlobalVar, lsTableCode, out lsFetchTable, out isStatus, out isReturn);
                        if (isStatus == "Failed")
                        {
                            odbcSyncParam.setStatusLog("status", isReturn, 1);
                            odbcSyncParam.auditLogReset(2);
                            continue;
                        }

                        if (lsTableType == "-1") //Delete or Raw Query to be wxwcuted as it is
                        {
                            odbcSyncParam.setStatusLog("status", "Process Start", 1);
                            odbcSyncParam.odbcGlobalVar._DestinationConfig.ExecuteRawQuery(odbcSyncParam.odbcGlobalVar, lsFetchTable, 1, out iiSuccess, out isReturn);
                            if (iiSuccess == 0)
                            {
                                odbcSyncParam.setStatusLog("status", isReturn, 1);
                                odbcSyncParam.auditLogReset(2);
                                continue;
                            }
                            odbcSyncParam.setStatusLog("status", "Process Done", 1);
                        }
                        else //Insert Query
                        {
                            //////////////////////////////////////////////////////////////////////////////
                            //Column List Fetch
                            //////////////////////////////////////////////////////////////////////////////
                            odbcSyncParam.getColList(lsTableName, odbcSyncParam.isCustCode, odbcSyncParam.isFirmCode, out lsPkeyCols, out lsColList,
                                                out lsJsonConList, out lsColsForInsert, out lsSetUpdateQuery, out lsSelJsonColList, out isReturn, out liInsType, out _);
                            if (isReturn.Contains("Failed"))
                            {
                                odbcSyncParam.setStatusLog("status", isReturn, 1);
                                odbcSyncParam.auditLogReset(2);
                                continue;
                            }

                            //////////////////////////////////////////////////////////////////////////////
                            //Execute the Truncate Query (Customer Database)
                            //////////////////////////////////////////////////////////////////////////////
                            odbcSyncParam.setStatusLog("status", "Truncate Start", 1);
                            odbcSyncParam.odbcGlobalVar._DestinationConfig.ExecuteRawQuery(odbcSyncParam.odbcGlobalVar,
                                "Truncate Table " + lsTableName + ";", 1, out iiSuccess, out isReturn);
                            if (iiSuccess == 0)
                            {
                                odbcSyncParam.setStatusLog("status", isReturn, 1);
                                odbcSyncParam.auditLogReset(2);
                                continue;
                            }
                            odbcSyncParam.setStatusLog("status", "Data Truncated", 1);
                            lsFetchTable = lsFetchTable.Replace("@finYear", odbcSyncParam.isFinYear)
                                .Replace("@lsCode", odbcSyncParam.odbcGlobalVar.custCode)
                                .Replace("@firmCode", odbcSyncParam.odbcGlobalVar.firmCode)
                                .Replace("@companyCode", odbcSyncParam.odbcGlobalVar.compCode);
                            //////////////////////////////////////////////////////////////////////////////
                            //Execute the select statement and insert Query (Customer Database)
                            //////////////////////////////////////////////////////////////////////////////
                            odbcSyncParam.setStatusLog("status", "Fetch And Insert", 1);

                            if (isReturn.Contains("Failed"))
                            {
                                odbcSyncParam.setStatusLog("status", isReturn, 1);
                                odbcSyncParam.auditLogReset(2);
                                continue;
                            }
                            if (liInsType == 1)
                            {
                                isSqlQuery = "MERGE INTO " + lsTableName + " WITH(HOLDLOCK) AS a USING("
                                                + lsFetchTable + ") AS new_a (" + lsColList + ") ON " + lsPkeyCols
                                                + lsSetUpdateQuery + " WHEN NOT MATCHED THEN INSERT(" + lsColList
                                                + ")" + " VALUES(" + lsColsForInsert + ");";
                            }
                            else
                            {
                                isSqlQuery = "INSERT INTO " + lsTableName + "(" + lsColList + ") "
                                                + " SELECT " + lsColList + " FROM ("
                                                + lsFetchTable + ") AS tbl (" + lsColList + ");";
                            }

                            odbcSyncParam.setStatusLog("status", "Insert Start", 1);
                            odbcSyncParam.odbcGlobalVar._DestinationConfig.ExecuteRawQuery(odbcSyncParam.odbcGlobalVar, isSqlQuery, 1, out iiSuccess, out isReturn);
                            if (iiSuccess == 0)
                            {
                                odbcSyncParam.setStatusLog("status", isReturn, 1);
                                odbcSyncParam.auditLogReset(2);
                                continue;
                            }
                            odbcSyncParam.setStatusLog("status", "Data Inserted", 1);
                            odbcSyncParam.setStatusLog("stat", "Success", 1);
                        }
                    }
                    odbcSyncParam.setStatusLog("eT", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 1);
                    odbcSyncParam.setStatusLog("", "", 2);
                }

                odbcSyncParam.setStatusLog("tblName", "Success", 1);
                odbcSyncParam.setStatusLog("status", "Successfully Summarized", 1);
                odbcSyncParam.setStatusLog("", "", 2);
            }
            catch (Exception e)
            {
                isLogString += e.Message + " |Failed";
                odbcSyncParam.setStatusLog("", "", 2);
            }
            return isRetStatus;
        }
    }
}