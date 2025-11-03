using Microsoft.Win32;
using System;
using System.Drawing;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace LSSyncApp.Functions
{
    public class GlobalFunctions
    {
        public static string isReturn, isQuery;
        private int iiSuccess;

        public void fetchTable(GlobalVariable gbl, string tblCode, out string fetchTable, out string status, out string message)
        {
            try
            {
                fetchTable = "";
                status = "Failed";
                message = "";
                gbl._MasterConfig.GetFetchTableQuery(gbl, tblCode, out int liSuccess, out isReturn);

                if (liSuccess == 0)
                {
                    message = isReturn;
                }
                fetchTable = isReturn;
                status = isReturn == "Fetch Query not found" ? "Failed" : "Success";
            }
            catch (Exception Ex)
            {
                fetchTable = "";
                status = "Failed";
                message = Ex.Message;
                return;
            }
        }

        //////////////////////////////////////////////////////////////////////////////
        //Customer Database Configuration
        //////////////////////////////////////////////////////////////////////////////
        public string custDBConfig(GlobalVariable _GlobalVar, string custCode, string firmCode, string asEmailID)
        {
            int iiSuccess, liDestDBAuthMode = 0;
            string lsCustServerName = "", lsCustUID = "", lsCustPwd = "", lsCustElasticPool = "", lsCustDbName = "", lsReturn;

            _GlobalVar._MasterConfig.GetFirmConfig(_GlobalVar, out iiSuccess, out _GlobalVar.gsSoftwareCode);

            if (iiSuccess == 1 && _GlobalVar.gsSoftwareCode != null && _GlobalVar.gsSoftwareCode != "")
            {
                JsonElement ljeConfig = _GlobalVar.createJsonElement(_GlobalVar.gsSoftwareCode);
                _GlobalVar.gsSoftwareName = ljeConfig[0].GetProperty("stName").ToString();
                _GlobalVar.gsSoftwareCode = ljeConfig[0].GetProperty("stCode").ToString();
                _GlobalVar.maxDaysToSync = int.Parse(ljeConfig[0].GetProperty("syncDays").ToString());
                _GlobalVar.gsReportCount = ljeConfig[0].GetProperty("Cnt").ToString();
                _GlobalVar.gsSyncType = ljeConfig[0].GetProperty("syncName").ToString();
                _GlobalVar.gsProfileId = ljeConfig[0].GetProperty("profileId").ToString();
                _GlobalVar.gsWorkspaseId = ljeConfig[0].GetProperty("workspaseId").ToString();
            }

            _GlobalVar._MasterConfig.GetCustomerCustomDB(_GlobalVar, out iiSuccess, out lsReturn);
            if (iiSuccess == 1)
            {
                JsonElement ljeDestDB = _GlobalVar.createJsonElement(lsReturn);
                if (ljeDestDB.EnumerateArray().Count() > 0)
                {
                    lsCustServerName = ljeDestDB[0].GetProperty("serverName").ToString();
                    lsCustUID = ljeDestDB[0].GetProperty("userID").ToString();
                    lsCustPwd = ljeDestDB[0].GetProperty("pwd").ToString();
                    lsCustElasticPool = ljeDestDB[0].GetProperty("grp").ToString();
                    lsCustDbName = ljeDestDB[0].GetProperty("dbName").ToString();
                    liDestDBAuthMode = 1;
                }
            }
            else
            {
                liDestDBAuthMode = 0;
                lsCustServerName = _GlobalVar.masterServerName;
                lsCustUID = _GlobalVar.masterUID;
                lsCustPwd = _GlobalVar.masterPwd;
                lsCustElasticPool = _GlobalVar.masterElasticPool;
                lsCustDbName = custCode + "_" + firmCode + _GlobalVar.staging;
                if (_GlobalVar.gsSoftwareName == "Tally")
                {
                    lsCustDbName = custCode + _GlobalVar.staging;
                }
            }
            _GlobalVar.custServerName = lsCustServerName;
            _GlobalVar.custUID = lsCustUID;
            _GlobalVar.custPwd = lsCustPwd;
            _GlobalVar.custElasticPool = lsCustElasticPool;
            _GlobalVar.custDbName = lsCustDbName;
            _GlobalVar.gsUserId = asEmailID;
            _GlobalVar.giCustDestDBAuth = liDestDBAuthMode;
            return "";
        }

        public string SetSettingsVariable(GlobalVariable _GlobalVar, string asJson)
        {
            JsonElement ljeUserDetails = _GlobalVar.createJsonElement(asJson);
            //Cust Code
            _GlobalVar.custCode = ljeUserDetails[0].GetProperty("lsCode").ToString();

            //User Role Id
            _GlobalVar.gsUserRoleCode = ljeUserDetails[0].GetProperty("userGrpCode").ToString();

            //Email ID
            string lsEmailID = ljeUserDetails[0].GetProperty("email").ToString();
            _GlobalVar.gsAADUserId = lsEmailID;

            //User Name
            _GlobalVar.gsUserName = ljeUserDetails[0].GetProperty("userName").ToString();

            //Customer Name
            _GlobalVar.custName = ljeUserDetails[0].GetProperty("custName").ToString();

            //User Role Name
            _GlobalVar.gsUserRoleName = ljeUserDetails[0].GetProperty("userGrpName").ToString();

            //Firm Name
            _GlobalVar.firmName = ljeUserDetails[0].GetProperty("firmName").ToString();

            //Set API
            _GlobalVar.giApi = int.Parse(ljeUserDetails[0].GetProperty("isApi").ToString());

            //Set User Code
            _GlobalVar.gsUserCode = ljeUserDetails[0].GetProperty("userCode").ToString();

            //Set Customer Application Code
            _GlobalVar.gsCustAppCode = ljeUserDetails[0].GetProperty("custAppCode").ToString();

            Triggers(_GlobalVar);

            return lsEmailID;
        }


        public void Triggers(GlobalVariable _GlobalVar)
        {
            //Insert Notifications / Triggers from General to Particular LSCode & FirmCode
            _GlobalVar._MasterConfig.InsertTriggers(_GlobalVar, out _, out _);

            //Fetch Notifications / Triggers from General to Particular LSCode & FirmCode
            _GlobalVar._MasterConfig.TriggerNotification(_GlobalVar, "0,1,2,3,4,5,6,7,8,9,10", out _, out _GlobalVar.gsNotificationList);
        }

        public void UpdateDBStats(GlobalVariable _GlobalVar, out int aiSuccess, out string asMessage)
        {
            try
            {
                long llMasters = 0, llTransactions = 0, llOthers = 0, llRows = 0;
                double ldDBSize = 0;

                string lsType, lsSize, lsRows, lsTable, lsDetailQuery = "", lsQuery = "";
                _GlobalVar._DestinationConfig.CheckTableListSize(_GlobalVar, out iiSuccess, out isReturn, "-1");
                
                aiSuccess = 1;
                asMessage = "Success";

                if (iiSuccess == 1)
                {
                    JsonElement ljeSizeList = _GlobalVar.createJsonElement(isReturn);
                    int liCount = ljeSizeList.EnumerateArray().Count();
                    for (int i = 0; i < liCount; i++)
                    {
                        lsType = ljeSizeList[i].GetProperty("tblType").ToString();
                        lsRows = ljeSizeList[i].GetProperty("rows").ToString();
                        lsSize = ljeSizeList[i].GetProperty("size").ToString();
                        lsTable = ljeSizeList[i].GetProperty("objName").ToString();

                        lsDetailQuery += (i == 0 ? "" : " Union All ") + $"Select '{lsTable}' as tbl,{lsSize} as tblSize,{lsRows} as tblRows,'"
                            + (lsType == "1" ? "Master" : lsType == "4" ? "Transaction" : "Others") + "' as tblType" + Environment.NewLine;

                        switch (lsType)
                        {
                            case "1":
                                llMasters += long.Parse(lsRows);
                                break;
                            case "2":
                            case "3":
                                llOthers += long.Parse(lsRows);
                                break;
                            case "4":
                                llTransactions += long.Parse(lsRows);
                                break;
                        }
                        llRows += long.Parse(lsRows);
                        ldDBSize += double.Parse(lsSize);
                    }
                    if (liCount > 0)
                    {
                        lsDetailQuery = "Merge Into LS_Customer_Database_Stats_Detail With(HoldLock) as a " + Environment.NewLine
                            + $" Using({lsDetailQuery}) as b " + Environment.NewLine
                            + $" On a.[LSCode] = '{_GlobalVar.custCode}' And a.[firm_code] = '{_GlobalVar.firmCode}' And a.[Table Name] = b.tbl " + Environment.NewLine
                            + " When Matched Then Update Set a.[Table Size] = b.tblSize, a.[Table Rows] = b.tblRows, a.[Table Category] = b.tblType, " + Environment.NewLine
                            + "                              a.[Modified User] = 'SyncApp', a.[Date Of Modification] = GetDate() " + Environment.NewLine
                            + " When Not Matched Then Insert([LSCode],[firm_code],[Table Name],[Table Size],[Table Rows],[Table Category]," + Environment.NewLine
                            + "                              [Created User],[Date Of Creation],[Modified User],[Date Of Modification]) " + Environment.NewLine
                            + $"                       Values('{_GlobalVar.custCode}', '{_GlobalVar.firmCode}', b.tbl, b.tblSize, b.tblRows," + Environment.NewLine
                            + "                               b.tblType, 'SyncApp', GetDate(), 'SyncApp', GetDate());";

                        _GlobalVar._MasterConfig.ExecuteRawQuery(_GlobalVar, lsDetailQuery, 1, out iiSuccess, out isReturn, 0, 2);
                        if (iiSuccess == 0)
                        {
                            aiSuccess = 0;
                            asMessage = isReturn;
                        }
                        else
                        {
                            lsQuery = "Merge Into LS_Customer_Database_Stats With(HoldLock) as a " + Environment.NewLine
                                + $" Using(Select '{_GlobalVar.custCode}' as custCode, '{_GlobalVar.firmCode}' as firmCode, " + Environment.NewLine
                                + $"            '{_GlobalVar.custDbName}' as dbName, {ldDBSize} as dbSize, {llRows} as dbRows, " + Environment.NewLine
                                + $"            {llMasters} as mstRows, {llTransactions} as tranRows, {llOthers} as otherRows ) as b " + Environment.NewLine
                                + $" On a.[LSCode] = b.custCode And a.[firm_code] = b.firmCode " + Environment.NewLine
                                + " When Matched Then Update Set a.[Database Name] = b.dbName, a.[Database Size] = b.dbSize, a.[Database Rows] = b.dbRows, " + Environment.NewLine
                                + "                              a.[Master Table Rows] = b.mstRows, a.[Transaction Table Rows] = b.tranRows, " + Environment.NewLine
                                + "                              a.[Other Table Rows] = b.otherRows, " + Environment.NewLine
                                + "                              a.[Modified User] = 'SyncApp', a.[Date Of Modification] = GetDate() " + Environment.NewLine
                                + " When Not Matched Then Insert([LSCode],[firm_code],[Database Name],[Database Size],[Database Rows]," + Environment.NewLine
                                + "                              [Master Table Rows],[Transaction Table Rows],[Other Table Rows]," + Environment.NewLine
                                + "                              [Created User],[Date Of Creation],[Modified User],[Date Of Modification]) " + Environment.NewLine
                                + "                       Values(b.custCode, b.firmCode, b.dbName, b.dbSize, b.dbRows, " + Environment.NewLine
                                + "                              b.mstRows, b.tranRows, b.otherRows, 'SyncApp', GetDate(), 'SyncApp', GetDate());";
                            _GlobalVar._MasterConfig.ExecuteRawQuery(_GlobalVar, lsQuery, 1, out iiSuccess, out isReturn, 0, 2);
                            if (iiSuccess == 0)
                            {
                                aiSuccess = 0;
                                asMessage = isReturn;
                            }
                            else
                            {
                                lsQuery = "Merge Into LS_Customer_Database_Stats_Daywise With(HoldLock) as a " + Environment.NewLine
                                    + $" Using(Select '{_GlobalVar.custCode}' as custCode, '{_GlobalVar.firmCode}' as firmCode,'{DateTime.Now.ToString("yyyy-MM-dd")}' as tDate, " + Environment.NewLine
                                    + $"            '{_GlobalVar.custDbName}' as dbName, {ldDBSize} as dbSize, {llRows} as dbRows, " + Environment.NewLine
                                    + $"            {llMasters} as mstRows, {llTransactions} as tranRows, {llOthers} as otherRows ) as b " + Environment.NewLine
                                    + $" On a.[LSCode] = b.custCode And a.[firm_code] = b.firmCode And a.[Date] = b.tDate " + Environment.NewLine
                                    + " When Matched Then Update Set a.[Database Name] = b.dbName, a.[Database Size] = b.dbSize, a.[Database Rows] = b.dbRows, " + Environment.NewLine
                                    + "                              a.[Master Table Rows] = b.mstRows, a.[Transaction Table Rows] = b.tranRows, " + Environment.NewLine
                                    + "                              a.[Other Table Rows] = b.otherRows, " + Environment.NewLine
                                    + "                              a.[Modified User] = 'SyncApp', a.[Date Of Modification] = GetDate() " + Environment.NewLine
                                    + " When Not Matched Then Insert([LSCode],[firm_code],[Date],[Database Name],[Database Size],[Database Rows]," + Environment.NewLine
                                    + "                              [Master Table Rows],[Transaction Table Rows],[Other Table Rows]," + Environment.NewLine
                                    + "                              [Created User],[Date Of Creation],[Modified User],[Date Of Modification]) " + Environment.NewLine
                                    + "                       Values(b.custCode, b.firmCode, b.tDate, b.dbName, b.dbSize, b.dbRows, " + Environment.NewLine
                                    + "                              b.mstRows, b.tranRows, b.otherRows, 'SyncApp', GetDate(), 'SyncApp', GetDate());";
                                _GlobalVar._MasterConfig.ExecuteRawQuery(_GlobalVar, lsQuery, 1, out iiSuccess, out isReturn, 0, 2);
                                if (iiSuccess == 0)
                                {
                                    aiSuccess = 0;
                                    asMessage = isReturn;
                                }
                            }
                        }
                    }
                }
                else
                {
                    aiSuccess = 0;
                    asMessage = isReturn;
                }
            }
            catch(Exception Ex)
            {
                aiSuccess = 0;
                asMessage = Ex.Message;
            }
        }

        public void Loader(bool abVisible, WebBrowser _Wb, string asPath = null, int aiWidth = 0, int aiHeight = 0, int aiX = 0, int aiY = 0)
        {
            try
            {
                if (abVisible == true)
                {
                    _Wb.Location = new Point(aiX, aiY);
                    _Wb.Size = new Size(aiWidth, aiHeight);
                    _Wb.Url = new Uri(String.Format("file:///{0}Loader.html", asPath));
                    _Wb.Visible = true;
                    _Wb.BringToFront();
                }
                else
                {
                    _Wb.Visible = false;
                }
            }
            catch
            {

            }
        }

        public int CheckDBExists(GlobalVariable _GblVariable, out int aiSuccess, out string asMessage)
        {
            string lsSqlQuery, lsReturn;
            int liCount = 0;
            aiSuccess = 0;
            try
            {
                lsSqlQuery = "SELECT COUNT(*) FROM master.dbo.sysdatabases where name = '" + _GblVariable.custDbName + "';";
                _GblVariable._DestinationConfig.ExecuteRawQuery(_GblVariable, lsSqlQuery, 0, out iiSuccess, out lsReturn, 0, 1);
                if(iiSuccess == 0)
                {
                    asMessage = lsReturn;
                }
                else
                {
                    asMessage = lsReturn;
                    if (int.TryParse(lsReturn,out _))
                    {
                        liCount = int.Parse(lsReturn);
                        aiSuccess = 1;
                        asMessage = "Success"; 
                    }
                    
                }
            }
            catch(Exception Ex)
            {
                asMessage = Ex.Message;
            }
            return liCount;
        }

        public void CreateDB(GlobalVariable _GblVariable, out int aiSuccess, out string asMessage)
        {
            string lsSqlQuery, lsReturn;

            try
            {
                if (_GblVariable.custElasticPool == null || _GblVariable.custElasticPool == "" || _GblVariable.custElasticPool == "None")
                {
                    lsSqlQuery = "CREATE DATABASE " + _GblVariable.custDbName + ";";
                }
                else
                {
                    lsSqlQuery = "CREATE DATABASE " + _GblVariable.custDbName + "(SERVICE_OBJECTIVE = ELASTIC_POOL(name = \"" + _GblVariable.custElasticPool + "\"));";
                }

                _GblVariable._DestinationConfig.ExecuteRawQuery(_GblVariable, lsSqlQuery, 1, out iiSuccess, out lsReturn, 0, 1);
                if (lsReturn.Contains("|Failed"))
                {
                    aiSuccess = 0;
                    asMessage = "Customer DB Creation Failed. DataBase Name : " + _GblVariable.custDbName;
                    return;
                }

                _GblVariable._DestinationConfig.ExecuteRawQuery(_GblVariable, "", 2, out iiSuccess, out lsReturn, 0);
                if (iiSuccess == 0)
                {
                    aiSuccess = 0;
                    asMessage = lsReturn;
                    return;
                }

                //Registry.SetValue(_GblVariable.regPath, "CC", _GblVariable.custCode);
                //Registry.SetValue(_GblVariable.regPath, "FC", _GblVariable.firmCode);
                //Registry.SetValue(_GblVariable.regPath, "UE", _GblVariable.gsUserId);

                aiSuccess = 1;
                asMessage = "Success";
            }
            catch(Exception Ex)
            {
                aiSuccess = 0;
                asMessage = Ex.Message;
            }
        }

        public void Scheduler(out int aiSuccess, out string asMessage)
        {

            /*
            0 - Not Yet Started
            1 - Running
            2 - Stopped
            3 - Completed
            */
            aiSuccess = 1;
            asMessage = "Success";
        }

        public void Templates(int aiType, string asAppCode, out int aiSuccess, out string asMessage)
        {
            switch(aiType)
            {
                case 1:
                    aiSuccess = 1;
                    asMessage = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional //EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">" + Environment.NewLine
                        + "<html xmlns=\"http://www.w3.org/1999/xhtml\" xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\">" + Environment.NewLine
                        + "<head>" + Environment.NewLine
                        + "<!--[if gte mso 9]>" + Environment.NewLine
                        + "<xml>" + Environment.NewLine
                        + "  <o:OfficeDocumentSettings>" + Environment.NewLine
                        + "    <o:AllowPNG/>" + Environment.NewLine
                        + "    <o:PixelsPerInch>96</o:PixelsPerInch>" + Environment.NewLine
                        + "  </o:OfficeDocumentSettings>" + Environment.NewLine
                        + "</xml>" + Environment.NewLine
                        + "<![endif]-->" + Environment.NewLine
                        + "  <meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\">" + Environment.NewLine
                        + "  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">" + Environment.NewLine
                        + "  <meta name=\"x-apple-disable-message-reformatting\">" + Environment.NewLine
                        + "  <!--[if !mso]><!--><meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\"><!--<![endif]-->" + Environment.NewLine
                        + "  <title></title>" + Environment.NewLine
                        + "  " + Environment.NewLine
                        + "    <style type=\"text/css\">" + Environment.NewLine
                        + "      @media only screen and (min-width: 620px) {" + Environment.NewLine
                        + "        .u-row {" + Environment.NewLine
                        + "            width: 600px !important;" + Environment.NewLine
                        + "        }" + Environment.NewLine
                        + "        .u-row .u-col {" + Environment.NewLine
                        + "            vertical-align: top;" + Environment.NewLine
                        + "        }" + Environment.NewLine
                        + "" + Environment.NewLine
                        + "        .u-row .u-col-18 {" + Environment.NewLine
                        + "            width: 108px !important;" + Environment.NewLine
                        + "        }" + Environment.NewLine
                        + "" + Environment.NewLine
                        + "        .u-row .u-col-18p34 {" + Environment.NewLine
                        + "            width: 110.04px !important;" + Environment.NewLine
                        + "        }" + Environment.NewLine
                        + "" + Environment.NewLine
                        + "        .u-row .u-col-63p66 {" + Environment.NewLine
                        + "            width: 381.96px !important;" + Environment.NewLine
                        + "        }" + Environment.NewLine
                        + "" + Environment.NewLine
                        + "        .u-row .u-col-100 {" + Environment.NewLine
                        + "            width: 600px !important;" + Environment.NewLine
                        + "        }" + Environment.NewLine
                        + "" + Environment.NewLine
                        + "        }" + Environment.NewLine
                        + "" + Environment.NewLine
                        + "        @media (max-width: 620px) {" + Environment.NewLine
                        + "        .u-row-container {" + Environment.NewLine
                        + "            max-width: 100% !important;" + Environment.NewLine
                        + "            padding-left: 0px !important;" + Environment.NewLine
                        + "            padding-right: 0px !important;" + Environment.NewLine
                        + "        }" + Environment.NewLine
                        + "        .u-row .u-col {" + Environment.NewLine
                        + "            min-width: 320px !important;" + Environment.NewLine
                        + "            max-width: 100% !important;" + Environment.NewLine
                        + "            display: block !important;" + Environment.NewLine
                        + "        }" + Environment.NewLine
                        + "        .u-row {" + Environment.NewLine
                        + "            width: calc(100% - 40px) !important;" + Environment.NewLine
                        + "        }" + Environment.NewLine
                        + "        .u-col {" + Environment.NewLine
                        + "            width: 100% !important;" + Environment.NewLine
                        + "        }" + Environment.NewLine
                        + "        .u-col > div {" + Environment.NewLine
                        + "            margin: 0 auto;" + Environment.NewLine
                        + "        }" + Environment.NewLine
                        + "        }" + Environment.NewLine
                        + "        body {" + Environment.NewLine
                        + "        margin: 0;" + Environment.NewLine
                        + "        padding: 0;" + Environment.NewLine
                        + "        }" + Environment.NewLine
                        + "" + Environment.NewLine
                        + "        table," + Environment.NewLine
                        + "        tr," + Environment.NewLine
                        + "        td {" + Environment.NewLine
                        + "        vertical-align: top;" + Environment.NewLine
                        + "        border-collapse: collapse;" + Environment.NewLine
                        + "        }" + Environment.NewLine
                        + "" + Environment.NewLine
                        + "        p {" + Environment.NewLine
                        + "        margin: 0;" + Environment.NewLine
                        + "        }" + Environment.NewLine
                        + "" + Environment.NewLine
                        + "        .ie-container table," + Environment.NewLine
                        + "        .mso-container table {" + Environment.NewLine
                        + "        table-layout: fixed;" + Environment.NewLine
                        + "        }" + Environment.NewLine
                        + "" + Environment.NewLine
                        + "        * {" + Environment.NewLine
                        + "        line-height: inherit;" + Environment.NewLine
                        + "        }" + Environment.NewLine
                        + "" + Environment.NewLine
                        + "        a[x-apple-data-detectors='true'] {" + Environment.NewLine
                        + "        color: inherit !important;" + Environment.NewLine
                        + "        text-decoration: none !important;" + Environment.NewLine
                        + "        }" + Environment.NewLine
                        + "" + Environment.NewLine
                        + "        table, td { color: #000000; } a { color: #cca250; text-decoration: none; } @media (max-width: 480px) { #u_content_image_4 .v-src-width { width: auto !important; } #u_content_image_4 .v-src-max-width { max-width: 57% !important; } #u_content_image_3 .v-container-padding-padding { padding: 46px 10px 10px !important; } #u_content_image_3 .v-src-width { width: auto !important; } #u_content_image_3 .v-src-max-width { max-width: 29% !important; } #u_content_heading_3 .v-container-padding-padding { padding: 10px 20px !important; } #u_content_heading_3 .v-font-size { font-size: 28px !important; } #u_content_text_3 .v-container-padding-padding { padding: 10px 22px 26px !important; } #u_content_heading_2 .v-container-padding-padding { padding: 22px 22px 10px !important; } #u_content_heading_2 .v-font-size { font-size: 24px !important; } }" + Environment.NewLine
                        + "    </style>" + Environment.NewLine
                        + "  " + Environment.NewLine
                        + "<!--[if !mso]><!--><link href=\"https://fonts.googleapis.com/css?family=Montserrat:400,700&display=swap\" rel=\"stylesheet\" type=\"text/css\"><!--<![endif]-->" + Environment.NewLine
                        + "" + Environment.NewLine
                        + "</head>" + Environment.NewLine
                        + "" + Environment.NewLine
                        + "<body class=\"clean-body u_body\" style=\"margin: 0;padding: 0;-webkit-text-size-adjust: 100%;background-color: #f9f9f9;color: #000000\">" + Environment.NewLine
                        + "  <!--[if IE]><div class=\"ie-container\"><![endif]-->" + Environment.NewLine
                        + "  <!--[if mso]><div class=\"mso-container\"><![endif]-->" + Environment.NewLine
                        + "  <table style=\"border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;min-width: 320px;Margin: 0 auto;background-color: #f9f9f9;width:100%\" cellpadding=\"0\" cellspacing=\"0\">" + Environment.NewLine
                        + "  <tbody>" + Environment.NewLine
                        + "  <tr style=\"vertical-align: top\">" + Environment.NewLine
                        + "    <td style=\"word-break: break-word;border-collapse: collapse !important;vertical-align: top\">" + Environment.NewLine
                        + "    <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td align=\"center\" style=\"background-color: #f9f9f9;\"><![endif]-->" + Environment.NewLine
                        + "    " + Environment.NewLine
                        + "" + Environment.NewLine
                        + "<div class=\"u-row-container\" style=\"padding: 0px;background-color: transparent\">" + Environment.NewLine
                        + "  <div class=\"u-row\" style=\"Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #111114;\">" + Environment.NewLine
                        + "    <div style=\"border-collapse: collapse;display: table;width: 100%;background-color: transparent;\">" + Environment.NewLine
                        + "      <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td style=\"padding: 0px;background-color: transparent;\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"width:600px;\"><tr style=\"background-color: #111114;\"><![endif]-->" + Environment.NewLine
                        + "      " + Environment.NewLine
                        + "<!--[if (mso)|(IE)]><td align=\"center\" width=\"600\" style=\"background-color: #ffffff;width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;\" valign=\"top\"><![endif]-->" + Environment.NewLine
                        + "<div class=\"u-col u-col-100\" style=\"max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;\">" + Environment.NewLine
                        + "  <div style=\"background-color: #ffffff; width: 100% !important;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;\">" + Environment.NewLine
                        + "  <!--[if (!mso)&(!IE)]><!--><div style=\"background-color: #ffffff; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;\"><!--<![endif]-->" + Environment.NewLine
                        + "  " + Environment.NewLine
                        + "<table id=\"u_content_image_4\" style=\"font-family:'Montserrat',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">" + Environment.NewLine
                        + "  <tbody>" + Environment.NewLine
                        + "    <tr>" + Environment.NewLine
                        + "      <td class=\"v-container-padding-padding\" style=\"background-color: #ffffff; overflow-wrap:break-word;word-break:break-word;padding:0;font-family:'Montserrat',sans-serif;\" align=\"left\">" + Environment.NewLine
                        + "        " + Environment.NewLine
                        + "<table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\">" + Environment.NewLine
                        + "  <tr>" + Environment.NewLine
                        + "    <td style=\"padding-right: 0px;padding-left: 0px;\" align=\"center\">" + Environment.NewLine
                        
                        + "        <a href=\"https://leapsurgebi.com\" target=\"_blank\">" + Environment.NewLine
                        + "        <img align=\"center\" border=\"0\" src=\"https://leapsurgebi.blob.core.windows.net/sync-application/EmailTemplates/Images/Company-logo.png\" alt=\"Leapsurge\" title=\"Leapsurge\" style=\"outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: inline-block !important;border: none;height: auto;float: none;width: 40%;min-width: 600px; min-height: 72px;\" width=\"600\" class=\"v-src-width v-src-max-width\"/>" + Environment.NewLine
                        
                        + "    </td>" + Environment.NewLine
                        + "  </tr>" + Environment.NewLine
                        + "</table>" + Environment.NewLine
                        + "" + Environment.NewLine
                        + "      </td>" + Environment.NewLine
                        + "    </tr>" + Environment.NewLine
                        + "  </tbody>" + Environment.NewLine
                        + "</table>" + Environment.NewLine
                        + "" + Environment.NewLine
                        + "  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->" + Environment.NewLine
                        + "  </div>" + Environment.NewLine
                        + "</div>" + Environment.NewLine
                        + "<!--[if (mso)|(IE)]></td><![endif]-->" + Environment.NewLine
                        + "      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->" + Environment.NewLine
                        + "    </div>" + Environment.NewLine
                        + "  </div>" + Environment.NewLine
                        + "</div>" + Environment.NewLine
                        + "" + Environment.NewLine
                        + "<div class=\"u-row-container\" style=\"padding: 0px;background-color: transparent\">" + Environment.NewLine
                        + "  <div class=\"u-row\" style=\"Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;\">" + Environment.NewLine
                        + "    <div style=\"border-collapse: collapse;display: table;width: 100%;background-color: transparent;\">" + Environment.NewLine
                        + "      <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td style=\"padding: 0px;background-color: transparent;\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"width:600px;\"><tr style=\"background-color: transparent;\"><![endif]-->" + Environment.NewLine
                        + "      " + Environment.NewLine
                        + "<!--[if (mso)|(IE)]><td align=\"center\" width=\"600\" style=\"background-color: #fffefe;width: 600px;padding: 0px;border-top: 1px solid #CCC;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 1px solid #CCC;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;\" valign=\"top\"><![endif]-->" + Environment.NewLine
                        + "<div class=\"u-col u-col-100\" style=\"background-color: #ffffff; max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;\">" + Environment.NewLine
                        + "  <div style=\"background-color: #ffffff; width: 100% !important;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;\">" + Environment.NewLine
                        + "  <!--[if (!mso)&(!IE)]><!--><div style=\"background-color: #ffffff; padding: 0px;border-top: 1px solid #CCC;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 1px solid #CCC;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;\"><!--<![endif]-->" + Environment.NewLine
                        + "  " + Environment.NewLine
                        + "<table id=\"u_content_text_3\" style=\"font-family:'Montserrat',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">" + Environment.NewLine
                        + "  <tbody>" + Environment.NewLine
                        + "    <tr>" + Environment.NewLine
                        + "      <td class=\"v-container-padding-padding\" style=\"overflow-wrap:break-word;word-break:break-word;padding:20px;font-family:'Montserrat',sans-serif;\" align=\"left\">" + Environment.NewLine
                        + "      <div style=\"color: #444444; line-height: 100%; text-align: left; word-wrap: break-word;\">" + Environment.NewLine
                        + "      <h4 style=\"margin-block-start: 0.5em; margin-block-end: 0.5em;\"><span style=\"font-size: 16px; line-height: 27.2px;font-weight:500;\">Hi </span>@CompanyName,</h4>" + Environment.NewLine
                        + "        <p style=\"font-size: 14px; line-height: 100%;\"><span style=\"font-size: 16px; line-height: 27.2px;\">Please check the @Software Server status.</span></p><br>@ServerName" + Environment.NewLine
                        + "      </td>" + Environment.NewLine
                        + "    </tr>" + Environment.NewLine
                        + "  </tbody>" + Environment.NewLine
                        + "</table>" + Environment.NewLine
                        + "" + Environment.NewLine
                        + "  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->" + Environment.NewLine
                        + "  </div>" + Environment.NewLine
                        + "</div>" + Environment.NewLine
                        + "<!--[if (mso)|(IE)]></td><![endif]-->" + Environment.NewLine
                        + "      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->" + Environment.NewLine
                        + "    </div>" + Environment.NewLine
                        + "  </div>" + Environment.NewLine
                        + "</div>" + Environment.NewLine
                        + "<div class=\"u-row-container\" style=\"padding: 0px;background-color: transparent\">" + Environment.NewLine
                        + "<br>" + Environment.NewLine
                        + "<div class=\"u-row-container\" style=\"padding: 0px;background-color: transparent\">" + Environment.NewLine
                        + "  <div class=\"u-row\" style=\"Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #111114;\">" + Environment.NewLine
                        + "    <div style=\"border-collapse: collapse;display: table;width: 100%;background-color: transparent;\">" + Environment.NewLine
                        + "      <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td style=\"padding: 0px;background-color: transparent;\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"width:600px;\"><tr style=\"background-color: #111114;\"><![endif]-->" + Environment.NewLine
                        + "      " + Environment.NewLine
                        + "<!--[if (mso)|(IE)]><td align=\"center\" width=\"600\" style=\"width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;\" valign=\"top\"><![endif]-->" + Environment.NewLine
                        + "<div class=\"u-col u-col-100\" style=\"max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;\">" + Environment.NewLine
                        + "  <div style=\"width: 100% !important;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;\">" + Environment.NewLine
                        + "  <!--[if (!mso)&(!IE)]><!--><div style=\"padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;\"><!--<![endif]-->" + Environment.NewLine
                        + "  " + Environment.NewLine
                        + "<table style=\"font-family:'Montserrat',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">" + Environment.NewLine
                        + "  <tbody>" + Environment.NewLine
                        + "    <tr>" + Environment.NewLine
                        + "      <td class=\"v-container-padding-padding\" style=\"overflow-wrap:break-word;word-break:break-word;padding:32px 10px 0px;font-family:'Montserrat',sans-serif;\" align=\"left\">" + Environment.NewLine
                        
                        + "          <div style=\"color: #ffffff; line-height: 140%; text-align: center; word-wrap: break-word;\">" + Environment.NewLine
                        + "            <p style=\"font-size: 14px; line-height: 140%;\"><span style=\"font-size: 18px; line-height: 25.2px;\"><strong>Leapsurge Business Innovations Private Limited</strong></span></p>" + Environment.NewLine
                        + "          </div>" + Environment.NewLine
                        
                        + "      </td>" + Environment.NewLine
                        + "    </tr>" + Environment.NewLine
                        + "  </tbody>" + Environment.NewLine
                        + "</table>" + Environment.NewLine
                        + "<!-- only for leapsurge start -->" + Environment.NewLine
                        + "<table style=\"font-family:'Montserrat',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">" + Environment.NewLine
                        + "  <tbody>" + Environment.NewLine
                        + "    <tr>" + Environment.NewLine
                        + "      <td class=\"v-container-padding-padding\" style=\"overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:'Montserrat',sans-serif;\" align=\"left\">" + Environment.NewLine
                        + "        " + Environment.NewLine
                        + "  <table height=\"0px\" align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"82%\" style=\"border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;border-top: 1px solid #9495a7;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%\">" + Environment.NewLine
                        + "    <tbody>" + Environment.NewLine
                        + "      <tr style=\"vertical-align: top\">" + Environment.NewLine
                        + "        <td style=\"word-break: break-word;border-collapse: collapse !important;vertical-align: top;font-size: 0px;line-height: 0px;mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%\">" + Environment.NewLine
                        + "          <span>&#160;</span>" + Environment.NewLine
                        + "        </td>" + Environment.NewLine
                        + "      </tr>" + Environment.NewLine
                        + "    </tbody>" + Environment.NewLine
                        + "  </table>" + Environment.NewLine
                        + "" + Environment.NewLine
                        + "      </td>" + Environment.NewLine
                        + "    </tr>" + Environment.NewLine
                        + "  </tbody>" + Environment.NewLine
                        + "</table>" + Environment.NewLine
                        + "" + Environment.NewLine
                        + "<table style=\"font-family:'Montserrat',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">" + Environment.NewLine
                        + "  <tbody>" + Environment.NewLine
                        + "    <tr>" + Environment.NewLine
                        + "      <td class=\"v-container-padding-padding\" style=\"overflow-wrap:break-word;word-break:break-word;padding:20px 0;font-family:'Montserrat',sans-serif;\" align=\"left\">" + Environment.NewLine
                        + "        " + Environment.NewLine
                        + "  <div style=\"color: #b0b1b4;line-height: 100%; text-align: center; word-wrap: break-word;\">" + Environment.NewLine
                        + "    <p style=\"font-size: 14px;\">&copy; " + (DateTime.Now.Year).ToString() + " All Rights Reserved</p> <br>" + Environment.NewLine
                        + "  </div>" + Environment.NewLine
                        + "  <div style=\"color: #b0b1b4;line-height: 100%; text-align: center; word-wrap: break-word;\">" + Environment.NewLine
                        + "    <p style=\"font-size: 14px;\"><span style=\" line-height: 20px;\">This email was sent from an unmonitored mailbox.</span> <br>" + Environment.NewLine
                        + "   <span style=\" line-height: 20px;\">You are receiving this email because you have subscribed to LeapsurgeBI</span></p>" + Environment.NewLine
                        + "  </div>" + Environment.NewLine
                        + "" + Environment.NewLine
                        + "      </td>" + Environment.NewLine
                        + "    </tr>" + Environment.NewLine
                        + "  </tbody>" + Environment.NewLine
                        + "</table>" + Environment.NewLine
                        + "" + Environment.NewLine
                        + "  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->" + Environment.NewLine
                        + "  </div>" + Environment.NewLine
                        + "</div>" + Environment.NewLine
                        + "<!--[if (mso)|(IE)]></td><![endif]-->" + Environment.NewLine
                        + "      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->" + Environment.NewLine
                        + "    </div>" + Environment.NewLine
                        + "  </div>" + Environment.NewLine
                        + "</div>" + Environment.NewLine
                        + "    <!--[if (mso)|(IE)]></td></tr></table><![endif]-->" + Environment.NewLine
                        + "    </td>" + Environment.NewLine
                        + "  </tr>" + Environment.NewLine
                        + "  </tbody>" + Environment.NewLine
                        + "  </table>" + Environment.NewLine
                        + "  <!--[if mso]></div><![endif]-->" + Environment.NewLine
                        + "  <!--[if IE]></div><![endif]-->" + Environment.NewLine
                        + "</body>" + Environment.NewLine
                        + "</html>";
                    break;
                default:
                    aiSuccess = 0;
                    asMessage = "";
                    break;
            }
        }

        /////////////////////////////////////////////////////////////////////
        //Tran Type Column List and Max time Fetch
        /////////////////////////////////////////////////////////////////////
        public string tranTypeFetchMax(GlobalVariable gbl, string asFinYear, string tableName, string dateColName, 
            string tranColName, string fetchSql, string brColName, string br, string whereCondition, out string tranInsert)
        {
            string lsReturn = "", lsFetchSql, lsTranColName, lsTransList, lsWhere = whereCondition, lsTranCol, lsInsertTran = "", lsBr;
            int i = 0;
            lsBr = br == "" ? ('-').ToString() : br;
            if (dateColName != "" && dateColName != null && tranColName != "" && tranColName != null)
            {
                lsTranColName = gbl.reverseString(tranColName.Replace("]", "").Replace("[", ""));
                lsFetchSql = gbl.reverseString(fetchSql.Replace("]", "").Replace("[", ""));

                while (lsFetchSql.Contains(lsTranColName))
                {
                    lsFetchSql = lsFetchSql.Substring(lsFetchSql.IndexOf(lsTranColName));
                    lsTransList = lsFetchSql.Substring(0, lsFetchSql.IndexOf(','));
                    lsFetchSql = lsFetchSql.Substring(lsFetchSql.IndexOf(','));
                    if (lsTransList.Contains("'"))
                    {
                        if (!lsReturn.Contains(gbl.reverseString(lsTransList)))
                        {
                            lsTranCol = gbl.reverseString(lsTransList);
                            lsInsertTran += "Select " + asFinYear + " as finYear,'" + tableName + "' as tblName,'" + (lsBr == "" || lsBr == null ? "-" : lsBr)
                                    + "' as brcode," + lsTranCol + ",'" + gbl.defTime + "' as maxTime {Union}";
                            lsReturn += "Select IsNull(CONVERT(Varchar,[Max Time],120),'" + gbl.defTime + "') as maxTime, "
                                + "Tran_Type From LS_MaxTime " + lsWhere + " and " + lsTranCol.Replace(" as ", " = ") + " {Union}";
                        }
                    }
                    i += 1;
                }
                if (i > 0)
                {
                    lsInsertTran = lsInsertTran.Replace("{Union}Select", " Union All Select").Replace("{Union}", "");

                    lsInsertTran = "MERGE INTO LS_MaxTime WITH(HOLDLOCK) AS a USING(" + lsInsertTran + ") as b(finYear,tblName,br_code,Tran_Type,maxTime)"
                            + " ON a.[Fin Year] = b.finYear And a.[Table Name] = b.tblName And a.br_code = b.br_code and a.Tran_Type = b.Tran_Type "
                            + " WHEN NOT MATCHED THEN INSERT([Fin Year],[Table Name],br_code,Tran_Type,[Max Time])"
                            + " Values(b.finYear,b.tblName,b.br_code,b.Tran_Type,b.maxTime);";

                    lsReturn = lsReturn.Replace("{Union}Select", " Union All Select").Replace("{Union}", "");
                    lsReturn = "Select '[' + String_Agg('{\"tranType\": \"' + " + gbl.reverseString(lsTranColName) + " + '\",\"maxTime\": \"' + maxTime + '\"}' , ',') + ']' From(" + lsReturn + ") a";
                }
            }
            tranInsert = lsInsertTran;
            return lsReturn;
        }
    }
}
