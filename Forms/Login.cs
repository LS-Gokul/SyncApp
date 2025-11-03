using System;
using System.IO;
using System.Linq;
using System.Drawing;
using Microsoft.Win32;
using LSSyncApp.Forms;
using LSSyncApp.Models;
using System.Text.Json;
using System.Diagnostics;
using LSSyncApp.Functions;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;

namespace LSSyncApp
{
    public partial class Login : Form
    {
        private DeserializeJWT _DeserializeJWT = new DeserializeJWT();
        public static GlobalVariable globalVar = new GlobalVariable();
        public static RestAPI restAPI = new RestAPI();
        public static MasterConfig _MasterConfig = new MasterConfig();

        //Master Database Configuration
        public static string lsMasterServerName, lsMasterUID, lsMasterPwd, lsElasticMasterPool, lsMasterDBName;

        //Customer Database Configuration
        public static string lsCustServerName, lsCustUID, lsCustPwd, lsCustElasticPool, lsCustDbName;

        //Customer Master DB
        public static string lsCustMasterDBName;

        public static string lsEmailID, isLogFileName = (DateTime.Now.ToString("HHmmss")) + ".txt";
        public static string lsReturn, lsSqlQuery;
        public static int liDBExists, iiWait = 0,iiSuccess, iiLogin = 0;

        /////////////////////////////////////////////////////
        public static string Domain, Tenant, ClientId, RedirectUrl, AuthorityBase, Authority;

        public Login(string[] cmdParams)
        {
            try 
            {
                InitializeComponent();
            }
            catch(Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
            btnSignIn.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btnSignIn.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btn_loginfirm.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btn_loginfirm.FlatAppearance.MouseDownBackColor = Color.Transparent;
            this.WindowState = FormWindowState.Minimized;
            globalVar.commandParams = cmdParams;
            variableSet();
            ApplicationCredentails();
            globalVar.gsProcessId = Process.GetCurrentProcess().Id.ToString();
            globalVar.gsProcessName = Process.GetCurrentProcess().ProcessName;
        }

        private void Login_Load(object sender, EventArgs e)
        {
            try
            {
                GetPublicIPAddressAsync();
                btnEnable(true);
                
                globalVar.gsLogFileName = isLogFileName;
                if (globalVar.commandParams.Length > 0)
                {
                    if(globalVar.commandParams[0] == "Updates")
                    {
                        globalVar.giCmd = int.TryParse(globalVar.commandParams[3],out _) ? int.Parse(globalVar.commandParams[3]) : 0;
                        globalVar.winState = globalVar.giCmd == 1 ? FormWindowState.Minimized : FormWindowState.Maximized;
                    }
                    else
                    {
                        globalVar.giCmd = 1;
                        globalVar.firmCode = null;
                        if (globalVar.commandParams[0].Contains("~NO~"))
                        {
                            globalVar.giNoUpdate = 1;
                            globalVar.commandParams[0] = globalVar.commandParams[0].Replace("~NO~", "");
                        }
                        if (globalVar.commandParams[0].Contains("~"))
                        {
                            globalVar.firmCode = globalVar.commandParams[0].Substring(globalVar.commandParams[0].IndexOf("~") + 1,4);
                            globalVar.commandParams[0] = globalVar.commandParams[0].Substring(0, globalVar.commandParams[0].IndexOf("~"));
                        }
                        globalVar.winState = FormWindowState.Minimized;
                    }
                }
                else
                {
                    globalVar.winState = FormWindowState.Maximized;
                }
                this.WindowState = FormWindowState.Minimized;
                //Print(3);
                loginIfExists();
                //Print(4);
                if (iiLogin == -1 && globalVar.giCmd == 1) Application.Exit();

                if(globalVar.giCmd == 1)
                {
                    this.WindowState = globalVar.winState;
                }
                else
                {
                    this.WindowState = FormWindowState.Normal;
                }
                btnSignIn.FlatStyle = FlatStyle.Flat;
                btnSignIn.FlatAppearance.BorderSize = 0;
                btnSignIn.FlatAppearance.BorderColor = Color.FromArgb(0, 255, 255, 255);
                
                btn_loginfirm.FlatStyle = FlatStyle.Flat;
                btn_loginfirm.FlatAppearance.BorderSize = 0;
                btn_loginfirm.FlatAppearance.BorderColor = Color.FromArgb(0, 255, 255, 255);
            }
            catch //(Exception Ex)
            {
                //globalVar.setMessageLog(isLogFileName, Ex.Message, globalVar.giCmd);
            }
        }

        /////////////////////////////////////////////
        //AAD B2C Validation/Signin
        /////////////////////////////////////////////
        private void button1_Click(object sender, EventArgs e)
        {
            setStatusValue("status", "Please Wait...");
            string lsUserName = tbUserName.Text;
            string lsPassword = tbPassword.Text;
            if (string.IsNullOrEmpty(lsUserName))
                setStatusValue("status", "Please Enter User Name");
            if (string.IsNullOrEmpty(lsPassword))
                setStatusValue("status", "Please Enter Password");

            _DeserializeJWT.SignIn(globalVar.gsSignInAPI, $"username={lsUserName}&password={lsPassword}", restAPI, out iiSuccess);
            if (iiSuccess != 1)
            {
                return;
            }


            iiWait = LoginValidation();
            //iiWait = await aadValidation();

            if (iiWait != 1)
            {
                setStatusValue("status", "");
                return;
            }
            //GetLocalIPAddress();

            btnSignIn.Enabled = false;
            if (verify()) btnEnable(false);
            else
            {
                btnSignIn.Enabled = true;
                btnEnable(true);
            }

            setStatusValue("status", "");
            
        }

        private int LoginValidation()
        {
            try
            {
                _DeserializeJWT.DeserializeJWTData(out iiSuccess, out lsReturn);
                if (iiSuccess == 0)
                {
                    iiLogin = -1;
                    return 0;
                }

                Token _Token = new Token();
                _Token = JsonSerializer.Deserialize<Token>(lsReturn);

                lsEmailID = _Token.AAD_User_Id;
                btnEnable(false);

                ////////////////////////////////////////////////////////////
                ///////////////Start the Background Scheduler Application
                //if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "LS_Initiator.exe"))
                //{
                //    try
                //    {
                //        if (Process.GetProcessesByName("LS_Initiator").Length <= 0)
                //            Process.Start(AppDomain.CurrentDomain.BaseDirectory + "LS_Initiator.exe");
                //    }
                //    catch
                //    {

                //    }
                //}
                ////////////////////////////////////////////////////////////
                return 1;
            }
            catch(Exception E)
            {
                btnEnable(true);
                globalVar.setMessageLog(isLogFileName, E.Message, globalVar.giCmd);

                iiLogin = -1;
                return 0;
            }

        }

        private void setStatusValue(string typeValue, string textValue)
        {
            if(textValue == "" || textValue == null)
            {
                lbStatus.Visible = false;
            }
            else
            {
                lbStatus.Visible = true;
            }
            switch (typeValue)
            {
                case "status"://Status
                    lbStatus.Text = textValue;
                    lbStatus.Refresh();
                    break;
                default:
                    break;
            }
        }

        private void btn_loginfirm_Click(object sender, EventArgs e)
        {
            setStatusValue("status", "Please wait.....");
            globalVar.firmCode = ((KeyValuePair<string, string>)comBFirm.SelectedItem).Key;
            globalVar.firmName = ((KeyValuePair<string, string>)comBFirm.SelectedItem).Value;
            
            globalVar._fun.custDBConfig(globalVar, globalVar.custCode, globalVar.firmCode, lsEmailID);
            //custDBConfig(globalVar.custCode,globalVar.firmCode);


            liDBExists = globalVar._fun.CheckDBExists(globalVar, out iiSuccess, out lsReturn);
            if (iiSuccess == 0)
            {
                setStatusValue("status", lsReturn);
                return;
            }
            
            if (liDBExists == 0)
            {
                setStatusValue("status", "We are Allocating a private space for you.....");
                globalVar._fun.CreateDB(globalVar, out iiSuccess, out lsReturn);
                if (iiSuccess == 0)
                {
                    setStatusValue("status", "");
                    globalVar.setMessageLog(isLogFileName, lsReturn, globalVar.giCmd);
                    return;
                }
            }
            
            _MasterConfig.GetUserConfigDetails(globalVar, lsEmailID, 2, out iiSuccess, out lsReturn);
            SetSettingsVariable(lsReturn);
            
            setStatusValue("status", "");
            appClose();
        }

        public void loginIfExists()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\LS"))
                {
                    if (key != null)
                    {
                        Registry.CurrentUser.DeleteSubKey("SOFTWARE\\LS");
                    };
                };
                
                //iiWait = await aadValidation();
                iiWait = LoginValidation();
                if (iiWait != 1)
                {
                    return;
                }

                /************************************/
                ///////////////Fetch Customer Deatils
                _MasterConfig.GetBasicDetails(globalVar, lsEmailID, 1, out iiSuccess, out lsReturn);
                if (iiSuccess == 0)
                {
                    iiLogin = -1;
                    return;
                }

                JsonElement ljeCustDet = globalVar.createJsonElement(lsReturn);

                globalVar.custCode = ljeCustDet[0].GetProperty("lsCode").ToString();
                lsEmailID = ljeCustDet[0].GetProperty("email").ToString();
                globalVar.firmCode = (globalVar.firmCode == "" || globalVar.firmCode == null ? 
                        ljeCustDet[0].GetProperty("defFirm").ToString() : globalVar.firmCode);

                if (globalVar.firmCode == "" || globalVar.firmCode == null || globalVar.firmCode == "None")
                {
                    globalVar.setMessageLog(isLogFileName, "Firm Not Registered/Login", globalVar.giCmd);
                    iiLogin = -1;
                    return;
                }

                ///////////////Fetch Customer DB Settings from Master
                _MasterConfig.GetUserConfigDetails(globalVar, lsEmailID, 2, out iiSuccess, out lsReturn);
                if (iiSuccess == 0)
                {
                    globalVar.setMessageLog(isLogFileName, lsReturn, globalVar.giCmd);
                    return;
                }
                SetSettingsVariable(lsReturn);
                globalVar._fun.custDBConfig(globalVar, globalVar.custCode, globalVar.firmCode, lsEmailID);
                
                globalVar._DestinationConfig.ExecuteRawQuery(globalVar, "", 2, out iiSuccess, out lsReturn, 0);
                GC.Collect();
                if (iiSuccess == 0)
                {
                    liDBExists = globalVar._fun.CheckDBExists(globalVar, out iiSuccess, out lsReturn);
                    if (iiSuccess == 0)
                    {
                        setStatusValue("status", lsReturn);
                        iiLogin = -1;
                        return;
                    }

                    if (liDBExists == 0)
                    {
                        setStatusValue("status", "We are Allocating a private space for you.....");
                        globalVar._fun.CreateDB(globalVar, out iiSuccess, out lsReturn);
                        if (iiSuccess == 0)
                        {
                            setStatusValue("status", "");
                            globalVar.setMessageLog(isLogFileName, lsReturn, globalVar.giCmd);
                            iiLogin = -1;
                            return;
                        }
                    }
                }
                _MasterConfig.GetCustFirmExists(globalVar, out iiSuccess, out lsReturn);
                //Print(5);

                if (iiSuccess == 0)
                {
                    globalVar.setMessageLog(isLogFileName, "Customer Database Login Failed", globalVar.giCmd);
                    iiLogin = -1;
                    return;
                }
                else if (int.TryParse(lsReturn, out _))
                {
                    if (int.Parse(lsReturn) > 0)
                    {
                        this.WindowState = globalVar.winState;
                        appClose();
                    }
                    else
                    {
                        globalVar.setMessageLog(isLogFileName, "Customer Not Logged in", globalVar.giCmd);
                        iiLogin = -1;
                        return;
                    }
                }
                //Print(6);

            }
            catch (Exception Ex)
            {
                globalVar.setMessageLog(isLogFileName, Ex.Message, globalVar.giCmd);
                return;
            }
        }

        private bool verify()
        {
            try 
            {
                _MasterConfig.GetUserConfigDetails(globalVar, lsEmailID, 1, out iiSuccess, out lsReturn);
                
                if (iiSuccess == 0)
                {
                    setStatusValue("status", "");
                    globalVar.setMessageLog(isLogFileName, lsReturn, globalVar.giCmd);
                    if (lsReturn == "User Not Exists")
                    {
                        logout();
                        Application.Exit();
                    }
                    return false;
                }

                SetSettingsVariable(lsReturn);

                _MasterConfig.GetFirmList(globalVar, out iiSuccess, out lsReturn);
                
                if (iiSuccess == 1)
                {
                    Dictionary<string, string> cb1Items = new Dictionary<string, string>();
                    JsonElement ljeFirmList = globalVar.createJsonElement(lsReturn);

                    int llFirmsCount = ljeFirmList.EnumerateArray().Count();
                    for (int i = 0; i < llFirmsCount; i++)
                    {
                        cb1Items.Add( ljeFirmList[i].GetProperty("firmCode").ToString(), ljeFirmList[i].GetProperty("firmName").ToString());
                    }
                    
                    comBFirm.DataSource = new BindingSource(cb1Items, null);
                    comBFirm.DisplayMember = "Value";
                    comBFirm.ValueMember = "Key";

                    //Login Automatically if there is a single firm
                    if (llFirmsCount == 1)
                    {
                        object loSender = new object();
                        EventArgs leEvntHndlr = new EventArgs();
                        btn_loginfirm_Click(loSender, leEvntHndlr);
                    }

                    setStatusValue("status", "Authentication Successful.");
                }
                else
                {
                    setStatusValue("status", "");
                    globalVar.setMessageLog(isLogFileName, "Firm Details not Found", globalVar.giCmd);
                    return false;
                }
                setStatusValue("status", "");
                return true;
            }

            catch (Exception Ex)
            {
                globalVar.setMessageLog(isLogFileName, Ex.Message, globalVar.giCmd);
                return false;
            }
        }

        
        //Variables Initialitation
        public static void variableSet()
        {
            //Master Database Configuration
            lsMasterServerName = globalVar.masterServerName;
            lsMasterUID = globalVar.masterUID;
            lsMasterPwd = globalVar.masterPwd;
            lsElasticMasterPool = globalVar.masterElasticPool;
            lsMasterDBName = globalVar.masterdbName;

            //Customer Master DB
            lsCustMasterDBName = globalVar.custMasterDB;

            //Log Details
            globalVar.gsSystemName = Environment.MachineName;
            globalVar.gsSystemUser = Environment.UserName;
            
        }

        private void appClose()
        {
            if (globalVar.gsSoftwareName == "Tally")
            {
                RestAPI restApi = new RestAPI();
                string lsTDLFile = restAPI.getAPICalling("https://leapsurgebi.blob.core.windows.net/sync-application/TALLY/C_LS_Collections.txt", out _);
                if (!lsTDLFile.Contains("Failed - API Calling "))
                {
                    if (File.Exists(globalVar.gsApplPath + "\\C_LS_Collections.txt"))
                    {
                        File.Delete(globalVar.gsApplPath + "\\C_LS_Collections.txt");
                    }
                    File.AppendAllText(globalVar.gsApplPath + "\\C_LS_Collections.txt", lsTDLFile);
                }
            }
            //Print(7);
            //////////////////////////////////////////////
            //Check Default Tables
            //////////////////////////////////////////////
            DefaultTables _DefaultTables = new DefaultTables();
            //Check Audit Log Table
            lsReturn = _DefaultTables.chkAuditLog(globalVar, isLogFileName, out iiSuccess);
            //Print(8);
            if (iiSuccess == 0) setStatusValue("status", "");
            if (lsReturn == "Success")
            {
                globalVar.giAudit = 1;
            }
            //Check RLS Table
            //Print(9);
            lsReturn = _DefaultTables.chkRLS(globalVar, isLogFileName, out iiSuccess);
            //Print(10);
            if (iiSuccess == 0) setStatusValue("status", "");
            //Check Version Table
            _DefaultTables.versionTable(globalVar, isLogFileName, out iiSuccess);
            //Print(11);
            if (iiSuccess == 0) setStatusValue("status", "");
            //Check MDM Table
            _DefaultTables.MDMTables(globalVar, isLogFileName, out iiSuccess);
            //Print(13);
            if (iiSuccess == 0) setStatusValue("status", "");
            //////////////////////////////////////////////

            //_Login.Hide();
            this.Hide();

            //Statistics
            globalVar._fun.UpdateDBStats(globalVar, out iiSuccess, out lsReturn);
            //Print(13);

            //Set BackgroundApp On Startup
            //Registry.SetValue("HKey_Current_User\\Software\\Microsoft\\Windows\\CurrentVersion\\Run", "LS_Initiator", "\"" + globalVar.gsApplPath + "LS_Initiator.exe\"");

            //Execute Command
            if (globalVar.giCmd == 1 && globalVar.commandParams[0] != "'-Export'")
            {
                for (int i = 0; i < globalVar.commandParams.Length; i++)
                {
                    globalVar.gsCmdParam += "'" + globalVar.commandParams[i] + (i == globalVar.commandParams.Length - 1 ? "'" : "',");
                }
                //Check Version
                UpdateVersion ver = new UpdateVersion();
                ver.versionCheck(globalVar);
                
                if (globalVar.gsCmdParam.Contains("-CheckStatus"))
                {
                    //Checking Status
                    globalVar._MasterConfig.SyncAppStatus(globalVar, out iiSuccess, out lsReturn);
                }
                else
                {
                    ODBCSyncParam oDBCSyncParam = new ODBCSyncParam();
                    var progress = new Progress<int> { };
                    if (globalVar.gsCmdParam.Contains("-SyncDataRpt"))//Sync Multiple Report
                    {
                        globalVar._MasterConfig.GetSchedules(globalVar, out iiSuccess, out lsReturn);
                        if(iiSuccess == 1)
                        {
                            JsonElement ljeRptSyncList = globalVar.createJsonElement(lsReturn);
                            int liCnt = ljeRptSyncList.EnumerateArray().Count();
                            for(int i = 0; i < liCnt; i++)
                            {
                                globalVar.gsCmdParam = ljeRptSyncList[i].GetProperty("rptParam").ToString();
                                oDBCSyncParam.sourceConfiguration(globalVar.gsCmdParam, progress, globalVar, 2);
                            }
                        }
                    }
                    else//Sync Single Report
                    {
                        oDBCSyncParam.sourceConfiguration(globalVar.gsCmdParam, progress, globalVar, 2);
                    }
                }
            }
            else
            {
                MainForm s = new MainForm(globalVar);
                s.ShowDialog();
            }
            this.Close();
        }

        private async Task<string> updateFirewall(string ip)
        {
            try
            {
                RestAPI restApi = new RestAPI();
                string ls_key, lsData = await restApi.PostUrlAsync("https://login.microsoftonline.com/9c0f89c6-6817-48d9-8461-286a9ce0dca3/oauth2/v2.0/token",
                    "&client_id=6a25f8b6-e424-4fe6-b639-c56810f438f9&scope=https://management.azure.com/.default&client_secret=Bz97Q~jsPxr0xbAiQYs17UNNV4Yqp0V0KP3AM&grant_type=client_credentials",
                    "application/x-www-form-urlencoded");

                JsonElement ljKey = new JsonElement();
                ljKey = globalVar.createJsonElement(lsData);
                ls_key = ljKey[0].GetProperty("access_token").ToString();

                string lsIpJson = "{\"properties\": {\"startIpAddress\": \"" + ip + "\",\"endIpAddress\": \"" + ip + "\"}}";
                lsData = await restApi.PostUrlAsync("https://management.azure.com/subscriptions/438981dc-e53c-472e-b6e6-bb6fbf058b3f/resourceGroups/LSProduction/providers/Microsoft.Sql/servers/leapsurgebi/firewallRules/" + ip.Replace(".", "_") + "?api-version=2021-02-01-preview",
                    lsIpJson, "application/json", "Bearer " + ls_key, "Put");
                return "Success";
            }
            catch
            {
                return "Failed";
            }
            
        }

        private void logout()
        {
            _DeserializeJWT.SignOut(out _);
            return;
        }

        private void btnEnable(bool abSignIn)
        {
            ////////////Sign In
            btnSignIn.Visible = abSignIn;
            lbUserName.Visible = abSignIn;
            lbPassword.Visible = abSignIn;
            tbUserName.Visible = abSignIn;
            tbPassword.Visible = abSignIn;

            ///////////Confirm Firm
            comBFirm.Visible = !abSignIn;
            btn_loginfirm.Visible = !abSignIn;
            lbl_firm.Visible = !abSignIn;
        }

        private void SetSettingsVariable(string asJson)
        {
            lsEmailID = globalVar._fun.SetSettingsVariable(globalVar, asJson);
            if (globalVar.giApi == 0)
            {
                //Task<string> str = updateFirewall(globalVar.gsSystemIp);
            }
        }

        //////////////////////////////////////////////////////////////////////////////
        //Set Application Configuration Variables
        //////////////////////////////////////////////////////////////////////////////
        public string ApplicationCredentails()
        {
            _MasterConfig.GetApplicationConfiguration(globalVar, out iiSuccess, out lsReturn);
            if (iiSuccess == 1)
            {
                JsonElement ljeAppConfig = globalVar.createJsonElement(lsReturn);
                string lsDomain,lsSignUpSignIn, lsEditProfile,lsResetPwd,lsTenant,lsAuthorityBase;
                
                lsDomain = ljeAppConfig[0].GetProperty("b2CIssuer").ToString();
                lsSignUpSignIn = ljeAppConfig[0].GetProperty("b2CSignInUpPolicy").ToString();
                lsEditProfile = ljeAppConfig[0].GetProperty("b2CEditPolicy").ToString();
                lsResetPwd = ljeAppConfig[0].GetProperty("b2CResetPolicy").ToString();
                
                lsTenant = ljeAppConfig[0].GetProperty("b2CTenantId").ToString().Replace("<Domain>", lsDomain);
                lsAuthorityBase = ljeAppConfig[0].GetProperty("b2CAuthorityBase").ToString()
                    .Replace("<Domain>", lsDomain).Replace("<Tenant>", lsTenant);
                
                globalVar.RedirectUrl = ljeAppConfig[0].GetProperty("b2CRedirectUrl").ToString();
                globalVar.ClientId = ljeAppConfig[0].GetProperty("b2CClientId").ToString();
                globalVar.gsClientUrl = ljeAppConfig[0].GetProperty("clientAPIUrl").ToString();//"http://192.168.11.19:9000/api/CustData";//
                globalVar.staging = ljeAppConfig[0].GetProperty("dBSuffix").ToString();
                globalVar.Scopes = new string[] { ljeAppConfig[0].GetProperty("b2CScope").ToString() };
                globalVar.gsSignInAPI = ljeAppConfig[0].GetProperty("signInAPI").ToString();

                globalVar.Authority = $"{lsAuthorityBase}{lsSignUpSignIn}";
                globalVar.AuthorityEditProfile = $"{lsAuthorityBase}{lsEditProfile}";
                globalVar.AuthorityPasswordReset = $"{lsAuthorityBase}{lsResetPwd}";
                globalVar.PolicySignUpSignIn = lsSignUpSignIn;
                globalVar.PolicyEditProfile = lsEditProfile;
                globalVar.PolicyResetPassword = lsResetPwd;

                globalVar.gsContainerLocation = ljeAppConfig[0].GetProperty("containerURL").ToString();
                globalVar.gsLogoContainer = ljeAppConfig[0].GetProperty("applicationContainerFolder").ToString();

                globalVar.gsEmbedAutenticationMode = ljeAppConfig[0].GetProperty("embedAutenticationMode").ToString();
                globalVar.gsEmbedTenantId = ljeAppConfig[0].GetProperty("embedTenantId").ToString();
                globalVar.gsEmbedClientId = ljeAppConfig[0].GetProperty("embedClientId").ToString();
                globalVar.gsEmbedClientSecret = ljeAppConfig[0].GetProperty("embedClientSecret").ToString();
                globalVar.gsEmbedScope = ljeAppConfig[0].GetProperty("embedScope").ToString();
                globalVar.gsEmbedAuthority = ljeAppConfig[0].GetProperty("embedAuthority").ToString();
                globalVar.gsEmbedApiDomain = ljeAppConfig[0].GetProperty("embedApiDomain").ToString();
                globalVar.gsEmbedResourceGroup = ljeAppConfig[0].GetProperty("embedResourceGroup").ToString();
                globalVar.gsEmbedResource = ljeAppConfig[0].GetProperty("embedResource").ToString();
                globalVar.gsEmbedResourceSubscriptionId = ljeAppConfig[0].GetProperty("embedResourceSubscriptionId").ToString();

                globalVar.giCheckEmbed = int.Parse(ljeAppConfig[0].GetProperty("checkEmbed").ToString());
            }
            else
            {

            }
            return "";
        }

        private void Print(int aiLine, string asStr = "")
        {
            try
            {
                File.AppendAllText(@"D:\TimeLog.txt", $"{aiLine} - {DateTime.Now}{(asStr == "" ? "" : " - " + asStr)}{Environment.NewLine}");
            }
            catch
            {

            }
        }

        public static string GetPublicIPAddressAsync()
        {
            string publicIP = "Unable to determine public IP";
            try
            {
                String address = "";
                WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
                using (WebResponse response = request.GetResponse())
                using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                {
                    address = stream.ReadToEnd();
                }

                int first = address.IndexOf("Address: ") + 9;
                int last = address.LastIndexOf("</body>");
                publicIP = address.Substring(first, last - first);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            globalVar.gsSystemIp = publicIP.Trim();
            return publicIP.Trim(); // Ensure no leading or trailing whitespaces
        }
    }
}