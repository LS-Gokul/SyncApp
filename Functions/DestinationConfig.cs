using System;
using System.Text;
using System.Threading.Tasks;

namespace LSSyncApp.Functions
{
    public class DestinationConfig
    {
        private string isReturn = "", isCustServerName, isCustDBName, isCustUID, isCustPwd, isJson, isCustKey;
        private RestAPI restAPI = new RestAPI();

        private void setVariable(GlobalVariable gblVar)
        {
            isCustServerName = gblVar.custServerName;
            isCustDBName = gblVar.custDbName;
            isCustUID = gblVar.custUID;
            isCustPwd = gblVar.custPwd;
        }

        private async Task<string> PostUrl(GlobalVariable _gblVar, string asSql, int aiType, int aiDBType = 0)
        {
            isReturn = await restAPI.PostUrlAsync(_gblVar.gsClientUrl + "/" + aiType.ToString() + "/" + isCustKey + "/" + aiDBType.ToString(),
                "\"" + asSql.Replace("\0", "    ").Replace("\t", "    ")
                .Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("	", "")
                    + "\"", "application/json");
            return isReturn;
        }

        /********************************************************************
        _________________________________________________________________________________________________
            Variable         DataType          I/O    Optional(Y/N)      Default Value      Purpose
        _________________________________________________________________________________________________
        -> _gblVar           GlobalVariable     I       N                  N/A              Get / Set Global variables for credientials
        -> asSql             String             I       N                  N/A              Sql to be executed
        -> aiType            Int                I       N                  N/A              Insert(aiType = 1) / Select(aiType = 0) /
                                                                                                Check Connection(aiType = 2)
        -> aiSuccess         Int                O       N                  N/A              Ouytput Variable for return Success(1)/Failure(0)
        -> aiDBType          Int                I       N                  N/A              Either Master DB(aiDBType = 1) /
                                                                                                Customer Destinaltion DB(aiDBType = 0)
        -> aiJson            Int                I       Y                  1                Result Set in JSON(aiJson = 1) or Normal(aiJson = 0)
        -> asMessageOpt      String             I       Y                  No Rows Found    Set the default failed message when no result returns
        ********************************************************************/
        private string custExecSql(GlobalVariable _gblVar, string asSql, int aiType, out int aiSuccess, int aiDBType = 0,
            int aiJson = 1, string asMessageOpt = "No Rows Found", int aiBulkDelete = 0)
        {

            isCustKey = _gblVar.Base64Encode(_gblVar.custCode + "|" + _gblVar.firmCode);

            isJson = aiJson == 1 ? " FOR JSON PATH" : "";
            aiSuccess = 1;
            try
            {
                if (_gblVar.giApi == 1)
                {
                    GC.Collect();
                    
                    StringBuilder lsSql = new StringBuilder(700000000, 700000000);
                    lsSql.Append(asSql);
                    lsSql = lsSql.Replace("\0", " ");
                    lsSql = lsSql.Replace("\t", " ");
                    lsSql = lsSql.Replace("\\", "\\\\");
                    lsSql = lsSql.Replace("\"", "\\\"");
                    lsSql = lsSql.Replace("	", "");

                    //File.WriteAllText(_gblVar.gsApplPath + "\\Log\\abc.txt" , _gblVar.gsClientUrl + "/" + aiType.ToString() + "/" + isCustKey + "/" + aiDBType.ToString() + Environment.NewLine + lsSql.ToString());

                    isReturn = restAPI.postAPICalling(
                        _gblVar.gsClientUrl + "/" + aiType.ToString() + "/" + isCustKey + "/" + aiDBType.ToString(),
                        "application/json", "\"" + lsSql + "\"",out _);
                    if (aiType == 1 || aiType == 2)
                    {
                        if (isReturn != "Success")
                        {
                            aiSuccess = 0;
                        }
                    }
                    else
                    {
                        if (isReturn.Length > 9)
                        {
                            if (isReturn.Substring(0, 9) == "Failed - ")
                            {
                                aiSuccess = 0;
                            }
                        }
                        else if (isReturn == "[]")
                        {
                            aiSuccess = 0;
                        }
                    }
                }
                else
                {
                    setVariable(_gblVar);
                    isReturn = _gblVar.custDBConn.destConnSetup(isCustServerName, (aiDBType == 1 ? "master" : isCustDBName), isCustUID, isCustPwd,_gblVar.giCustDBAuthMethod);
                    if (isReturn == "Failed")
                    {
                        aiSuccess = 0;
                        isReturn += " - LS Server Connection Failed.";
                        //isReturn += " - " + _gblVar.giCustDBAuthMethod.ToString() + "-" + aiDBType.ToString() + "-" + isCustDBName + "-" + isCustUID + "-" + isCustPwd;
                    }
                    else
                    {
                        if (aiType == 2)
                        {

                        }
                        else if (aiType == 1)
                        {
                            isReturn = _gblVar.custDBConn.destExecQuery(asSql, aiBulkDelete);
                            if (isReturn == "")
                            {
                                isReturn = "Success";
                            }
                        }
                        else
                        {
                            if (aiJson == 1)
                                isReturn = _gblVar.custDBConn.destDBExecRetOne("SELECT CAST((" + asSql + isJson + ") AS VARCHAR(MAX)) AS JSONDATA");
                            else
                                isReturn = _gblVar.custDBConn.destDBExecRetOne(asSql);
                        }
                        if (isReturn.Contains("|Failed"))
                        {
                            aiSuccess = 0;
                        }
                    }
                }
                if (isReturn == "" || isReturn == null)
                {
                    aiSuccess = 0;
                    isReturn = asMessageOpt;
                }
            }
            catch (Exception Ex)
            {
                aiSuccess = 0;
                isReturn = "Failed - " + Ex.Message;
            }
            return isReturn;
        }

        /////////////////////////////////////////////////////////////////////////
        //Execute Raw Sql
        /////////////////////////////////////////////////////////////////////////
        public void ExecuteRawQuery(GlobalVariable globalVariable, string asSql, int aiType, out int aiSuccess,
            out string asMessage, int aiJson = 1, int aiDBType = 0, int aiBulkDelete = 0)
        {
            asMessage = custExecSql(globalVariable, asSql, aiType, out aiSuccess, aiDBType, aiJson, "No Rows Found", aiBulkDelete);
        }

        /////////////////////////////////////////////////////////////////////////
        //Table List & Size
        /////////////////////////////////////////////////////////////////////////
        public void CheckTableListSize(GlobalVariable globalVariable, out int aiSuccess,out string asMessage, string asRows = "0")
        {
            asMessage = custExecSql(globalVariable, "Select objName,sum(rows) as rows,sum(size) as size,Max(tblTyp) as tblType From("
                        + "     SELECT Trim(replace(replace(replace(replace(replace(t.Name,'Details',''),'Detail',' '),'LS_',''),'_',' '),'Master','')) AS objName, "
                        + "         p.rows AS rows, CAST(ROUND((SUM(a.total_pages) / 128.00), 2) AS NUMERIC(18, 2)) AS size, "
                        + "         (Case When t.Name like '%Master%' Then 1 When t.Name like '%Log%' Then 2 "
                        + "             When t.Name like '%Outstanding%' Then 3 Else 4 End) as tblTyp"
                        + "     FROM sys.tables t JOIN sys.indexes i ON t.OBJECT_ID = i.object_id "
                        + "             JOIN sys.partitions p ON i.object_id = p.OBJECT_ID AND i.index_id = p.index_id "
                        + "             JOIN sys.allocation_units a ON p.partition_id = a.container_id "
                        + "             JOIN sys.schemas s ON t.schema_id = s.schema_id "
                        + $"     WHERE t.name NOT IN ('LS_Version') AND p.rows > {asRows} GROUP BY t.Name, p.Rows) as a GROUP BY objName ",
                    0, out aiSuccess);
        }

        /////////////////////////////////////////////////////////////////////////
        //Audit Log Details
        /////////////////////////////////////////////////////////////////////////
        public void AuditLogDetails(GlobalVariable globalVariable, out int aiSuccess,out string asMessage)
        {
            asMessage = custExecSql(globalVariable, "Select LogId,objCnt,brCount,stTime, endTime, "
                            + "    (Case when st like '%Failed%' And st like '%Success%' then 1 when st like '%Failed%' then 2 else 0 end) as  st,null as Status "
                            + "From( "
                            + "    Select LogId,Sum(tblCnt) as  objCnt,Sum(brCount) as brCount,Convert(Varchar,stTime,120) as stTime,"
                            + "         Convert(Varchar,endTime,120) as endTime, String_Agg(st,',') as st "
                            + "    From( "
                            + "        SELECT  Distinct a.LogId, "
                            + "                Count( Distinct a.[Object]) as tblCnt, "
                            + "                Count( Distinct Case When a.[Child Object] = '-' then '000' else a.[Child Object] end) as brCount, "
                            + "                b.[Start Time] as stTime, "
                            + "                b.[End Time] as endTime, "
                            + "                a.[Status] as st "
                            + "        from LS_Audit_Log_Detail a Join LS_Audit_Log b On a.LogId = b.LogId "
                            + "        where a.[Status] In ('Success','Failed') And b.[Start Time] >= DATEADD(Day, -10, GetDate()) "
                            + "            And a.[Object] Not Like ('%Summary%') And a.[Child Object] Not Like ('%Summary%')  "
                            + "        Group By a.LogId,b.[Start Time],b.[End Time],a.[Status] "
                            + "    ) as a "
                            + "    group by LogId,stTime, endTime "
                            + ") as b ",
                    0, out aiSuccess);
        }

        /////////////////////////////////////////////////////////////////////////
        //Check Current Destination DB Version
        /////////////////////////////////////////////////////////////////////////
        public void CheckCurrentVersion(GlobalVariable globalVariable, out int aiSuccess, out string asMessage)
        {
            asMessage = custExecSql(globalVariable, "Select Top 1 [Version],[Released Date],[Date of Updation] "
                        + " From LS_Version Where software_code = '" + globalVariable.gsSoftwareCode + "'"
                        + " Order by [Version] desc ",
                    0, out aiSuccess);
        }

        /////////////////////////////////////////////////////////////////////////
        //Fetch Column List as per Table
        /////////////////////////////////////////////////////////////////////////
        public void FetchColumnList(GlobalVariable globalVariable, string asTableName, out int aiSuccess, out string asMessage)
        {
            asMessage = custExecSql(globalVariable, "SELECT  '[' + STRING_AGG(CONVERT(NVARCHAR(max),'{\"colName\" : \"' + e.COLUMN_NAME + '\",' + "
                            + "         '\"colType\" : \"' + e.DATA_TYPE + (CASE "
                            + "         WHEN(e.CHARACTER_MAXIMUM_LENGTH IS NOT NULL) THEN "
                            + "             '(' + (CASE WHEN e.CHARACTER_MAXIMUM_LENGTH < 0 THEN 'MAX' ELSE trim(str(e.CHARACTER_MAXIMUM_LENGTH)) END) +')' "
                            + "         WHEN(e.NUMERIC_PRECISION IS NOT NULL) THEN "
                            + "            (CASE WHEN e.DATA_TYPE = 'int' THEN '' ELSE "
                            + "            ('(' + trim(str(e.NUMERIC_PRECISION)) + (CASE WHEN e.NUMERIC_SCALE > 0 THEN ',' + trim(str(e.NUMERIC_SCALE)) ELSE '' END) + ')') "
                            + "             END) "
                            + "         ELSE '' END) +'\",' + "
                            + "         '\"colPKey\" : \"' + (CASE WHEN COALESCE(d.column_id, 0) > 0 THEN 'Y' ELSE 'N' END) + '\"}'),',' ) WITHIN GROUP (ORDER BY b.column_id) + ']' AS colList "
                            + " from INFORMATION_SCHEMA.COLUMNS e JOIN sys.tables a ON a.name = e.TABLE_NAME "
                            + "                 JOIN sys.columns b ON a.object_id = b.object_id AND e.COLUMN_NAME = b.name "
                            + "                 LEFT OUTER JOIN sys.indexes c ON a.object_id = c.object_id AND c.is_primary_key = 1 "
                            + "                 LEFT OUTER JOIN sys.index_columns d ON c.object_id = d.object_id AND c.index_id = d.index_id AND b.column_id = d.column_id "
                            + " WHERE e.TABLE_NAME = '" + asTableName + "'",
                    0, out aiSuccess, 0, 0);
        }

        /////////////////////////////////////////////////////////////////////////
        //Fetch Log Details using Log Id
        /////////////////////////////////////////////////////////////////////////
        public void LogDetails(GlobalVariable globalVariable, string asLogId, out int aiSuccess, out string asMessage)
        {
            asMessage = custExecSql(globalVariable, "Select Replace(obj + '$Objects : ' + Cast(sC + fC as varchar(10)) + '  (' +"
                    + "            (Case when st like '%Failed%' And st like '%Success%' then "
                    + "                 'Success : ' + Cast(sC as varchar(10)) + '  |  ' + 'Failed : ' + Cast(fC as varchar(10))"
                    + "                 when st = 'Failed' then 'Failed : ' + Cast(fC as varchar(10))"
                    + "                 else 'Success : ' + Cast(sC as varchar(10)) end) + ')$' + "
                    + "             'Started At: ' + ISNULL(CONVERT(Varchar,sTime, 120), '') + '*+*+*+*-*+*+*+*' + "
                    + "             'Ended At: ' + ISNULL(CONVERT(Varchar,eTime, 120), '') + '$' + "
                    + "         'Time Taken -> ' + (Case when Floor(diff / 3600) > 0 then Cast(Floor(diff / 3600) as VarChar) + ' Hours ' else '' End) + "
                    + "             (Case when Floor((diff - (Floor(diff / 3600) * 3600)) / 60) > 0 then "
                    + "              Cast(Floor((diff - (Floor(diff / 3600) * 3600)) / 60) as VarChar) + ' Minutes ' else '' End) + "
                    + "             (Case when(diff - (Floor(diff / 3600) * 3600) - (Floor((diff - (Floor(diff / 3600) * 3600)) / 60) * 60)) > 0 then "
                    + "             Cast(Floor((diff - (Floor(diff / 3600) * 3600) - (Floor((diff - (Floor(diff / 3600) * 3600)) / 60) * 60))) as VarChar) + ' Seconds ' else '' End) + '$' "
                    + "             + lD + '$' + "
                    + "             '__________________________________________________________________' + '!$!','$$','~~~~~') as logDet "
                    + " From("
                    + "     Select obj, st, sC, fC,sTime,eTime,lD,DateDiff(Second,sTime,eTime) as diff "
                    + "     From("
                    + "     Select obj, String_Agg(stat,',') as st, Sum(successCnt) as sC, Sum(failedCnt) as fC,"
                    + "         Min(startTime) as sTime,Max(endTime) as eTime,"
                    + "         String_Agg((Case When logDet = '' then '' else 'Error Details:$' end) + logDet,',') as lD"
                    + "     From("
                    + "         Select obj,stat,(Case When stat = 'Success' then childObjCnt else 0 end) as successCnt,"
                    + "                 (Case When stat = 'Failed' then childObjCnt else 0 end) as failedCnt,"
                    + "                 startTime,endTime,logDet"
                    + "         From("
                    + "             Select ISNULL(Replace(Replace(Replace([Object],'Master',''),'Details',''),'Details',''),'') as obj,"
                    + "                 Count([Child Object]) as childObjCnt,"
                    + "                 MIn([Start Time]) as startTime,"
                    + "                 [Status] as stat,"
                    + "                 (Case When [Status] = 'Failed' then "
                    + "                     String_Agg((Case When [Child Object] = '-' then '' else [Child Object] end) + "
                    + "                     Replace(Replace(Replace('$' + LogDetails,'$$','~~~~'),'~~~~','$'),'$','$*+*+*+*'),',') else '' end) as logDet,"
                    + "                 Max([End Time]) as endTime"
                    + "             From LS_Audit_Log_Detail"
                    + "             Where LogId = '" + asLogId + "' And [Status] in ('Success','Failed') "
                    + "                  And [Object] Not Like ('%Summary%') And [Child Object] Not Like ('%Summary%')  "
                    + "             Group by [Object],[Status]"
                    + "         ) as a"
                    + "     ) as d"
                    + "     Group By obj"
                    + "     ) as b"
                    + " ) as c",
                    0, out aiSuccess);
        }

        /////////////////////////////////////////////////////////////////////////
        //Max Time Check for the Table
        /////////////////////////////////////////////////////////////////////////
        public void CheckMaxTime(GlobalVariable globalVariable, string asWhere, out int aiSuccess, out string asMessage)
        {
            asMessage = custExecSql(globalVariable, "Select IsNull(CONVERT(Varchar,[Max Time],120),'" +
                    globalVariable.defTime + "') as maxTime From LS_MaxTime " + asWhere,
                    0, out aiSuccess, 0, 0);
        }

        /////////////////////////////////////////////////////////////////////////
        //Branch List Fetch
        /////////////////////////////////////////////////////////////////////////
        public void GetBranchList(GlobalVariable globalVariable, out int aiSuccess, out string asMessage)
        {
            asMessage = custExecSql(globalVariable, "SELECT [br_code] AS brCode, [Branch Name] AS brName FROM LS_Branch_Master",
                    0, out aiSuccess);
        }
    }
}
