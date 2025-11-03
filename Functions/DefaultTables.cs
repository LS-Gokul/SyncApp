using System;

namespace LSSyncApp.Functions
{
    class DefaultTables
    {
        int iiSuccess;
        string lsReturn;
        //Check Audit Log table
        public string chkAuditLog(GlobalVariable globalVar, string asLogFileName, out int aiSuccess)
        {
            string lsCnt, lsTable;
            aiSuccess = 0;
            try
            {
                globalVar._DestinationConfig.ExecuteRawQuery(globalVar, "", 2, out iiSuccess, out lsReturn, 0);
                aiSuccess = iiSuccess;
                if (iiSuccess == 0)
                {
                    return lsReturn;
                }

                //LS_Audit_Log Check
                globalVar._DestinationConfig.ExecuteRawQuery(globalVar, "Select Count(*) From sys.tables where [name] = 'LS_Audit_Log'",
                    0, out iiSuccess, out lsCnt, 0);
                aiSuccess = iiSuccess;
                if (int.TryParse(lsCnt, out _))
                {
                    if (int.Parse(lsCnt) == 0)
                    {
                        lsTable = "Create Table LS_Audit_Log([LogId] Varchar(50) NOT NULL,[Process] Varchar(100) NULL,"
                            + "[Status] Varchar(10) NULL,[LogDetails] Varchar(Max) NULL,[System Ip] Varchar(50) NULL,"
                            + "[System User] Varchar(50) NULL,[System Name] Varchar(50) NULL,[Start Time] [datetime] NOT NULL,"
                            + "[End Time] [datetime] NOT NULL,[Session Start Time] [datetime] NOT NULL,"
                            + "[Created User] [varchar](100) NOT NULL,[Date Of Creation] [datetime] NOT NULL,"
                            + "PRIMARY KEY CLUSTERED([LogId]));";
                        globalVar._DestinationConfig.ExecuteRawQuery(globalVar, lsTable, 1, out iiSuccess, out lsReturn, 0);
                        aiSuccess = iiSuccess;
                        if (iiSuccess == 0)
                        {
                            globalVar.setMessageLog(asLogFileName, lsReturn, globalVar.giCmd);
                        }
                    }
                }
                else
                {
                    globalVar.setMessageLog(asLogFileName, lsCnt, globalVar.giCmd);
                    return "Failed";
                }

                //LS_Audit_Log_Detail Check
                globalVar._DestinationConfig.ExecuteRawQuery(globalVar, "Select Count(*) From sys.tables where [name] = 'LS_Audit_Log_Detail'",
                    0, out iiSuccess, out lsCnt, 0);
                aiSuccess = iiSuccess;
                if (int.TryParse(lsCnt, out _))
                {
                    if (int.Parse(lsCnt) == 0)
                    {
                        lsTable = "Create Table LS_Audit_Log_Detail([LogId] Varchar(50) NOT NULL,[Object] Varchar(100) NOT NULL,"
                            + "[Child Object] Varchar(100) NOT NULL,[Sequence] Int Not Null,[Object From Time] [Varchar](200) NULL,"
                            + "[Start Time] [datetime] NOT NULL,[End Time] [datetime] NOT NULL,[LogDetails] Varchar(Max) NULL,"
                            + "[Status] Varchar(10) NULL,[Created User] [varchar](100) NOT NULL,[Date Of Creation] [datetime] NOT NULL,"
                            + "PRIMARY KEY CLUSTERED([LogId], [Object], [Child Object], [Sequence]));";
                        globalVar._DestinationConfig.ExecuteRawQuery(globalVar, lsTable, 1, out iiSuccess, out lsReturn, 0);
                        aiSuccess = iiSuccess;
                        if (iiSuccess == 0)
                        {
                            globalVar.setMessageLog(asLogFileName, lsReturn, globalVar.giCmd);
                        }
                    }
                }
                else
                {
                    globalVar.setMessageLog(asLogFileName, lsCnt, globalVar.giCmd);
                    return "Failed";
                }

                return "Success";
            }
            catch (Exception Ex)
            {
                aiSuccess = 0;
                globalVar.setMessageLog(asLogFileName, Ex.Message, globalVar.giCmd);
                return "Failed";
            }
        }
        
        //Check RLS table
        public string chkRLS(GlobalVariable globalVar, string asLogFileName, out int aiSuccess)
        {
            string lsCnt, lsTable;
            aiSuccess = 0;
            try
            {
                globalVar._DestinationConfig.ExecuteRawQuery(globalVar, "", 2, out iiSuccess, out lsReturn, 0);
                aiSuccess = iiSuccess;
                if (iiSuccess == 0)
                {
                    return lsReturn;
                }

                //LS_User_rls_Access Check
                globalVar._DestinationConfig.ExecuteRawQuery(globalVar, "Select Count(*) From sys.tables where [name] = 'LS_User_rls_Access'",
                    0, out iiSuccess, out lsCnt, 0);
                aiSuccess = iiSuccess;
                if (int.TryParse(lsCnt, out _))
                {
                    if (int.Parse(lsCnt) == 0)
                    {
                        lsTable = "CREATE TABLE [dbo].[LS_User_rls_Access]([LSCode] [varchar](10) NOT NULL,[LSFirmCode] [numeric](4, 0) NOT NULL,"
                            + "[Rls Name] [varchar](200) NOT NULL,[Report Name] [varchar](200) NOT NULL,[User Email] [varchar](100) NOT NULL,"
                            + "[br_codes] [varchar](max) NOT NULL,[Remarks] [varchar](100) NULL,[Created By] [varchar](50) NOT NULL,"
                            + "[Date Of Creation] [date] NOT NULL,[Modified By] [varchar](50) NOT NULL,[Date Of Modification] [datetime] NOT NULL,"
                            + "[isgroup] [int] NOT NULL Default 1,PRIMARY KEY CLUSTERED ([Rls Name] ASC,[Report Name] ASC,[User Email] ASC));";
                        globalVar._DestinationConfig.ExecuteRawQuery(globalVar, lsTable, 1, out iiSuccess, out lsReturn, 0);
                        aiSuccess = iiSuccess;
                        if (iiSuccess == 0)
                        {
                            globalVar.setMessageLog(asLogFileName, lsReturn, globalVar.giCmd);
                        }
                    }
                }
                else
                {
                    globalVar.setMessageLog(asLogFileName, lsCnt, globalVar.giCmd);
                    return "Failed";
                }
                return "Success";
            }
            catch (Exception Ex)
            {
                aiSuccess = 0;
                globalVar.setMessageLog(asLogFileName, Ex.Message, globalVar.giCmd);
                return "Failed";
            }
        }

        public void versionTable(GlobalVariable globalVar, string asLogFileName, out int aiSuccess)
        {
            aiSuccess = 0;
            try
            {
                string lsTable;
                globalVar._DestinationConfig.ExecuteRawQuery(globalVar, "Select Count(*) From sys.tables where name = 'LS_Version'", 
                    0, out iiSuccess, out lsReturn, 0);
                aiSuccess = iiSuccess;
                if (!int.TryParse(lsReturn, out _))
                {
                    return;
                }
                if (int.Parse(lsReturn) <= 0)
                {
                    lsTable = "CREATE TABLE LS_Version(application_code VarChar(6) Not Null, Version Numeric(7,2) Not Null,"
                        + " software_code VarChar(6) Not Null, [Released Date] DateTime Not Null, "
                        + " [Date of Updation] DateTime Not Null, Primary Key Clustered(application_code,Version,software_code));";
                    globalVar._DestinationConfig.ExecuteRawQuery(globalVar, lsTable, 1, out iiSuccess, out lsReturn);
                    aiSuccess = iiSuccess;
                    if (iiSuccess == 0)
                    {
                        return;
                    }
                }
                else
                {
                    lsTable = "Select Count(*) From sys.tables a join sys.columns b on a.object_id = b.object_id where a.name = 'LS_Version' And b.name = 'application_code'";
                    globalVar._DestinationConfig.ExecuteRawQuery(globalVar, lsTable, 0, out iiSuccess, out lsReturn, 0);
                    if (!int.TryParse(lsReturn, out _))
                    {
                        return;
                    }
                    if (int.Parse(lsReturn) <= 0)
                    {
                        lsTable = "If Exists(Select * From sys.tables where name = 'tmpVer') Begin Drop Table tmpVer; End;";
                        globalVar._DestinationConfig.ExecuteRawQuery(globalVar, lsTable, 1, out iiSuccess, out lsReturn);
                        if (iiSuccess == 0)
                        {
                            return;
                        }

                        lsTable = " Select * INTO tmpVer FROM LS_Version;";
                        globalVar._DestinationConfig.ExecuteRawQuery(globalVar, lsTable, 1, out iiSuccess, out lsReturn);
                        if (iiSuccess == 0)
                        {
                            return;
                        }

                        lsTable = " Drop Table LS_Version;";
                        globalVar._DestinationConfig.ExecuteRawQuery(globalVar, lsTable, 1, out iiSuccess, out lsReturn);
                        if (iiSuccess == 0)
                        {
                            return;
                        }

                        lsTable = " CREATE TABLE LS_Version(application_code VarChar(6) Not Null, Version Numeric(7,2) Not Null,"
                            + " software_code VarChar(6) Not Null, [Released Date] DateTime Not Null, "
                            + " [Date of Updation] DateTime Not Null, Primary Key Clustered(application_code,Version,software_code));";
                        globalVar._DestinationConfig.ExecuteRawQuery(globalVar, lsTable, 1, out iiSuccess, out lsReturn);
                        if (iiSuccess == 0)
                        {
                            return;
                        }

                        lsTable = " INSERT INTO LS_Version(application_code, Version,software_code, [Released Date],[Date of Updation]) "
                            + $" SELECT '{globalVar.gsAppCode}',Version,software_code, [Released Date],[Date of Updation] "
                            + " FROM tmpVer;";
                        globalVar._DestinationConfig.ExecuteRawQuery(globalVar, lsTable, 1, out iiSuccess, out lsReturn);
                        if (iiSuccess == 0)
                        {
                            return;
                        }

                        lsTable = " DROP TABLE tmpVer;";
                        globalVar._DestinationConfig.ExecuteRawQuery(globalVar, lsTable, 1, out iiSuccess, out lsReturn);
                        if (iiSuccess == 0)
                        {
                            return;
                        }
                    }
                }
                return;
            }
            catch
            {
                aiSuccess = 0;
                return;
            }
        }

        //Check RLS table
        public string MDMTables(GlobalVariable globalVar, string asLogFileName, out int aiSuccess)
        {
            string lsCnt, lsTable;
            aiSuccess = 0;
            try
            {
                globalVar._DestinationConfig.ExecuteRawQuery(globalVar, "", 2, out iiSuccess, out lsReturn, 0);
                aiSuccess = iiSuccess;
                if (iiSuccess == 0)
                {
                    return lsReturn;
                }

                //LS_Additional_Values Check
                globalVar._DestinationConfig.ExecuteRawQuery(globalVar, "Select Count(*) From sys.tables where [name] = 'LS_Additional_Values'",
                    0, out iiSuccess, out lsCnt, 0);
                aiSuccess = iiSuccess;
                if (int.TryParse(lsCnt, out _))
                {
                    if (int.Parse(lsCnt) == 0)
                    {
                        lsTable = "CREATE TABLE [dbo].[LS_Additional_Values]([LSCode] [varchar](10) NOT NULL,[LSFirmCode] [numeric](4, 0) NOT NULL,"
                            + "[Table] [varchar](100) NOT NULL,[pKey] [varchar](50) NOT NULL,[Custom1] [varchar](500) NULL,[Custom2] [varchar](500) NULL,[Custom3] [varchar](500) NULL,[Custom4] [varchar](500) NULL,[Custom5] [varchar](500) NULL,[Custom6] [varchar](500) NULL,"
                            + "[Custom7] [varchar](500) NULL,[Custom8] [varchar](500) NULL,[Custom9] [varchar](500) NULL,[Custom10] [varchar](500) NULL,[Custom11] [varchar](500) NULL,[Custom12] [varchar](500) NULL,[Custom13] [varchar](500) NULL,[Custom14] [varchar](500) NULL,"
                            + "[Custom15] [varchar](500) NULL,[Custom16] [varchar](500) NULL,[Custom17] [varchar](500) NULL,[Custom18] [varchar](500) NULL,[Custom19] [varchar](500) NULL,[Custom20] [varchar](500) NULL,[Custom21] [varchar](500) NULL,[Custom22] [varchar](500) NULL,"
                            + "[Custom23] [varchar](500) NULL,[Custom24] [varchar](500) NULL,[Custom25] [varchar](500) NULL,[Custom26] [varchar](500) NULL,[Custom27] [varchar](500) NULL,[Custom28] [varchar](500) NULL,[Custom29] [varchar](500) NULL,[Custom30] [varchar](500) NULL,"
                            + "[Custom31] [varchar](500) NULL,[Custom32] [varchar](500) NULL,[Custom33] [varchar](500) NULL,[Custom34] [varchar](500) NULL,[Custom35] [varchar](500) NULL,[Custom36] [varchar](500) NULL,[Custom37] [varchar](500) NULL,[Custom38] [varchar](500) NULL,"
                            + "[Custom39] [varchar](500) NULL,[Custom40] [varchar](500) NULL,[Custom41] [varchar](500) NULL,[Custom42] [varchar](500) NULL,[Custom43] [varchar](500) NULL,[Custom44] [varchar](500) NULL,[Custom45] [varchar](500) NULL,[Custom46] [varchar](500) NULL,"
                            + "[Custom47] [varchar](500) NULL,[Custom48] [varchar](500) NULL,[Custom49] [varchar](500) NULL,[Custom50] [varchar](500) NULL,[Custom51] [varchar](500) NULL,[Custom52] [varchar](500) NULL,[Custom53] [varchar](500) NULL,[Custom54] [varchar](500) NULL,"
                            + "[Custom55] [varchar](500) NULL,[Custom56] [varchar](500) NULL,[Custom57] [varchar](500) NULL,[Custom58] [varchar](500) NULL,[Custom59] [varchar](500) NULL,[Custom60] [varchar](500) NULL,[Custom61] [varchar](500) NULL,[Custom62] [varchar](500) NULL,"
                            + "[Custom63] [varchar](500) NULL,[Custom64] [varchar](500) NULL,[Custom65] [varchar](500) NULL,[Custom66] [varchar](500) NULL,[Custom67] [varchar](500) NULL,[Custom68] [varchar](500) NULL,[Custom69] [varchar](500) NULL,[Custom70] [varchar](500) NULL,"
                            + "[Custom71] [varchar](500) NULL,[Custom72] [varchar](500) NULL,[Custom73] [varchar](500) NULL,[Custom74] [varchar](500) NULL,[Custom75] [varchar](500) NULL,[Custom76] [varchar](500) NULL,[Custom77] [varchar](500) NULL,[Custom78] [varchar](500) NULL,"
                            + "[Custom79] [varchar](500) NULL,[Custom80] [varchar](500) NULL,[Custom81] [varchar](500) NULL,[Custom82] [varchar](500) NULL,[Custom83] [varchar](500) NULL,[Custom84] [varchar](500) NULL,[Custom85] [varchar](500) NULL,[Custom86] [varchar](500) NULL,"
                            + "[Custom87] [varchar](500) NULL,[Custom88] [varchar](500) NULL,[Custom89] [varchar](500) NULL,[Custom90] [varchar](500) NULL,[Custom91] [varchar](500) NULL,[Custom92] [varchar](500) NULL,[Custom93] [varchar](500) NULL,[Custom94] [varchar](500) NULL,"
                            + "[Custom95] [varchar](500) NULL,[Custom96] [varchar](500) NULL,[Custom97] [varchar](500) NULL,[Custom98] [varchar](500) NULL,[Custom99] [varchar](500) NULL,[Custom100] [varchar](500) NULL,[Created By] [varchar](100) NULL,[Date Of Item Created] [datetime] NOT NULL,"
                            + "[Modified By] [varchar](100) NULL,[Date of Modification] [datetime] NOT NULL,[File_name] [varchar](max) NULL,[Batch Id] [varchar](70) NULL, PRIMARY KEY CLUSTERED ([LSCode] ASC,[LSFirmCode] ASC,[Table] ASC,[pKey] ASC));";
                        globalVar._DestinationConfig.ExecuteRawQuery(globalVar, lsTable, 1, out iiSuccess, out lsReturn, 0);
                        aiSuccess = iiSuccess;
                        if (iiSuccess == 0)
                        {
                            globalVar.setMessageLog(asLogFileName, lsReturn, globalVar.giCmd);
                        }
                    }
                }
                else
                {
                    globalVar.setMessageLog(asLogFileName, lsCnt, globalVar.giCmd);
                    return "Failed";
                }

                //LS_Additional_Values_Pre_Insert Check
                globalVar._DestinationConfig.ExecuteRawQuery(globalVar, "Select Count(*) From sys.tables where [name] = 'LS_Additional_Values_Pre_Insert'",
                    0, out iiSuccess, out lsCnt, 0);
                aiSuccess = iiSuccess;
                if (int.TryParse(lsCnt, out _))
                {
                    if (int.Parse(lsCnt) == 0)
                    {
                        lsTable = "CREATE TABLE [dbo].[LS_Additional_Values_Pre_Insert]([LSCode] [varchar](10) NOT NULL,[LSFirmCode] [numeric](4, 0) NOT NULL,[Table] [varchar](100) NOT NULL,[pKey] [varchar](50) NOT NULL,[Batch Id] [varchar](70) NOT NULL,[Custom1] [varchar](500) NULL,"
                            + "[Custom2] [varchar](500) NULL,[Custom3] [varchar](500) NULL,[Custom4] [varchar](500) NULL,[Custom5] [varchar](500) NULL,[Custom6] [varchar](500) NULL,[Custom7] [varchar](500) NULL,[Custom8] [varchar](500) NULL,[Custom9] [varchar](500) NULL,[Custom10] [varchar](500) NULL,"
                            + "[Custom11] [varchar](500) NULL,[Custom12] [varchar](500) NULL,[Custom13] [varchar](500) NULL,[Custom14] [varchar](500) NULL,[Custom15] [varchar](500) NULL,[Custom16] [varchar](500) NULL,[Custom17] [varchar](500) NULL,[Custom18] [varchar](500) NULL,[Custom19] [varchar](500) NULL,"
                            + "[Custom20] [varchar](500) NULL,[Custom21] [varchar](500) NULL,[Custom22] [varchar](500) NULL,[Custom23] [varchar](500) NULL,[Custom24] [varchar](500) NULL,[Custom25] [varchar](500) NULL,[Custom26] [varchar](500) NULL,[Custom27] [varchar](500) NULL,[Custom28] [varchar](500) NULL,"
                            + "[Custom29] [varchar](500) NULL,[Custom30] [varchar](500) NULL,[Custom31] [varchar](500) NULL,[Custom32] [varchar](500) NULL,[Custom33] [varchar](500) NULL,[Custom34] [varchar](500) NULL,[Custom35] [varchar](500) NULL,[Custom36] [varchar](500) NULL,[Custom37] [varchar](500) NULL,"
                            + "[Custom38] [varchar](500) NULL,[Custom39] [varchar](500) NULL,[Custom40] [varchar](500) NULL,[Custom41] [varchar](500) NULL,[Custom42] [varchar](500) NULL,[Custom43] [varchar](500) NULL,[Custom44] [varchar](500) NULL,[Custom45] [varchar](500) NULL,[Custom46] [varchar](500) NULL,"
                            + "[Custom47] [varchar](500) NULL,[Custom48] [varchar](500) NULL,[Custom49] [varchar](500) NULL,[Custom50] [varchar](500) NULL,[Custom51] [varchar](500) NULL,[Custom52] [varchar](500) NULL,[Custom53] [varchar](500) NULL,[Custom54] [varchar](500) NULL,[Custom55] [varchar](500) NULL,"
                            + "[Custom56] [varchar](500) NULL,[Custom57] [varchar](500) NULL,[Custom58] [varchar](500) NULL,[Custom59] [varchar](500) NULL,[Custom60] [varchar](500) NULL,[Custom61] [varchar](500) NULL,[Custom62] [varchar](500) NULL,[Custom63] [varchar](500) NULL,[Custom64] [varchar](500) NULL,"
                            + "[Custom65] [varchar](500) NULL,[Custom66] [varchar](500) NULL,[Custom67] [varchar](500) NULL,[Custom68] [varchar](500) NULL,[Custom69] [varchar](500) NULL,[Custom70] [varchar](500) NULL,[Custom71] [varchar](500) NULL,[Custom72] [varchar](500) NULL,[Custom73] [varchar](500) NULL,"
                            + "[Custom74] [varchar](500) NULL,[Custom75] [varchar](500) NULL,[Custom76] [varchar](500) NULL,[Custom77] [varchar](500) NULL,[Custom78] [varchar](500) NULL,[Custom79] [varchar](500) NULL,[Custom80] [varchar](500) NULL,[Custom81] [varchar](500) NULL,[Custom82] [varchar](500) NULL,"
                            + "[Custom83] [varchar](500) NULL,[Custom84] [varchar](500) NULL,[Custom85] [varchar](500) NULL,[Custom86] [varchar](500) NULL,[Custom87] [varchar](500) NULL,[Custom88] [varchar](500) NULL,[Custom89] [varchar](500) NULL,[Custom90] [varchar](500) NULL,[Custom91] [varchar](500) NULL,"
                            + "[Custom92] [varchar](500) NULL,[Custom93] [varchar](500) NULL,[Custom94] [varchar](500) NULL,[Custom95] [varchar](500) NULL,[Custom96] [varchar](500) NULL,[Custom97] [varchar](500) NULL,[Custom98] [varchar](500) NULL,[Custom99] [varchar](500) NULL,[Custom100] [varchar](500) NULL,"
                            + "[Is Error] [int] NULL,[Description] [varchar](2000) NULL,[Created By] [varchar](100) NULL,[Date Of Item Created] [datetime] NOT NULL,[Modified By] [varchar](100) NULL,[Date of Modification] [datetime] NOT NULL,[Is Duplicate] [int] NULL DEFAULT 0,[Is Excel Duplicate] [int] NULL DEFAULT 0,"
                            + "[File_name] [varchar](max) NULL,PRIMARY KEY CLUSTERED ([LSCode] ASC,[LSFirmCode] ASC,[Table] ASC,[pKey] ASC,[Batch Id] ASC));";
                        globalVar._DestinationConfig.ExecuteRawQuery(globalVar, lsTable, 1, out iiSuccess, out lsReturn, 0);
                        aiSuccess = iiSuccess;
                        if (iiSuccess == 0)
                        {
                            globalVar.setMessageLog(asLogFileName, lsReturn, globalVar.giCmd);
                        }
                    }
                }
                else
                {
                    globalVar.setMessageLog(asLogFileName, lsCnt, globalVar.giCmd);
                    return "Failed";
                }

                //LS_Custom_Columns Check
                globalVar._DestinationConfig.ExecuteRawQuery(globalVar, "Select Count(*) From sys.tables where [name] = 'LS_Custom_Columns'",
                    0, out iiSuccess, out lsCnt, 0);
                aiSuccess = iiSuccess;
                if (int.TryParse(lsCnt, out _))
                {
                    if (int.Parse(lsCnt) == 0)
                    {
                        lsTable = "CREATE TABLE [dbo].[LS_Custom_Columns]([LSCode] [varchar](10) NOT NULL,[LSFirmCode] [numeric](4, 0) NOT NULL,[Table] [varchar](100) NOT NULL,"
                            + "[column_id] [varchar](20) NOT NULL,[Column Name] [varchar](100) NOT NULL,[Data Type] [varchar](100) NOT NULL,[Length] [numeric](4, 0) NULL DEFAULT 0,"
                            + "[Scale] [numeric](4, 0) NULL DEFAULT 0,[Required] [numeric](1, 0) NULL DEFAULT 0,[Column Type] [varchar](20) NULL,[Reference Type] [varchar](50) NULL,"
                            + "[Description] [varchar](max) NULL,[Default Value] [varchar](max) NULL,[Compute Formula] [varchar](max) NULL,[Sequence] [int] NULL DEFAULT 0,"
                            + "[Active Flag] [int] NOT NULL,[Created By] [varchar](100) NULL,[Date Of Item Created] [datetime] NOT NULL,[Modified By] [varchar](100) NULL,"
                            + "[Date of Modification] [datetime] NOT NULL,[Pkey] [int] NULL DEFAULT 0,[Delete_on] [int] NULL,PRIMARY KEY CLUSTERED ([LSCode] ASC,[LSFirmCode] ASC,[Table] ASC,[column_id] ASC));";
                        globalVar._DestinationConfig.ExecuteRawQuery(globalVar, lsTable, 1, out iiSuccess, out lsReturn, 0);
                        aiSuccess = iiSuccess;
                        if (iiSuccess == 0)
                        {
                            globalVar.setMessageLog(asLogFileName, lsReturn, globalVar.giCmd);
                        }
                    }
                }
                else
                {
                    globalVar.setMessageLog(asLogFileName, lsCnt, globalVar.giCmd);
                    return "Failed";
                }

                //LS_Custom_Refrence Check
                globalVar._DestinationConfig.ExecuteRawQuery(globalVar, "Select Count(*) From sys.tables where [name] = 'LS_Custom_Refrence'",
                    0, out iiSuccess, out lsCnt, 0);
                aiSuccess = iiSuccess;
                if (int.TryParse(lsCnt, out _))
                {
                    if (int.Parse(lsCnt) == 0)
                    {
                        lsTable = "CREATE TABLE [dbo].[LS_Custom_Refrence]([LSCode] [varchar](10) NOT NULL,[LSFirmCode] [numeric](4, 0) NOT NULL,[Table] [varchar](100) NOT NULL,"
                            + "[column_id] [varchar](20) NOT NULL,[Key] [int] NOT NULL,[Value] [varchar](100) NOT NULL,[Active Flag] [numeric](1, 0) NOT NULL,"
                            + "[Created By] [varchar](100) NULL,[Date Of Item Created] [datetime] NOT NULL,[Modified By] [varchar](100) NULL,[Date of Modification] [datetime] NOT NULL,"
                            + "PRIMARY KEY CLUSTERED ([LSCode] ASC,[LSFirmCode] ASC,[Table] ASC,[column_id] ASC,[Key] ASC));";
                        globalVar._DestinationConfig.ExecuteRawQuery(globalVar, lsTable, 1, out iiSuccess, out lsReturn, 0);
                        aiSuccess = iiSuccess;
                        if (iiSuccess == 0)
                        {
                            globalVar.setMessageLog(asLogFileName, lsReturn, globalVar.giCmd);
                        }
                    }
                }
                else
                {
                    globalVar.setMessageLog(asLogFileName, lsCnt, globalVar.giCmd);
                    return "Failed";
                }

                //LS_Custom_Table_Reference Check
                globalVar._DestinationConfig.ExecuteRawQuery(globalVar, "Select Count(*) From sys.tables where [name] = 'LS_Custom_Table_Reference'",
                    0, out iiSuccess, out lsCnt, 0);
                aiSuccess = iiSuccess;
                if (int.TryParse(lsCnt, out _))
                {
                    if (int.Parse(lsCnt) == 0)
                    {
                        lsTable = "CREATE TABLE [dbo].[LS_Custom_Table_Reference]([LSCode] [varchar](10) NOT NULL,[LSFirmCode] [numeric](4, 0) NOT NULL,[Table] [varchar](100) NOT NULL,"
                            + "[Column] [varchar](100) NOT NULL,[Refrence Table] [varchar](100) NOT NULL,[Data Column] [varchar](100) NOT NULL,[Display Column1] [varchar](100) NULL,"
                            + "[Display Column2] [varchar](100) NULL,[Display Column3] [varchar](100) NULL,[Display Column4] [varchar](100) NULL,[Display Column5] [varchar](100) NULL,"
                            + "[Active Flag] [numeric](1, 0) NOT NULL,[Created By] [varchar](100) NULL,[Date Of Item Created] [datetime] NOT NULL,[Modified By] [varchar](100) NULL,"
                            + "[Date of Modification] [datetime] NOT NULL,PRIMARY KEY CLUSTERED ([LSCode] ASC,[LSFirmCode] ASC,[Table] ASC,[Column] ASC));";
                        globalVar._DestinationConfig.ExecuteRawQuery(globalVar, lsTable, 1, out iiSuccess, out lsReturn, 0);
                        aiSuccess = iiSuccess;
                        if (iiSuccess == 0)
                        {
                            globalVar.setMessageLog(asLogFileName, lsReturn, globalVar.giCmd);
                        }
                    }
                }
                else
                {
                    globalVar.setMessageLog(asLogFileName, lsCnt, globalVar.giCmd);
                    return "Failed";
                }

                //LS_Custom_Tables Check
                globalVar._DestinationConfig.ExecuteRawQuery(globalVar, "Select Count(*) From sys.tables where [name] = 'LS_Custom_Tables'",
                    0, out iiSuccess, out lsCnt, 0);
                aiSuccess = iiSuccess;
                if (int.TryParse(lsCnt, out _))
                {
                    if (int.Parse(lsCnt) == 0)
                    {
                        lsTable = "CREATE TABLE [dbo].[LS_Custom_Tables]([LSCode] [varchar](10) NOT NULL,[LSFirmCode] [numeric](4, 0) NOT NULL,[Table] [varchar](100) NOT NULL,"
                            + "[Description] [varchar](500) NULL,[Active Flag] [numeric](1, 0) NOT NULL DEFAULT 1,[Created By] [varchar](100) NULL,[Date Of Item Created] [datetime] NOT NULL,"
                            + "[Modified By] [varchar](100) NULL,[Date of Modification] [datetime] NOT NULL,[PKey] [numeric](10, 0) NOT NULL DEFAULT 0,[delete_type] [int] NULL,"
                            + "[Error_type_validation] [int] NULL,CONSTRAINT [PK_LS_Custom_Tables] PRIMARY KEY CLUSTERED ([LSCode] ASC,[LSFirmCode] ASC,[Table] ASC,[PKey] ASC));";
                        globalVar._DestinationConfig.ExecuteRawQuery(globalVar, lsTable, 1, out iiSuccess, out lsReturn, 0);
                        aiSuccess = iiSuccess;
                        if (iiSuccess == 0)
                        {
                            globalVar.setMessageLog(asLogFileName, lsReturn, globalVar.giCmd);
                        }
                    }
                }
                else
                {
                    globalVar.setMessageLog(asLogFileName, lsCnt, globalVar.giCmd);
                    return "Failed";
                }

                //RCVY_LS_Additional_Values Check
                globalVar._DestinationConfig.ExecuteRawQuery(globalVar, "Select Count(*) From sys.tables where [name] = 'RCVY_LS_Additional_Values'",
                    0, out iiSuccess, out lsCnt, 0);
                aiSuccess = iiSuccess;
                if (int.TryParse(lsCnt, out _))
                {
                    if (int.Parse(lsCnt) == 0)
                    {
                        lsTable = "CREATE TABLE [dbo].[RCVY_LS_Additional_Values]([LSCode] [varchar](10) NOT NULL,[LSFirmCode] [numeric](4, 0) NOT NULL,[Table] [varchar](100) NOT NULL,"
                            + "[pKey] [varchar](50) NOT NULL,[Batch Id] [varchar](70) NOT NULL,[Time of Deletion] [datetime] NOT NULL,[Custom1] [varchar](500) NULL,[Custom2] [varchar](500) NULL,"
                            + "[Custom3] [varchar](500) NULL,[Custom4] [varchar](500) NULL,[Custom5] [varchar](500) NULL,[Custom6] [varchar](500) NULL,[Custom7] [varchar](500) NULL,"
                            + "[Custom8] [varchar](500) NULL,[Custom9] [varchar](500) NULL,[Custom10] [varchar](500) NULL,[Custom11] [varchar](500) NULL,[Custom12] [varchar](500) NULL,"
                            + "[Custom13] [varchar](500) NULL,[Custom14] [varchar](500) NULL,[Custom15] [varchar](500) NULL,[Custom16] [varchar](500) NULL,[Custom17] [varchar](500) NULL,"
                            + "[Custom18] [varchar](500) NULL,[Custom19] [varchar](500) NULL,[Custom20] [varchar](500) NULL,[Custom21] [varchar](500) NULL,[Custom22] [varchar](500) NULL,"
                            + "[Custom23] [varchar](500) NULL,[Custom24] [varchar](500) NULL,[Custom25] [varchar](500) NULL,[Custom26] [varchar](500) NULL,[Custom27] [varchar](500) NULL,"
                            + "[Custom28] [varchar](500) NULL,[Custom29] [varchar](500) NULL,[Custom30] [varchar](500) NULL,[Custom31] [varchar](500) NULL,[Custom32] [varchar](500) NULL,"
                            + "[Custom33] [varchar](500) NULL,[Custom34] [varchar](500) NULL,[Custom35] [varchar](500) NULL,[Custom36] [varchar](500) NULL,[Custom37] [varchar](500) NULL,"
                            + "[Custom38] [varchar](500) NULL,[Custom39] [varchar](500) NULL,[Custom40] [varchar](500) NULL,[Custom41] [varchar](500) NULL,[Custom42] [varchar](500) NULL,"
                            + "[Custom43] [varchar](500) NULL,[Custom44] [varchar](500) NULL,[Custom45] [varchar](500) NULL,[Custom46] [varchar](500) NULL,[Custom47] [varchar](500) NULL,"
                            + "[Custom48] [varchar](500) NULL,[Custom49] [varchar](500) NULL,[Custom50] [varchar](500) NULL,[Custom51] [varchar](500) NULL,[Custom52] [varchar](500) NULL,"
                            + "[Custom53] [varchar](500) NULL,[Custom54] [varchar](500) NULL,[Custom55] [varchar](500) NULL,[Custom56] [varchar](500) NULL,[Custom57] [varchar](500) NULL,"
                            + "[Custom58] [varchar](500) NULL,[Custom59] [varchar](500) NULL,[Custom60] [varchar](500) NULL,[Custom61] [varchar](500) NULL,[Custom62] [varchar](500) NULL,"
                            + "[Custom63] [varchar](500) NULL,[Custom64] [varchar](500) NULL,[Custom65] [varchar](500) NULL,[Custom66] [varchar](500) NULL,[Custom67] [varchar](500) NULL,"
                            + "[Custom68] [varchar](500) NULL,[Custom69] [varchar](500) NULL,[Custom70] [varchar](500) NULL,[Custom71] [varchar](500) NULL,[Custom72] [varchar](500) NULL,"
                            + "[Custom73] [varchar](500) NULL,[Custom74] [varchar](500) NULL,[Custom75] [varchar](500) NULL,[Custom76] [varchar](500) NULL,[Custom77] [varchar](500) NULL,"
                            + "[Custom78] [varchar](500) NULL,[Custom79] [varchar](500) NULL,[Custom80] [varchar](500) NULL,[Custom81] [varchar](500) NULL,[Custom82] [varchar](500) NULL,"
                            + "[Custom83] [varchar](500) NULL,[Custom84] [varchar](500) NULL,[Custom85] [varchar](500) NULL,[Custom86] [varchar](500) NULL,[Custom87] [varchar](500) NULL,"
                            + "[Custom88] [varchar](500) NULL,[Custom89] [varchar](500) NULL,[Custom90] [varchar](500) NULL,[Custom91] [varchar](500) NULL,[Custom92] [varchar](500) NULL,"
                            + "[Custom93] [varchar](500) NULL,[Custom94] [varchar](500) NULL,[Custom95] [varchar](500) NULL,[Custom96] [varchar](500) NULL,[Custom97] [varchar](500) NULL,"
                            + "[Custom98] [varchar](500) NULL,[Custom99] [varchar](500) NULL,[Custom100] [varchar](500) NULL,[Created By] [varchar](100) NULL,[Date Of Item Created] [datetime] NOT NULL,"
                            + "[Modified By] [varchar](100) NULL,[Date of Modification] [datetime] NOT NULL,PRIMARY KEY CLUSTERED ([LSCode] ASC,[LSFirmCode] ASC,[Table] ASC,[pKey] ASC,[Batch Id] ASC));";
                        globalVar._DestinationConfig.ExecuteRawQuery(globalVar, lsTable, 1, out iiSuccess, out lsReturn, 0);
                        aiSuccess = iiSuccess;
                        if (iiSuccess == 0)
                        {
                            globalVar.setMessageLog(asLogFileName, lsReturn, globalVar.giCmd);
                        }
                    }
                }
                else
                {
                    globalVar.setMessageLog(asLogFileName, lsCnt, globalVar.giCmd);
                    return "Failed";
                }

                //LS_User_Table_Access Check
                globalVar._DestinationConfig.ExecuteRawQuery(globalVar, "Select Count(*) From sys.tables where [name] = 'LS_User_Table_Access'",
                    0, out iiSuccess, out lsCnt, 0);
                aiSuccess = iiSuccess;
                if (int.TryParse(lsCnt, out _))
                {
                    if (int.Parse(lsCnt) == 0)
                    {
                        lsTable = "CREATE TABLE [dbo].[LS_User_Table_Access]([application_code] [varchar](6) NOT NULL,[User_code] [varchar](10) NOT NULL,[table_name] [varchar](500) NOT NULL,"
                            + "[Access] [int] NOT NULL,[Upload_Download] [int] NOT NULL,[Active] [int] NOT NULL,[Created By] [varchar](100) NULL,[Date Of Item Created] [datetime] NULL,"
                            + "[Modified By] [varchar](100) NULL,[Date Of Modification] [datetime] NULL,PRIMARY KEY CLUSTERED ([application_code] ASC,[User_code] ASC,[table_name] ASC));";
                        globalVar._DestinationConfig.ExecuteRawQuery(globalVar, lsTable, 1, out iiSuccess, out lsReturn, 0);
                        aiSuccess = iiSuccess;
                        if (iiSuccess == 0)
                        {
                            globalVar.setMessageLog(asLogFileName, lsReturn, globalVar.giCmd);
                        }
                    }
                }
                else
                {
                    globalVar.setMessageLog(asLogFileName, lsCnt, globalVar.giCmd);
                    return "Failed";
                }


                return "Success";
            }
            catch (Exception Ex)
            {
                aiSuccess = 0;
                globalVar.setMessageLog(asLogFileName, Ex.Message, globalVar.giCmd);
                return "Failed";
            }
        }
    }
}
