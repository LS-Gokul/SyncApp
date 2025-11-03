using LSSyncApp.Controllers;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Notifications;
using FluentFTP;

namespace LSSyncApp.Functions
{
    public class MasterConfig
    {
        private string isReturn = "", isMasterServerName, isMasterDBName, isMasterUID, isMasterPwd, isJson;
        private int iiAPi;
        private RestAPI restAPI = new RestAPI();

        private void setVariable(GlobalVariable gblVar)
        {
            isMasterServerName = gblVar.masterServerName;
            isMasterDBName = gblVar.masterdbName;
            isMasterUID = gblVar.masterUID;
            isMasterPwd = gblVar.masterPwd;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Execute Query in Master Database (Returns String Value)                                                                       //
        //______________________________________________________________________________________________________________________________//
        //| Parameters Name |  Parameter Type | Optional |                Definition                                   |  Default      |//
        //|_________________|_________________|__________|_____________________________________________________________|_______________|//
        //|   _gblVar       |  GlobalVariable |    No    | All the global variables                                    |               |//
        //|   asSql         |  string         |    No    | Sql Query for Execution                                     |               |//
        //|   aiSuccess     |  int (Out)      |    No    | Returns 1/0 (1 == Success / 0 == Failed)                    |               |//
        //|   aiJson        |  int            |    Yes   | Flag to tell whether Response should be in JSON / Raw       |      1        |//
        //|   asMessageOpt  |  string         |    Yes   | Optional Error Message to return(if Error)                  | No Rows Found |//
        //|   aiType        |  Int            |    Yes   | Flag to tell Select / Insert data (1 == Insert else Select) |      0        |//
        //|   aiDBType      |  string         |    Yes   | Flag to tell needs to connect 1 == 'master' DB              |      0        |//
        //|                 |                 |          |  0/2 == LSMASTER(if staging)/Leapsurgebi(if production) DB  |               |//
        //|                 |                 |          |  (if 0 only Select if 2 Select and Insert as per aiType     |               |//
        //|_________________|_________________|__________|_____________________________________________________________|_______________|//
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private string execSql(GlobalVariable _gblVar,string asSql, out int aiSuccess, int aiJson = 1,
            string asMessageOpt = "No Rows Found", int aiType = 0, int aiDBType = 0)
        {
            iiAPi = 1;//_gblVar.giApi;
            isJson = aiJson == 1 ? " FOR JSON PATH" : "";
            aiSuccess = 1;
            try
            {
                if (iiAPi == 1)
                {
                    isReturn = restAPI.postAPICalling(_gblVar.AppDataUrl + (aiDBType == 1 ? "/Master/" + aiType.ToString() :
                        (aiDBType == 2 ? "/Insert/" + aiType.ToString() : "")), "application/json", 
                        "\"" + asSql.Replace("\0", " ").Replace("\t", " ")
                                .Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("	", " ") + "\"",out _);
                    if (aiType == 1)
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
                        else if(isReturn == "[]")
                        {
                            aiSuccess = 0;
                        }
                    }
                }
                else
                {
                    setVariable(_gblVar);
                    isReturn = _gblVar.masterLogin.destConnSetup(isMasterServerName, (aiDBType == 1 ? "master" : isMasterDBName), isMasterUID, isMasterPwd, _gblVar.giMasterDBAuth);
                    if (isReturn == "Failed")
                    {
                        aiSuccess = 0;
                        isReturn += " - LS Server Connection Failed.";
                    }
                    else
                    {
                        if (aiType == 1)
                        {
                            isReturn = _gblVar.custDBConn.destExecQuery(asSql);
                            if (isReturn == "")
                            {
                                isReturn = "Success";
                            }
                        }
                        else
                        {
                            if (aiJson == 1)
                                isReturn = _gblVar.masterLogin.destDBExecRetOne("SELECT CAST((" + asSql + isJson + ") AS VARCHAR(MAX)) AS JSONDATA");
                            else
                                isReturn = _gblVar.masterLogin.destDBExecRetOne(asSql);
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
        //User Configuration Details
        /////////////////////////////////////////////////////////////////////////
        public void GetUserConfigDetails(GlobalVariable globalVariable, string asEmail, int aiType,
                    out int aiSuccess, out string asMessage, int aiJson = 1)
        {
            string lsEmail = aiType == 1 ? "[AAD User Id]" : "[Email]";
            aiSuccess = 1;

            isReturn = execSql(globalVariable, "Select ISNULL(a.[LSCode],'') lsCode,ISNULL(a.[user_group_code],'') userGrpCode,ISNULL(a.[Email],'') as email,"
                        + "        Trim(isnull(a.[First Name] + ' ', '') + isnull(a.[Last Name] + ' ', ''))  as userName,"
                        + "        Trim(isnull(b.[First_Name] +  ' ','') + isnull(b.[Middle_Name] + ' ','') + isnull(b.[Last_Name],'')) as custName,"
                        + "        c.[User Group Name] as userGrpName,ISNULL(d.[Firm Name],'') as firmName,ISNULL(d.ISLOGIN,1) AS isApi, "
                        + "        isnull(a.user_code,'') as userCode, ISNULL(a.application_code,'') as custAppCode "
                        + " from ls_user_master a JOIN ls_customer_master b ON a.LSCode = b.LSCode "
                        + "     JOIN LS_User_Group_Master c On a.user_group_code = c.user_group_code "
                        + $"     Left JOIN LS_customer_firm_details d ON a.LSCode = d.LSCode AND d.LSCode = '{globalVariable.custCode}'"
                        + $"        AND d.firm_code = '{globalVariable.firmCode}' AND d.Active = 1 "
                        + $" Where a.{lsEmail} = '{asEmail}' And a.[Access Sync Application] = 1 And a.[Isactive] = 1",
                    out aiSuccess, aiJson, "User Not Exists");
            
            asMessage = isReturn;
        }

        /////////////////////////////////////////////////////////////////////////
        //Customer Details
        /////////////////////////////////////////////////////////////////////////
        public void GetBasicDetails(GlobalVariable globalVariable, string asEmail, int aiType,
                    out int aiSuccess, out string asMessage, int aiJson = 1)
        {
            string lsEmail = aiType == 1 ? "[AAD User Id]" : "[Email]";
            aiSuccess = 1;

            isReturn = execSql(globalVariable, "Select ISNULL(a.[LSCode],'') lsCode, ISNULL(a.[Email],'') as email, "
                + "     Coalesce(a.[Default Firm],Min(c.firm_code),'0001') defFirm"
                + " from ls_user_master a JOIN ls_customer_master b ON a.LSCode = b.LSCode And b.Status = 1 "
                + "         Left Join LS_Customer_Firm_Details c On b.LSCode = c.LSCode And Active = 1 "
                + $" Where a.{lsEmail} = '{asEmail}' And a.[Access Sync Application] = 1 And a.[Isactive] = 1 "
                + " Group By a.[LSCode], a.[Email], a.[Default Firm] ",
                    out aiSuccess, aiJson, "User Not Exists");
            
            asMessage = isReturn;
        }

        /////////////////////////////////////////////////////////////////////////
        //Customer Software
        /////////////////////////////////////////////////////////////////////////
        public void GetFirmConfig(GlobalVariable globalVariable, out int aiSuccess, out string asMessage, int aiJson = 1)
        {
            asMessage = execSql(globalVariable, "SELECT Count(a.report_code) as Cnt,c.[Software Name] as stName,"
                                + "     c.software_code as stCode,ISNULL(e.SyncDays,7) as syncDays,d.[Sync Name] as syncName,"
                                + "     ISNULL(f.[Profile Id],'') as profileId, ISNULL(e.[Workspace Id],'') as workspaseId "
                                + " FROM LS_Report_Config a join LS_Software_Config b On a.LSCode = b.LSCode And a.firm_code = b.firm_code"
                                + "     Join LS_Software_Master c On b.software_code = c.software_code"
                                + "     Join LS_Sync_Type d On b.sync_code = d.sync_code"
                                + "     JOIN LS_Customer_Firm_Details e ON a.LSCode = e.LSCode AND a.firm_code = e.firm_code"
                                + "     JOIN LS_Customer_Master f On a.LSCode = f.LSCode "
                                + $" WHERE a.LSCode = '{globalVariable.custCode}' AND a.firm_code = '{globalVariable.firmCode}' "
                                + "Group By c.[Software Name],c.software_code,e.SyncDays,d.[Sync Name],f.[Profile Id],e.[Workspace Id]",
                out aiSuccess, aiJson);
        }

        /////////////////////////////////////////////////////////////////////////
        //List of Source Table
        /////////////////////////////////////////////////////////////////////////
        public void GetSourceTableList(GlobalVariable globalVariable, out int aiSuccess, out string asMessage, int aiJson = 1)
        {
            asMessage = execSql(globalVariable, "SELECT [custTableName] as tableName,brColName as brColName,[timeQuery] as timeQuery"
                    + " FROM (select distinct a.[Customer Table Name] as custTableName,a.ISBranchwise as brWise, "
                    + " IsNull(a.[Branch Column Name],'') as brColName, isnull(a.[Maxtime Condition],'') as timeQuery "
                    + " FROM LS_Software_Table_Master a JOIN LS_Software_Config b ON a.software_code = b.software_code "
                    + "                                 JOIN LS_Report_Config c ON b.LSCode = c.LSCode AND b.firm_code = c.firm_code "
                    + "                                 JOIN LS_Table_Config d ON a.table_code = d.table_code AND c.report_code = d.report_code "
                    + $"                                        and d.software_code = '{globalVariable.gsSoftwareCode}' "
                    + $" WHERE a.[Active Flag] = 1 AND b.LSCode = '{globalVariable.custCode}' AND b.firm_code = '{globalVariable.firmCode}' "
                    + "         AND c.Selected = 1 AND c.Status = 2 AND d.Active = 1) as a",
                out aiSuccess, aiJson);
        }

        /////////////////////////////////////////////////////////////////////////
        //Customer & Firm Exiss
        /////////////////////////////////////////////////////////////////////////
        public void GetCustFirmExists(GlobalVariable globalVariable, out int aiSuccess, out string asMessage, int aiJson = 0)
        {
            asMessage = execSql(globalVariable, "SELECT COUNT(*) FROM LS_Customer_Firm_Details "
                    + $" WHERE [LSCode] = '{globalVariable.custCode}' AND [firm_code] = '{globalVariable.firmCode}' AND Active = 1",
                out aiSuccess, aiJson);
        }

        /////////////////////////////////////////////////////////////////////////
        //Get Customer's Custom DataBase Configuration
        /////////////////////////////////////////////////////////////////////////
        public void GetCustomerCustomDB(GlobalVariable globalVariable, out int aiSuccess, out string asMessage, int aiJson = 1)
        {
            asMessage = execSql(globalVariable, "SELECT TOP 1 [sequence] as Seq,[Server Name] as serverName,[User Id] as userID,[Password] as pwd,"
                    + "   [Group] as grp,[DataBase Name] as dbName,[Host] as host,[Port] as port "
                    + " FROM LS_Destination_Config "
                    + $" WHERE LSCode = '{globalVariable.custCode}' AND firm_code = '{globalVariable.firmCode}' AND [Active Flag] = 1 "
                    + " Order By [Date of Modification] desc ",
                out aiSuccess, aiJson);
        }

        /////////////////////////////////////////////////////////////////////////
        //Get Customer's Firm List
        /////////////////////////////////////////////////////////////////////////
        public void GetFirmList(GlobalVariable globalVariable, out int aiSuccess, out string asMessage, int aiJson = 1)
        {
            asMessage = execSql(globalVariable, "SELECT [firm_code] as firmCode,[Firm Name] as firmName FROM  LS_Customer_Firm_Details"
                    + $" WHERE [LSCode] = '{globalVariable.custCode}' AND Active = 1",
                out aiSuccess, aiJson);
        }

        /////////////////////////////////////////////////////////////////////////
        //Get Fetch Table List
        /////////////////////////////////////////////////////////////////////////
        public void GetFetchTableList(GlobalVariable globalVariable, int aiSyncType, string asParam, out int aiSuccess,
                out string asMessage, string asType = "Not In (0, -1)", int aiJson = 1, string asTable = "")
        {
            asMessage = execSql(globalVariable, "SELECT DISTINCT b.table_code as tableCode, c.[Table Name] as tableName, ISNULL(cq.[Table Type],b.[Table Type]) as tableType,"
                    + "     ISNULL(c.[Branch Column Name],'') AS brColName, ISNULL(c.[LastSync Time Column Name],'') AS timeColName, "
                    + "     ISNULL(c.[Tranwise Column Name],'') as tranColName, ISNULL(c.[Fin Year Column Name],'') as finColName, "
                    + "     ISNULL(c.[Del Ref Column Name],'') as delColName, c.SortSeq, "
                    + "     Replace(Replace(Replace(Replace(c.[Table Name],'LS_',''),'Master',''),'Detail',''),'_',' ') as tblShort "
                    + " FROM LS_Report_Config a JOIN LS_Table_Config b ON a.report_code = b.report_code " + (asType == " In (0, -1) " ? "" : $" and b.software_code = '{globalVariable.gsSoftwareCode}' ")
                    + "                         JOIN LS_Table_Master c ON b.table_code = c.table_code"
                    + "                         JOIN LS_Report_Master d ON a.report_code = d.report_code"
                    + "                         JOIN LS_Report_Module_Master e ON d.module_code = e.module_code"
                    + "                         LEFT JOIN LS_Cust_Query cq ON a.LSCode = cq.LSCode AND a.firm_code = cq.firm_code And b.table_code = cq.table_code And cq.[Active] = 1 "
                    + $" WHERE a.LSCode = '{globalVariable.custCode}' AND a.firm_code = '{globalVariable.firmCode}'"
                    + "     AND a.[Status] In (2,1) AND b.Active = 1 AND c.Active = 1 AND d.Active = 1 "
                    + $"     AND b.[Table Type] {asType} And (Case when {aiSyncType} = 1 then Trim(Replace(e.[Sync Param],'-All','')) when "
                    + $"{aiSyncType} = 2 then Trim(Replace(d.[Sync Param],'-All','')) end) in ({asParam})"
                    + (asTable == "" ? "" : $" And b.table_code = '{asTable}'")
                    + " ORDER BY c.SortSeq,c.[Table Name]",
                out aiSuccess, aiJson);
        }

        /////////////////////////////////////////////////////////////////////////
        //Get Fetch Table Select Query
        /////////////////////////////////////////////////////////////////////////
        public void GetFetchTableQuery(GlobalVariable globalVariable, string asTableCode, out int aiSuccess, out string asMessage, int aiJson = 0)
        {
            asMessage = execSql(globalVariable, "SELECT Top 1 fetchQuery FROM("
                    + " SELECT a.[Version] as ver,1 as seq,IsNull([Query],'') as fetchQuery FROM LS_Cust_Query a "
                    + $" WHERE a.LSCode = '{globalVariable.custCode}' AND a.firm_code = '{globalVariable.firmCode}' "
                    + $" AND a.table_code = '{asTableCode}' AND a.Active = 1 And a.[Version] <= {globalVariable.gsVersion}"
                    + " UNION ALL "
                    + " SELECT b.[Version] as ver,2 as seq,IsNull(b.[Query],'') as fetchQuery "
                    + " FROM LS_Table_Master a JOIN LS_Query b ON a.table_code = b.table_code "
                    + "                        JOIN LS_Source_Config c ON b.software_code = c.software_code "
                    + $" WHERE a.Active = 1 AND b.Active = 1 AND a.table_code = '{asTableCode}' "
                    + $"        AND c.LSCode = '{globalVariable.custCode}' AND c.firm_code = '{globalVariable.firmCode}'"
                    + $" And b.[Version] <= {globalVariable.gsVersion}) as a "
                    + " Order By seq,ver desc",
                out aiSuccess, aiJson, "Fetch Query not found");
        }

        /////////////////////////////////////////////////////////////////////////
        //Get Source List
        /////////////////////////////////////////////////////////////////////////
        public void GetSourceList(GlobalVariable globalVariable, out int aiSuccess, out string asMessage, 
            int aiJson = 1, int aiType = 0, string asSource = "")
        {
            asMessage = execSql(globalVariable, aiType == 0 ? "SELECT finYear, serverName, dbName, stName, host, path, port, "
                    + "       curYear, uID, pwd, syncType, minTime, seq, dbTypeName, authType "
                    + "FROM( SELECT isnull(a.[Financial Year],'') AS finYear, isnull(a.[Server Name],'') AS serverName, "
                    + "         isnull(a.[DataBase Name],'') AS dbName, isnull(a.Host,'') AS host,isnull(a.Path,'') AS [path], "
                    + "         isnull(Cast(a.Port as VarChar(6)),'0') AS port, isnull(a.[Current Year],0) AS curYear,d.[Software Name] as stName, "
                    + "         isnull(a.[User ID],'') AS uID,isnull(a.[Password],'') as pwd,isnull(c.[Sync Name],'') as syncType,"
                    + "         isnull(CONVERT(VARCHAR,a.[Min Time],120),'') as minTime, a.sequence as seq, isnull(dm.[Database Name],'') as dbTypeName, "
                    + "         ISNULL((Case When sdm.IsCredentialRequired = 1 Then 1 When am.[Authentication Name] = 'Service Principal' Then 0 "
                    + "             When am.[Authentication Name] = 'Windows Authentication' Then 2 Else -1 End),-1) As authType "
                    + "     FROM LS_Source_Config a "//JOIN LS_Software_Config b ON a.LSCode = b.LSCode AND  a.firm_code = b.firm_code And b.[Active Flag] = 1 "
                    + "                         JOIN LS_Sync_Type c ON a.sync_code = c.sync_code and c.[Active] = 1 "
                    + "                         JOIN LS_Software_Master d ON a.software_code = d.software_code and d.[Active] = 1 "
                    + "                         LEFT JOIN LS_Database_Master dm ON a.database_code = dm.database_code and dm.[Active] = 1 "
                    + "                         LEFT JOIN LS_DataBase_Auth_Master am ON a.auth_code = am.auth_code and am.[Active] = 1 "
                    + "                         LEFT JOIN LS_Software_Database_Master sdm ON a.software_code = sdm.software_code and "
                    + "                                  a.database_code = sdm.database_code and a.auth_code = sdm.auth_code and sdm.[Active] = 1 "
                    + $"     WHERE a.LSCode = '{globalVariable.custCode}' AND a.firm_code = '{globalVariable.firmCode}' And a.[Active Flag] = 1 "
                    + (asSource == "" ? "" : " And a.[Server Name] = '" + asSource + "' ")
                    + ") AS a " 
                    
                    : 
                    
                    "SELECT a.[Server Name] AS [Server], a.[DataBase Name] AS [Database], a.Host AS [Host],a.Port AS [Port]"
                    + " FROM LS_Source_Config a"
                    + $" WHERE a.LSCode = '{globalVariable.custCode}' AND a.firm_code = '{globalVariable.firmCode}' And a.[Active Flag] = 1",
                out aiSuccess, aiJson);
        }

        /////////////////////////////////////////////////////////////////////////
        //Get Latest Version Application
        /////////////////////////////////////////////////////////////////////////
        public void GetAppLatstVersion(GlobalVariable globalVariable, out int aiSuccess, out string asMessage, int aiJson = 1)
        {
            asMessage = execSql(globalVariable, "Select TOP 1 [Version] as ver,[Application Url] as appUrl from LS_App_URL "
                    + $" Where application_code = '{globalVariable.gsAppCode}' AND [Version] > {globalVariable.gsAppVersion}"
                    + " And [Active Flag] = 1 Order By [Version] DESC ",
                out aiSuccess, aiJson, "No");
            globalVariable.gsAppUpdateList = asMessage;
        }

        /////////////////////////////////////////////////////////////////////////
        //Get Reports Schedular list for Task Schedular Creation
        /////////////////////////////////////////////////////////////////////////
        public void GetSchedulerList(GlobalVariable globalVariable, out int aiSuccess, out string asMessage, int aiJson = 1)
        {
            asMessage = execSql(globalVariable, "SELECT c.[Report Name] as rptName, CONVERT(VARCHAR,ISNULL(a.[Start Time],getdate()),120) as startTime,"
                    + "     ISNULL(a.[Sync Days],0) as syncDays, ISNULL(a.[Sync Interval],0) as syncInterval, ISNULL(Trim(Replace(c.[Sync Param],'-All','')),'') as syncParam "
                    + " FROM LS_Sync_Setup a Join LS_Report_Config b On a.LSCode = b.LSCode And a.firm_code = b.firm_code And a.report_code = b.report_code "
                    + "                     Join LS_Report_Master c On b.report_code = c.report_code "
                    + " Where b.[Selected] = 1 And b.[Status] In (2,1) And a.[Active Flag] = 1 And c.[Active] = 1 "
                    + $"           And a.LSCode = '{globalVariable.custCode}' And a.firm_code = '{globalVariable.firmCode}'",
                out aiSuccess, aiJson);
        }

        /////////////////////////////////////////////////////////////////////////
        //Get Summary Tables
        /////////////////////////////////////////////////////////////////////////
        public void GetReportList(GlobalVariable globalVariable, string aiType, out int aiSuccess, out string asMessage, int aiJson = 1)
        {
            asMessage = execSql(globalVariable, "SELECT modName, params, Row_Number() Over(Order By modName) as seq, rptCodeList "
                    + "FROM("
                    + " SELECT ISNULL(modName,'') as modName, ISNULL(params,'') as params,STRING_AGG('''' +  ISNULL(rptCode,'') +  '''',',') as rptCodeList"
                    + " FROM( SELECT DISTINCT e.[Module Name] as modName, e.[Sync Param] as params, a.report_code as rptCode "
                    + $"       FROM LS_Report_Config a JOIN LS_Table_Config b ON a.report_code = b.report_code and b.software_code = '{globalVariable.gsSoftwareCode}'"
                    + "                         JOIN LS_Table_Master c ON b.table_code = c.table_code"
                    + "                         JOIN LS_Report_Master d ON a.report_code = d.report_code"
                    + "                         JOIN LS_Report_Module_Master e ON d.module_code = e.module_code"
                    + $"       WHERE a.LSCode = '{globalVariable.custCode}' AND a.firm_code = '{globalVariable.firmCode}'"
                    + "             AND a.[Status] In (2,1) AND b.Active = 1 AND c.Active = 1 AND d.Active = 1 "
                    + $"            AND b.[Table Type] <> 0 AND {aiType} = 1 "
                    + "       Union All "
                    + "       SELECT DISTINCT d.[Report Name] as modName, d.[Sync Param] as params,a.report_code as rptCode "
                    + $"       FROM LS_Report_Config a JOIN LS_Table_Config b ON a.report_code = b.report_code /*and b.software_code = '{globalVariable.gsSoftwareCode}'*/"
                    + "                         JOIN LS_Table_Master c ON b.table_code = c.table_code"
                    + "                         JOIN LS_Report_Master d ON a.report_code = d.report_code"
                    + $"       WHERE a.LSCode = '{globalVariable.custCode}' AND a.firm_code = '{globalVariable.firmCode}'"
                    + "             AND a.[Status] In (2,1) AND b.Active = 1 AND c.Active = 1 AND d.Active = 1 "
                    + $"            AND b.[Table Type] <> 0 AND {aiType} = 2 "
                    + "     ) as a "
                    + " GROUP BY modName, params"
                    + ") as modList ",
                out aiSuccess, aiJson);
        }

        /////////////////////////////////////////////////////////////////////////
        //aiType = 1 => Check Master Version Table Exists
        //aiType = 2 => Get Master Latest Version Details
        /////////////////////////////////////////////////////////////////////////
        public void CheckMasterVersionTable(GlobalVariable globalVariable, int aiType, out int aiSuccess, out string asMessage, string asVer = null)
        {
            switch (aiType)
            {
                case 1:
                    asMessage = execSql(globalVariable, "Select Count(*) From sys.tables where name = 'LS_Version_Master'",
                        out aiSuccess, 0);
                    break;
                case 2:
                    asMessage = execSql(globalVariable, "Select TOP 1 [Version] as Ver,"
                            + " Convert(VARCHAR,ISNULL([Released Date],GetDate()),20) as relDate,ISNULL([Release Notes],'') as relNotes "
                            + " From LS_Version_Master "
                            + $" Where application_code = '{globalVariable.gsAppCode}' And [Active Flag] = 1 And "
                            + $"     software_code = '{globalVariable.gsSoftwareCode}' Order By [Version] Desc ",
                        out aiSuccess);
                    break;
                case 3:
                    asMessage = execSql(globalVariable, "Select ISNULL([Release Notes],'') as relNotes "
                            + " From LS_Version_Master "
                            + $" Where application_code = '{globalVariable.gsAppCode}' And "
                            + $" [Active Flag] = 1 And software_code = '{globalVariable.gsSoftwareCode}' And Version = {asVer}",
                        out aiSuccess);
                    break;
                case 4:
                    asMessage = execSql(globalVariable, "Select convert(varchar,[Version]) as Ver,"
                            + "     Convert(varchar,ISNULL([Released Date],GETDATE()),20) as releasedDate,"
                            + "     COALESCE([Script],'-') as verScript, COALESCE([Application Url],'-') as appUrl "
                            + $" From LS_Version_Master "
                            + $" Where application_code = '{globalVariable.gsAppCode}' And [Active Flag] = 1 And Version > {asVer}"
                            + $" And software_code = '{globalVariable.gsSoftwareCode}' Order By [Version]",
                        out aiSuccess);
                    break;
                default:
                    aiSuccess = 0;
                    asMessage = "Failed";
                    break;
            }
        }

        /////////////////////////////////////////////////////////////////////////
        //Get Report Name
        /////////////////////////////////////////////////////////////////////////
        public void GetReportName(GlobalVariable globalVariable, string asParam, int aiSyncType,
            out int aiSuccess, out string asMessage, int aiJson = 0)
        {
            asMessage = execSql(globalVariable, "SELECT a.report_code as reportCode, a.[Report Name] as reportName "
                    + " FROM LS_Report_Master a JOIN LS_Report_Module_Master b ON a.module_code = b.module_code"
                    + $"         JOIN LS_Sync_Setup c On a.report_code = c.report_code And c.LSCode = '{globalVariable.custCode}' And "
                    + $"                 c.firm_code = '{globalVariable.firmCode}' And c.[Active Flag] = 1 And c.[Auto Refresh Dataset] = 1 "
                    + $" WHERE (Case when {aiSyncType} = 1 then Trim(Replace(b.[Sync Param],'-All','')) when "
                    + $"{aiSyncType} = 2 then Trim(Replace(a.[Sync Param],'-All','')) end) in ({asParam})"
                , out aiSuccess, aiJson);
        }
        
        /////////////////////////////////////////////////////////////////////////
        //Get Application Configuration Settings
        /////////////////////////////////////////////////////////////////////////
        public void GetApplicationConfiguration(GlobalVariable globalVariable, out int aiSuccess, out string asMessage, int aiJson = 1)
        {
            asMessage = execSql(globalVariable, "SELECT ISNULL([Embed Autentication Mode] ,'') AS embedAutenticationMode," + Environment.NewLine
                    + "     ISNULL([Embed Tenant Id], '') AS embedTenantId,ISNULL([Embed Client Id] ,'') AS embedClientId," + Environment.NewLine
                    + "     ISNULL([Embed Client Secret] ,'') AS embedClientSecret,ISNULL([Embed Scope] ,'') AS embedScope," + Environment.NewLine
                    + "     ISNULL([Embed Authority] ,'') AS embedAuthority,ISNULL([B2C Issuer] ,'') AS b2CIssuer," + Environment.NewLine
                    + "     ISNULL([B2C Tenant Id] ,'') AS b2CTenantId,ISNULL([B2C Client Id] ,'') AS b2CClientId," + Environment.NewLine
                    + "     ISNULL([B2C Client Secret] ,'') AS b2CClientSecret,ISNULL([Application Container Folder] ,'') AS applicationContainerFolder," + Environment.NewLine
                    + "     ISNULL([B2C Sign InUp Policy] ,'') AS b2CSignInUpPolicy,ISNULL([B2C Edit Policy] ,'') AS b2CEditPolicy," + Environment.NewLine
                    + "     ISNULL([B2C Reset Policy] ,'') AS b2CResetPolicy,ISNULL([B2C Redirect Url] ,'') AS b2CRedirectUrl," + Environment.NewLine
                    + "     ISNULL([B2C Authority Base] ,'') AS b2CAuthorityBase,ISNULL([B2C Scope] ,'') AS b2CScope," + Environment.NewLine
                    + "     ISNULL([Client API Url] ,'') AS clientAPIUrl,ISNULL([DB Suffix] ,'') AS dBSuffix,ISNULL([Container URL] ,'') AS containerURL," + Environment.NewLine
                    + "     ISNULL([Embed Api Domain],'') AS embedApiDomain, ISNULL([Embed Resource Group Name],'') AS embedResourceGroup," + Environment.NewLine
                    + "     ISNULL([Embed Resource Name],'') AS embedResource, ISNULL([Embed Resource Subscription Id],'') AS embedResourceSubscriptionId, " + Environment.NewLine
                    + "     IsNull([Check Embed],0) as checkEmbed, IsNull([LS API Generate Token URL], '') as signInAPI " + Environment.NewLine
                    + $" FROM[dbo].[LS_App_Setting] Where[Active] = 1 And[application_code] = '{globalVariable.gsAppCode}'" , 
                out aiSuccess, aiJson);
        }

        /////////////////////////////////////////////////////////////////////////
        //Execute Raw Sql
        /////////////////////////////////////////////////////////////////////////
        public void ExecuteRawQuery(GlobalVariable globalVariable, string asSql, int aiType, out int aiSuccess, 
            out string asMessage, int aiJson = 1, int aiDBType = 0)
        {
            asMessage = execSql(globalVariable, asSql, out aiSuccess, aiJson, "Failed", aiType, aiDBType);
        }

        /////////////////////////////////////////////////////////////////////////
        //Create Sync Application / Source System Status
        /////////////////////////////////////////////////////////////////////////
        public void SyncAppStatus(GlobalVariable globalVariable, out int aiSuccess, out string asMessage, string asSyncAppStatus = "1")
        {
            string lsServerName, lsStName = "", lsHost, lsPort, lsUID, lsPwd, lsSyncType, lsDbName;//, lsDbName, lsFinYear, lsMinTime, lsPath, lsCurYear ;
            string lsStatus = "0", lsQuery, lsNotification = "", lsErrMsg = "", lsLogFile = "CheckStatus.txt";
            int liAuthType;

            globalVariable.logFile(lsLogFile, DateTime.Now.ToString() + " - Start" , 1);
            GetSourceList(globalVariable, out aiSuccess, out asMessage);
            
            if (aiSuccess == 1)
            {
                JsonElement _ljeSourceList = globalVariable.createJsonElement(asMessage);
                int liCount = _ljeSourceList.EnumerateArray().Count(), liRows = 3;
                string[,] asSourceList = new string[liCount,liRows];
                
                for (int i = 0; i < liCount; i++)
                {
                    //lsFinYear = _ljeSourceList[i].GetProperty("finYear").ToString();
                    //lsPath = _ljeSourceList[i].GetProperty("path").ToString();
                    //lsCurYear = _ljeSourceList[i].GetProperty("curYear").ToString();
                    //lsMinTime = _ljeSourceList[i].GetProperty("minTime").ToString();
                    lsDbName = _ljeSourceList[i].GetProperty("dbName").ToString();
                    lsServerName = _ljeSourceList[i].GetProperty("serverName").ToString();
                    lsStName = _ljeSourceList[i].GetProperty("stName").ToString();
                    lsHost = _ljeSourceList[i].GetProperty("host").ToString();
                    lsPort = _ljeSourceList[i].GetProperty("port").ToString();
                    lsUID = _ljeSourceList[i].GetProperty("uID").ToString();
                    lsPwd = _ljeSourceList[i].GetProperty("pwd").ToString();
                    lsSyncType = _ljeSourceList[i].GetProperty("syncType").ToString();
                    liAuthType = int.Parse(_ljeSourceList[i].GetProperty("authType").ToString());

                    switch (lsStName)
                    {
                        case "FTP":
                        case "SFTP":
                            try
                            {
                                FtpClient client = new FtpClient(lsHost);
                                client.Port = int.Parse(lsPort);
                                client.Credentials = new System.Net.NetworkCredential(lsUID, lsPwd);
                                client.Connect();
                                client.Disconnect();
                                lsStatus = "1";
                                lsErrMsg = "Success";
                            }
                            catch (Exception ExFtp)
                            {
                                lsStatus = "0";
                                lsErrMsg = ExFtp.Message;
                            }
                            break;
                        case "SAP":
                            string custDb, custCurSch;
                            custDb = (lsDbName.IndexOf(";") > 0 ? lsDbName.Substring(0, lsDbName.IndexOf(";")) : lsDbName);
                            custCurSch = (lsDbName.IndexOf(";") > 0 ? lsDbName.Substring(lsDbName.IndexOf(";") + 1) : "");
                            lsErrMsg = globalVariable.odbcConn.sapHanaDBConn(lsServerName, lsHost, lsPort, lsUID, lsPwd, custDb, custCurSch);
                            lsStatus = (lsErrMsg == "Success" ? "1" : "0");
                            break;
                        case "6 Orbit ERP":
                            lsErrMsg = globalVariable.odbcConn.mySqlDBConn(lsHost, lsPort, lsDbName, lsUID, lsPwd);
                            lsStatus = (lsErrMsg == "Success" ? "1" : "0");
                            break;
                        case "LOGIC ERP":
                        case "EasySol":
                        case "Disha":
                        lsErrMsg = globalVariable.odbcConn.destConnSetup(lsServerName, lsDbName, lsUID, lsPwd, liAuthType);
                            lsStatus = (lsErrMsg == "Success" ? "1" : "0");
                            break;
                        case "Busy":
                            lsErrMsg = globalVariable.odbcConn.srcDBConn(lsServerName, lsUID, lsPwd);
                            lsStatus = (lsErrMsg == "Success" ? "1" : "0");
                            break;
                        case "Bizom":
                            break;
                        case "Tally":
                            TallyODBC _TallyODBC = new TallyODBC();
                            Task<int> liTallyConn = _TallyODBC.CheckConnection(lsHost, lsPort);
                            lsStatus = liTallyConn.Result.ToString();
                            lsErrMsg = "Not Running";
                            _TallyODBC.Dispose();
                            break;
                        default:
                            switch (lsSyncType)
                            {
                                case "API":
                                    Task<string> lsApiConn = ApiCallAsync(globalVariable, "http://" + lsHost + ":" + lsPort + "/ws_leapsurge_service",
                                        "valueType=checkStatus", "application/x-www-form-urlencoded");
                                    lsErrMsg = lsApiConn.Result;
                                    lsStatus = (lsErrMsg == "Success" ? "1" : "0");
                                    break;
                                case "ODBC":
                                    lsErrMsg = globalVariable.custDBConn.srcDBConn(lsServerName, lsUID, lsPwd);
                                    lsStatus = (lsErrMsg == "Success" ? "1" : "0");
                                    break;
                                default:
                                    continue;
                            }
                            break;
                    }
                    string lsTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    globalVariable.logFile(lsLogFile, lsStName + "-" + lsSyncType + "-" + lsStatus + Environment.NewLine + lsErrMsg, 1);
                    lsQuery = "Merge Into LS_Sync_App With(HoldLock) as a " + Environment.NewLine
                        + $" Using (Select '{globalVariable.custCode}' as LSCode, '{globalVariable.firmCode}' as firm_code, " + Environment.NewLine
                        + $"               '{globalVariable.gsSystemName}' as [Device Name], '{globalVariable.gsSystemIp}' as [IP], " + Environment.NewLine
                        + $"               '{lsServerName}' as [Source Server], {lsStatus} as [SS Status]) as b " + Environment.NewLine
                        + " On a.[LSCode] = b.[LSCode] And a.[firm_code] = b.firm_code And a.[Source Server] = b.[Source Server] " + Environment.NewLine
                        + " When Matched Then " + Environment.NewLine
                        + $"     Update Set a.[SS Status] = b.[SS Status], a.[SS Last Checked Time] = '{lsTime}', " + Environment.NewLine
                        + "                 a.[Device Name] = b.[Device Name], a.[IP] = b.[IP], " + Environment.NewLine
                        + $"                a.[Last Active] = '{lsTime}', a.[Status] = {asSyncAppStatus}"
                        + (lsStatus == "1" ? $", a.[SS Last Active Time] = '{lsTime}' " : " ") + Environment.NewLine
                        + " When Not Matched Then " + Environment.NewLine
                        + "     Insert([LSCode], [firm_code], [Device Name], [IP], [Source Server], [SS Status], " + Environment.NewLine
                        + "             [SS Last Checked Time], [SS Last Active Time], [Status], [Last Active], [Intimate]) " + Environment.NewLine
                        + "     Values(b.[LSCode], b.[firm_code], b.[Device Name], b.[IP],[Source Server], b.[SS Status], " + Environment.NewLine
                        + $"            '{lsTime}', '{lsTime}', {asSyncAppStatus}, '{lsTime}', 0);";
                    
                    asSourceList[i, 0] = lsServerName;
                    asSourceList[i, 1] = (lsStatus == "1" ? "Success" : "Failed");
                    asSourceList[i, 2] = lsErrMsg;
                    
                    lsNotification += (lsStatus == "1" ? "" : lsServerName + " is not Running!" + Environment.NewLine);
                    asMessage = execSql(globalVariable, lsQuery, out aiSuccess, 0, "Failed", 1, 2);
                    globalVariable.logFile(lsLogFile, asMessage + " - End", 1);
                }


                if(lsNotification != "" && lsNotification != null)
                {
                    ToastContentBuilder _ToastContentBuilder = new ToastContentBuilder()
                        .AddArgument("action", "viewConversation")
                        .AddArgument("conversationId", 9813)
                        .AddText("LeapSurge")
                        .AddText(lsStName + " Server" + Environment.NewLine + lsNotification);
                    _ToastContentBuilder.Show();
                }
                lsQuery = "";
                lsNotification = "";
                int liUpdateCount = 0;
                for (int i = 0; i < liCount; i++)
                {
                    CheckIntimationStatus(globalVariable, asSourceList[i, 0], out int liSuccess, out string lsReturn);
                    if(liSuccess == 1)
                    {
                        if (lsReturn == "1")
                        {
                            if (asSourceList[i, 1] == "Success")
                            {
                                lsNotification += $"<p style=\"font-size: 16px;\"><span style=\" color:#272727; font-size: 14px;\"><strong>{asSourceList[i, 0]}</strong> Started Running</span></p>";
                                lsQuery += $"Update LS_Sync_App Set [Intimate] = 0 Where LSCode = '{globalVariable.custCode}' And " + Environment.NewLine
                                    + $"     firm_code = '{globalVariable.firmCode}' And [Source Server] = '{asSourceList[i, 0]}';";
                                liUpdateCount++;
                            }
                        }
                        else
                        {
                            if (asSourceList[i, 1] == "Failed")
                            {
                                
                                lsNotification += $"<p style=\"font-size: 16px;\"><span style=\" color:#272727; font-size: 14px;\"><strong>{asSourceList[i, 0]}</strong> Not Running</span></p><br>" 
                                    + $"<p style=\"font-size: 16px;\"><span style=\" color:#272727; font-size: 14px;\"><strong>Reason : </strong>{asSourceList[i, 2]}</span></p>";
                                lsQuery += $"Update LS_Sync_App Set [Intimate] = 1 Where LSCode = '{globalVariable.custCode}' And " + Environment.NewLine
                                    + $"     firm_code = '{globalVariable.firmCode}' And [Source Server] = '{asSourceList[i, 0]}';";
                                liUpdateCount++;
                            }
                        }
                    }
                }

                if(liUpdateCount > 0)
                {
                    string lsTo = "";
                    asMessage = execSql(globalVariable, lsQuery, out aiSuccess, 0, "Failed", 1, 2);
                    globalVariable.logFile(lsLogFile, "UpdateStatus - " + asMessage + " - End", 1);

                    GetSpoc(globalVariable, out aiSuccess, out asMessage);
                    if (aiSuccess == 1)
                    {
                        JsonElement ljeSpoc = globalVariable.createJsonElement(asMessage);
                        liUpdateCount = ljeSpoc.EnumerateArray().Count();
                        for (int i = 0; i < liUpdateCount; i++)
                        {
                            lsTo += ljeSpoc[i].GetProperty("email").ToString() + ";";
                        }
                        if(liUpdateCount > 0)
                        {
                            globalVariable._fun.Templates(1, globalVariable.gsCustAppCode, out _, out string lsTemplate);
                            lsTemplate = lsTemplate.Replace("@ServerName", lsNotification).
                                Replace("@Software", globalVariable.gsSoftwareName).Replace("@CompanyName",globalVariable.firmName);
                            Notifications _N = new Notifications();
                            //_N.SendMail(lsTo, "LS Engine", lsTemplate, out aiSuccess, out asMessage);
                            //lsTo = "gokul@leapsurge.in;naveen@leapsurge.in";
                            asMessage = _N.SendMail(lsTo, lsStName + " Server Status", lsTemplate).ToString();
                            globalVariable.logFile(lsLogFile,"SPOC Mail - " + asMessage + " - End", 1);
                        }
                    }
                    else
                    {
                        globalVariable.logFile(lsLogFile, "SPOC Mail - " + asMessage + " - End", 1);
                    }
                }
            }
        }

        public void GetSpoc(GlobalVariable globalVariable, out int aiSuccess, out string asMessage)
        {
            asMessage = execSql(globalVariable, "Select b.Email as email, b.[First Name] as fName " + Environment.NewLine
                    + " From LS_Spoc_Config a Join LS_User_Master b On a.user_code = b.user_code " + Environment.NewLine
                    + $" Where a.LSCode = '{globalVariable.custCode}' Union All " + Environment.NewLine
                    + $" Select Email as email, [First Name] as fName from LS_User_Master Where LSCode = '{globalVariable.custCode}' And is_support_admin = 1 ",
                out aiSuccess);
        }

        private void CheckIntimationStatus(GlobalVariable globalVariable, string asSource, out int aiSuccess, out string asMessage)
        {
            asMessage = execSql(globalVariable,
                $"Select [Intimate] as intiMate From LS_Sync_App Where LSCode = '{globalVariable.custCode}' And " + Environment.NewLine
                    + $"     firm_code = '{globalVariable.firmCode}' And [Source Server] = '{asSource}'" + Environment.NewLine,
                out aiSuccess);
        }


        private async Task<string> ApiCallAsync(GlobalVariable globalVariable, string asUrl, string asBody, string asContentType)
        {
            return await globalVariable._grRestAPI.PostUrlAsync(asUrl, asBody, asContentType);
        }

        /////////////////////////////////////////////////////////////////////////
        //Notifications / Triggers
        //>> Trigger Type ==> 0 - Push Notification  1 - Trigger Report to sync   2 - Exe Update  3 - Just Notification  4 - Notification With Buttons  5 - Just Notification With Button
        //>> Status       ==> 0 - No Action Taken    1 - Running                  2 - Success     3 - Partially Failed   4 - Failed
        //>> Action       ==> 0 - Not Sent           1 - Sent                     2 - Read        3 - Clear
        /////////////////////////////////////////////////////////////////////////
        public void TriggerNotification(GlobalVariable globalVariable, string asType, out int aiSuccess, out string asMessage)
        {
            asMessage = execSql(globalVariable,
                "Select a.[Id] as triggerId,IsNull(a.[Parameter],'') as param , IsNull(a.[Message],'') as cMessage, " + Environment.NewLine
                    + "     a.[Trigger Type] as triggerType, [Action] as actionTaken, [Date Of Creation] as createdDate, a.[Status] as notStatus, " + Environment.NewLine
                    + "     a.[Button Name 1] as buttonName1,a.[Button Value 1] as buttonValue1,a.[Button Name 2] as buttonName2, " + Environment.NewLine
                    + "     a.[Button Value 2] as buttonValue2,a.[Button Name 3] as buttonName3,a.[Button Value 3] as buttonValue3 " + Environment.NewLine
                    + " FROM [LS_Trigger] a "
                    + $" Where a.[application_code] = '{globalVariable.gsAppCode}' And a.LSCode = '{globalVariable.custCode}' And " + Environment.NewLine
                    + $"     a.firm_code = '{globalVariable.firmCode}' And a.[Trigger Type] In ({asType}) And [Action] <> 3 " + Environment.NewLine,
                out aiSuccess);
        }

        /////////////////////////////////////////////////////////////////////////
        //Insert Notifications / Triggers to Particular Customer & Company
        /////////////////////////////////////////////////////////////////////////
        public void InsertTriggers(GlobalVariable globalVariable, out int aiSuccess, out string asMessage)
        {
            asMessage = execSql(globalVariable, " Insert Into [LS_Trigger]([application_code],[LSCode],[firm_code],[Id],[Parameter],[Message]," + Environment.NewLine
                    + "         [Trigger Type],[Status],[Action],[Button Name 1],[Button Value 1],[Button Name 2],[Button Value 2]," + Environment.NewLine
                    + "         [Button Name 3],[Button Value 3],[Created User],[Date Of Creation],[Modified User],[Date Of Modification]) " + Environment.NewLine
                    + " Select c.[application_code],a.LSCode,a.firm_code,c.[Id],c.[Parameter],c.[Message]," + Environment.NewLine
                    + "        c.[Trigger Type],c.[Status],c.[Action],c.[Button Name 1],c.[Button Value 1],c.[Button Name 2]," + Environment.NewLine
                    + "        c.[Button Value 2],c.[Button Name 3],c.[Button Value 3],c.[Created User],c.[Date Of Creation]," + Environment.NewLine
                    + "        c.[Modified User],c.[Date Of Modification]" + Environment.NewLine
                    + " FROM LS_Trigger c Join LS_Customer_Firm_Details a On c.LSCode = '-' and c.firm_code = '-' " + Environment.NewLine
                    + "         Left Join LS_Trigger b On a.LSCode = b.LSCode and a.firm_code = b.firm_code And c.Id = b.Id " + Environment.NewLine
                    + $" Where a.LSCode = '{globalVariable.custCode}' And b.firm_code is null ",
                out aiSuccess, 0, "Failed", 1, 2);
            //asMessage = execSql(globalVariable, " Insert Into [LS_Trigger]([application_code],[LSCode],[firm_code],[Id],[Parameter],[Message],[Trigger Type],[Status],[Action],[Button Name 1],[Button Value 1],[Button Name 2],[Button Value 2],[Button Name 3],[Button Value 3],[Created User],[Date Of Creation],[Modified User],[Date Of Modification]) Select c.[application_code],a.LSCode,a.firm_code,c.[Id],c.[Parameter],c.[Message],c.[Trigger Type],c.[Status],c.[Action],c.[Button Name 1],c.[Button Value 1],c.[Button Name 2],c.[Button Value 2],c.[Button Name 3],c.[Button Value 3],c.[Created User],c.[Date Of Creation],c.[Modified User],c.[Date Of Modification] FROM LS_Trigger c Join LS_Customer_Firm_Details a On c.LSCode = '-' and c.firm_code = '-' Left Join LS_Trigger b On a.LSCode = b.LSCode and a.firm_code = b.firm_code And c.Id = b.Id Where a.LSCode = 'LS00000118' And b.firm_code is null ", out aiSuccess, 0, "Failed", 1, 2);
        }

        /////////////////////////////////////////////////////////////////////////
        //Get List of Report Sync Schedules
        /////////////////////////////////////////////////////////////////////////
        public void GetSchedules(GlobalVariable globalVariable, out int aiSuccess, out string asMessage)
        {
            asMessage = execSql(globalVariable,
                "Select lsCode,firmCode,rptName,rptParam,syncInterval,rptTime From( " + Environment.NewLine
                    + "     Select	a.LSCode as lsCode,a.firm_code as firmCode,b.[Report Name] as rptName, " + Environment.NewLine
                    + "             Trim(Replace(b.[Sync Param],'-All','')) as rptParam, " + Environment.NewLine
                    + "             (Case When a.[Sync Interval] = 0 then 1440 else a.[Sync Interval] End) as syncInterval, " + Environment.NewLine
                    + "             (DateDiff(Minute,Cast(Cast(Getdate() as date) as varchar) + ' ' + Right('00' + Cast(DatePart(Hour,a.[Start Time]) as varchar),2) + ':00:00', " + Environment.NewLine
                    + "                 Cast(DateAdd(Minute,330,Getdate()) as varchar)) % (Case When a.[Sync Interval] = 0 then 1440 else a.[Sync Interval] End)) as rptTime " + Environment.NewLine
                    + "     FROM LS_Sync_Setup a Join LS_Report_Master b On a.report_code = b.report_code " + Environment.NewLine
                    + "             Join LS_Report_Config c On a.LSCode = c.LSCode And a.firm_code = c.firm_code And a.report_code = c.report_code " + Environment.NewLine
                    + "     Where a.[Current Status] <> 1 And a.[Active Flag] = 1 And b.Active = 1 And c.[Status] In (1,2) And c.Selected = 1 And " + Environment.NewLine
                    + $"          a.LSCode  = '{globalVariable.custCode}' And a.firm_code = '{globalVariable.firmCode}' " + Environment.NewLine
                    + " ) as a Where rptTime >= 0 And rptTime <= 15 And rptParam is not null " + Environment.NewLine,
                out aiSuccess);
        }

        /////////////////////////////////////////////////////////////////////////
        //Get List Tables to check Mismatch Count
        /////////////////////////////////////////////////////////////////////////
        public void GetMismatchTableList(GlobalVariable globalVariable, out int aiSuccess, out string asMessage)
        {
            asMessage = execSql(globalVariable,
                "Select mq.table_code as tblCode, mq.[Source] as src, mq.[Destination] as dest, " + Environment.NewLine
                    + "    Trim(Replace(Replace(Replace(Replace(Replace(tm.[Table Name], 'LS_', ''), 'Details', ''), 'Detail', ''), 'Master', ''), '_', ' ')) as tblName, " + Environment.NewLine
                    + "    IsNull(mq.[Common Column],'') as columnList, Max(IsNull(Trim(Replace(rm.[Sync Param],'-All','')),'')) as rptParam "
                    + " From LS_Mismatch_Query mq Join LS_Table_Master tm On mq.table_code = tm.table_code And tm.Active = 1 " + Environment.NewLine
                    + "             Join LS_Table_Config tc On mq.table_code = tc.table_code And mq.software_code = tc.software_code And tc.Active = 1 " + Environment.NewLine
                    + $"            Join LS_Report_Config rc On tc.report_code = rc.report_code And rc.LSCode = '{globalVariable.custCode}' And rc.firm_code = '{globalVariable.firmCode}' And rc.Status In (1,2) " + Environment.NewLine
                    + "             Join LS_Report_Master rm On rc.report_Code = rm.report_code And rm.Active = 1 " + Environment.NewLine
                    + $" Where mq.application_code = '{globalVariable.gsAppCode}' And mq.software_code = '{globalVariable.gsSoftwareCode}' And mq.Active = 1 " + Environment.NewLine
                    + " Group By mq.table_code, mq.[Source], mq.[Destination], tm.[Table Name], mq.[Common Column] " + Environment.NewLine
                    + " Order By tm.[Table Name] ",
                out aiSuccess);
        }

        /////////////////////////////////////////////////////////////////////////
        //Get List Tables to check Mismatch Count
        /////////////////////////////////////////////////////////////////////////
        public void GetSourceDataList(GlobalVariable globalVariable, int seq, out int aiSuccess, out string asMessage)
        {
            asMessage = execSql(globalVariable,
                "Select [Data1] as d1, [Data2] as d2, [Data3] as d3, [Data4] as d4, [Data5] as d5, [Item Id] as itemSeq " + Environment.NewLine
                    + " From LS_Source_Data_list sdl " + Environment.NewLine
                    + $" Where sdl.LSCode = '{globalVariable.custCode}' And sdl.firm_code = '{globalVariable.firmCode}' "
                    + $"    And sdl.sequence = {seq} And sdl.[Active Flag] = 1 "
                    + " Order By [Item Id] ",
                out aiSuccess);
        }

        /////////////////////////////////////////////////////////////////////////
        //Get List Tables to check Mismatch Count
        /////////////////////////////////////////////////////////////////////////
        public void GetAppStatusList(GlobalVariable globalVariable, out int aiSuccess, out string asMessage)
        {
            asMessage = execSql(globalVariable,
                "Select b.[Report Name] as [Report],IsNull(Cast(a.[Last Sync Time] as VarChar),'') as AppLastSyncTime, IsNull(a.[Last Sync Status],'') as AppLastSyncStatus, " + Environment.NewLine
                    + " IsNull(Cast(a.[Dataset Last Sync Time] as VarChar),'') as DataSetLastSyncTime, IsNull(a.[Dataset Last Sync Status],'') as DataSetLastSyncStatus, " + Environment.NewLine
                    + " (Case When (a.[Dataset Last Sync Status] <> 'Completed') Then (Case When CharIndex('errorDescription',a.[Dataset Last Sync Message]) > 0 Then  " + Environment.NewLine
                    + " Replace(SUBSTRING(a.[Dataset Last Sync Message],CharIndex('errorDescription',a.[Dataset Last Sync Message]) + 19,LEN(a.[Dataset Last Sync Message])),'}','') " + Environment.NewLine
                    + " Else IsNull(a.[Dataset Last Sync Message],'') End) Else '' End) as DataSetLastSyncMsg " + Environment.NewLine
                    + " From LS_Sync_Setup a Join LS_Report_Master b On a.report_code = b.report_code " + Environment.NewLine
                    + $" Where LSCode = '{globalVariable.custCode}' And firm_code = '{globalVariable.firmCode}' ",
                out aiSuccess);
        }

        /////////////////////////////////////////////////////////////////////////
        //Get List Tables to check Mismatch Count
        /////////////////////////////////////////////////////////////////////////
        public void GetCustomerApiList(GlobalVariable globalVariable, out int aiSuccess, out string asMessage)
        {
            asMessage = execSql(globalVariable,
                "Select api.[table_code] as tableCode, api.[Table Type] as tableType, api.[Req Method] as reqMethod, api.[Req URL] as reqUrl, " + Environment.NewLine
                    + "     api.[Req Body] as reqBody, api.[Req Head] as reqHead, api.[Req Token Type] as reqTokenType, api.[Req Auth] as reqAuth, " + Environment.NewLine
                    + "     api.[Param From Time] as paramFromTime, api.[Param To Time] as paramToTime, api.[Res Data Type] as resDataType, " + Environment.NewLine
                    + "     api.[Res Code] as resCode, api.[Res Head] as resHead, api.[Res Root] as resRoot, api.[Res Column List] as resColList, " + Environment.NewLine
                    + "     tbl.[Table Name] as tblName, tbl.[Del Ref Column Name] as delCol, tbl.[LastSync Time Column Name] as modTimeCol," + Environment.NewLine
                    + "     api.[dep_table_code1] as depTabCode1, api.[Dep Root1] as depRoot1, api.[Dep Column List1] as depcolList1," + Environment.NewLine
                    + "     api.[dep_table_code2] as depTabCode2, api.[Dep Root2] as depRoot2, api.[Dep Column List2] as depcolList2," + Environment.NewLine
                    + "     api.[dep_table_code3] as depTabCode3, api.[Dep Root3] as depRoot3, api.[Dep Column List3] as depcolList3," + Environment.NewLine
                    + "     api.[dep_table_code4] as depTabCode4, api.[Dep Root4] as depRoot4, api.[Dep Column List4] as depcolList4," + Environment.NewLine
                    + "     api.[dep_table_code5] as depTabCode5, api.[Dep Root5] as depRoot5, api.[Dep Column List5] as depcolList5" + Environment.NewLine
                    + " From [dbo].[LS_Customer_API_Sync] api Join LS_Table_Master tbl On api.table_code = tbl.table_code " + Environment.NewLine
                    + $" Where api.[LSCode = '{globalVariable.custCode}' And api.[firm_code = '{globalVariable.firmCode}' and api.[Active] = 1 and tbl.Active = 1 ",
                out aiSuccess);
        }

        /////////////////////////////////////////////////////////////////////////
        //Get List Tables to check Mismatch Count
        /////////////////////////////////////////////////////////////////////////
        public void GetFTPTableList(GlobalVariable globalVariable,int aiSeq, int aiSyncType, string asParam, out int aiSuccess, out string asMessage)
        {
            asMessage = execSql(globalVariable,
                " Select tblMst.[Table Name] as tblName, ftp.[File Path] as filePath, ftp.[Type] as colType, IsNull(ftp.[Column],'') as col " + Environment.NewLine
                    + " From LS_Customer_FTP_Share ftp Join LS_Table_Config tblCon On ftp.table_code = tblCon.table_code And tblCon.Active = 1 " + Environment.NewLine
                    + "     Join LS_Report_Master rptMst On tblCon.report_code = rptMst.report_code And rptMst.Active = 1 " + Environment.NewLine
                    + "     Join LS_Report_Module_Master rptMod On rptMst.module_code = rptMod.module_code And rptMod.Active = 1 " + Environment.NewLine
                    + "     Join LS_Table_Master tblMst On ftp.table_code = tblMst.table_code And tblMst.Active = 1 " + Environment.NewLine
                    + $" Where ftp.LSCode = '{globalVariable.custCode}' And ftp.firm_code = '{globalVariable.firmCode}' " + Environment.NewLine
                    + $"      And ftp.Active = 1 And ftp.sequence = {aiSeq} " + Environment.NewLine
                    + $"      And (Case when {aiSyncType} = 1 then Trim(Replace(rptMod.[Sync Param],'-All','')) when " + Environment.NewLine
                    + $"        {aiSyncType} = 2 then Trim(Replace(rptMst.[Sync Param],'-All','')) end) in ({asParam}) ",
                out aiSuccess);
        }
    }
}
