using System;
using System.IO;
using System.Linq;
using System.Data;
using Newtonsoft.Json;
using System.Text.Json;
using LSSyncApp.Functions;
using System.Windows.Forms;
using LSSyncApp.Controllers;
using System.Runtime.InteropServices;

namespace LSSyncApp
{
    public class GlobalVariable
    {
        public int maxRowCount = 25000,maxDaysToSync = 7, giCmd = 0, giApi = 0, giTheme = 0, giNoUpdate = 0;
        public string firmCode, custCode, firmName, custName, tableListToFetch, branchColName = "c_br_code", gsLogFileName, compCode;
        public string gsSoftwareCode = "", gsSoftwareName = "", gsAppCode = "AP0003", gsCustAppCode;
        public string gsFinYear,defTime = "2000-01-01 00:00:00", gsCmdParam = "", gsReportCount, gsSyncType;
        public string[] commandParams;
        public FormWindowState winState;
        public string gsVersion = "0.00", gsAppVersion = "13.10", gsUpdateStatus = "Success", gsUpdateMessage;
        public string gsProcessId, gsProcessName;

        //Notification / Trigger Variables
        public string gsNotificationList, gsAppUpdateList;

        //AAd Variables
        public string RedirectUrl, ClientId, gsClientUrl, Authority, AuthorityEditProfile, AuthorityPasswordReset;
        public string staging, PolicySignUpSignIn, PolicyEditProfile, PolicyResetPassword, gsSignInAPI;
        public string[] Scopes;

        //Embbed Variables
        public string gsProfileId, gsWorkspaseId, gsEmbedAutenticationMode, gsEmbedTenantId;
        public string gsEmbedClientId, gsEmbedClientSecret, gsEmbedScope, gsEmbedAuthority, gsEmbedApiDomain;
        public string gsEmbedResourceGroup, gsEmbedResource, gsEmbedResourceSubscriptionId;
        public int giCheckEmbed;

        //LeapSurge Database Spec
        public string masterServerName = "leapsurgebi.database.windows.net";
        public string masterUID = "";
        public string masterPwd = "";
        public string masterElasticPool = "LeapSurgeBI_ElasticSQLPool";
        public int giMasterDBAuth = 0, giCustDestDBAuth = 0;

        //API & DB Variables - Production
        public string masterdbName = "Leapsurgebi", AppDataUrl = "https://api.leapsurgebi.com/api/AppData";

        //API & DB Variables - Staging
        //public string masterdbName = "LSMASTER", AppDataUrl = "https://staging.api.leapsurgebi.com/api/AppData";

        //Customers Database Spec
        public string custDbName, custServerName, custUID, custPwd, custElasticPool, custMasterDB = "master";

        //App path and Log file Path
        public string gsApplPath = AppDomain.CurrentDomain.BaseDirectory;
        public string gsLogPath = AppDomain.CurrentDomain.BaseDirectory + "Log\\";
        public string gsTemplatePath = AppDomain.CurrentDomain.BaseDirectory + "Templates\\";

        //Registry path
        public string regPath = "HKEY_CURRENT_USER\\SOFTWARE\\LS" 
            + "\\" + (AppDomain.CurrentDomain.BaseDirectory).Replace(" ", "").Replace("\\", "_")
            .Replace("/", "_").Replace("\"", "_").Replace("'", "_").Replace(" ", "_").Replace(":", "_");

        //Database Connection Objects
        public Connections custDBConn = new Connections();
        public Connections masterLogin = new Connections();
        public Connections odbcConn = new Connections();

        //Configurations
        public MasterConfig _MasterConfig = new MasterConfig();
        public DestinationConfig _DestinationConfig = new DestinationConfig();
        public GlobalFunctions _fun = new GlobalFunctions();
        public Theme _Theme = new Theme();
        public RestAPI _grRestAPI = new RestAPI();

        //Users
        public string gsUserRoleCode, gsUserRoleName, gsAADUserId, gsUserId, gsUserName, gsUserCode;

        //Audit Log
        public AuditLog gAuditLog = new AuditLog();
        public string gdSessionStartAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        public int giAudit = 0, giCustDBAuthMethod = 0;
        public string gsSystemIp, gsSystemLocalIp, gsSystemUser, gsSystemName;

        //Image Links
        public string gsContainerLocation, gsLogoContainer;
        
        //////////////////////////////////////////////////////////////////////////////
        //Creating Log File.
        //////////////////////////////////////////////////////////////////////////////
        public void logFile(string fileName, string fileValue, int fileType)
        {
            string lsLogPath = gsLogPath + DateTime.Now.ToString("yyyyMMdd") + "\\";
            bool exists = Directory.Exists(lsLogPath);

            if (!exists)
            {
                Directory.CreateDirectory(lsLogPath);
            }
            switch (fileType)
            {
                case 1:
                    File.AppendAllText(lsLogPath + fileName, Environment.NewLine + fileValue);
                    break;
                case 2:
                    File.WriteAllText(lsLogPath + fileName, fileValue);
                    break;
                default:
                    break;
            }
        }
        
        //////////////////////////////////////////////////////////////////////////////
        //Creating Log File.
        //////////////////////////////////////////////////////////////////////////////
        public void TemplateFile(string fileName, string fileValue, int fileType)
        {
            string lsLogPath = gsTemplatePath;
            bool exists = Directory.Exists(lsLogPath);
            if (!exists)
            {
                Directory.CreateDirectory(lsLogPath);
            }
            
            switch (fileType)
            {
                case 1:
                    File.AppendAllText(lsLogPath + fileName, Environment.NewLine + fileValue);
                    break;
                case 2:
                    if (!File.Exists(lsLogPath + fileName))
                    {
                        File.Delete(lsLogPath + fileName);
                    }
                    File.WriteAllText(lsLogPath + fileName, fileValue);
                    break;
                default:
                    break;
            }
        }

        //////////////////////////////////////////////////////////////////////////////
        //Creating the Parse string of the JSON Value
        //////////////////////////////////////////////////////////////////////////////
        public JsonElement createJsonElement(string jsonString)
        {
            //jsonString = jsonString.Replace("\\", "");
            if(jsonString.Substring(0,1) == "\"")
            {
                jsonString = jsonString.Substring(1, jsonString.Length - 2);
            }
            if (jsonString.Substring(0, 1) != "[")
            {
                jsonString = "[" + jsonString + "]";
            }
            JsonDocument jsonStrList = JsonDocument.Parse(jsonString);
            JsonElement jsonTblArray = jsonStrList.RootElement;
            return jsonTblArray;
        }

        //////////////////////////////////////////////////////////////////////////////
        //Replacing the Special Characters.
        //////////////////////////////////////////////////////////////////////////////
        public string replaceSpecialCharacters(string inputString)
        {
            try
            {
                string outputString;
                outputString = inputString.Replace("", "-").Replace("", "-").Replace("", "-").Replace("", "-").Replace("", "-");
                outputString = outputString.Replace("'", "-").Replace("	", "-").Replace("\\", "\\/").Replace("'", "-");
                return outputString;
            }
            catch (Exception e)
            {
                return e.Message + " -- |Failed to Replace String";
            }
        }

        //////////////////////////////////////////////////////////////////////////////
        //String Reverse.
        //////////////////////////////////////////////////////////////////////////////
        public string reverseString(string inputString)
        {
            string lsString;
            lsString = new string(inputString.ToCharArray().Reverse().ToArray());
            return lsString;
        }

        //////////////////////////////////////////////////////////////////////////////
        //Set Log Message.
        //////////////////////////////////////////////////////////////////////////////
        public void setMessageLog(string fileName,string msg,int cmd)
        {
            if (cmd == 1)
            {
                logFile(gsLogPath + fileName, msg, 1);
            }
            else
            {
                MessageBox.Show(msg);
            }
        }

        public IntPtr createRoundRect(int nLeftRect,int nTopRect,int nRightRect,int nBottomRect,int nWidthEllipse,int nHeightEllipse)
        {
            return CreateRoundRectRgn(nLeftRect, nTopRect, nRightRect, nBottomRect, nWidthEllipse, nHeightEllipse);
        }


        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );

        public string Base64Decode(string base64EncodedData, string asType)
        {
            try
            {
                var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
                return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            }
            catch
            {
                return "Failed";
            }
        }

        public string Base64Encode(string plainText)
        
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public DataTable GetJSONToDataTable(string JSONData)
        {
            DataTable dt = (DataTable)JsonConvert.DeserializeObject(JSONData, (typeof(DataTable)));
            return dt;
        }

        public string DataTableToJSONWithJSONNet(DataTable table)
        {
            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(table);
            return JSONString;
        }
    }
}
