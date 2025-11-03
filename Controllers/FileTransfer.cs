using System;
using System.Data;
using System.IO;
using OfficeOpenXml;
using System.Linq;
//using System.Net;
using System.Text.Json;
using System.Collections.Generic;
using Microsoft.VisualBasic.FileIO;
//using static System.Net.WebRequestMethods;
using FluentFTP;
using System.Net;
using Windows.Media.SpeechRecognition;

namespace LSSyncApp.Controllers
{
    public class FileTransfer
    {
        public static ODBCSyncParam _OdbcSyncParam = new ODBCSyncParam();
        public static string isReturn, isParam, isFinColName, isStatus, isSqlQuery;
        public static int rCnt, iiSyncType, iiCmd, iiSuccess, iiRetryCount = 5, iiBr;
        private bool disposedValue;

        public AuditLogVar init(ODBCSyncParam osp, IProgress<int> progress, string asParam, int aiSyncType, string asType)
        {
            _OdbcSyncParam = osp;
            _OdbcSyncParam = osp;
            isParam = asParam;
            iiSyncType = aiSyncType;
            iiCmd = _OdbcSyncParam.odbcGlobalVar.giCmd;

            _OdbcSyncParam._auditLogVar.LogId = _OdbcSyncParam.isLogTime;
            _OdbcSyncParam._auditLogVar.StartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//Set Audit Log Var
            if (iiCmd == 0)
            {
                if (progress != null) progress.Report(1);//Progress Bar
            }
            string lsRet;
            if (asType == "FTP")
                lsRet = FTP(progress);
            else if (asType == "SFTP")
                //lsRet = SFTP(progress);
                lsRet = "Failed - No Source Found";
            else
                lsRet = "Failed - No Source Found";

            if (iiCmd == 0)
            {
                if (progress != null) progress.Report(100);//Progress Bar
            }
            if (lsRet.Contains("Failed"))
            {
                _OdbcSyncParam._auditLogVar.Object = "Sync";//Set Audit Log Var
                _OdbcSyncParam._auditLogVar.ChildObject = "FTP/SFTP-Sync";//Set Audit Log Var
                _OdbcSyncParam._auditLogVar.Sequence = 1;//Set Audit Log Var
                _OdbcSyncParam._auditLogVar.ObjectFromTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//Set Audit Log Var
                _OdbcSyncParam._auditLogVar.EndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//Set Audit Log Var
                _OdbcSyncParam._auditLogVar.LogDetails = lsRet;//Set Audit Log Var
                _OdbcSyncParam._auditLogVar.Status = "Failed";//Set Audit Log Var
                _OdbcSyncParam.setStatusLog("", "", 2);
            }
            return _OdbcSyncParam._auditLogVar;
        }

        private void GetTableList(out string asResult, out int aiSuccess)
        {
            aiSuccess = 0;
            try
            {
                _OdbcSyncParam.odbcGlobalVar._MasterConfig.GetFTPTableList(_OdbcSyncParam.odbcGlobalVar, 
                        int.Parse(_OdbcSyncParam.isSeq), iiSyncType, isParam, out aiSuccess, out asResult);
            }
            catch(Exception Ex)
            {
                asResult = Ex.Message;
            }
        }

        private string FTP(IProgress<int> progress)
        {
            try
            {
                GetTableList(out isReturn, out iiSuccess);
                if(iiSuccess == 0)
                {
                    return isReturn;
                }

                JsonElement ljeTblList = _OdbcSyncParam.odbcGlobalVar.createJsonElement(isReturn);
                int liCnt = ljeTblList.EnumerateArray().Count();
                int liProcessPerLoop = 100 / (liCnt == 0 ? 1 : liCnt);
                int liProcessInLoop = liProcessPerLoop / 4, liFIleType = 0;

                for (int i = 0; i < liCnt; i++)
                {
                    if (iiCmd == 0)
                    {
                        if (progress != null) progress.Report((i * liProcessPerLoop) + (liProcessInLoop));
                    }

                    string lsTableName = ljeTblList[i].GetProperty("tblName").ToString();
                    string lsRemoteFilePath = ljeTblList[i].GetProperty("filePath").ToString();
                    //if (lsRemoteFilePath != "/LeapSurge/Secondary_Sales_Details/SKPL_SecondarySales_report.csv") continue;
                    int liColType = int.Parse(ljeTblList[i].GetProperty("colType").ToString());
                    string lsColumn = ljeTblList[i].GetProperty("col").ToString();

                    _OdbcSyncParam.setStatusLog("tblName", lsTableName, 1);

                    //string ftpUrl = $"ftp://{_OdbcSyncParam.ip}:{_OdbcSyncParam.port}{lsRemoteFilePath}";

                    //Get File Name
                    string lsLocalFile = _OdbcSyncParam.odbcGlobalVar.reverseString(lsRemoteFilePath);
                    string lsFileType = lsLocalFile.Substring(0, lsLocalFile.IndexOf("."));
                    lsFileType = _OdbcSyncParam.odbcGlobalVar.reverseString(lsFileType);
                    liFIleType = (lsFileType == "csv" ? 1 : (lsFileType == "xlsx" || lsFileType == "xls" ? 2 : 0));/* 1 = CSV, 2 = XLSX */
                    if (lsLocalFile.Contains("/"))
                    {
                        lsLocalFile = lsLocalFile.Substring(0, lsLocalFile.IndexOf("/"));
                        lsLocalFile = _OdbcSyncParam.odbcGlobalVar.reverseString(lsLocalFile);
                    }
                    else if (lsLocalFile.Contains("\\"))
                    {
                        lsLocalFile = lsLocalFile.Substring(0, lsLocalFile.IndexOf("\\"));
                        lsLocalFile = _OdbcSyncParam.odbcGlobalVar.reverseString(lsLocalFile);
                    }
                    else
                    {
                        lsLocalFile = "data.csv";
                    }
                    try
                    {
                        if (System.IO.File.Exists(_OdbcSyncParam.odbcGlobalVar.gsApplPath + $"Files\\{lsLocalFile}"))
                            System.IO.File.Delete(_OdbcSyncParam.odbcGlobalVar.gsApplPath + $"Files\\{lsLocalFile}");
                    }
                    catch(Exception fileEx)
                    {
                        _OdbcSyncParam.setStatusLog("status", fileEx.Message, 1);
                        continue;
                    }
                    string localFilePath = _OdbcSyncParam.odbcGlobalVar.gsApplPath + $"Files\\{lsLocalFile}";
                    _OdbcSyncParam.setStatusLog("status", "File Fetching Start", 1);
                    try
                    {
                        var config = new FluentFTP.FtpConfig
                        {
                            ConnectTimeout = 15000,
                            ReadTimeout = 60000,
                            DataConnectionReadTimeout = 60000,
                            EncryptionMode = FtpEncryptionMode.None,   // Optional, if plain FTP
                            ValidateAnyCertificate = true,             // Optional, only for FTPS
                        };
                        var client = new FtpClient(_OdbcSyncParam.ip, int.Parse(_OdbcSyncParam.port), config);

                        //FtpClient client = new FtpClient(_OdbcSyncParam.ip);
                        //client.Port = int.Parse(_OdbcSyncParam.port);
                        client.Credentials = new System.Net.NetworkCredential(_OdbcSyncParam.isODBCUID, _OdbcSyncParam.isODBCPwd);
                        client.Connect();
                        client.DownloadFile(localFilePath, lsRemoteFilePath);
                        client.Disconnect();
                    }
                    catch(Exception ExFtp)
                    {
                        GC.Collect();
                        _OdbcSyncParam.setStatusLog("status", ExFtp.Message, 1);
                        _OdbcSyncParam.setStatusLog("status", ExFtp.StackTrace, 1);
                        _OdbcSyncParam.auditLogReset(2);
                        continue;
                    }
                    /*
                    try
                    {
                        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUrl);
                        request.Method = WebRequestMethods.Ftp.DownloadFile;
                        request.Credentials = new NetworkCredential(_OdbcSyncParam.isODBCUID, _OdbcSyncParam.isODBCPwd);
                        request.UsePassive = true;
                        request.UseBinary = true;
                        request.EnableSsl = false; // FTP (not FTPS)
                        request.Timeout = 60000;
                        request.Proxy = null;

                        using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                        {
                            _OdbcSyncParam.setStatusLog("status", response.StatusCode.ToString(), 1);
                            _OdbcSyncParam.setStatusLog("status", response.StatusDescription, 1);
                            _OdbcSyncParam.setStatusLog("status", localFilePath, 1);
                            _OdbcSyncParam.setStatusLog("status", ftpUrl, 1);
                            using (Stream responseStream = response.GetResponseStream())
                            {
                                
                                _OdbcSyncParam.setStatusLog("status", responseStream.Length.ToString(), 1);
                                using (FileStream outputStream = new FileStream(localFilePath, FileMode.Create))
                                {
                                    responseStream.CopyTo(outputStream);
                                }
                            }
                        }
                        
                    }
                    catch(Exception ExFtp)
                    {
                        GC.Collect();
                        _OdbcSyncParam.setStatusLog("status", ExFtp.Message, 1);
                        _OdbcSyncParam.setStatusLog("status", ExFtp.StackTrace, 1);
                        _OdbcSyncParam.auditLogReset(2);
                        continue;
                    }
                    */
                    _OdbcSyncParam.setStatusLog("status", "File Fetching Completed Successfully", 1);

                    if (iiCmd == 0)
                    {
                        if (progress != null) progress.Report((i * liProcessPerLoop) + (liProcessInLoop * 2));
                    }

                    // Step 2: Load into DataTable
                    //DataTable dt = LoadCSVToDT(localFilePath, out isReturn);

                    _OdbcSyncParam.setStatusLog("status", "Starting File Processing", 1);
                    var dt = new DataTable();
                    
                    if(liFIleType == 1)
                    {
                        dt = LoadCsvToDataTable(localFilePath, out isReturn);
                    }
                    else if(liFIleType == 2)
                    {
                        dt = ImportExcelToDataTable(localFilePath, out isReturn);
                    }
                    else
                    {
                        return "Failed - File Type not Found";
                    }

                    if (isReturn != "")
                    {
                        GC.Collect();
                        _OdbcSyncParam.setStatusLog("status", isReturn, 1);
                        _OdbcSyncParam.auditLogReset(2);
                        continue;
                    }
                    _OdbcSyncParam.setStatusLog("status", "File Processing Completed", 1);
                    string lsPkeyCols = "";

                    if (liColType == 1)
                    {
                        _OdbcSyncParam.getColList(lsTableName, _OdbcSyncParam.isCustCode, _OdbcSyncParam.isFirmCode, out lsPkeyCols,
                            out _, out _, out _, out _, out _, out isReturn, out _, out _);
                    }

                    int liRowsCnt = dt.Rows.Count;

                    if (iiCmd == 0)
                    {
                        if (progress != null) progress.Report((i * liProcessPerLoop) + (liProcessInLoop * 3));
                    }

                    _OdbcSyncParam.setStatusLog("s", liRowsCnt.ToString(), 1);
                    if (liRowsCnt <= 0) continue;

                    isReturn = BulkDeleteInsertFile(dt, lsTableName, (liColType == 1 ? lsPkeyCols : lsColumn),
                        liColType, out iiSuccess);
                }
            }
            catch (Exception ex)
            {
                return "Failed - " + ex.Message;
            }
            return "";
        }

        public string BulkDeleteInsertFile(DataTable adtTable, string asTableName,
            string asColList, int aiType, out int aiSuccess)
        {
            aiSuccess = 0;
            try
            {
                string lsQuery = "";
                _OdbcSyncParam.setStatusLog("status", "Temp Insert", 1);
                _OdbcSyncParam.odbcGlobalVar._DestinationConfig.ExecuteRawQuery(_OdbcSyncParam.odbcGlobalVar,
                            $"If Exists (Select * from sys.tables where name = '{asTableName}_TmpApp') Begin Drop table [{asTableName}_TmpApp]; End;",
                            1, out iiSuccess, out isReturn, 1, 0);

                _OdbcSyncParam.odbcGlobalVar._DestinationConfig.ExecuteRawQuery(_OdbcSyncParam.odbcGlobalVar,
                        $"Select * Into [{asTableName}_TmpApp] from [{asTableName}] Where 1 = 2",
                        1, out iiSuccess, out isReturn, 1, 0);

                if (iiSuccess == 0)
                {
                    _OdbcSyncParam.setStatusLog("status", isReturn, 1);
                    _OdbcSyncParam.auditLogReset(4);
                    return "Failed";
                }

                isReturn = _OdbcSyncParam.BulkInsert($"{asTableName}_TmpApp", adtTable);
                if (isReturn.Contains("Failed"))
                {
                    _OdbcSyncParam.setStatusLog("status", isReturn, 1);
                    _OdbcSyncParam.auditLogReset(4);
                    return "Failed";
                }
                _OdbcSyncParam.setStatusLog("status", "Data Processing", 1);
                switch (aiType)
                {
                    case 0: //Do Nothing
                        break;
                    case 1: //Delete using Primary Key
                        lsQuery = $"Delete a From [{asTableName}] a Join [{asTableName}_TmpApp] new_a On {asColList}";
                        break;
                    case 2: //Delete using the columns specified
                        string colList = "";
                        int i = 0;
                        asColList = asColList + ",";
                        while (asColList.IndexOf(",") > 0)
                        {
                            int liIndex = asColList.IndexOf(",");
                            string colName = asColList.Substring(0, liIndex);
                            colList += (i == 0 ? "" : " And ") +  $"a.[{colName}] = new_a.[{colName}]";
                            asColList = asColList.Substring(liIndex + 1);
                        }
                        lsQuery = $"Delete a From [{asTableName}] a Join [{asTableName}_TmpApp] new_a On {colList}";
                        break;
                    case 3: //Fetch the max Value using the column specified
                        //lsQuery = $"Select Max({asColList}) as val From {asTableName};";
                        break;
                    case 4: //Truncate the table
                        lsQuery = $"Truncate Table [{asTableName}];";
                        break;
                }
                _OdbcSyncParam.setStatusLog("status", "Insert Start", 1);
                if (aiType == 0 || aiType == 2 || aiType == 4)
                {
                    _OdbcSyncParam.odbcGlobalVar._DestinationConfig.ExecuteRawQuery(_OdbcSyncParam.odbcGlobalVar,
                        lsQuery, 1, out iiSuccess, out isReturn, 1, 0);
                    lsQuery = $"Insert Into [{asTableName}] Select * from [{asTableName}_TmpApp];";
                }
                else if (aiType == 3)
                {
                    lsQuery = $"Insert Into [{asTableName}] Select * from [{asTableName}_TmpApp] Where [{asColList}] > IsNull((Select Max([{asColList}]) as val From [{asTableName}]),'1900-01-01');";
                }

                _OdbcSyncParam.odbcGlobalVar._DestinationConfig.ExecuteRawQuery(_OdbcSyncParam.odbcGlobalVar,
                        lsQuery, 1, out iiSuccess, out isReturn, 1, 0);
                if (isReturn.Contains("Failed"))
                {
                    _OdbcSyncParam.setStatusLog("status", "Insert Failed - " + isReturn, 1);
                    _OdbcSyncParam.auditLogReset(4);
                    return "Failed";
                }

                _OdbcSyncParam.odbcGlobalVar._DestinationConfig.ExecuteRawQuery(_OdbcSyncParam.odbcGlobalVar, $"Drop Table [{asTableName}_TmpApp]", 1, out iiSuccess, out isReturn, 1, 0);
                if (isReturn.Contains("Failed"))
                {
                    _OdbcSyncParam.setStatusLog("status", isReturn, 1);
                    _OdbcSyncParam.auditLogReset(4);
                    return "Failed";
                }
                _OdbcSyncParam.setStatusLog("status", "Done", 1);
                aiSuccess = 1;
                return "Success";
            }
            catch (Exception Ex)
            {
                return $"Failed - {Ex.Message}";
            }
        }

        /*
        private string SFTP(IProgress<int> progress)
        {
            try
            {
                string host = "115.243.160.133";
                int port = 2124;
                string username = "Leapsurge";
                string password = "St0ve@1999";
                string remoteFilePath = "/Master_Table/SKPL_DealerMaster.csv";
                string localFilePath = Path.Combine(_OdbcSyncParam.odbcGlobalVar.gsApplPath, "\\Files\\data.csv");

                var connectionInfo = new ConnectionInfo(host, port, username,
                    new PasswordAuthenticationMethod(username, password))
                {
                    Timeout = TimeSpan.FromSeconds(90) // Increase from 30s to 90s
                };

                // Step 1: Download file from SFTP
                using (var sftp = new SftpClient(connectionInfo))
                {
                    sftp.Connect();
                    using (var file = File.OpenWrite(localFilePath))
                    {
                        sftp.DownloadFile(remoteFilePath, file);
                    }
                    sftp.Disconnect();
                }

                // Step 2: Load file into DataTable
                DataTable dt = LoadCSVToDT(localFilePath, out isReturn);
                
                // Step 3: Check the posibilities and Insert
                
            }
            catch (Exception Ex)
            {
                return "Failed - " + Ex.Message;
            }
            return "";
        }
        */



        ///////////////////////////////////////////////
        //Excel Fill Processing
        ///////////////////////////////////////////////
        public DataTable ImportExcelToDataTable(string filePath, out string asReturn)
        {
            var dt = new DataTable();
            asReturn = "";
            try
            {
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // First worksheet
                    bool hasHeader = true; // Assumes first row is header
                    foreach (var firstRowCell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                    {
                        dt.Columns.Add(hasHeader ? firstRowCell.Text : $"Column {firstRowCell.Start.Column}");
                    }

                    var startRow = hasHeader ? 2 : 1;
                    for (int rowNum = startRow; rowNum <= worksheet.Dimension.End.Row; rowNum++)
                    {
                        var wsRow = worksheet.Cells[rowNum, 1, rowNum, worksheet.Dimension.End.Column];
                        var row = dt.NewRow();
                        foreach (var cell in wsRow)
                        {
                            row[cell.Start.Column - 1] = cell.Text;
                        }
                        dt.Rows.Add(row);
                    }
                }
            }
            catch (Exception Ex)
            {
                asReturn = "Failed - " + Ex.Message;
            }
            return dt;
        }

        ///////////////////////////////////////////////
        //CSV Fill Processing
        ///////////////////////////////////////////////
        public static DataTable LoadCsvToDataTable(string filePath, out string asReturn)
        {
            var dt = new DataTable();
            asReturn = "";
            try
            {
                using (TextFieldParser parser = new TextFieldParser(filePath))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(","); // Change this if you use semicolon or tab
                    parser.HasFieldsEnclosedInQuotes = true;
                    parser.TrimWhiteSpace = true;

                    bool headersRead = false;

                    while (!parser.EndOfData)
                    {
                        string[] fields = parser.ReadFields();

                        if (!headersRead)
                        {
                            foreach (string header in fields)
                            {
                                dt.Columns.Add(header);
                            }
                            headersRead = true;
                        }
                        else
                        {
                            object[] rowValues = new object[fields.Length];

                            for (int i = 0; i < fields.Length; i++)
                            {
                                rowValues[i] = (fields[i].Trim() == "" ? null : fields[i].Trim());
                            }
                            dt.Rows.Add(rowValues);
                        }
                    }
                }
            }
            catch(Exception Ex)
            {
                asReturn = "Failed - " + Ex.Message;
            }
            return dt;
        }

        //////////////////////////////////////////////////
        //Converting File to Datatable
        //////////////////////////////////////////////////
        private DataTable LoadCSVToDT(string filePath, out string asReturn)
        {
            asReturn = "";
            DataTable dataTable = new DataTable();
            try
            {
                string[] lines = System.IO.File.ReadAllLines(filePath);

                if (lines.Length > 0)
                {
                    // Adding columns to DataTable from first line (header)
                    string[] headers = lines[0].Split(',');
                    foreach (string header in headers)
                    {
                        dataTable.Columns.Add(header);
                    }

                    // Adding rows to DataTable
                    int r = 0;
                    foreach (string line in lines.Skip(1))
                    {
                        r += 1;
                        string[] data = ParseCsvLine(line, out isReturn);
                        if (isReturn != "")
                        {
                            asReturn = isReturn;
                            return null;
                        }
                        dataTable.Rows.Add(data);
                    }
                }
                return dataTable;
            }
            catch (Exception Ex)
            {
                asReturn = Ex.Message;
                return null;
            }
        }

        //////////////////////////////////////////////////
        //Parcing CSV File Line by Line
        //////////////////////////////////////////////////
        private string[] ParseCsvLine(string line, out string asResult)
        {
            List<string> result = new List<string>();
            bool insideQuotes = false;
            string currentValue = "";
            asResult = "";
            try
            {
                for (int i = 0; i < line.Length; i++)
                {
                    char c = line[i];

                    if (c == '"' && (i == 0 || line[i - 1] != '\\')) // Check for quotes (ignore escaped quotes)
                    {
                        insideQuotes = !insideQuotes;
                    }
                    else if (c == ',' && !insideQuotes) // If comma outside quotes, split the value
                    {
                        result.Add((currentValue == "" ? null : currentValue.Trim()));
                        currentValue = "";
                    }
                    else
                    {
                        currentValue += c;
                    }
                }
                // Add the last value to the list
                result.Add((currentValue == "" ? null : currentValue.Trim()));
            }
            catch (Exception ex)
            {
                asResult = ex.Message;
            }
            return result.ToArray();
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    GC.Collect();
                }

                disposedValue = true;
            }
        }
    }
}
