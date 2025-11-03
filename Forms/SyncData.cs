using LSSyncApp.Controllers;
using LSSyncApp.Functions;
using OfficeOpenXml;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LSSyncApp.Forms
{
    public partial class SyncData : Form
    {
        public int rCnt = 0, iiCY, iiChkErrFile = 0, iiSeq = 0, iiCurIdx;
        public string isReturn;
        public static string lsWhat, isSqlString;
        public static int i = 0, b1, b2, rIDX, iiSyncType = 0, iiSuccess;//, iiSyncCount = 0;
        private SyncParams[] _SyncParams = new SyncParams[30];
        public static SyncData _Sync;
        public static MainForm _MDISync;
        //readonly Thread[] thread = new Thread[20];
        //readonly Task[] task = new Task[20];
        //private BackgroundWorker[] bgSync = new BackgroundWorker[30];
        private Task[] bgTask = new Task[30];
        //private CancellationTokenSource[] tokenSource = new CancellationTokenSource[30];

        public SyncData(MainForm mDISync)
        {
            InitializeComponent();
            _Sync = this;
            _MDISync = mDISync;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _Sync.progressBar1.Visible = true;
            Scheduler sch = new Scheduler();
            var progress = new Progress<int>(v =>
            {
                _Sync.progressBar1.Value = v;
            });
            string lsSchRet = sch.init(_MDISync.mdiGlobalVar, progress);
            MessageBox.Show(lsSchRet);
            _Sync.progressBar1.Value = 0;
            _Sync.progressBar1.Visible = false;
        }

        private void Test()
        {
            DataGridViewTextBoxColumn colReport = new DataGridViewTextBoxColumn
            {
                Name = "Report",
                HeaderText = "Report",
                ReadOnly = true
            };

            DataGridViewProgressColumn colProgress = new DataGridViewProgressColumn
            {
                Name = "Progress",
                HeaderText = "Progress",
                ReadOnly = true
            };

            DataGridViewDisableButtonColumn colCustSyncButton = new DataGridViewDisableButtonColumn
            {
                Name = "CustomSync",
                HeaderText = "",
                ReadOnly = true
            };

            DataGridViewDisableButtonColumn colSyncButton = new DataGridViewDisableButtonColumn
            {
                Name = "SyncButton",
                HeaderText = "",
                ReadOnly = true
            };

            DataGridViewDisableButtonColumn colCancelButton = new DataGridViewDisableButtonColumn
            {
                Name = "CancelButton",
                HeaderText = "",
                ReadOnly = true
            };

            DataGridViewTextBoxColumn colStatus = new DataGridViewTextBoxColumn
            {
                Name = "Status",
                HeaderText = "Status",
                ReadOnly = true
            };

            DataGridViewTextBoxColumn colSeq = new DataGridViewTextBoxColumn
            {
                Name = "Seq",
                HeaderText = "Seq",
                ReadOnly = true,
                Visible = false
            };

            DataGridViewTextBoxColumn colParams = new DataGridViewTextBoxColumn
            {
                Name = "Params",
                HeaderText = "Params",
                ReadOnly = true,
                Visible = false
            };

            DataGridViewTextBoxColumn colRptCode = new DataGridViewTextBoxColumn
            {
                Name = "rptCode",
                HeaderText = "rptCode",
                ReadOnly = true,
                Visible = false
            };

            DataGridViewTextBoxColumn colSel = new DataGridViewTextBoxColumn
            {
                Name = "Sel",
                HeaderText = "Sel",
                ReadOnly = true,
                Visible = false
            };

            dgvTest.Columns.AddRange(new DataGridViewColumn[] { colReport, colProgress, colCustSyncButton, colSyncButton, 
                    colCancelButton, colStatus, colSeq, colParams, colRptCode, colSel });
            //loadReport(1, dgvTest);
        }

        private void SyncData_PostLoad(object sender, EventArgs e)
        {
            int liPanelWidth = (this.Width / 100) * 100, liPanelHeight = (this.Height / 100) * 100, liDgvWidth;
            mainPanel.Size = new Size(liPanelWidth, liPanelHeight);
            mainPanel.Location = new Point((this.Width - mainPanel.Width) / 2, (this.Height - mainPanel.Height) / 2);

            liDgvWidth = liPanelWidth / 100;
            gbType.Size = new Size(liDgvWidth * 99, gbType.Height);

            comboBox1.Text = _MDISync.mdiGlobalVar.maxDaysToSync.ToString();

            /*dgvModule.Size = new Size(liDgvWidth * 99, (liPanelHeight / 100) * 93);
            dgvModule.Columns[0].Width = liDgvWidth * 40;
            dgvModule.Columns[1].Width = liDgvWidth * 18;
            dgvModule.Columns[2].Width = liDgvWidth * 10;
            dgvModule.Columns[3].Width = liDgvWidth * 10;
            dgvModule.Columns[4].Width = liDgvWidth * 20;*/
            Test();
            dgvModule.Visible = false;
            dgvTest.Size = new Size(liDgvWidth * 99, ((liPanelHeight - gbType.Height) / 100) * 98);
            dgvTest.Columns[0].Width = liDgvWidth * 30;
            dgvTest.Columns[1].Width = liDgvWidth * 18;
            dgvTest.Columns[2].Width = liDgvWidth * 10;
            dgvTest.Columns[3].Width = liDgvWidth * 10;
            dgvTest.Columns[4].Width = liDgvWidth * 10;
            dgvTest.Columns[5].Width = liDgvWidth * 20;

            rbModuleWise.Checked = true;
            
            mainPanel.Region = Region.FromHrgn(_MDISync.mdiGlobalVar.createRoundRect(0, 0, mainPanel.Width, mainPanel.Height, 50, 50));
            dgvTest.Region = Region.FromHrgn(_MDISync.mdiGlobalVar.createRoundRect(0, 0, dgvTest.Width, dgvTest.Height, 50, 50));

            lbCancel.FlatStyle = FlatStyle.Flat;
            lbCancel.FlatAppearance.BorderSize = 0;
            lbCancel.FlatAppearance.BorderColor = Color.FromArgb(0, 255, 255, 255);

            button1.FlatStyle = FlatStyle.Flat;
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatAppearance.BorderColor = Color.FromArgb(0, 255, 255, 255);

            _Sync.progressBar1.Visible = false;
            dgvTest.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            
            //Set Theme Color
            _MDISync.mdiGlobalVar._Theme.SetTheme(this, _MDISync.mdiGlobalVar.giTheme);
        }


        private void rbReportWise_CheckedChanged(object sender, EventArgs e)
        {
            rbModuleWise.ForeColor = Color.FromArgb(220, 221, 225);
            rbReportWise.ForeColor = Color.FromArgb(89, 131, 250);
            loadReport(2, dgvTest);
        }

        private void rbModuleWise_CheckedChanged(object sender, EventArgs e)
        {
            rbModuleWise.ForeColor = Color.FromArgb(89, 131, 250);
            rbReportWise.ForeColor = Color.FromArgb(220, 221, 225);
            loadReport(1, dgvTest);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int seq;
            string lsParam;

            seq = int.Parse(((DataGridView)(sender)).Rows[e.RowIndex].Cells["Seq"].Value.ToString());
            lsParam = ((DataGridView)(sender)).Rows[e.RowIndex].Cells["Params"].Value.ToString();
            switch (((DataGridView)(sender)).Columns[e.ColumnIndex].Name)
            {
                case "SyncButton":
                    ButtonEnable(e.RowIndex, (DataGridView)sender, false);
                    ((DataGridView)(sender)).Rows[e.RowIndex].Cells["Status"].Value = "Started";
                    ((DataGridView)(sender)).Rows[e.RowIndex].Cells["Status"].Style = new DataGridViewCellStyle { ForeColor = Color.White, BackColor = Color.FromArgb(41, 41, 75) };
                    ((DataGridView)(sender)).Refresh();
                    rIDX = e.RowIndex;
                    RunMulitpleThread(seq, "Sync", lsParam, ((DataGridView)(sender)));
                    break;
                case "CancelButton":
                    RunMulitpleThread1(seq, "Cancel", e.RowIndex.ToString(), ((DataGridView)(sender)));
                    ((DataGridView)(sender)).Rows[e.RowIndex].Cells["Status"].Value = "Stopped";
                    ((DataGridView)(sender)).Rows[e.RowIndex].Cells["Status"].Style = new DataGridViewCellStyle { ForeColor = Color.FromArgb(250, 89, 89), BackColor = Color.FromArgb(41, 41, 75) };
                    ((DataGridView)(sender)).Refresh();
                    break;
                case "CustomSync":
                    SetTime _SetTime = new SetTime(_MDISync.mdiGlobalVar, lsParam, iiSyncType);
                    _SetTime.ShowDialog();
                    if(_SetTime.iiRet == 1)
                    {
                        dataGridView1_CellContentClick((DataGridView)sender, new DataGridViewCellEventArgs(3, e.RowIndex));
                    }
                    break;
            }
        }

        private void bSyncDays_Click(object sender, EventArgs e)
        {
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string lsSyncDays = ((ComboBox)sender).Text;
            if (lsSyncDays == "" || lsSyncDays == null)
            {

            }
            else
            {
                _MDISync.mdiGlobalVar.maxDaysToSync = int.TryParse(lsSyncDays, out _) ? int.Parse(lsSyncDays) : 7;
            }
        }

        private void mainPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void SyncData_Load(object sender, EventArgs e)
        {

        }

        private void lbCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        private void AboutClosing(object sender, FormClosingEventArgs e)
        {
            MenuSettings.EnableMenuItem(this.MdiParent, "dataSync");
        }

        Task LoadBackGroundAsync(int aiSeqNo, CancellationToken ct)
        {
            return Task.Run(() => 
            {
                ct.ThrowIfCancellationRequested();
                RunMulitpleThread1(_SyncParams[aiSeqNo].aiSeqNo, _SyncParams[aiSeqNo].asType, _SyncParams[aiSeqNo].asParam, _SyncParams[aiSeqNo].dgv);
            }, ct);
        }

        private async void RunMulitpleThread(int aiSeqNo, string asType, string asParam, DataGridView dgv)
        {
            /*bgSync[aiSeqNo] = new BackgroundWorker();
            this.bgSync[aiSeqNo].DoWork += new DoWorkEventHandler(this.bgList_DoWork);
            this.bgSync[aiSeqNo].RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.bgList_RunWorkerCompleted);

            bgSync[aiSeqNo].RunWorkerAsync();
            */
            _SyncParams[aiSeqNo] = new SyncParams
            {
                aiSeqNo = aiSeqNo,
                asType = asType,
                asParam = asParam,
                dgv = dgv,
                ts = new CancellationTokenSource()
            };
            iiCurIdx = aiSeqNo;
            bgTask[aiSeqNo] = LoadBackGroundAsync(aiSeqNo, _SyncParams[aiSeqNo].ts.Token);
            await bgTask[aiSeqNo];
            //task[aiSeqNo] = RunMulitpleThread1(aiSeqNo, asType, asParam, dgv);
            //task[aiSeqNo].Start();
            return;
            //return (task[aiSeqNo]).Run(() => RunMulitpleThread1(aiSeqNo, asType, asParam, dgv));
        }

        private void bgList_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Thread.Sleep(1000);
                RunMulitpleThread1(_SyncParams[iiCurIdx].aiSeqNo, _SyncParams[iiCurIdx].asType, _SyncParams[iiCurIdx].asParam, _SyncParams[iiCurIdx].dgv);
            }
            catch(Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
            
        }
        void bgList_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }
        private void RunMulitpleThread1(int aiSeqNo, string asType, string asParam, DataGridView dgv)
        {
            switch (asType)
            {
                case "Sync":
                    iiSeq++;
                    sourceConfiguration(rIDX, asParam, dgv);
                    //thread[aiSeqNo] = new Thread(() => sourceConfiguration(rIDX, asParam, dgv));
                    //thread[aiSeqNo].Start();
                    break;
                case "Cancel":
                    iiSeq--;
                    if (((DataGridViewDisableButtonCell)dgv.Rows[int.Parse(asParam)].Cells["CancelButton"]).Enabled)
                    {
                        //bgSync[aiSeqNo].CancelAsync();
                        _SyncParams[aiSeqNo].ts.Cancel();
                        //tokenSource[aiSeqNo].Cancel();
                        //task[aiSeqNo].Cancel();
                        //thread[aiSeqNo].Abort();
                        ButtonEnable(int.Parse(asParam), dgv, true);
                    }
                    break;
                case "Completed":
                    iiSeq--;
                    ButtonEnable(aiSeqNo, dgv, true);
                    dgv.Rows[aiSeqNo].Cells["Status"].Value = "Completed";
                    dgv.Rows[aiSeqNo].Cells["Status"].Style = new DataGridViewCellStyle { ForeColor = Color.FromArgb(89, 250, 159), BackColor = Color.FromArgb(41, 41, 75) };
                    dgv.Rows[aiSeqNo].Cells["SyncButton"].Style = new DataGridViewCellStyle { ForeColor = Color.FromArgb(41, 41, 75), BackColor = Color.FromArgb(250, 202, 89) };
                    break;
            }
            MoDRptEnable();
            Thread.Sleep(2000);
            //return task[aiSeqNo];
        }

        private void ButtonEnable(int aiSeqNo, DataGridView dgv, bool abEnable)
        {
            ((DataGridViewDisableButtonCell)dgv.Rows[aiSeqNo].Cells["CancelButton"]).Enabled = !abEnable;
            ((DataGridViewDisableButtonCell)dgv.Rows[aiSeqNo].Cells["SyncButton"]).Enabled = abEnable;
            ((DataGridViewDisableButtonCell)dgv.Rows[aiSeqNo].Cells["CustomSync"]).Enabled = abEnable;
        }

        private void MoDRptEnable()
        {
            rbModuleWise.Enabled = iiSeq > 0 ? false : true;
            rbReportWise.Enabled = iiSeq > 0 ? false : true;
        }

        private void loadReport(int aiType, DataGridView dgv)
        {
            string lsParam;
            try
            {
                dgv.Rows.Clear();

                _MDISync.mdiGlobalVar._MasterConfig.GetReportList(_MDISync.mdiGlobalVar, aiType.ToString(), out int liSuccess, out isReturn);

                if (liSuccess == 1 && isReturn != "" && isReturn != null)
                {
                    JsonElement ljeRptList = new JsonElement();
                    ljeRptList = _MDISync.mdiGlobalVar.createJsonElement(isReturn);
                    isReturn = "";
                    rCnt = ljeRptList.EnumerateArray().Count();

                    for (int repModCnt = 0; repModCnt < rCnt; repModCnt++)
                    {
                        var seq = dgv.Rows.Add();
                        dgv.Rows[seq].Cells["Sel"].Value = 0;
                        ((DataGridViewDisableButtonCell)dgv.Rows[seq].Cells["CancelButton"]).Value = "Cancel";
                        ((DataGridViewDisableButtonCell)dgv.Rows[seq].Cells["CancelButton"]).FlatStyle = FlatStyle.Flat;
                        dgv.Rows[seq].Cells["CancelButton"].Style = new DataGridViewCellStyle { 
                            ForeColor = Color.White, BackColor = Color.FromArgb(250, 89, 89),
                            Font = new Font("Times New Roman", 9, FontStyle.Italic | FontStyle.Bold)
                        };
                        
                        ((DataGridViewDisableButtonCell)dgv.Rows[seq].Cells["SyncButton"]).Value = "Sync";
                        ((DataGridViewDisableButtonCell)dgv.Rows[seq].Cells["SyncButton"]).FlatStyle = FlatStyle.Flat;
                        dgv.Rows[seq].Cells["SyncButton"].Style = new DataGridViewCellStyle {
                            ForeColor = Color.FromArgb(41, 41, 75), BackColor = Color.FromArgb(250, 202, 89),
                            Font = new Font("Times New Roman", 9, FontStyle.Italic | FontStyle.Bold)
                        };
                        
                        ((DataGridViewDisableButtonCell)dgv.Rows[seq].Cells["CustomSync"]).Value = "Full Sync";
                        ((DataGridViewDisableButtonCell)dgv.Rows[seq].Cells["CustomSync"]).FlatStyle = FlatStyle.Flat;
                        dgv.Rows[seq].Cells["CustomSync"].Style = new DataGridViewCellStyle { 
                            ForeColor = Color.FromArgb(41, 41, 75), BackColor = Color.FromArgb(95, 208, 104),
                            Font = new Font("Times New Roman", 9, FontStyle.Italic | FontStyle.Bold)
                        };

                        ButtonEnable(seq, dgv, true);

                        dgv.Rows[seq].Cells["Status"].Value = "";
                        dgv.Rows[seq].Cells["Status"].Style = new DataGridViewCellStyle { ForeColor = Color.White };
                        dgv.Rows[seq].Cells["Report"].Value = ljeRptList[repModCnt].GetProperty("modName").ToString();
                        lsParam = ljeRptList[repModCnt].GetProperty("params").ToString().Replace("-All", "").Trim();
                        dgv.Rows[seq].Cells["Params"].Value = lsParam;
                        dgv.Rows[seq].Cells["Seq"].Value = ljeRptList[repModCnt].GetProperty("seq").ToString();
                        dgv.Rows[seq].Cells["rptCode"].Value = ljeRptList[repModCnt].GetProperty("rptCodeList").ToString();

                        ((DataGridViewProgressCell)dgv.Rows[seq].Cells["Progress"]).Red = 89;
                        ((DataGridViewProgressCell)dgv.Rows[seq].Cells["Progress"]).Green = 250;
                        ((DataGridViewProgressCell)dgv.Rows[seq].Cells["Progress"]).Blue = 159;
                        dgv.Rows[seq].Cells["Progress"].Style = new DataGridViewCellStyle { ForeColor = Color.FromArgb(41, 41, 75) };
                    }
                    dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.LightBlue;
                }
            }
            catch
            {

            }
            iiSyncType = aiType;
        }

        public static string sourceConfiguration(int idx, string asParam, DataGridView dgv)
        {
            //var progress = new Progress<int>(v => 0.ToString());
            
            var progress = new Progress<int>(v => { dgv.Rows[idx].Cells["Progress"].Value = v.ToString();});
            
            ODBCSyncParam _ODBCSync = new ODBCSyncParam();
            _ODBCSync.sourceConfiguration("'" + asParam + "'", progress, _MDISync.mdiGlobalVar, iiSyncType);
            _Sync.RunMulitpleThread1(idx, "Completed", "",dgv);
            return "";
        }
    }

    public class SyncParams
    {
        public int aiSeqNo { get; set; }
        public string asType { get; set; }
        public string asParam { get; set; }
        public DataGridView dgv { get; set; }
        public CancellationTokenSource ts {get; set;}
    }
}