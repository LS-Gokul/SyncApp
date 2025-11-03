using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading;
using LSSyncApp.Controllers;
using LSSyncApp.Functions;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace LSSyncApp
{
    public class ODBCSyncParam
    {
        public GlobalVariable odbcGlobalVar = new GlobalVariable();
        public AuditLogVar _auditLogVar = new AuditLogVar();
        public SummaryTables lsComSumm = new SummaryTables();
        public RestAPI _RestApi = new RestAPI();

        public string ip, port, path, isODBCDBName, isSeq;
        public string isFinYear, isLogFileName, isLogTime;
        public int iiCY, iiSuccess, iiAuthType, iiRetryCount = 5;
        public string isCustDbName, isCustUID, isCustPwd, isCustServer, isFirmCode, isCustCode;
        public string isMasterDBName, isMasterUID, isMasterPwd, isMasterServer;
        public string isODBCUID, isODBCPwd, isODBCServer, isLogString, isDbTypeName, isMail = "";
        public static string isReturn, isSqlQuery, isRetString = "", isSoftwareName, isSyncType;

        public string dbConnect(string dbType, int aiAuthMethod = 0)
        {
            try
            {
                switch (dbType)
                {
                    case "cust":      //SqlServer
                        return odbcGlobalVar.custDBConn.destConnSetup(isCustServer, isCustDbName, isCustUID, isCustPwd, odbcGlobalVar.giCustDBAuthMethod);
                    case "source":    //SqlServer
                        return odbcGlobalVar.odbcConn.destConnSetup(isODBCServer, isODBCDBName, isODBCUID, isODBCPwd, aiAuthMethod);
                    case "odbc":      //ODBC
                        return odbcGlobalVar.odbcConn.srcDBConn(isODBCServer, isODBCUID, isODBCPwd);
                    case "mysql":     //ODBC
                        return odbcGlobalVar.odbcConn.mySqlDBConn(ip, port, isODBCDBName, isODBCUID, isODBCPwd);
                    case "sapHana":   //SAP Hana
                        string custDb, custCurSch;
                        custDb = (isODBCDBName.IndexOf(";") > 0 ? isODBCDBName.Substring(0, isODBCDBName.IndexOf(";")) : isODBCDBName);
                        custCurSch = (isODBCDBName.IndexOf(";") > 0 ? isODBCDBName.Substring(isODBCDBName.IndexOf(";") + 1) : "");
                        return odbcGlobalVar.odbcConn.sapHanaDBConn(isODBCServer, ip, port, isODBCUID, isODBCPwd, custDb, custCurSch);
                    default:
                        return "Failed - DB Not Found";
                }
            }
            catch (Exception e)
            {
                return "Failed " + e.Message;
            }
        }

        public string setVariables()
        {
            isCustServer = odbcGlobalVar.custServerName;
            isCustUID = odbcGlobalVar.custUID;
            isCustPwd = odbcGlobalVar.custPwd;
            isCustDbName = odbcGlobalVar.custDbName;

            isMasterDBName = odbcGlobalVar.masterdbName;
            isMasterUID = odbcGlobalVar.masterUID;
            isMasterPwd = odbcGlobalVar.masterPwd;
            isMasterServer = odbcGlobalVar.masterServerName;

            isCustCode = odbcGlobalVar.custCode;
            isFirmCode = odbcGlobalVar.firmCode;

            return "";
        }

        public void getColList(string tableName, string custcode, string firmcode, out string pkeyColList, out string fullColList, out string jsonColList,
                                        out string insColList, out string updColList, out string selJsonColList, out string ret, out int typ, out string pkeyCols,
                                        int colType = 2, int isTally = 0, string formColList = null)
        
        {
            try
            {
                string lsColName, lsColType, lsColPkey;//, lsColConst;
                JsonElement ljecolList;
                int liColCnt, liUnWantedCol = 0;

                pkeyColList = "";
                pkeyCols = "";
                fullColList = "";
                jsonColList = "";
                insColList = "";
                updColList = "";
                selJsonColList = "";
                typ = 0;
                ret = "";
                
                odbcGlobalVar._DestinationConfig.FetchColumnList(odbcGlobalVar, tableName, out iiSuccess, out isReturn);
                if (iiSuccess == 0)
                {
                    ret = "Failed - " + isReturn;
                }
                else
                {
                    ljecolList = odbcGlobalVar.createJsonElement(isReturn);
                    liColCnt = ljecolList.EnumerateArray().Count();
                    for (int col = 0; col < liColCnt; col++)
                    {
                        lsColName = ljecolList[col].GetProperty("colName").ToString();
                        lsColType = ljecolList[col].GetProperty("colType").ToString();
                        //lsColConst = ljecolList[col].GetProperty("colConst").ToString();
                        lsColPkey = ljecolList[col].GetProperty("colPKey").ToString();

                        if (lsColPkey == "Y")
                        {
                            pkeyCols += (typ == 0 ? "" : ",") + "[" + lsColName + "]";
                            if (isTally == 1)
                            {
                                if (lsColName != "LSCode" && lsColName != "LSFirmCode")
                                {
                                    pkeyColList += "[" + lsColName + "],";
                                }

                            }
                            else
                            {
                                pkeyColList += "new_a.[" + lsColName + "] = a.[" + lsColName + "] AND ";
                            }

                            typ = 1;
                        }
                        else
                        {
                            updColList += "a.[" + lsColName + "] = new_a.[" + lsColName + "],";
                        }
                        insColList += "new_a.[" + lsColName + "],";

                        if (colType == 1)//XML
                        {
                            if (lsColType == "datetime" || lsColType == "date" || lsColType.Contains("numeric") || lsColType.Contains("int"))
                            {
                                jsonColList += "[" + lsColName + "] VARCHAR(30)";
                            }
                            else
                            {
                                jsonColList += "[" + lsColName + "] " + lsColType;
                            }
                            if (formColList != null)
                            {
                                if (col == liColCnt - 1)
                                {
                                    jsonColList += " '" + formColList + "',";
                                }
                                else
                                {
                                    jsonColList += " '" + formColList.Substring(0, formColList.IndexOf(",")) + "',";
                                    formColList = formColList.Substring(formColList.IndexOf(",") + 1);
                                }
                            }
                        }
                        else if (colType == 2)//Json
                        {
                            if (lsColType == "datetime" || lsColType == "date")
                            {
                                jsonColList += "[" + lsColName + "] VARCHAR(30) '$.\"" + lsColName + "\"',";
                            }
                            else
                            {
                                jsonColList += "[" + lsColName + "] " + lsColType + " '$.\"" + lsColName + "\"',";
                            }
                        }
                        else if (colType == 3)//Column list comes with name as COL 
                        {
                            if (lsColName == "LSCode" || lsColName == "LSFirmCode")
                            {
                                liUnWantedCol += 1;
                            }
                            else
                            {
                                if (!lsColType.Contains("varchar"))
                                {
                                    jsonColList += "Tbl.Col.value('COL[" + ((col + 1) - liUnWantedCol).ToString() + "]', 'VARCHAR(50)') AS [" + lsColName + "],";
                                }
                                else
                                {
                                    jsonColList += "Tbl.Col.value('COL[" + ((col + 1) - liUnWantedCol).ToString() + "]', '" + lsColType + "') AS [" + lsColName + "],";
                                }
                            }
                        }

                        fullColList += "[" + lsColName + "],";

                        if (lsColType == "datetime" || lsColType == "date")
                        {
                            if (isTally == 1)
                            {
                                selJsonColList += "(CASE WHEN Len([" + lsColName + "]) > 7 and CHARINDEX('-',[" + lsColName + "]) <= 0 Then "
                                        + " CONVERT(DATETIME,LEFT([" + lsColName + "],4) + '-' + SUBSTRING([" + lsColName + "],5,2) + '-' + SUBSTRING([" + lsColName + "],7,2)) "
                                        + " WHEN CHARINDEX('-',[" + lsColName + "]) > 0 Then cast([" + lsColName + "] as " + lsColType + ")"
                                        + " Else GetDate() END) AS [" + lsColName + "],";
                            }
                            else
                            {
                                selJsonColList += "CONVERT(" + lsColType + ",LEFT([" + lsColName + "],19)) AS [" + lsColName + "],";
                            }

                        }
                        else
                        {
                            if ((lsColType.Contains("numeric") || lsColType.Contains("int")) && isTally == 1 && lsColName != "LSFirmCode")
                            {
                                if (lsColName == "SequenceT")
                                {
                                    selJsonColList += "[" + lsColName + "],";
                                }
                                else
                                {
                                    selJsonColList += "(case when [" + lsColName + "] = '' or [" + lsColName + "] is null then 0 else Cast(["
                                        + lsColName + "] as " + lsColType + ") end) as [" + lsColName + "],";
                                }
                            }
                            else
                            {
                                selJsonColList += "[" + lsColName + "],";
                            }
                        }
                    }

                    if (liColCnt > 0)
                    {
                        insColList = insColList.Substring(0, insColList.Length - 1);
                        jsonColList = jsonColList.Substring(0, jsonColList.Length - 1);
                        fullColList = fullColList.Substring(0, fullColList.Length - 1);
                        selJsonColList = selJsonColList.Substring(0, selJsonColList.Length - 1).
                                Replace("[LSCode]", "'" + custcode + "' as [LSCode]").Replace("[LSFirmCode]", firmcode + " as [LSFirmCode]");
                        if (pkeyColList.Length > 0)
                        {
                            if (isTally == 1)
                            {
                                pkeyColList = pkeyColList.Substring(0, pkeyColList.Length - 1);
                            }
                            else
                            {
                                pkeyColList = pkeyColList.Substring(0, pkeyColList.Length - 4);
                            }
                        }
                        if (updColList.Length > 0)
                        {
                            updColList = " WHEN MATCHED THEN UPDATE SET " + updColList.Substring(0, updColList.Length - 1);
                        }
                        else
                        {
                            updColList = "";
                        }

                        ret = "Success";
                    }
                }
            }
            catch (Exception Ex)
            {
                pkeyColList = "";
                pkeyCols = "";
                fullColList = "";
                jsonColList = "";
                insColList = "";
                updColList = "";
                selJsonColList = "";
                typ = 0;
                ret = "Failed - " + Ex.Message;
            }
        }

        public string setStatusLog(string asType, string asMessage, int aiType)
        {
            if (aiType == 1)
            {
                if (asMessage != "")
                {
                    switch (asType)
                    {
                        case "tblName"://Table Name
                            _auditLogVar.Object = asMessage.Replace("LS_", "").Replace("_", " ");
                            break;
                        case "s"://No of Rows in Source Table
                            isLogString += "$No of Rows in Source Table:" + asMessage;
                            break;
                        case "t"://max Time in destination table
                            _auditLogVar.ObjectFromTime = asMessage;
                            break;
                        case "status":
                            if (asMessage.Contains("No Rows Found"))
                            {
                                asMessage = "No Rows Found";
                            }
                            isLogString += "$" + asMessage;
                            break;
                        case "sT":
                            _auditLogVar.StartTime = asMessage;
                            return "";
                        case "eT":
                            _auditLogVar.EndTime = asMessage;
                            return "";
                        case "br":
                        case "comp":
                            _auditLogVar.ChildObject = asMessage;
                            break;
                        case "stat":
                            _auditLogVar.Status = asMessage;
                            return "";
                        case "fy":
                            isLogString += "$" + asMessage;
                            break;
                        default:
                            break;
                    }
                }
                setStatusValue(asType, asMessage);
            }
            else if (aiType == 2)
            {
                if (isLogString == null) isLogString = "";

                _auditLogVar.LogDetails = isLogString.Replace("'", "\"");
                if (isLogString.Contains("Failed"))
                {
                    isRetString += "$" + isLogString;
                    isMail += isLogString + Environment.NewLine;
                }
                isLogString = "";
                odbcGlobalVar.gAuditLog.auditLogInsert(aiType, odbcGlobalVar, _auditLogVar);
            }

            return "";
        }

        public void auditLogReset(int aiType)
        {
            if (aiType == 2 || aiType == 3 || aiType == 4)
            {
                if (_auditLogVar.ChildObject == "" || _auditLogVar.ChildObject == null)
                {
                    _auditLogVar.ChildObject = "-";
                }
                if (_auditLogVar.Sequence <= 0)
                {
                    _auditLogVar.Sequence = 1;
                }
                if (_auditLogVar.ObjectFromTime == "" || _auditLogVar.ObjectFromTime == null
                            || DateTime.TryParse(_auditLogVar.ObjectFromTime, out _))
                {
                    _auditLogVar.ObjectFromTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }
                if (_auditLogVar.EndTime == "" || _auditLogVar.EndTime == null || DateTime.TryParse(_auditLogVar.EndTime, out _))
                {
                    _auditLogVar.EndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }
                _auditLogVar.Status = "Failed";
                setStatusLog("", "", 2);
            }
            _auditLogVar.Status = "";
            _auditLogVar.LogDetails = "";
            if (aiType == 2 || aiType == 1)
            {
                _auditLogVar.Object = "";
                _auditLogVar.ChildObject = "";
            }
            _auditLogVar.Sequence = 1;
            _auditLogVar.ObjectFromTime = "";
            _auditLogVar.StartTime = "";
            _auditLogVar.EndTime = "";
        }

        /////////////////////////////////////////
        //Log Maintain
        /////////////////////////////////////////
        public void sumSetStatusValue(string typeValue, string textValue)
        {
            setStatusValue(typeValue, textValue);
        }
        public void setStatusValue(string typeValue, string textValue)
        {
            string lsText = "";
            if (textValue.Trim() != "" && textValue != null)
            {
                switch (typeValue)
                {
                    case "comp":
                        lsText = $"~~~~~~~~~~~~~~~ Company - {textValue} ~~~~~~~~~~~~~~~";
                        break;
                    case "tblName":
                        
                        lsText = $"******************{textValue.Replace("LS_", "").Replace("_", " ")}({DateTime.Now.ToString("HH:mm:ss")})******************";
                        break;
                    case "s":
                        lsText = "==>Number of Rows in Source Table : " + textValue;
                        break;
                    case "t":
                        lsText = "==>Max Time in Dest Table : " + textValue;
                        break;
                    case "status":
                        lsText = "==>" + textValue;
                        break;
                    case "startTime":
                        lsText = $"--------------------------> Start Time : {textValue} <----------------------------------";
                        break;
                    case "endTime":
                        lsText = $"--------------------------> End Time : {textValue} <----------------------------------";
                        break;
                    case "tT":
                        lsText = $"Time Taken : {textValue}";
                        break;
                    case "br":
                        lsText = $"<--- Branch : {textValue} --->";
                        break;
                    case "fy":
                        lsText = $"############## Financial Year : {textValue} ##############";
                        break;
                    default:
                        break;
                }
                odbcGlobalVar.logFile(isLogFileName, lsText, 1);
            }
        }

        ///////////////////////////////////////////////////
        //Insert Audit Log
        ///////////////////////////////////////////////////
        public void insAuditLog(int aiType, AuditLogVar aaVar)
        {
            switch (aiType)
            {
                case 1:
                    isLogTime = DateTime.Now.ToString("yyyyMMddHHmmss") + odbcGlobalVar.gsCmdParam.Replace("'-", "_").Replace("'", "");
                    _auditLogVar.LogId = isLogTime;//Set Audit Log Var
                    _auditLogVar.Process = "Sync Data";//Set Audit Log Var
                    _auditLogVar.StartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//Set Audit Log Var
                    _auditLogVar.EndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//Set Audit Log Var
                    _auditLogVar.LogDetails = "-";//Set Audit Log Var
                    _auditLogVar.Status = "Started";
                    isReturn = odbcGlobalVar.gAuditLog.auditLogInsert(aiType, odbcGlobalVar, _auditLogVar);
                    if(isReturn.Contains("Failed"))
                    {
                        setStatusValue("status",isReturn);
                    }
                    break;
                case 3:
                    aaVar.Status = "Success";
                    if (aaVar.LogDetails.Contains("Failed"))
                    {
                        aaVar.Status = "Failed";//Set Audit Log Var
                        if (aaVar.LogDetails.Contains("Success")) aaVar.Status = "Partially Failed";//Set Audit Log Var
                        sendMail(isMail);
                        isMail = "";
                    }
                    odbcGlobalVar.gAuditLog.auditLogInsert(aiType, odbcGlobalVar, aaVar);
                    break;
            }
        }

        public string sourceConfiguration(string asParam, IProgress<int> progress, GlobalVariable globalVar, int aiSyncType,
            string asSource = "", string asTable = "", string asDateList = "", string asBrList = "")
        {
            odbcGlobalVar = globalVar;
            
            isLogFileName = odbcGlobalVar.firmCode + "_" + odbcGlobalVar.gsCmdParam.Replace("'-", "").Replace("'", "_") + odbcGlobalVar.gsLogFileName;
            /************************************************************************************/
            //Source Server IP Configuratiuon
            string lsRet = "";

            JsonElement ljeSourceConfig;
            DateTime ldStart, ldStop;

            int liSourceCnt;
            ldStart = DateTime.Now;
            setVariables();
            _auditLogVar.Param = asParam;
            insAuditLog(1, _auditLogVar);
            try
            {
                setStatusValue("startTime", "");
                setStatusValue("endTime", "");
                /************************************************************************************/
                //Source Configuration
                odbcGlobalVar._MasterConfig.GetSourceList(odbcGlobalVar, out int liStatus, out lsRet, 1, 0, asSource);

                if (liStatus == 1 && lsRet != null && lsRet != "")
                {
                    ljeSourceConfig = odbcGlobalVar.createJsonElement(lsRet);
                    lsRet = "";
                    liSourceCnt = ljeSourceConfig.EnumerateArray().Count();

                    string lsFinYear, lsMinTimeFY;
                    setStatusValue("startTime", ldStart.ToString("HH:mm:ss"));

                    //Update Current Status

                    for (int i = 0; i < liSourceCnt; i++)
                    {
                        ip = ljeSourceConfig[i].GetProperty("host").ToString();
                        port = ljeSourceConfig[i].GetProperty("port").ToString();
                        path = ljeSourceConfig[i].GetProperty("path").ToString();
                        iiCY = int.Parse(ljeSourceConfig[i].GetProperty("curYear").ToString());
                        isODBCServer = ljeSourceConfig[i].GetProperty("serverName").ToString();
                        isODBCDBName = ljeSourceConfig[i].GetProperty("dbName").ToString();
                        isODBCUID = ljeSourceConfig[i].GetProperty("uID").ToString();//Fetching From LS_Source_Config
                        isODBCPwd = ljeSourceConfig[i].GetProperty("pwd").ToString();//Fetching From LS_Source_Config
                        isSyncType = ljeSourceConfig[i].GetProperty("syncType").ToString();
                        isSoftwareName = ljeSourceConfig[i].GetProperty("stName").ToString();
                        lsFinYear = ljeSourceConfig[i].GetProperty("finYear").ToString();
                        lsMinTimeFY = ljeSourceConfig[i].GetProperty("minTime").ToString();
                        isSeq = ljeSourceConfig[i].GetProperty("seq").ToString();
                        isDbTypeName = ljeSourceConfig[i].GetProperty("dbTypeName").ToString();
                        iiAuthType = int.Parse(ljeSourceConfig[i].GetProperty("authType").ToString());

                        if (lsFinYear != null && lsFinYear != "" && lsFinYear.Contains("FY"))
                        {
                            lsFinYear = lsFinYear.Replace("FY", "").Trim();
                            if (lsMinTimeFY == "" || lsMinTimeFY == null)
                            {
                                odbcGlobalVar.defTime = "20" + lsFinYear.Substring(0, 2) + "-04-01 00:00:00";
                            }
                            else
                            {
                                odbcGlobalVar.defTime = lsMinTimeFY;
                            }
                            odbcGlobalVar.gsFinYear = lsFinYear.Substring(0, 2);
                            isFinYear = odbcGlobalVar.gsFinYear;
                        }
                        else
                        {
                            continue;
                        }

                        setStatusValue("fy", lsFinYear);
                        
                        switch (isSoftwareName)
                        {
                            case "FTP":
                            case "SFTP":
                                FileTransfer _FileTransfer = new FileTransfer();
                                _FileTransfer.init(this, progress, asParam, aiSyncType, isSoftwareName);
                                _FileTransfer.Dispose();
                                break;
                            case "Disha":
                                Disha _Disha = new Disha();
                                _auditLogVar = _Disha.init(this, progress, asParam, aiSyncType);
                                _Disha.Dispose();
                                break;
                            case "SAP":
                                SAPHana _SAPHana = new SAPHana();
                                _auditLogVar = _SAPHana.init(this, progress, asParam, aiSyncType);
                                _SAPHana.Dispose();
                                break;
                            case "6 Orbit ERP":
                                SixOrbit _SixOrbit = new SixOrbit();
                                _auditLogVar = _SixOrbit.init(this, progress, asParam, aiSyncType);
                                _SixOrbit.Dispose();
                                break;
                            case "LOGIC ERP":
                                LogicERP _LogicERP = new LogicERP();
                                _auditLogVar = _LogicERP.init(this, progress, asParam, aiSyncType);
                                _LogicERP.Dispose();
                                break;
                            case "EasySol":
                                EasySol _EasySol = new EasySol();
                                _auditLogVar = _EasySol.init(this, progress, asParam, aiSyncType);
                                _EasySol.Dispose();
                                break;
                            case "Busy":
                                Busy _Busy = new Busy();
                                _Busy.init(this, progress, asParam, aiSyncType);
                                _Busy.Dispose();
                                break;
                            case "Bizom":
                                Bizom _Bizom = new Bizom();
                                _auditLogVar = _Bizom.init(this, progress, asParam, aiSyncType);
                                _Bizom.Dispose();
                                break;
                            case "Tally":
                                TallyODBC _TallyODBC = new TallyODBC();
                                _auditLogVar = _TallyODBC.init(this, progress, asParam, aiSyncType);
                                _TallyODBC.Dispose();
                                break;
                            default:
                                switch (isSyncType)
                                {
                                    case "API":
                                        APIQuery _APICall = new APIQuery();
                                        _APICall.init(this, progress, asParam, aiSyncType);
                                        _APICall.Dispose();
                                        break;
                                    case "ODBC":
                                        CSQ cSQ = new CSQ();
                                        _auditLogVar = cSQ.init(this, progress, asParam, aiSyncType, asTable, asDateList, asBrList);
                                        cSQ.Dispose();
                                        break;
                                    default:
                                        continue;
                                }
                                break;
                        }
                    }
                    lsRet += " " + summarizeCommonData(progress, asParam, aiSyncType);
                }
                else
                {
                    lsRet = "Failed | Source not Configured";
                    setStatusValue("status", lsRet);
                    sendMail(lsRet);

                }
                ldStop = DateTime.Now;
                setStatusValue("endTime", ldStop.ToString("HH:mm:ss"));
                setStatusValue("tT", ldStop.Subtract(ldStart).ToString());
                
            }
            catch (Exception Ex)
            {
                lsRet += " Failed - " + Ex.Message;
            }
            _auditLogVar.LogDetails = lsRet;
            insAuditLog(3, _auditLogVar);
            return lsRet;
        }

        //////////////////////////////////////////////////////////////////////////////
        //Format and Send Mail
        //////////////////////////////////////////////////////////////////////////////
        public void sendMail(string asMessage)
        {
            try
            {
                string lsReplace = $"<p style=\"font-size: 16px;\"><span style=\" color:#272727; font-size: 14px;\"><strong>{asMessage}</strong></span></p><br>";
                string lsMessage, lsTo = "";
                odbcGlobalVar._MasterConfig.GetSpoc(odbcGlobalVar, out iiSuccess, out lsMessage);
                if (iiSuccess == 1)
                {
                    JsonElement ljeSpoc = odbcGlobalVar.createJsonElement(lsMessage);
                    int liUpdateCount = ljeSpoc.EnumerateArray().Count();
                    for (int i = 0; i < liUpdateCount; i++)
                    {
                        lsTo += ljeSpoc[i].GetProperty("email").ToString() + ";";
                    }
                    odbcGlobalVar._fun.Templates(1, odbcGlobalVar.gsCustAppCode, out _, out string lsTemplate);
                    lsTemplate = lsTemplate.Replace("@ServerName", lsReplace).
                        Replace("@Software", odbcGlobalVar.gsSoftwareName).Replace("@CompanyName", odbcGlobalVar.firmName);
                    Notifications _N = new Notifications();

                    lsMessage = _N.SendMail(lsTo, odbcGlobalVar.gsSoftwareName + " Server Status", lsTemplate).ToString();
                    odbcGlobalVar.logFile("CheckStatus", "SPOC Mail - " + lsMessage + " - End", 1);
                }
            }
            catch (Exception Ex)
            {
                setStatusValue("status", Ex.Message);
            }
        }

        //////////////////////////////////////////////////////////////////////////////
        //Summarizing the Full Data
        //////////////////////////////////////////////////////////////////////////////
        private string summarizeCommonData(IProgress<int> progress, string asParam, int aiSyncType)
        {
            try
            {
                DateTime ldStart = DateTime.Now, ldStop;
                setStatusValue("status", "Statrting Full Summarization");
                setStatusValue("startTime", "");
                setStatusValue("endTime", "");
                setStatusValue("startTime", ldStart.ToString("HH:mm:ss"));
                
                //Summirize Data
                string lsRet = lsComSumm.summData(progress, isLogTime, this, asParam, aiSyncType);

                //Refresh Dataset
                setStatusValue("status", "Statrting Dataset Refresh"); 
                RefreshData(asParam, aiSyncType);
                setStatusValue("status", "Dataset Refresh Done");

                ldStop = DateTime.Now;
                setStatusValue("endTime", ldStop.ToString("HH:mm:ss"));
                setStatusValue("tT", ldStop.Subtract(ldStart).ToString());

                //Statistics
                odbcGlobalVar._fun.UpdateDBStats(odbcGlobalVar, out iiSuccess, out isReturn);
                setStatusValue("status", "Stats - " + isReturn);

                return lsRet;
            }
            catch (Exception Ex)
            {
                return "Failed | " + Ex.Message;
            }
        }

        //////////////////////////////////////////////////////////////////////////////
        //Dataset Refresh After Summary
        //////////////////////////////////////////////////////////////////////////////
        private void RefreshData(string asParam, int aiSyncType)
        {
            try
            {
                CloudConfig.AadService _AadService = new CloudConfig.AadService();
                string lsToken = _AadService.GetAccessToken(odbcGlobalVar), lsDataSetId = "", lsUrl, lsrequestbody, lsReportName, lsReportsList;
                if(lsToken == "")
                {
                    setStatusValue("status", "Cannot Refresh Dashboard");
                    return;
                }
                //Get Reports from Workspase Id by using URL for DataSet ID
                lsUrl = odbcGlobalVar.gsEmbedApiDomain + $"groups/{odbcGlobalVar.gsWorkspaseId}/reports";
                lsReportsList = _RestApi.getAPICalling(lsUrl, out _, "application/json", "Bearer " + lsToken,
                    $"X-PowerBI-Profile-Id : {odbcGlobalVar.gsProfileId}");
                JsonElement ljeDataSetList = odbcGlobalVar.createJsonElement(lsReportsList);

                odbcGlobalVar._MasterConfig.GetReportName(odbcGlobalVar, asParam, aiSyncType, out iiSuccess, out isReturn);
                setStatusValue("status", "Dataset Get Report - " + iiSuccess.ToString());
                if (iiSuccess == 1)
                {
                    if (isReturn.Length > 0)
                    {
                        JsonElement ljeReportList = odbcGlobalVar.createJsonElement(isReturn);
                        for (int i = 0; i < ljeReportList.EnumerateArray().Count(); i++)
                        {
                            lsReportName = ljeReportList[i].GetProperty("reportName").ToString();
                            for (int j = 0; j < ljeDataSetList[0].GetProperty("value").EnumerateArray().Count(); j++)
                            {
                                if (ljeDataSetList[0].GetProperty("value")[j].GetProperty("name").ToString() == lsReportName)
                                {
                                    setStatusValue("status", "Checking Status");
                                    if (CheckStatus() == 1)
                                    {
                                        Thread.Sleep(15000);
                                        setStatusValue("status", "Session Insert");
                                        if (odbcGlobalVar.giCheckEmbed == 1) 
                                        { 
                                            SessionInsert(lsReportName); 
                                        }
                                        lsDataSetId = ljeDataSetList[0].GetProperty("value")[j].GetProperty("datasetId").ToString();
                                        lsUrl = odbcGlobalVar.gsEmbedApiDomain + $"groups/{odbcGlobalVar.gsWorkspaseId}/datasets/{lsDataSetId}/refreshes";
                                        lsrequestbody = "{\"type\": \"Full\",\"commitMode\": \"transactional\",\"maxParallelism\": 2}";

                                        lsReportsList = _RestApi.postAPICalling(lsUrl, "application/json", lsrequestbody, out string lsStatusCode, "Bearer " + lsToken,
                                            $"X-PowerBI-Profile-Id : {odbcGlobalVar.gsProfileId}");
                                        setStatusValue("status", "Refresh Status - " + lsStatusCode + " - " + lsReportName);
                                    }
                                    else
                                    {
                                        setStatusValue("status", "Refresh Status - Failed");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                setStatusValue("status", "On RefreshData()" + Ex.Message);
            }
            return;
        }

        //////////////////////////////////////////////////////////////////////////////
        //Check Embed status before Dataset Refresh & Turn On
        //////////////////////////////////////////////////////////////////////////////
        private int CheckStatus()
        {
            int liStatus = 0;
            string lsUrl, lsBody, lsReturn, lsToken, lsStatus;
            if (odbcGlobalVar.giCheckEmbed == 0) return 1;
            try
            {
                lsUrl = $"https://login.microsoftonline.com/{odbcGlobalVar.gsEmbedTenantId}/oauth2/v2.0/token";
                lsBody = $"&client_id={odbcGlobalVar.gsEmbedClientId}&scope=https://management.azure.com/.default"
                    + $"&client_secret={odbcGlobalVar.gsEmbedClientSecret}&grant_type=client_credentials";
                lsReturn = _RestApi.postAPICalling(lsUrl, "application/x-www-form-urlencoded", lsBody, out _);
                setStatusValue("status", "Check Status");
                if (lsReturn.Contains("access_token"))
                {
                    JsonElement ljeAccessToken = new JsonElement();
                    ljeAccessToken = odbcGlobalVar.createJsonElement(lsReturn);
                    lsToken = "Bearer " + ljeAccessToken[0].GetProperty("access_token").ToString();

                    lsUrl = $"https://management.azure.com/subscriptions/{odbcGlobalVar.gsEmbedResourceSubscriptionId}/resourceGroups/"
                        + $"{odbcGlobalVar.gsEmbedResourceGroup}/providers/Microsoft.PowerBIDedicated/capacities/"
                        + $"{odbcGlobalVar.gsEmbedResource}?api-version=2021-01-01";
                    lsReturn = _RestApi.getAPICalling(lsUrl, out _, "", lsToken);
                    
                    if (lsReturn.Contains("state"))
                    {
                        setStatusValue("status", "Check Status 2 - Success");
                        ljeAccessToken = odbcGlobalVar.createJsonElement(lsReturn);
                        lsStatus = ljeAccessToken[0].GetProperty("properties").GetProperty("state").ToString();
                        setStatusValue("status", "Check Status 3 - " + lsStatus);
                        if (lsStatus != "Succeeded" && lsStatus != "Resuming")
                        {
                            lsUrl = $"https://management.azure.com/subscriptions/{odbcGlobalVar.gsEmbedResourceSubscriptionId}/resourceGroups" +
                                $"/{odbcGlobalVar.gsEmbedResourceGroup}/providers/Microsoft.PowerBIDedicated/capacities/" +
                                $"{odbcGlobalVar.gsEmbedResource}/resume?api-version=2021-01-01";
                            lsReturn = _RestApi.postAPICalling(lsUrl, "", "", out string lsStatusCode, lsToken);
                            setStatusValue("status", "Check Status 4 - " + lsStatusCode);
                            if (lsStatusCode.ToLower() == "accepted")
                            {
                                setStatusValue("status", "Check Status 5 - " + lsStatusCode);
                                liStatus = 1;
                            }
                        }
                        else
                        {
                            setStatusValue("status", "Check Status 4 - " + lsStatus);
                            liStatus = 1;
                        }
                    }
                }
            }
            catch(Exception Ex)
            {
                setStatusValue("status", "On Embbedded CheckStatus()" + Ex.Message);
            }
            setStatusValue("status", "Check Status End - " + liStatus.ToString());
            return liStatus;
        }

        //////////////////////////////////////////////////////////////////////////////
        //Insert Session data in LS_App_Log Table for not to turn off embed
        //////////////////////////////////////////////////////////////////////////////
        private void SessionInsert(string asReportName)
        {
            DateTime ldtTime = DateTime.Now;
            try
            {
                string lsQuery = "Insert Into LS_App_Log([LSCode],[LogId],[User Id],[Page],[Event Type],[Event Name],[Sub Event Name],"
                    + "[Description],[Event Date],[Load Duration],[Render Duration],[System User],[System IP],[Browser],"
                    + "[Device Type],[Geo Location],[application_code],[Device ID])"
                    + $"Values('{odbcGlobalVar.custCode}','{ldtTime.ToString("yyyyMMddhhmmssfff")}','{odbcGlobalVar.gsUserCode}',"
                    + $"'Reports','Dataset Refresh','{asReportName}',null,null,'{ldtTime.ToString("yyyy-MM-dd hh:mm:ss")}',"
                    + $"null,null,Left('{odbcGlobalVar.gsSystemUser}',100),Left('{odbcGlobalVar.gsSystemIp}',100),"
                    + $"null,'Desktop',null,'{odbcGlobalVar.gsCustAppCode}',null);";
                //setStatusValue("status", lsQuery);
                odbcGlobalVar._MasterConfig.ExecuteRawQuery(odbcGlobalVar, lsQuery,1, out int _, out string lsRet,0,2);
                setStatusValue("status", "On SessionInsert - " + lsRet);
            }
            catch (Exception Ex)
            {
                setStatusValue("status", "On SessionInsert - " + Ex.Message);
            }
        }



        public void UpdateMaxTimeBr(string asbrList, string asTable, string asFinYear, string asFromDate,
            out int aiSuccess, out string asMessage, string asBr = "")
        {
            try
            {
                string lsBrList = "", lsQuery = $"Update LS_MaxTime Set [Max Time] = '{asFromDate}' Where [Table Name] = '{asTable}' And [Fin Year] = '{asFinYear}' ";
                
                if(asBr != "" && asBr != null)
                {
                    lsQuery += $" And br_code = '{asBr}';";
                }
                else
                {
                    int liBrCount;
                    JsonElement ljeBr = odbcGlobalVar.createJsonElement(asbrList);
                    liBrCount = ljeBr.EnumerateArray().Count();

                    for (int i = 0; i < liBrCount; i++)
                    {
                        lsBrList = (i == 0 ? "" : ",") + "'" + ljeBr[i].GetProperty("brCode").ToString() + "'";
                    }
                    lsQuery += (liBrCount > 0 ? $" And br_code In ({lsBrList});" : ";");
                }
                odbcGlobalVar._DestinationConfig.ExecuteRawQuery(odbcGlobalVar, lsQuery, 1, out iiSuccess, out isReturn);
                aiSuccess = iiSuccess;
                asMessage = isReturn;
            }
            catch (Exception Ex)
            {
                aiSuccess = 0;
                asMessage = Ex.Message;
            }
        }


        public string UpdateMaxTime(string asTime, string asTableName, string asFinYear, string asTranType, string asBrCode)
        {
            try
            {
                string lsSqlQuery = "MERGE INTO LS_MaxTime WITH(HOLDLOCK) AS a USING("
                    + " Select '" + asTableName + "' as tableName, '" + asTime + "' as maxTime, '"
                    + asFinYear + "' as finYear, '" + asTranType + "' as tranType, '" + (asBrCode == "" || asBrCode == null ? "-" : asBrCode) + "' as brCode ) as b "
                    + " On a.[Fin Year] = b.finYear And a.[Table Name] = b.tableName And a.[br_code] = b.brCode And a.[Tran_Type] = b.tranType "
                    + " WHEN MATCHED THEN UPDATE SET a.[Max Time] = b.maxTime"
                    + " WHEN NOT MATCHED THEN INSERT([Fin Year],[Table Name],br_code,Tran_Type,[Max Time])"
                    + "     VALUES(b.finYear,b.tableName,b.brCode,b.tranType,b.maxTime);";
                odbcGlobalVar._DestinationConfig.ExecuteRawQuery(odbcGlobalVar, lsSqlQuery, 1, out iiSuccess, out isReturn);
                return isReturn;
            }
            catch
            {
                return "Failed";
            }
        }


        public string UpdateMaxTimeLoopZero()
        {
            try
            {
                
                return isReturn;
            }
            catch
            {
                return "Failed";
            }
        }

        public void MaxTimeUpdateOnError(string asTableName, string asFinYearCol, string asBrCodeCol,
            string asTranColName, string asTimeColName, string asFinYear, int aiTran, string asBrCode)
        {
            string lsColList, lsWhereCondition, lsReturn, lsQuery = "";
            try
            {
                lsColList = (asFinYearCol == "" || asFinYearCol == null ? "" : asFinYearCol + ",") +
                    (asBrCodeCol == "" || asBrCodeCol == null ? "" : asBrCodeCol + ",") +
                    (asTranColName == "" || asTranColName == null ? "" : asTranColName + ",");
                lsColList = lsColList.Trim();
                lsWhereCondition = (asFinYearCol == "" || asFinYearCol == null ? "" : " a.[Fin Year] = tbl." + asFinYearCol + " And ") +
                    (asBrCodeCol == "" || asBrCodeCol == null ? "" : " a.br_code = tbl." + asBrCodeCol + " And ") +
                    (asTranColName == "" || asTranColName == null ? "" : " a.Tran_Type = tbl." + asTranColName + " And ");

                lsQuery = " Merge Into LS_MaxTime WITH(HOLDLOCK) AS a USING(Select " + lsColList
                    + $"Max({asTimeColName}) as dm From " + asTableName
                    + (lsColList.Length > 0 ? " Group By "
                    + (Microsoft.VisualBasic.Strings.Right(lsColList, 1) == "," ? lsColList.Substring(0, lsColList.Length - 1) : lsColList) : "")
                    + $" ) as tbl on {lsWhereCondition} a.[Table Name] = '{asTableName}'"
                    + " When Matched Then Update Set [Max Time] = tbl.dm "
                    + " When Not Matched Then Insert([Fin Year],[Table Name],[br_code],[Tran_Type],[Max Time])"
                    + $" Values('{asFinYear}','{asTableName}','{asBrCode}'," 
                    + (aiTran == 1 ? "tbl." + asTranColName : "'-'") + ", tbl.dm);";

                odbcGlobalVar._DestinationConfig.ExecuteRawQuery(odbcGlobalVar, lsQuery, 1, out iiSuccess, out lsReturn);
                setStatusLog("status", (iiSuccess == 1 ? "Max Time Success" : lsReturn + Environment.NewLine + lsQuery), 1);
                return;
            }
            catch (Exception Ex)
            {
                setStatusLog("status", "Max Time Update Failed : " + Environment.NewLine +
                    lsQuery + Environment.NewLine + Ex.Message, 1);
                return;
            }
        }


        public int RetryInsert(string asTimeColName, string asTableName, string asBrColName, string asTranWiseColName, 
            string asBr, int aiLoop, string asReturn, string asSqlQuery, string asFinColName)
        {
            int liLoop = aiLoop, i = 0;
            if (asReturn.Contains("transport") && asReturn.Contains("level") && asReturn.Contains("error") && asTimeColName != "" && asTimeColName != null)
            {
                while (iiRetryCount > i)
                {
                    Thread.Sleep(8000);
                    odbcGlobalVar._DestinationConfig.ExecuteRawQuery(odbcGlobalVar, asSqlQuery, 1, out iiSuccess, out isReturn);
                    setStatusLog("status", isReturn, 1);
                    if (iiSuccess == 0 && isReturn.Contains("transport") && isReturn.Contains("level") && isReturn.Contains("error"))
                        i++;
                    else
                        i = iiRetryCount;
                }

                if (iiSuccess == 0)
                {
                    liLoop = 0;
                    MaxTimeUpdateOnError(asTableName, asFinColName, asBrColName, asTranWiseColName, asTimeColName,
                        isFinYear, (asTranWiseColName == null || asTranWiseColName == "" ? 0 : 1), (asBr == "" ? "-" : asBr));
                }
            }
            else
            {
                liLoop = 0;
            }
            return liLoop;
        }


        public string BulkInsert(string asTableName, DataTable adtData)
        {
            try
            {
                int liTryCount = 0;
                
                isReturn = odbcGlobalVar.custDBConn.destConnSetup(odbcGlobalVar.custServerName,
                    odbcGlobalVar.custDbName, odbcGlobalVar.custUID,
                    odbcGlobalVar.custPwd, odbcGlobalVar.giCustDestDBAuth);
                if (isReturn == "Failed")
                {
                    setStatusLog("status", isReturn += " - LS Server Connection Failed.", 1);
                    auditLogReset(4);
                    return "Failed";
                }
            InsertIFails:
                liTryCount += 1;
                isReturn = odbcGlobalVar.custDBConn.destDBInsertBulk(asTableName, adtData);
                setStatusLog("status", "Insert " + (isReturn == "" ? "Done" : "Failed - " + isReturn), 1);
                if (isReturn.Contains("Failed"))
                {
                    if (liTryCount <= 3)
                    {
                        setStatusLog("status", "Number of Tries - " + liTryCount.ToString(), 1);
                        goto InsertIFails;
                    }
                    setStatusLog("status", isReturn, 1);
                    auditLogReset(4);
                    return "Failed";
                }
                return "Success";
            }
            catch (Exception Ex)
            {
                setStatusLog("status", "Failed - " + Ex.Message, 1);
                auditLogReset(4);
                return "Failed";
            }
        }

        /////////////////////////////////////////////////////////////////////
        //Check Tran Type Exists
        /////////////////////////////////////////////////////////////////////
        public int CheckTranType(string tranColName, string fetchSql)
        {
            string lsFetchSql, lsTranColName, lsTransList;
            int i = 0;
            if (tranColName != "" && tranColName != null)
            {
                lsTranColName = odbcGlobalVar.reverseString(tranColName.Replace("]", "").Replace("[", ""));
                lsFetchSql = odbcGlobalVar.reverseString(fetchSql.Replace("]", "").Replace("[", ""));

                while (lsFetchSql.Contains(lsTranColName))
                {
                    lsFetchSql = lsFetchSql.Substring(lsFetchSql.IndexOf(lsTranColName));
                    lsTransList = lsFetchSql.Substring(0, lsFetchSql.IndexOf(','));
                    lsFetchSql = lsFetchSql.Substring(lsFetchSql.IndexOf(','));
                    if (lsTransList.Contains("'"))
                    {
                        i += 1;
                    }
                }
            }
            return i;
        }

        /////////////////////////////////////////////////////////////////////
        //Branch List Fetch
        /////////////////////////////////////////////////////////////////////
        public string brList(string fetchDB, string changeDB, string asSql = "", int aiChangeDB = 0)
        {
            string lsBrList;
            int licnt;
            odbcGlobalVar._DestinationConfig.ExecuteRawQuery(odbcGlobalVar, "", 2, out iiSuccess, out isReturn);
            if (iiSuccess == 0)
            {
                setStatusLog("status", "Failed to Connect Customer Server on Checking Table Exists", 1);
                lsBrList = "";
            }
            else
            {
                isSqlQuery = "SELECT COUNT(*) FROM " + 
                    (asSql == "" || asSql == null ? "sys.tables WHERE name = 'LS_Branch_Master';" : $"({asSql}) a");
                odbcGlobalVar._DestinationConfig.ExecuteRawQuery(odbcGlobalVar, isSqlQuery, 0, out iiSuccess, out lsBrList, 0);
                if (int.TryParse(lsBrList, out _))
                {
                    licnt = int.Parse(lsBrList);
                    if (licnt > 0)
                    {
                        if(asSql == "" || asSql == null)
                            odbcGlobalVar._DestinationConfig.GetBranchList(odbcGlobalVar, out iiSuccess, out lsBrList);
                        else
                            odbcGlobalVar._DestinationConfig.ExecuteRawQuery(odbcGlobalVar, asSql, 0, out iiSuccess, out lsBrList); 
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
            if (aiChangeDB == 0) 
                isReturn = dbConnect(changeDB, iiAuthType);

            return lsBrList;
        }

        //public string BulkDelete(DataTable adtTable, int aiFIrstIns, string asTableName, string asDelColName, string asPkeyCols,
        //    string asTimeColName, string asBrColName, string asTranWiseColName, string asBr, string asFinColName, int aiLoop,
        //    string asSelJsonColList, string asJsonConList, out int aiSuccess, string plCol = "")
        //{
        //    aiSuccess = 0;
        //    try
        //    {
        //        if (aiFIrstIns == 0)
        //        {
        //            string lsQuery = "";

        //            int liColCountVerify = 0, liBulkDetete = 1;

        //            foreach (DataColumn colName in adtTable.Columns)
        //            {
        //                if (colName.ColumnName == asDelColName.Replace("[", "").Replace("]", ""))
        //                    liColCountVerify++;
        //            }

        //            if (liColCountVerify <= 0) asDelColName = "";

        //            //if (lsTableName != "LS_Sale_Order" && lsTableName != "LS_Quotation"){
        //            if (asDelColName != "" && asDelColName != null)
        //            {
        //                List<dynamic> lsPkys = adtTable.AsEnumerable()
        //                            .Select(al => al.Field<dynamic>(asDelColName.Replace("[", "").Replace("]", "")))
        //                            .Distinct().ToList();
        //                if (lsPkys.Count > 0)
        //                {
        //                    string lsPkyList = "";
        //                    lsPkyList = String.Join("','", lsPkys);
        //                    lsQuery = $"Delete From {asTableName} Where {asDelColName} in ('{lsPkyList}')";
        //                }
        //            }
        //            else if (asPkeyCols != "" && asPkeyCols != null)
        //            {
        //                setStatusLog("status", "Delete From Destination Table -- Start", 1);
                        
        //                odbcGlobalVar._DestinationConfig.ExecuteRawQuery(odbcGlobalVar,
        //                      $"If Exists (Select * from sys.tables where name = '{asTableName}_TmpApp') Begin Drop table {asTableName}_TmpApp; End;", 
        //                      1, out iiSuccess, out isReturn, 1, 0);

        //                odbcGlobalVar._DestinationConfig.ExecuteRawQuery(odbcGlobalVar,
        //                      $"Select * Into {asTableName}_TmpApp from {asTableName} Where 1 = 2", 
        //                      1, out iiSuccess, out isReturn, 1, 0);
                        
        //                if (iiSuccess == 0)
        //                {
        //                    setStatusLog("status", isReturn, 1);
        //                    auditLogReset(4);
        //                    return "Failed";
        //                }
                        
        //                isReturn = BulkInsert($"{asTableName}_TmpApp", adtTable);
        //                if (isReturn == "Failed")
        //                {
        //                    setStatusLog("status", isReturn, 1);
        //                    auditLogReset(4);
        //                    return "Failed";
        //                }
                        
        //                odbcGlobalVar._DestinationConfig.ExecuteRawQuery(odbcGlobalVar,
        //                      $"Delete a From {asTableName} a Join {asTableName}_TmpApp new_a On {asPkeyCols}",
        //                      1, out iiSuccess, out isReturn, 1, 0);
        //                if (isReturn == "Failed")
        //                {
        //                    setStatusLog("status", isReturn + Environment.NewLine + $"Delete a From {asTableName} a Join {asTableName}_TmpApp new_a On {asPkeyCols}", 1);
        //                    auditLogReset(4);
        //                    return "Failed";
        //                }
                        
        //                odbcGlobalVar._DestinationConfig.ExecuteRawQuery(odbcGlobalVar,
        //                      $"Drop Table {asTableName}_TmpApp",
        //                      1, out iiSuccess, out isReturn, 1, 0);
        //                if (isReturn == "Failed")
        //                {
        //                    setStatusLog("status", isReturn , 1);
        //                    auditLogReset(4);
        //                    return "Failed";
        //                }
        //                setStatusLog("status", "Deleted Successfully", 1);
        //                aiSuccess = 1;
        //            }
        //            //}
        //            if (lsQuery != "" && lsQuery != null)
        //            {
        //                setStatusLog("status", "Delete From Destination Table -- Start", 1);
        //                odbcGlobalVar._DestinationConfig.ExecuteRawQuery(odbcGlobalVar,
        //                                lsQuery, 1, out iiSuccess, out isReturn, 1, 0, liBulkDetete);
                        
        //                GC.Collect();
        //                if (iiSuccess == 0)
        //                {
        //                    int liLoop = RetryInsert(asTimeColName, asTableName, asBrColName,
        //                        asTranWiseColName, asBr, aiLoop, isReturn, lsQuery, asFinColName);
        //                    if (liLoop == 0)
        //                    {
        //                        setStatusLog("status", isReturn, 1);
        //                        auditLogReset(4);
        //                        return "Failed";
        //                    }
        //                }
        //                aiSuccess = 1;
        //                setStatusLog("status", "Deleted Successfully", 1);
        //            }
        //        }
        //        else
        //        {
        //            aiSuccess = 1;
        //        }
        //        return "Success";
        //    }
        //    catch(Exception Ex)
        //    {
        //        return $"Failed - {Ex.Message}";
        //    }
        //}

        public string BulkDeleteInsert(DataTable adtTable, int aiFIrstIns, string asTableName, string asDelColName, string asPkeyCols,
            string asTimeColName, string asBrColName, string asTranWiseColName, string asBr, string asFinColName, int aiLoop,
            string asSelJsonColList, string asJsonConList, out int aiSuccess, string plCol = "")
        {
            aiSuccess = 0;
            try
            {
                if (aiFIrstIns == 0)
                {
                    string lsQuery = "";

                    int liColCountVerify = 0;

                    foreach (DataColumn colName in adtTable.Columns)
                    {
                        if (colName.ColumnName == asDelColName.Replace("[", "").Replace("]", ""))
                            liColCountVerify++;
                    }

                    if (liColCountVerify <= 0) asDelColName = "";

                    setStatusLog("status", "Delete From Destination Table -- Start", 1);

                    odbcGlobalVar._DestinationConfig.ExecuteRawQuery(odbcGlobalVar,
                            $"If Exists (Select * from sys.tables where name = '{asTableName}_TmpApp') Begin Drop table {asTableName}_TmpApp; End;",
                            1, out iiSuccess, out isReturn, 1, 0);

                    odbcGlobalVar._DestinationConfig.ExecuteRawQuery(odbcGlobalVar,
                            $"Select * Into {asTableName}_TmpApp from {asTableName} Where 1 = 2",
                            1, out iiSuccess, out isReturn, 1, 0);

                    if (iiSuccess == 0)
                    {
                        setStatusLog("status", isReturn, 1);
                        auditLogReset(4);
                        return "Failed";
                    }

                    isReturn = BulkInsert($"{asTableName}_TmpApp", adtTable);
                    if (isReturn == "Failed")
                    {
                        setStatusLog("status", isReturn, 1);
                        auditLogReset(4);
                        return "Failed";
                    }

                    if (asDelColName != "" && asDelColName != null)
                    {
                        lsQuery = $"Delete a From {asTableName} a Join {asTableName}_TmpApp new_a On a.{asDelColName} = new_a.{asDelColName}";
                    }
                    else if (asPkeyCols != "" && asPkeyCols != null)
                    {
                        lsQuery = $"Delete a From {asTableName} a Join {asTableName}_TmpApp new_a On {asPkeyCols}";
                    }

                    odbcGlobalVar._DestinationConfig.ExecuteRawQuery(odbcGlobalVar, lsQuery, 1, out iiSuccess, out isReturn, 1, 0);
                    if (isReturn == "Failed")
                    {
                        setStatusLog("status", "Delete Failed - " + isReturn, 1);
                        auditLogReset(4);
                        return "Failed";
                    }

                    odbcGlobalVar._DestinationConfig.ExecuteRawQuery(odbcGlobalVar,
                        $"Insert Into {asTableName} Select * from {asTableName}_TmpApp", 1, out iiSuccess, out isReturn, 1, 0);
                    if (isReturn == "Failed")
                    {
                        setStatusLog("status", "Insert Failed - " + isReturn, 1);
                        auditLogReset(4);
                        return "Failed";
                    }

                    odbcGlobalVar._DestinationConfig.ExecuteRawQuery(odbcGlobalVar, $"Drop Table {asTableName}_TmpApp", 1, out iiSuccess, out isReturn, 1, 0);
                    if (isReturn == "Failed")
                    {
                        setStatusLog("status", isReturn, 1);
                        auditLogReset(4);
                        return "Failed";
                    }
                    setStatusLog("status", "Done", 1);
                    aiSuccess = 1;
                }
                else
                {
                    aiSuccess = 1;
                }
                return "Success";
            }
            catch(Exception Ex)
            {
                return $"Failed - {Ex.Message}";
            }
        }

        //////////////////////////////////////////////////////////////////////////////
        //Primary Key Columns / Table Type Check & Truncate
        //////////////////////////////////////////////////////////////////////////////
        public string TruncateTable(string asTableName, string asQuery = "", string asMaxTime = "")
        {
            try
            {
                string lsQuery;
                setStatusLog("status", (asQuery == "" ? "Truncate Start" : "Start Full Delete"), 1);
                lsQuery = (asQuery == "" ? "Truncate Table " + asTableName : asQuery);
                odbcGlobalVar._DestinationConfig.ExecuteRawQuery(odbcGlobalVar,
                                        lsQuery, 1, out iiSuccess, out isReturn);
                if (iiSuccess == 0)
                {
                    setStatusLog("status", isReturn, 1);
                    return "Failed";
                }
                setStatusLog("status", "Truncated", 1);

                setStatusLog("status", "MaxTime", 1);
                lsQuery = (asMaxTime == "" ? "Delete From LS_MaxTime Where [Table Name] = '" + asTableName + "'" : asMaxTime);
                odbcGlobalVar._DestinationConfig.ExecuteRawQuery(odbcGlobalVar,
                                        lsQuery, 1, out iiSuccess, out isReturn);
                if (iiSuccess == 0)
                {
                    setStatusLog("status", isReturn, 1);
                    return "Failed";
                }
            }
            catch(Exception Ex)
            {
                setStatusLog("status", Ex.Message, 1);
                return "Failed";
            }
            return "";
        }

    }
}
