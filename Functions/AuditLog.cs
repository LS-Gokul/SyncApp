using System;

namespace LSSyncApp
{
    public class AuditLog
    {
        public static string isReturn, isLogQuery, isMstUpdateQuery;
        public static int iiSuccess;
        public static GlobalVariable gblVar = new GlobalVariable();
        public static AuditLogVar auditLogVar = new AuditLogVar();

        public string auditLogInsert(int aiType, GlobalVariable agVar, AuditLogVar aaVar)
        {
            try
            {
                setVar(agVar, aaVar);
                auditLogVar.LogDetails = auditLogVar.LogDetails.Replace("'", "");
                if (gblVar.giAudit == 0)
                {
                    return "";
                }
                gblVar._DestinationConfig.ExecuteRawQuery(gblVar,"",2, out iiSuccess, out isReturn);
                if (iiSuccess == 0)
                {
                    return "Failed | Db Not Connected";
                }

                if (aiType == 1)
                {
                    isLogQuery = "Insert Into LS_Audit_Log([LogId],[Process],[Status],[LogDetails],[System Ip],[System User],"
                        + "[System Name],[Start Time],[End Time],[Session Start Time],[Created User],[Date Of Creation])"
                        + $"Values('{auditLogVar.LogId}','{auditLogVar.Process}','{auditLogVar.Status}',"
                            + $"'{auditLogVar.LogDetails}','{gblVar.gsSystemUser}','{gblVar.gsSystemIp}','"
                            + gblVar.gsSystemName + "','" + auditLogVar.StartTime + "','" + auditLogVar.EndTime + "','"
                            + gblVar.gdSessionStartAt + "','" + gblVar.gsUserId + "',GetDate());";
                    
                    isMstUpdateQuery = $"Update LS_Sync_Setup Set [Last Sync Time] = '{auditLogVar.StartTime}',"
                        + $" [Last Sync Status] = '{auditLogVar.Status}', [Current Status] = 1 "
                        + (gblVar.giNoUpdate == 0
                            ? $" ,[Next Sync Time] = DateAdd(Minute,(Case When [Sync Interval] = 0 then 1440 else [Sync Interval] End),'{auditLogVar.StartTime}') "
                            : "" 
                          )
                        + " From LS_Report_Master a "
                        + $" Where LS_Sync_Setup.LSCode = '{gblVar.custCode}' And LS_Sync_Setup.firm_code = '{gblVar.firmCode}' And "
                        + $"    LS_Sync_Setup.report_code = a.report_code And Trim(Replace(a.[Sync Param],'-All','')) In ({auditLogVar.Param}) ";
                    

                }
                else if (aiType == 2)
                {
                    if (auditLogVar.Object == "Success") return "";
                    isLogQuery = "Insert Into LS_Audit_Log_Detail([LogId],[Object],[Child Object],[Sequence],"
                        + "[Object From Time],[Start Time],[End Time],[LogDetails],[Status],[Created User],[Date Of Creation])"
                        + "Values('" + auditLogVar.LogId + "','" + auditLogVar.Object + "','" + auditLogVar.ChildObject + "','"
                            + auditLogVar.Sequence + "','" + auditLogVar.ObjectFromTime + "','" + auditLogVar.StartTime + "','"
                            + auditLogVar.EndTime + "','" + auditLogVar.LogDetails + "','" + auditLogVar.Status + "','"
                            + gblVar.gsUserId + "',GetDate());";
                    isMstUpdateQuery = "";
                }
                else if(aiType == 3)
                {
                    isLogQuery = "Update LS_Audit_Log "
                        + " Set [Status] = '" + auditLogVar.Status
                        + "',[LogDetails] = '" + auditLogVar.LogDetails 
                        + "',[End Time] = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' "
                        + " Where [LogId] = '" + auditLogVar.LogId + "';";

                    isMstUpdateQuery = $"Update LS_Sync_Setup Set [Last Sync Time] = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}',"
                        + $" [Last Sync Status] = '{auditLogVar.Status}', [Current Status] = 3 "
                        + " From LS_Report_Master a "
                        + $" Where LS_Sync_Setup.LSCode = '{gblVar.custCode}' And LS_Sync_Setup.firm_code = '{gblVar.firmCode}' And "
                        + $"    LS_Sync_Setup.report_code = a.report_code And Trim(Replace(a.[Sync Param],'-All','')) In ({auditLogVar.Param}) "; ;
                }
                else
                {
                    return "";
                }

                gblVar._DestinationConfig.ExecuteRawQuery(gblVar, isLogQuery, 1, out iiSuccess, out isReturn);
                if(iiSuccess == 0)
                {
                    return (iiSuccess == 0 ? "Failed - " : "") + isReturn;
                }

                //Update Report Status
                if(isMstUpdateQuery != "" && gblVar.giCmd == 1)
                {
                    gblVar._MasterConfig.ExecuteRawQuery(gblVar, isMstUpdateQuery, 1, out iiSuccess, out isReturn, 0, 2);
                }
                return (iiSuccess == 0 ? "Failed - " : "") + isReturn;
            }
            catch(Exception Ex)
            {
                return "Failed - " + Ex.Message + Environment.NewLine + isLogQuery;
            }
        }

        public static void setVar(GlobalVariable lgVar,AuditLogVar laVar)
        {
            gblVar = lgVar;
            auditLogVar = laVar;
        }
    }
}
