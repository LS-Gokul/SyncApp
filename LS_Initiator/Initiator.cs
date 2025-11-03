using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Text.Json;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Drawing;

namespace LS_Initiator
{
    public partial class Initiator : Form
    {
        private int iiClose = 0, iiSuccess, iiInterval = 60, iiCount = 0; //Time in Seconds
        private Timer timer;
        private ContextMenuStrip contextMenu;
        private string masterApiURL = "https://api.leapsurgebi.com/api/AppData", isQuery, isReturn, isCustCode;
        private SyncSchedule[] _SyncSchedule;
        private DefaultReports[] _DefaultReports;

        public Initiator()
        {
            try
            {
                InitializeComponent();
                if (Process.GetProcessesByName("LS_Initiator").Length > 0) Application.Exit();
                //SetStartup();
                GetLoginDet(out iiSuccess);
                GetSyncList();
                InitializeTimer();
                InitializeNotifyIcon();
            }
            catch { }
        }

        private void Initiator_Load(object sender, EventArgs e)
        {
            
        }

        private void InitializeTimer()
        {
            try
            {
                timer = new Timer();
                timer.Interval = iiInterval * 1000; // Time in Milliseconds
                timer.Tick += Timer_Tick;
                timer.Start();
            }
            catch
            {

            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (isCustCode == "" || isCustCode == null)
                {
                    GetLoginDet(out _);
                }

                if (isCustCode == "" || isCustCode == null)
                {

                }
                else
                {
                    if (DateTime.Now.Minute % 10 == 0)
                    {
                        Process.Start(AppDomain.CurrentDomain.BaseDirectory + "LSEngine.exe", "-CheckStatus");
                        //File.AppendAllText(@"C:\Leapsurge\Sch.txt", AppDomain.CurrentDomain.BaseDirectory + "LSEngine.exe" + " -CheckStatus");
                        System.Threading.Thread.Sleep(10000);
                    }
                    for (int i = 0; i < _SyncSchedule.Length; i++)
                    {
                        if (_SyncSchedule[i].CurrentStatus != 1)
                        {
                            if (DateTime.Now.ToString("HH:mm") == _SyncSchedule[i].NextSyncTime.ToString("HH:mm"))
                            {
                                //File.AppendAllText(@"C:\Leapsurge\Sch.txt", AppDomain.CurrentDomain.BaseDirectory + "LSEngine.exe " + _SyncSchedule[i].SyncParam);
                                Process.Start(AppDomain.CurrentDomain.BaseDirectory + "LSEngine.exe", _SyncSchedule[i].SyncParam);
                                System.Threading.Thread.Sleep(10000);
                                _SyncSchedule[i].NextSyncTime = DateTime.Now.AddMinutes(_SyncSchedule[i].SyncInterval);
                                //_SyncSchedule[i].CurrentStatus = 1;
                            }
                        }
                    }

                    if (DateTime.Now.Minute % 60 == 0)
                    {
                        GetSyncList();
                    }
                }
            }
            catch
            {

            }
        }

        private void InitializeNotifyIcon()
        {
            try
            {
                Initiate.Visible = true;
                Initiate.DoubleClick += logger_DoubleClick;

                contextMenu = new ContextMenuStrip();
                contextMenu.Items.Add("Open", null, OpenMenu_Click);
                contextMenu.Items.Add("Quit", null, ExitMenu_Click);

                Initiate.ContextMenuStrip = contextMenu;

                this.Resize += Initiator_Resize;
                this.FormClosing += Initiator_Closing;
                this.ShowInTaskbar = false;
                this.WindowState = FormWindowState.Minimized;
            }
            catch
            {

            }
        }

        private void logger_DoubleClick(object sender, EventArgs e)
        {
            ShowForm();
        }

        private void OpenMenu_Click(object sender, EventArgs e)
        {
            ShowForm();
        }

        private void ExitMenu_Click(object sender, EventArgs e)
        {
            try
            {
                iiClose = 1;
                Initiate.Visible = false;
                Application.Exit();
            }
            catch
            {

            }
        }

        private void Initiator_Resize(object sender, EventArgs e)
        {
            try
            {
                if (this.WindowState == FormWindowState.Minimized)
                {
                    this.Hide();
                }
            }
            catch
            {

            }
        }

        private void ShowForm()
        {
            try
            {
                this.Show();
                this.WindowState = FormWindowState.Maximized;
                double liWidth = double.Parse(this.Width.ToString()) / 100.00;
                double liHeight = double.Parse(this.Height.ToString()) / 100.00;

                splitContainer1.SplitterDistance = int.Parse(Math.Ceiling(liWidth * 30.00).ToString());
                splitContainer2.SplitterDistance = int.Parse(Math.Ceiling(liHeight * 50.00).ToString());
                splitContainer5.SplitterDistance = int.Parse(Math.Ceiling(liHeight * 20.00).ToString());

                double panelHeight = double.Parse(splitContainer5.SplitterDistance.ToString()) / 100.00;
                double panelWidth = double.Parse(splitContainer5.Width.ToString()) / 100.00;
                splitContainer6.SplitterDistance = int.Parse(Math.Ceiling(panelHeight * 40.00).ToString());
                splitContainer7.SplitterDistance = int.Parse(Math.Ceiling(panelWidth * 90.00).ToString());
                splitContainer8.SplitterDistance = splitContainer8.Height / 2;

                double dgvDetWidth = double.Parse(dgvDet.Width.ToString()) / 100.00;
                dgvDet.Columns[0].Width = int.Parse(Math.Ceiling(dgvDetWidth * 20.00).ToString());
                dgvDet.Columns[1].Width = int.Parse(Math.Ceiling(dgvDetWidth * 20.00).ToString());
                dgvDet.Columns[2].Width = int.Parse(Math.Ceiling(dgvDetWidth * 20.00).ToString());
                dgvDet.Columns[3].Width = int.Parse(Math.Ceiling(dgvDetWidth * 20.00).ToString());
                dgvDet.Columns[4].Width = int.Parse(Math.Ceiling(dgvDetWidth * 20.00).ToString());


                double dgvSchPWidth = double.Parse(dgvSchedulers.Width.ToString()) / 100.00;
                dgvSchedulers.Rows.Clear();
                dgvSchedulers.Columns[0].Width = int.Parse(Math.Ceiling(dgvSchPWidth * 50.00).ToString());
                dgvSchedulers.Columns[1].Width = int.Parse(Math.Ceiling(dgvSchPWidth * 50.00).ToString());
                dgvSchedulers.Columns[2].Width = int.Parse(Math.Ceiling(dgvSchPWidth * 50.00).ToString());
                dgvSchedulers.Columns[3].Width = int.Parse(Math.Ceiling(dgvSchPWidth * 50.00).ToString());
                dgvSchedulers.Columns[4].Width = int.Parse(Math.Ceiling(dgvSchPWidth * 50.00).ToString());
                dgvSchedulers.Columns[5].Width = int.Parse(Math.Ceiling(dgvSchPWidth * 50.00).ToString());
                dgvSchedulers.Columns[6].Width = int.Parse(Math.Ceiling(dgvSchPWidth * 50.00).ToString());

                splitContainer1.Panel1.Region = Region.FromHrgn(createRoundRect(0, 0, splitContainer1.Panel1.Width, splitContainer1.Panel1.Height, 20, 20));
                splitContainer1.Panel2.Region = Region.FromHrgn(createRoundRect(0, 0, splitContainer1.Panel2.Width, splitContainer1.Panel2.Height, 20, 20));

                splitContainer2.Panel1.Region = Region.FromHrgn(createRoundRect(0, 0, splitContainer2.Panel1.Width, splitContainer2.Panel1.Height, 20, 20));
                splitContainer2.Panel2.Region = Region.FromHrgn(createRoundRect(0, 0, splitContainer2.Panel2.Width, splitContainer2.Panel2.Height, 20, 20));

                splitContainer5.Panel1.Region = Region.FromHrgn(createRoundRect(0, 0, splitContainer5.Panel1.Width, splitContainer5.Panel1.Height, 20, 20));
                splitContainer5.Panel2.Region = Region.FromHrgn(createRoundRect(0, 0, splitContainer5.Panel2.Width, splitContainer5.Panel2.Height, 20, 20));

                splitContainer6.Panel1.Region = Region.FromHrgn(createRoundRect(0, 0, splitContainer6.Panel1.Width, splitContainer6.Panel1.Height, 20, 20));
                splitContainer6.Panel2.Region = Region.FromHrgn(createRoundRect(0, 0, splitContainer6.Panel2.Width, splitContainer6.Panel2.Height, 20, 20));

                splitContainer7.Panel1.Region = Region.FromHrgn(createRoundRect(0, 0, splitContainer7.Panel1.Width, splitContainer7.Panel1.Height, 20, 20));
                splitContainer7.Panel2.Region = Region.FromHrgn(createRoundRect(0, 0, splitContainer7.Panel2.Width, splitContainer7.Panel2.Height, 20, 20));

                splitContainer8.Panel1.Region = Region.FromHrgn(createRoundRect(0, 0, splitContainer8.Panel1.Width, splitContainer8.Panel1.Height, 20, 20));
                splitContainer8.Panel2.Region = Region.FromHrgn(createRoundRect(0, 0, splitContainer8.Panel2.Width, splitContainer8.Panel2.Height, 20, 20));

                int j;
                string lsCurState, lsInter;
                for (int i = 0; i < _SyncSchedule.Length; i++)
                {
                    j = dgvSchedulers.Rows.Add();
                    
                    dgvSchedulers.Rows[j].Cells["ReportName"].Value = _SyncSchedule[i].ReportName;
                    dgvSchedulers.Rows[j].Cells["FirmName"].Value = _SyncSchedule[i].FirmName;
                    dgvSchedulers.Rows[j].Cells["ReportCode"].Value = _SyncSchedule[i].ReportCode;
                    dgvSchedulers.Rows[j].Cells["FirmCode"].Value = _SyncSchedule[i].FirmCode;
                    dgvSchedulers.Rows[j].Cells["StartTime"].Value = _SyncSchedule[i].StartTime;

                    lsCurState = (_SyncSchedule[i].CurrentStatus == 0 ? "Not Started"
                        : _SyncSchedule[i].CurrentStatus == 1 ? "Running"
                        : _SyncSchedule[i].CurrentStatus == 2 ? "Failed"
                        : _SyncSchedule[i].CurrentStatus == 3 ? "Completed" : "NA");
                    lsInter = (_SyncSchedule[i].SyncInterval / 60 > 0
                        ? $"{double.Parse(_SyncSchedule[i].SyncInterval.ToString()) / 60.00} Hrs"
                        : $"{_SyncSchedule[i].SyncInterval} Mins");

                    dgvSchedulers.Rows[j].Cells["SyncInterval"].Value = lsInter;
                    dgvSchedulers.Rows[j].Cells["CurrentStatus"].Value = lsCurState;
                    dgvSchedulers.Rows[j].Cells["LastSyncTime"].Value = _SyncSchedule[i].LastSyncTime;
                    dgvSchedulers.Rows[j].Cells["NextSyncTime"].Value = _SyncSchedule[i].NextSyncTime;

                    if(i == 0)
                    {
                        label1.Text = _SyncSchedule[i].ReportName;
                        dgvDet.Rows.Clear();
                        int j1 = dgvDet.Rows.Add();
                        btnEnable((lsCurState == "Running" ? false : true));
                        dgvDet.Rows[j1].Cells["SyncTime"].Value = _SyncSchedule[i].StartTime;
                        dgvDet.Rows[j1].Cells["SyncInter"].Value = lsInter;
                        dgvDet.Rows[j1].Cells["CurrentState"].Value = lsCurState;
                        dgvDet.Rows[j1].Cells["LastSyncOn"].Value = _SyncSchedule[i].LastSyncTime;
                        dgvDet.Rows[j1].Cells["NextSyncOn"].Value = _SyncSchedule[i].NextSyncTime;
                        dgvDet.Rows[j1].Cells["RepoCode"].Value = _SyncSchedule[i].ReportCode;
                        dgvDet.Rows[j1].Cells["SyncParam"].Value = _SyncSchedule[i].SyncParam;
                        dgvDet.Rows[j1].Cells["FCode"].Value = _SyncSchedule[i].FirmCode;
                    }
                }
            }
            catch
            {

            }
        }

        private void SetStartup()
        {
            try
            {
                RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                rk.SetValue("BI_Initiator", Application.ExecutablePath);
            }
            catch
            {

            }
        }

        private void btnSync_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvDet.Rows.Count > 0)
                {
                    string lsParam = dgvDet.Rows[0].Cells["SyncParam"].Value.ToString();
                    if (lsParam == "" || lsParam == null) return;

                    btnEnable(false);
                    Process.Start(AppDomain.CurrentDomain.BaseDirectory + "LSEngine.exe", lsParam + "~NO~");
                    dgvDet.Rows[0].Cells["CurrentState"].Value = "Running";
                    MessageBox.Show("Process has been started");
                    btnEnable(true);
                }
            }
            catch
            {

            }
        }

        private void btnEnable(bool abEnable)
        {
            btnSync.Enabled = abEnable;
            btnFullSync.Enabled = abEnable;
        }

        private void dgvSchedulers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int i = e.RowIndex;
            if(e.RowIndex >= 0)
            {
                GetSyncList(dgvSchedulers.Rows[i].Cells["ReportCode"].Value.ToString(), 
                    1, dgvDet, dgvSchedulers.Rows[i].Cells["FirmCode"].Value.ToString());
            }
        }

        private void Initiator_Closing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (iiClose == 1)
                {
                    return;
                }
                e.Cancel = true; // Cancel the close event
                this.WindowState = FormWindowState.Minimized; // Minimize the form
            }
            catch
            {

            }

        }


        public string postAPICalling(string aContentType, string bodyContent,
            out string statusCode, int aiType = 0, string authentication = null, string Header2 = null)
        {
            string Url = String.Format(masterApiURL + (aiType == 1 ? "/Insert/1" : ""));
            statusCode = "No";
            try
            {
                int timeOut = 900000;
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Url);

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12;
                req.KeepAlive = false;
                req.Method = "Post";
                //req.Accept = "application/json";

                //Adding Headers
                if (aContentType != null && aContentType != "") req.ContentType = aContentType;
                if (Header2 != null && Header2 != "") req.Headers.Add(Header2.Substring(0, Header2.IndexOf(":")).Trim(),
                            Header2.Substring(Header2.IndexOf(":") + 1).Trim());
                if (authentication != null && authentication != "") req.Headers.Add(HttpRequestHeader.Authorization, authentication);

                req.Timeout = timeOut;
                string datavalue = bodyContent.Replace(Environment.NewLine, " ");

                using (var strWr = new StreamWriter(req.GetRequestStream()))
                {
                    strWr.Write(datavalue);
                    strWr.Flush();
                    strWr.Close();

                    var respon = (HttpWebResponse)req.GetResponse();
                    statusCode = respon.StatusCode.ToString();
                    Stream stream = respon.GetResponseStream();
                    StreamReader sr = new StreamReader(stream);
                    string ApiResult = sr.ReadToEnd();
                    return ApiResult;
                }
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                return "Failed - API Calling " + e.Message;
            }
        }

        private void GetSyncList(string asRptCode = "", int aiDw = 0, DataGridView advg = null, string asFirmCode = "")
        {
            try
            {
                isQuery = "Select rptMst.[Report Name] as rptName,syncSet.LSCode as lsCode, syncSet.firm_code as firmCode, "
                    + "   syncSet.[Start Time] as stTime, syncSet.[Sync Interval] as syncInter, syncSet.[Current Status] as curStat, "
                    + "   syncSet.[Last Sync Time] as lastSyncTime, syncSet.[Next Sync Time] as nextSyncTime, "
                    + "   Trim(Replace(rptMst.[Sync Param],'-All','')) + '~' + syncSet.firm_code as syncParam, "
                    + "   rptMst.report_code as rptCode, fdet.[Firm Name] as firmName "
                    + " From LS_Sync_Setup syncSet, LS_Report_Master rptMst, LS_Report_Config rptConfig, LS_Customer_Firm_Details fdet "
                    + " Where syncSet.report_code = rptMst.report_code And "
                    + "     syncSet.[Active Flag] = 1 And syncSet.LSCode = rptConfig.LSCode And "
                    + "     syncSet.firm_code = rptConfig.firm_code And syncSet.report_code = rptConfig.report_code And "
                    + "     syncSet.LSCode = fdet.LSCode And syncSet.firm_code = fdet.firm_code And "
                    + $"    rptMst.Active = 1 And syncSet.LSCode = '{isCustCode}' and rptMst.[Sync Param] Is Not Null "
                    + (asRptCode == "" ? "" : $" And rptMst.report_code = '{asRptCode}' ")
                    + (asFirmCode == "" ? "" : $" And syncSet.firm_code = '{asFirmCode}' ");
                isReturn = postAPICalling("application/x-www-form-urlencoded", isQuery, out string lsResult);
                if(lsResult.ToLower() == "ok")
                {
                    JsonElement _SyncList = createJsonElement(isReturn);
                    int liCount = _SyncList.EnumerateArray().Count();
                    if (liCount > 0)
                    {
                        _SyncSchedule = null;
                        iiCount = liCount;
                        _SyncSchedule = new SyncSchedule[iiCount];
                        if (aiDw == 1)
                        {
                            string lsCurState;
                            advg.Rows.Clear();
                            for (int i = 0; i < iiCount; i++)
                            {

                                lsCurState = (_SyncList[i].GetProperty("curStat").ToString() == "0" ? "Not Started"
                                    : _SyncList[i].GetProperty("curStat").ToString() == "1" ? "Running"
                                    : _SyncList[i].GetProperty("curStat").ToString() == "2" ? "Failed"
                                    : _SyncList[i].GetProperty("curStat").ToString() == "3" ? "Completed" : "NA");
                                
                                label1.Text = _SyncList[i].GetProperty("rptName").ToString();
                                btnEnable((lsCurState == "Running" ? false : true));
                                int j = advg.Rows.Add();

                                advg.Rows[j].Cells["RepoCode"].Value = _SyncList[i].GetProperty("rptCode").ToString();
                                advg.Rows[j].Cells["FCode"].Value = _SyncList[i].GetProperty("firmCode").ToString();
                                advg.Rows[j].Cells["SyncTime"].Value = _SyncList[i].GetProperty("stTime").ToString();

                                int liSyncInter = int.Parse(_SyncList[i].GetProperty("syncInter").ToString());
                                advg.Rows[j].Cells["SyncInter"].Value = liSyncInter / 60 > 0
                                        ? $"{double.Parse(liSyncInter.ToString()) / 60.00} Hrs" : $"{liSyncInter} Mins";
                                advg.Rows[j].Cells["CurrentState"].Value = lsCurState;
                                
                                advg.Rows[j].Cells["LastSyncOn"].Value = (DateTime.TryParse(_SyncList[i].GetProperty("lastSyncTime").ToString(), out _) ?
                                       DateTime.Parse(_SyncList[i].GetProperty("lastSyncTime").ToString()) : DateTime.Now);

                                DateTime ldtStart = DateTime.Parse(_SyncList[i].GetProperty("stTime").ToString());
                                int liInter = int.Parse(_SyncList[i].GetProperty("syncInter").ToString());
                                advg.Rows[j].Cells["NextSyncOn"].Value = NextRunTime(ldtStart, liInter, _SyncList[i].GetProperty("nextSyncTime").ToString());

                                advg.Rows[j].Cells["SyncParam"].Value = _SyncList[i].GetProperty("syncParam").ToString();
                            }
                        }
                        else
                        {
                            for (int i = 0; i < iiCount; i++)
                            {
                                _SyncSchedule[i] = new SyncSchedule();

                                _SyncSchedule[i].LSCode = _SyncList[i].GetProperty("lsCode").ToString();
                                _SyncSchedule[i].FirmCode = _SyncList[i].GetProperty("firmCode").ToString();
                                _SyncSchedule[i].FirmName = _SyncList[i].GetProperty("firmName").ToString();
                                _SyncSchedule[i].ReportName = _SyncList[i].GetProperty("rptName").ToString();
                                _SyncSchedule[i].ReportCode = _SyncList[i].GetProperty("rptCode").ToString();
                                _SyncSchedule[i].StartTime = DateTime.Parse(_SyncList[i].GetProperty("stTime").ToString());
                                _SyncSchedule[i].SyncInterval = int.Parse(_SyncList[i].GetProperty("syncInter").ToString());
                                _SyncSchedule[i].CurrentStatus = int.Parse(_SyncList[i].GetProperty("curStat").ToString());
                                _SyncSchedule[i].LastSyncTime = (DateTime.TryParse(_SyncList[i].GetProperty("lastSyncTime").ToString(), out _) ?
                                       DateTime.Parse(_SyncList[i].GetProperty("lastSyncTime").ToString()) : DateTime.Now);
                                _SyncSchedule[i].SyncParam = _SyncList[i].GetProperty("syncParam").ToString();
                                _SyncSchedule[i].NextSyncTime = NextRunTime(_SyncSchedule[i].StartTime, _SyncSchedule[i].SyncInterval,
                                    _SyncList[i].GetProperty("nextSyncTime").ToString());
                            }
                        }
                    }
                }
                
            }
            catch
            {

            }   
        }

        private DateTime NextRunTime(DateTime adStartTime, int aiSyncInter, string asNextRunTime)
        {
            /*
            DateTime _ldtTemnp = DateTime.Parse(_SyncSchedule[i].StartTime.ToString("hh:mm"));
            // (Current Time - Start Time) / Sync Interval
            int llDiff = int.Parse(Math.Ceiling(DateTime.Now.Subtract(_ldtTemnp).TotalMinutes / 
                (_SyncSchedule[i].SyncInterval > 0 ? double.Parse(_SyncSchedule[i].SyncInterval.ToString()) : 1.00)).ToString());
            _SyncSchedule[i].NextSyncTime = _ldtTemnp.AddMinutes(llDiff * _SyncSchedule[i].SyncInterval);
            */
            DateTime _ldtTemnp = DateTime.Parse(adStartTime.ToString("HH:mm"));
            try
            {
                if (asNextRunTime == "")
                {
                    // Difference in No. of Times = (Current Time - Start Time) / Sync Interval
                    int llDiff = int.Parse(Math.Ceiling(DateTime.Now.Subtract(_ldtTemnp).TotalMinutes /
                        (aiSyncInter > 0 ? double.Parse(aiSyncInter.ToString()) : 1.00)).ToString());
                    //NextSyncTime = Start Time + No. of Minutes(Difference in No. of Times * Sync Interval)
                    _ldtTemnp = _ldtTemnp.AddMinutes(llDiff * aiSyncInter);

                }
                else
                {
                    try
                    {
                        if (DateTime.Now > DateTime.Parse(asNextRunTime))
                        {
                            _ldtTemnp = NextRunTime(adStartTime, aiSyncInter, "");
                        }
                        else
                        {
                            _ldtTemnp = DateTime.Parse(asNextRunTime);
                        }
                    }
                    catch
                    {
                        _ldtTemnp = DateTime.Now;
                    }
                }
            }
            catch
            {
                _ldtTemnp = DateTime.Now;
            }
            return _ldtTemnp;
        }

        //////////////////////////////////////////////////////////////////////////////
        //Creating the Parse string of the JSON Value
        //////////////////////////////////////////////////////////////////////////////
        public JsonElement createJsonElement(string jsonString)
        {
            JsonElement jsonTblArray = new JsonElement();
            try
            {
                //jsonString = jsonString.Replace("\\", "");
                if (jsonString.Substring(0, 1) == "\"")
                {
                    jsonString = jsonString.Substring(1, jsonString.Length - 2);
                }
                if (jsonString.Substring(0, 1) != "[")
                {
                    jsonString = "[" + jsonString + "]";
                }
                JsonDocument jsonStrList = JsonDocument.Parse(jsonString);
                jsonTblArray = jsonStrList.RootElement;
                return jsonTblArray;
            }
            catch
            {
                return jsonTblArray;
            }
        }

        private void GetLoginDet(out int aiSuccess)
        {
            string CacheFilePath = "LSEngine.exe.lscache.bin3";
            aiSuccess = 0;
            try
            {
                if (File.Exists(CacheFilePath))
                {
                    var token = File.ReadAllText(CacheFilePath);
                    var handler = new JwtSecurityTokenHandler();
                    var jsonToken = handler.ReadToken(token);
                    var tokenS = jsonToken as JwtSecurityToken;
                    var jti = tokenS.Claims.First(claim => claim.Type == "user_details").Value;
                    
                    Token _Token = new Token();
                    _Token = JsonSerializer.Deserialize<Token>(jti);

                    isCustCode = _Token.LSCode;
                    aiSuccess = 1;
                }
            }
            catch 
            {
                
            }
        }

        private IntPtr createRoundRect(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse)
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
        //private void Update

    }

    public class SyncSchedule
    {
        public string LSCode { get; set; }
        public string FirmCode { get; set; }
        public string FirmName { get; set; }
        public string ReportCode { get; set; }
        public string ReportName { get; set; }
        public DateTime StartTime { get; set; }
        public int SyncInterval { get; set; }
        public int CurrentStatus { get; set; }
        public DateTime LastSyncTime { get; set; }
        public DateTime NextSyncTime { get; set; }
        public string SyncParam { get; set; }
    }

    public class Token
    {
        public string user_code { get; set; }
        public string LSCode { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Email { get; set; }
        public string Country_Calling_Code { get; set; }
        public string Mobile { get; set; }
        public string Telephone { get; set; }
        public string Approved_By { get; set; }
        public int? Isactive { get; set; }
        public int? ISAdministrator { get; set; }
        public int? Access_Sync_Application { get; set; }
        public string user_group_code { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string city_code { get; set; }
        public string state_code { get; set; }
        public string country_code { get; set; }
        public int? Postal_Code { get; set; }
        public string AAD_User_Id { get; set; }
        public string Created_User { get; set; }
        public string Modified_User { get; set; }
        public string Page_Theme { get; set; }
        public string Theme_Color { get; set; }
        public int? Access_MDM { get; set; }
        public int? IsLicensed { get; set; }
        public int? Config_Flag { get; set; }
        public int? MFA { get; set; }
        public string Default_Firm { get; set; }
        public string Zoho_Contact_ID { get; set; }
        public int? is_support_admin { get; set; }
        public int? Default_Spoc { get; set; }
        public int? isgroup { get; set; }
        public string Mobile_Key { get; set; }
    }

    public class DefaultReports
    {
        public string ReportCode { get; set; }
        public string FirmCode { get; set;}
        public int Sync { get; set;}
    }
}
