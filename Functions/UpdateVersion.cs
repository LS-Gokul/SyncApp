using LSSyncApp.Models;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text.Json;

namespace LSSyncApp.Functions
{
    public class UpdateVersion
    {
        public static string isSqlQuery, isReturn, isLogFileName;
        public static GlobalVariable uGloblaVar = new GlobalVariable();
        public static AboutPage _AboutPage = new AboutPage();
        public int iiSuccess;

        public AboutPage updateVersion(int aiType, GlobalVariable agGloblaVar)
        {
            uGloblaVar = agGloblaVar;
            _AboutPage.Version = 0.0;
            _AboutPage.ReleaseDate = "-";
            _AboutPage.ReleaseNotes = "-";
            _AboutPage.UpdatedOn = "-";
            switch (aiType)
            {
                case 1://Finding Current Version
                    /*versionTable();
                    if(_AboutPage.Status == "Failed")
                    {
                        break;
                    }*/
                    currentVersion();
                    break;
                case 2://Finding Latest Version
                    latestVersion();
                    break;
                default:
                    break;
            }
            return _AboutPage;
        }

        private void currentVersion()
        {
            try
            {
                uGloblaVar._DestinationConfig.ExecuteRawQuery(uGloblaVar, "", 2, out iiSuccess, out isReturn);
                if(iiSuccess == 0)
                {
                    _AboutPage.Status = "Failed";
                    _AboutPage.Message = isReturn;
                    return;
                }

                uGloblaVar._DestinationConfig.CheckCurrentVersion(uGloblaVar, out iiSuccess, out isReturn);
                if (iiSuccess == 1)
                {
                    DataTable ver = uGloblaVar.GetJSONToDataTable(isReturn);
                    _AboutPage.Status = "Success";
                    _AboutPage.Message = "";
                    _AboutPage.Version = double.Parse(ver.Rows[0].ItemArray[0].ToString());
                    _AboutPage.ReleaseDate = ver.Rows[0].ItemArray[1].ToString();
                    _AboutPage.UpdatedOn = ver.Rows[0].ItemArray[2].ToString();
                    _AboutPage.ReleaseNotes = releseNotes(ver.Rows[0].ItemArray[0].ToString());
                }
                else
                {
                    _AboutPage.Status = "Success";
                    _AboutPage.Message = "No Rows found";
                }
                return;
            }
            catch (Exception Ex)
            {
                _AboutPage.Status = "Failed";
                _AboutPage.Message = Ex.Message;
                return;
            }

        }

        private void latestVersion()
        {
            try
            {
                uGloblaVar._MasterConfig.CheckMasterVersionTable(uGloblaVar, 1, out iiSuccess, out isReturn);
                if (int.TryParse(isReturn, out _) && iiSuccess == 1)
                {
                    if (int.Parse(isReturn) <= 0)
                    {
                        _AboutPage.Status = "Success";
                        _AboutPage.Message = "";
                        _AboutPage.Version = 0.0;
                        _AboutPage.ReleaseNotes = "";
                    }
                    else
                    {
                        uGloblaVar._MasterConfig.CheckMasterVersionTable(uGloblaVar, 2, out iiSuccess, out isReturn);
                        if (iiSuccess == 1 && isReturn != "" && isReturn != null)
                        {
                            JsonElement ljeVer = new JsonElement();
                            ljeVer = uGloblaVar.createJsonElement(isReturn);
                            if(ljeVer.EnumerateArray().Count() >  0)
                            {
                                _AboutPage.Status = "Success";
                                _AboutPage.Message = "";
                                _AboutPage.Version = double.Parse(ljeVer[0].GetProperty("Ver").ToString());
                                _AboutPage.ReleaseDate = ljeVer[0].GetProperty("relDate").ToString();
                                _AboutPage.ReleaseNotes = ljeVer[0].GetProperty("relNotes").ToString();
                            }
                            else
                            {
                                _AboutPage.Status = "Success";
                                _AboutPage.Message = "No Rows found";
                            }
                        }
                        else if(iiSuccess == 0)
                        {
                            _AboutPage.Status = "Failed";
                            _AboutPage.Message = isReturn;
                        }
                        else
                        {
                            _AboutPage.Status = "Success";
                            _AboutPage.Message = "No Rows found";
                        }
                        return;
                    }
                }
                else
                {
                    _AboutPage.Status = "Failed";
                    _AboutPage.Message = isReturn;
                    return;
                }
                
                return;
            }
            catch(Exception Ex)
            {
                _AboutPage.Status = "Failed";
                _AboutPage.Message = Ex.Message;
                return;
            }
        }

        private string releseNotes(string asVer)
        {
            isReturn = "";
            try
            {
                uGloblaVar._MasterConfig.CheckMasterVersionTable(uGloblaVar, 1, out iiSuccess, out isReturn);
                if (int.TryParse(isReturn, out _) && iiSuccess == 1)
                {
                    if (int.Parse(isReturn) > 0)
                    {
                        uGloblaVar._MasterConfig.CheckMasterVersionTable(uGloblaVar, 3, out iiSuccess, out isReturn, asVer);
                        if (iiSuccess == 1)
                        {
                            return isReturn;
                        }
                    }
                }
            }
            catch
            {

            }
            return isReturn;
        }

        public AboutPage updateVersion(string currentVersion, int aiCmd, string latestVersion, IProgress<int> progress = null)
        {
            _AboutPage.Status = "Failed";
            _AboutPage.Message = "-";
            _AboutPage.ReleaseDate = "";
            _AboutPage.ReleaseNotes = "";
            _AboutPage.Restart = 0;
            _AboutPage.UpdatedOn = "";
            _AboutPage.Url = "";
            _AboutPage.Version = 0.00; 
            
            if(aiCmd == 0)
            {
                DialogResult res = MessageBox.Show("Latest Version is now Avaliable..\r\n\r\nCurrent Version : " + currentVersion
                                + "\r\nLatest Version: " + latestVersion + "\r\n\r\nDo you want to update", "Version", MessageBoxButtons.YesNo);

                if (res == DialogResult.No)
                {
                    _AboutPage.Status = "-";
                    return _AboutPage;
                }
            }
            isLogFileName = uGloblaVar.firmCode + "_" + uGloblaVar.gsLogFileName;
            try
            {
                if (aiCmd == 1)
                {
                    if (progress != null) progress.Report(20);
                }
                //Ver,releasedDate,verScript,appUrl

                uGloblaVar._MasterConfig.CheckMasterVersionTable(uGloblaVar, 4, out iiSuccess, out isReturn, currentVersion);

                if(iiSuccess == 1 && isReturn != "" && isReturn != null)
                {
                    string[] lsVersion = { }, lsReleaseDate = { }, lsVersionScript = { }, lsAppUrl = { };
                    

                    JsonElement ljeVersion = new JsonElement();
                    ljeVersion = uGloblaVar.createJsonElement(isReturn);

                    int rowCount = ljeVersion.EnumerateArray().Count();

                    lsVersion = new string[rowCount];
                    lsReleaseDate = new string[rowCount];
                    lsVersionScript = new string[rowCount];
                    lsAppUrl = new string[rowCount];

                    for (int ver = 0; ver < rowCount; ver++)
                    {
                        lsVersion[ver] = ljeVersion[ver].GetProperty("Ver").ToString();
                        lsReleaseDate[ver] = ljeVersion[ver].GetProperty("releasedDate").ToString();
                        lsVersionScript[ver] = ljeVersion[ver].GetProperty("verScript").ToString();
                        lsAppUrl[ver] = ljeVersion[ver].GetProperty("appUrl").ToString();
                    }

                    if (aiCmd == 1)
                    {
                        if (progress != null) progress.Report(30);
                    }

                    uGloblaVar._DestinationConfig.ExecuteRawQuery(uGloblaVar, "", 2, out iiSuccess, out isReturn);
                    if (iiSuccess == 0)
                    {
                        _AboutPage.Status = "Failed";
                        _AboutPage.Message = isReturn;
                    }
                    else
                    {
                        if (aiCmd == 1)
                        {
                            if (progress != null) progress.Report(40);
                        }

                        int liProgress = 50 / rowCount;
                        _AboutPage.Status = "Success";
                        _AboutPage.Message = "";
                        for (int ver = 0; ver < rowCount; ver++)
                        {
                            if (lsVersionScript[ver] != "-")
                            {
                                isSqlQuery = lsVersionScript[ver];

                                uGloblaVar._DestinationConfig.ExecuteRawQuery(uGloblaVar, isSqlQuery, 1, out iiSuccess, out isReturn);
                                if (iiSuccess == 0)
                                {
                                    _AboutPage.Status = "Failed";
                                    _AboutPage.Message = isReturn;
                                    break;
                                }
                            }
                            _AboutPage.Version = double.Parse(lsVersion[ver]);
                            if (lsAppUrl[ver] != "-" && lsAppUrl[ver].Contains("http"))
                            {
                                _AboutPage.Url = lsAppUrl[ver];
                                _AboutPage.Restart = 1;
                            }
                            isSqlQuery = "Insert Into LS_Version(application_code, Version, software_code, [Released Date], [Date of Updation])"
                                + $"Values('{uGloblaVar.gsAppCode}',{lsVersion[ver]},'{uGloblaVar.gsSoftwareCode}','{lsReleaseDate[ver]}',GetDate());";
                            uGloblaVar._DestinationConfig.ExecuteRawQuery(uGloblaVar, isSqlQuery, 1, out iiSuccess, out isReturn);
                            if (iiSuccess == 0)
                            {
                                _AboutPage.Status = "Failed";
                                _AboutPage.Message = isReturn;
                                break;
                            }
                            if (aiCmd == 1)
                            {
                                if (progress != null) progress.Report(40 + (liProgress * ver));
                            }
                        }
                        if (aiCmd == 1)
                        {
                            if (progress != null) progress.Report(90);
                        }
                    }
                }
                if (_AboutPage.Status == "Failed")
                {
                    uGloblaVar.setMessageLog(isLogFileName, _AboutPage.Message, aiCmd);
                }
                else
                {
                    uGloblaVar.setMessageLog(isLogFileName, "Version Updated Successfully", aiCmd);
                    if (aiCmd == 0)
                    {
                        if (_AboutPage.Restart == 1)
                        {
                            MessageBox.Show("Please Re-Open the Application", "Restart");
                            Application.Exit();
                        }
                    }
                    else
                    {
                        Application.Exit();
                    }
                }
                if (aiCmd == 1)
                {
                    if (progress != null) progress.Report(100);
                }
                return _AboutPage;
            }
            catch(Exception Ex)
            {
                uGloblaVar.setMessageLog(isLogFileName, Ex.Message, aiCmd);
                return _AboutPage;
            }
        }

        public AboutPage appUpdate(string asUrl)
        {
            _AboutPage.Status = "Success";
            _AboutPage.Message = "-";
            _AboutPage.ReleaseDate = "";
            _AboutPage.ReleaseNotes = "";
            _AboutPage.Restart = 0;
            _AboutPage.UpdatedOn = "";
            _AboutPage.Url = "";
            _AboutPage.Version = 0.00;

            if (uGloblaVar.gsUpdateStatus == "Failed")
            {
                _AboutPage.Status = "Failed";
                _AboutPage.Message = uGloblaVar.gsUpdateMessage;
            }
            else
            {
                /////////////////////////////File Download & Extract - Start//////////////////////////////////
                try
                {
                    string lsAppUrlLatest = asUrl, lsCmdParms = "";
                    MasterConfig masterConfig = new MasterConfig();
                    masterConfig.GetAppLatstVersion(uGloblaVar, out iiSuccess, out lsAppUrlLatest);
                    if (iiSuccess == 1 && lsAppUrlLatest.Contains("https://"))
                    {
                        JsonElement ljeAppUrl = uGloblaVar.createJsonElement(lsAppUrlLatest);
                        lsAppUrlLatest = ljeAppUrl[0].GetProperty("appUrl").ToString();
                        try
                        {
                            for (int i = 0; i < uGloblaVar.commandParams.Count(); i++)
                            {
                                lsCmdParms = " " + uGloblaVar.commandParams[i];
                            }
                            try
                            {
                                int liProcessCount = Process.GetProcessesByName(uGloblaVar.gsProcessName).Length;
                                if (liProcessCount > 1)
                                {
                                    for (int i = 0; i < liProcessCount; i++)
                                    {
                                        if ((Process.GetProcessesByName(uGloblaVar.gsProcessName))[i].Id.ToString() != uGloblaVar.gsProcessId)
                                        {
                                            (Process.GetProcessesByName(uGloblaVar.gsProcessName))[i].Kill();
                                        }
                                    }
                                }
                            }
                            catch
                            {

                            }
                            Process.Start(uGloblaVar.gsApplPath + "Updates.exe", lsAppUrlLatest + " " + uGloblaVar.giCmd.ToString() + lsCmdParms);
                            Application.Exit();
                        }
                        catch (Exception Ex)
                        {
                            _AboutPage.Status = "Failed";
                            _AboutPage.Message = Ex.Message;
                        }
                    }
                }
                catch (Exception dEx)
                {
                    _AboutPage.Status = "Failed";
                    _AboutPage.Message = dEx.Message;
                }
                /////////////////////////////File Download & Extract - End//////////////////////////////////
            }
            return _AboutPage;
        }

        public string getUpdateURL()
        {
            MasterConfig masterConfig = new MasterConfig();
            masterConfig.GetAppLatstVersion(uGloblaVar, out iiSuccess, out string lsAppUrlLatest);
            if(lsAppUrlLatest.Contains("{"))
            {
                JsonElement ljeAppUrl = uGloblaVar.createJsonElement(lsAppUrlLatest);
                lsAppUrlLatest = ljeAppUrl[0].GetProperty("appUrl").ToString();
            }
            return lsAppUrlLatest;
        }


        public void versionCheck(GlobalVariable mdiGlobalVar)
        {
            AboutPage abt = new AboutPage();
            double ldCurrent, ldLatest;

            abt = updateVersion(1, mdiGlobalVar);
            if (abt.Status != "Failed")
            {
                ldCurrent = abt.Version;
                mdiGlobalVar.gsVersion = ldCurrent.ToString("0.00");
                abt = updateVersion(2, mdiGlobalVar);
                if (abt.Status != "Failed")
                {
                    ldLatest = abt.Version;
                    if (ldLatest > ldCurrent)
                    {
                        abt = updateVersion(ldCurrent.ToString("0.00"), mdiGlobalVar.giCmd, ldLatest.ToString("0.00"));
                        if (abt.Status != "-")
                        {
                            mdiGlobalVar.gsVersion = abt.Version.ToString("0.00");
                        }
                    }
                }
            }

            if (mdiGlobalVar.giCmd == 0)
            {
                string lsUrl = getUpdateURL();
                if (lsUrl.Contains("https://"))
                {
                    DialogResult res = MessageBox.Show("Latest Version of App is available\r\n\r\nDo you want to update", "Version", MessageBoxButtons.YesNo);
                    if (res == DialogResult.Yes)
                        appUpdate(lsUrl);
                }
            }
        }
    }
}
