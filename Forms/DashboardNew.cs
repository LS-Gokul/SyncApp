using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace LSSyncApp.Forms
{
    public partial class DashboardNew : Form
    {
        private MainForm _MDISync;
        private Color _SetColor;
        private int iiSuccess;
        private string isReturn;

        public DashboardNew(MainForm mForm)
        {
            InitializeComponent();
            _MDISync = mForm;
        }

        private void DashboardNew_Load(object sender, EventArgs e)
        {
            DataFill();
        }

        private void DashboardNew_Shown(object sender, EventArgs e)
        {
            
        }

        private void DashboardNew_Resize(object sender, EventArgs e)
        {
            try
            {
                splitContainer1.SplitterWidth = 10;
                splitContainer2.SplitterWidth = 10;
                splitContainer3.SplitterWidth = 10;
                splitContainer4.SplitterWidth = 10;
                splitContainer5.SplitterWidth = 10;
                splitContainer6.SplitterWidth = 10;
                splitContainer7.SplitterWidth = 10;
                splitContainer8.SplitterWidth = 10;
                splitContainer9.SplitterWidth = 10;
                splitContainer10.SplitterWidth = 10;

                int liPanelWidth = (this.Width / 100) * 100, liPanelHeight = (this.Height / 100) * 100;
                mainPanel.Size = new Size(liPanelWidth, liPanelHeight);
                int liWidth = mainPanel.Width / 100, liHeight = mainPanel.Height / 100;

                mainPanel.Location = new Point((this.Width - mainPanel.Width) / 2, (this.Height - mainPanel.Height) / 2);

                lRows.Location = new Point(splitContainer4.Panel1.Width / 2 - lRows.Width / 2, splitContainer4.Panel1.Height / 2 - lRows.Height);
                label2.Location = new Point(splitContainer4.Panel1.Width / 2 - label2.Width / 2, lRows.Location.Y + lRows.Height);

                lReports.Location = new Point(splitContainer4.Panel2.Width / 2 - lReports.Width / 2, splitContainer4.Panel2.Height / 2 - lReports.Height);
                label4.Location = new Point(splitContainer4.Panel2.Width / 2 - label4.Width / 2, lReports.Location.Y + lReports.Height);

                mainPanel.Region = Region.FromHrgn(_MDISync.mdiGlobalVar.createRoundRect(0, 0, mainPanel.Width, mainPanel.Height, 50, 50));

                splitContainer1.Panel1.Region = Region.FromHrgn(_MDISync.mdiGlobalVar.createRoundRect(0, 0, splitContainer1.Panel1.Width, splitContainer1.Panel1.Height, 50, 50));
                splitContainer1.Panel2.Region = Region.FromHrgn(_MDISync.mdiGlobalVar.createRoundRect(0, 0, splitContainer1.Panel2.Width, splitContainer1.Panel2.Height, 50, 50));

                splitContainer2.Panel1.Region = Region.FromHrgn(_MDISync.mdiGlobalVar.createRoundRect(0, 0, splitContainer2.Panel1.Width, splitContainer2.Panel1.Height, 50, 50));
                splitContainer2.Panel2.Region = Region.FromHrgn(_MDISync.mdiGlobalVar.createRoundRect(0, 0, splitContainer2.Panel2.Width, splitContainer2.Panel2.Height, 50, 50));

                splitContainer3.Panel1.Region = Region.FromHrgn(_MDISync.mdiGlobalVar.createRoundRect(0, 0, splitContainer3.Panel1.Width, splitContainer3.Panel1.Height, 50, 50));
                splitContainer3.Panel2.Region = Region.FromHrgn(_MDISync.mdiGlobalVar.createRoundRect(0, 0, splitContainer3.Panel2.Width, splitContainer3.Panel2.Height, 50, 50));

                splitContainer4.Panel1.Region = Region.FromHrgn(_MDISync.mdiGlobalVar.createRoundRect(0, 0, splitContainer4.Panel1.Width, splitContainer4.Panel1.Height, 50, 50));
                splitContainer4.Panel2.Region = Region.FromHrgn(_MDISync.mdiGlobalVar.createRoundRect(0, 0, splitContainer4.Panel2.Width, splitContainer4.Panel2.Height, 50, 50));

                splitContainer5.Panel1.Region = Region.FromHrgn(_MDISync.mdiGlobalVar.createRoundRect(0, 0, splitContainer5.Panel1.Width, splitContainer5.Panel1.Height, 50, 50));
                splitContainer5.Panel2.Region = Region.FromHrgn(_MDISync.mdiGlobalVar.createRoundRect(0, 0, splitContainer5.Panel2.Width, splitContainer5.Panel2.Height, 50, 50));

                splitContainer6.Panel1.Region = Region.FromHrgn(_MDISync.mdiGlobalVar.createRoundRect(0, 0, splitContainer6.Panel1.Width, splitContainer6.Panel1.Height, 50, 50));
                splitContainer6.Panel2.Region = Region.FromHrgn(_MDISync.mdiGlobalVar.createRoundRect(0, 0, splitContainer6.Panel2.Width, splitContainer6.Panel2.Height, 50, 50));

                splitContainer7.Panel1.Region = Region.FromHrgn(_MDISync.mdiGlobalVar.createRoundRect(0, 0, splitContainer7.Panel1.Width, splitContainer7.Panel1.Height, 50, 50));
                splitContainer7.Panel2.Region = Region.FromHrgn(_MDISync.mdiGlobalVar.createRoundRect(0, 0, splitContainer7.Panel2.Width, splitContainer7.Panel2.Height, 50, 50));

                splitContainer8.Panel1.Region = Region.FromHrgn(_MDISync.mdiGlobalVar.createRoundRect(0, 0, splitContainer8.Panel1.Width, splitContainer8.Panel1.Height, 50, 50));
                splitContainer8.Panel2.Region = Region.FromHrgn(_MDISync.mdiGlobalVar.createRoundRect(0, 0, splitContainer8.Panel2.Width, splitContainer8.Panel2.Height, 50, 50));

                splitContainer9.Panel1.Region = Region.FromHrgn(_MDISync.mdiGlobalVar.createRoundRect(0, 0, splitContainer9.Panel1.Width, splitContainer9.Panel1.Height, 50, 50));
                splitContainer9.Panel2.Region = Region.FromHrgn(_MDISync.mdiGlobalVar.createRoundRect(0, 0, splitContainer9.Panel2.Width, splitContainer9.Panel2.Height, 50, 50));

                splitContainer10.Panel1.Region = Region.FromHrgn(_MDISync.mdiGlobalVar.createRoundRect(0, 0, splitContainer10.Panel1.Width, splitContainer10.Panel1.Height, 50, 50));
                splitContainer10.Panel2.Region = Region.FromHrgn(_MDISync.mdiGlobalVar.createRoundRect(0, 0, splitContainer10.Panel2.Width, splitContainer10.Panel2.Height, 50, 50));


                splitContainer1.SplitterDistance = int.Parse(((double.Parse((mainPanel.Height.ToString("#"))) / 100.00) * 35.00).ToString("#")) - 10;
                splitContainer2.SplitterDistance = int.Parse(((double.Parse((mainPanel.Width.ToString("#"))) / 100.00) * 35.00).ToString("#")) - 10;
                splitContainer3.SplitterDistance = splitContainer4.Panel2.Height - 10;
                //splitContainer4.SplitterDistance = 10;
                splitContainer5.SplitterDistance = int.Parse(((double.Parse((splitContainer1.Panel1.Height.ToString("#"))) / 100.00) * 25.00).ToString("#")) -10;
                splitContainer6.SplitterDistance = int.Parse(((double.Parse((splitContainer1.Panel1.Height.ToString("#"))) / 100.00) * 25.00).ToString("#")) - 10;
                splitContainer7.SplitterDistance = int.Parse(((double.Parse((splitContainer1.Panel1.Height.ToString("#"))) / 100.00) * 25.00).ToString("#")) - 10;
                splitContainer8.SplitterDistance = int.Parse(((double.Parse((splitContainer8.Height.ToString("#"))) / 100.00) * 35.00).ToString("#")) - 10;
                splitContainer9.SplitterDistance = (splitContainer8.Width - splitContainer9.Height) -10;
                splitContainer10.SplitterDistance = int.Parse(((double.Parse((splitContainer10.Width.ToString("#"))) / 100.00) * 35.00).ToString("#")) - 10;

                lMaster.Location = new Point(pbMaster.Location.X + pbMaster.Width, (pbMaster.Location.Y) + ((pbMaster.Height / 2) - (lMaster.Height)));
                lMaster1.Location = new Point(pbMaster.Location.X + pbMaster.Width, (pbMaster.Location.Y) + ((pbMaster.Height / 2) + 10));
                lMasterR.Location = new Point((pbMaster.Location.X + pMaster.Width) - lMasterR.Width - 10, (pMaster.Height / 2) - (lMasterR.Height / 2));

                lTran.Location = new Point(pbTran.Location.X + pbTran.Width, (pbTran.Location.Y) + ((pbTran.Height / 2) - (lTran.Height)));
                lTran1.Location = new Point(pbTran.Location.X + pbTran.Width, (pbTran.Location.Y) + ((pbTran.Height / 2) + 10));
                lTranR.Location = new Point((pbTran.Location.X + pTran.Width) - lTranR.Width - 10, (pTran.Height / 2) - (lTranR.Height / 2));

                lOut.Location = new Point(pbOut.Location.X + pbOut.Width, (pbOut.Location.Y) + ((pbOut.Height / 2) - (lOut.Height)));
                lOut1.Location = new Point(pbOut.Location.X + pbOut.Width, (pbOut.Location.Y) + ((pbOut.Height / 2) + 10));
                lOutR.Location = new Point((pbOut.Location.X + pOut.Width) - lOutR.Width - 10, (pOut.Height / 2) - (lOutR.Height / 2));

                lLog.Location = new Point(pbLog.Location.X + pbLog.Width, (pbLog.Location.Y) + ((pbLog.Height / 2) - (lLog.Height)));
                lLog1.Location = new Point(pbLog.Location.X + pbLog.Width, (pbLog.Location.Y) + ((pbLog.Height / 2) + 10));
                lLogR.Location = new Point((pbLog.Location.X + pLog.Width) - lLogR.Width - 10, (pLog.Height / 2) - (lLogR.Height / 2));

            }
            catch
            {

            }
        }


        private void Panel_MouseHower(object sender, EventArgs e)
        {
            try
            {
                _SetColor = ((Control)sender).BackColor;
                switch (((Control)sender).Name)
                {
                    case "pMaster":
                        ((Control)sender).BackColor = Color.FromArgb(111, 204, 115);
                        pMaster.BackColor = Color.FromArgb(111, 204, 115);
                        pbMaster.BackColor = Color.FromArgb(111, 204, 115);
                        lMaster.BackColor = Color.FromArgb(111, 204, 115);
                        lMaster1.BackColor = Color.FromArgb(111, 204, 115);
                        lMasterR.BackColor = Color.FromArgb(111, 204, 115);
                        break;
                    case "pTran":
                        ((Control)sender).BackColor = Color.FromArgb(113, 191, 252);
                        pTran.BackColor = Color.FromArgb(113, 191, 252);
                        pbTran.BackColor = Color.FromArgb(113, 191, 252);
                        lTran.BackColor = Color.FromArgb(113, 191, 252);
                        lTran1.BackColor = Color.FromArgb(113, 191, 252);
                        lTranR.BackColor = Color.FromArgb(113, 191, 252);
                        break;
                    case "pOut":
                        ((Control)sender).BackColor = Color.FromArgb(234, 127, 118);
                        pOut.BackColor = Color.FromArgb(234, 127, 118);
                        pbOut.BackColor = Color.FromArgb(234, 127, 118);
                        lOut.BackColor = Color.FromArgb(234, 127, 118);
                        lOut1.BackColor = Color.FromArgb(234, 127, 118);
                        lOutR.BackColor = Color.FromArgb(234, 127, 118);
                        break;
                    case "pLog":
                        ((Control)sender).BackColor = Color.FromArgb(255, 199, 116);
                        pLog.BackColor = Color.FromArgb(255, 199, 116);
                        pbLog.BackColor = Color.FromArgb(255, 199, 116);
                        lLog.BackColor = Color.FromArgb(255, 199, 116);
                        lLog1.BackColor = Color.FromArgb(255, 199, 116);
                        lLogR.BackColor = Color.FromArgb(255, 199, 116);
                        break;
                }
            }
            catch
            {

            }
        }

        private void Panel_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                ((Control)sender).BackColor = _SetColor;
                switch (((Control)sender).Name)
                {
                    case "pMaster":
                        pMaster.BackColor = _SetColor;
                        pbMaster.BackColor = _SetColor;
                        lMaster.BackColor = _SetColor;
                        lMaster1.BackColor = _SetColor;
                        lMasterR.BackColor = _SetColor;
                        break;
                    case "pTran":
                        pTran.BackColor = _SetColor;
                        pbTran.BackColor = _SetColor;
                        lTran.BackColor = _SetColor;
                        lTran1.BackColor = _SetColor;
                        lTranR.BackColor = _SetColor;
                        break;
                    case "pOut":
                        pOut.BackColor = _SetColor;
                        pbOut.BackColor = _SetColor;
                        lOut.BackColor = _SetColor;
                        lOut1.BackColor = _SetColor;
                        lOutR.BackColor = _SetColor;
                        break;
                    case "pLog":
                        pLog.BackColor = _SetColor;
                        pbLog.BackColor = _SetColor;
                        lLog.BackColor = _SetColor;
                        lLog1.BackColor = _SetColor;
                        lLogR.BackColor = _SetColor;
                        break;
                }
            }
            catch
            {

            }
        }

        private void DataFill()
        {
            /******************************************* Data Rows & Size ****************************************************/
            try
            {
                _MDISync.mdiGlobalVar._DestinationConfig.CheckTableListSize(_MDISync.mdiGlobalVar, out iiSuccess, out isReturn);
                if (iiSuccess == 1)
                {
                    int liRowCount;
                    long llMaster = 0, llTransaction = 0, llLog = 0, llOutstanding = 0;
                    double llMasterSize = 0, llTransactionSize = 0, llLogSize = 0, llOutstandingSize = 0;

                    DataTable lDTRows = _MDISync.mdiGlobalVar.GetJSONToDataTable(isReturn);
                    isReturn = "";

                    //dgvRows.DataSource = lDTRows;
                    liRowCount = lDTRows.Rows.Count;
                    DataGridViewTextBoxColumn objName = new DataGridViewTextBoxColumn
                    {
                        Name = "objName",
                        HeaderText = "Object Name",
                        ReadOnly = true
                    };
                    DataGridViewTextBoxColumn rows = new DataGridViewTextBoxColumn
                    {
                        Name = "rows",
                        HeaderText = "Rows",
                        ReadOnly = true
                    };
                    DataGridViewTextBoxColumn size = new DataGridViewTextBoxColumn
                    {
                        Name = "size",
                        HeaderText = "Size(MB)",
                        ReadOnly = true,
                        Visible = false
                    };
                    DataGridViewTextBoxColumn tblType = new DataGridViewTextBoxColumn
                    {
                        Name = "tblType",
                        HeaderText = "",
                        ReadOnly = true
                    };
                    dgvRows.Columns.AddRange(new DataGridViewColumn[] { objName, rows, size, tblType });
                    dgvRows.Columns.Cast<DataGridViewColumn>().ToList().ForEach(f => f.SortMode = DataGridViewColumnSortMode.NotSortable);

                    for (int j = 1; j <= 4; j++)
                    {
                        int liHeaderRow = dgvRows.Rows.Add();
                        dgvRows.Rows[liHeaderRow].Cells[0].Value = j == 1 ? "Master" : j == 2 ? "Log" : j == 3 ? "Outstanding" : "Transaction";

                        DataGridViewCellStyle boldStyle = new DataGridViewCellStyle();
                        boldStyle.Font = new Font("Ebrima", 8.25F, FontStyle.Bold);
                        dgvRows.Rows[liHeaderRow].DefaultCellStyle = boldStyle;
                        dgvRows.Rows[liHeaderRow].DefaultCellStyle.BackColor = Color.FromArgb(216, 216, 216);

                        for (int i = 0; i < liRowCount; i++)
                        {
                            if (lDTRows.Rows[i].ItemArray[3].ToString() == j.ToString())
                            {
                                int liCrRow = dgvRows.Rows.Add();
                                dgvRows.Rows[liCrRow].Cells[0].Value = lDTRows.Rows[i].ItemArray[0].ToString();
                                dgvRows.Rows[liCrRow].Cells[1].Value = double.Parse(lDTRows.Rows[i].ItemArray[1].ToString());
                                dgvRows.Rows[liCrRow].Cells[2].Value = double.Parse(lDTRows.Rows[i].ItemArray[2].ToString());
                                dgvRows.Rows[liCrRow].Cells[3].Value = lDTRows.Rows[i].ItemArray[3].ToString();
                            }

                            if (j == 1)
                            {
                                string lsObjName, lsRows, lsSize, lsType;
                                lsObjName = lDTRows.Rows[i].ItemArray[0].ToString();
                                lsRows = lDTRows.Rows[i].ItemArray[1].ToString();
                                lsSize = lDTRows.Rows[i].ItemArray[2].ToString();
                                lsType = lDTRows.Rows[i].ItemArray[3].ToString();

                                switch (lsType)
                                {
                                    case "1":
                                        llMaster += long.Parse(lsRows);
                                        llMasterSize += double.Parse(lsSize);
                                        break;
                                    case "2":
                                        llLog += long.Parse(lsRows);
                                        llLogSize += double.Parse(lsSize);
                                        break;
                                    case "3":
                                        llOutstanding += long.Parse(lsRows);
                                        llOutstandingSize += double.Parse(lsSize);
                                        break;
                                    case "4":
                                        llTransaction += long.Parse(lsRows);
                                        llTransactionSize += double.Parse(lsSize);
                                        break;
                                }
                            }
                            dgvRows.Rows[liHeaderRow].Cells[1].Value = j == 1 ? llMaster : j == 2 ? llLog : j == 3 ? llOutstanding : llTransaction;
                            dgvRows.Rows[liHeaderRow].Cells[2].Value = j == 1 ? llMasterSize : j == 2 ? llLogSize : j == 3 ? llOutstandingSize : llTransactionSize;
                        }
                    }
                    Decimal ldNoOfRows = 0, ldDataSize = Convert.ToDecimal(lDTRows.Compute("SUM(size)", string.Empty));
                    ldNoOfRows = long.Parse(lDTRows.Compute("SUM(rows)", string.Empty).ToString());
                    lDTRows.Dispose();
                    lRows.Text = getConvertedData("rows", ldNoOfRows);

                    //circularProgressBar1.Text = "Size" + Environment.NewLine + getConvertedData("size", ldDataSize);
                    //circularProgressBar1.Value = 77;

                    //Card Values
                    lMasterR.Text = getConvertedData("rows", llMaster);
                    lTranR.Text = getConvertedData("rows", llTransaction);
                    lLogR.Text = getConvertedData("rows", llLog);
                    lOutR.Text = getConvertedData("rows", llOutstanding);

                    dgvRows.EnableHeadersVisualStyles = false;
                    dgvRows.Refresh();
                }
                else
                {
                    lRows.Text = getConvertedData("rows", 0);
                }
            }
            catch
            {

            }
            /************************************ App Sync Status & Dataset Refresh Status ***************************************/
            try
            {
                _MDISync.mdiGlobalVar._MasterConfig.GetAppStatusList(_MDISync.mdiGlobalVar, out iiSuccess, out isReturn);
                if (iiSuccess == 1)
                {
                    int liRowCount;
                    DataTable lDTAppStat = _MDISync.mdiGlobalVar.GetJSONToDataTable(isReturn);

                    liRowCount = lDTAppStat.Rows.Count;
                    if (liRowCount > 0)
                    {
                        foreach (DataColumn dc in lDTAppStat.Columns)
                        {
                            dgvAppStat.Columns.Add(new DataGridViewTextBoxColumn()
                            {
                                Name = dc.ColumnName.Replace(" ", ""),
                                HeaderText = dc.ColumnName//,
                                                          //HeaderCell = 
                                                          //HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft
                            });
                        }

                        foreach (DataRow dr in lDTAppStat.Rows)
                        {
                            dgvAppStat.Rows.Add(dr.ItemArray);
                        }

                        //dgvAppStat.DataSource = lDTAppStat;
                        dgvAppStat.Columns[0].Width = (dgvAppStat.Width / 100) * 20;
                        dgvAppStat.Columns[1].Width = (dgvAppStat.Width / 100) * 20;
                        dgvAppStat.Columns[2].Width = (dgvAppStat.Width / 100) * 20;
                        dgvAppStat.Columns[3].Width = (dgvAppStat.Width / 100) * 20;
                        dgvAppStat.Columns[4].Width = (dgvAppStat.Width / 100) * 20;
                        dgvAppStat.Columns[5].Width = (dgvAppStat.Width / 100) * 40;
                        //dgvAppStat.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                        foreach (DataGridViewColumn dc in dgvAppStat.Columns)
                        {
                            dc.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                        }
                    }
                    lDTAppStat.Dispose();
                    //circularProgressBar4.Text = liRowCount.ToString() + Environment.NewLine + "Source";

                    dgvAppStat.Refresh();

                    dgvAppStat.EnableHeadersVisualStyles = false;
                    dgvAppStat.Refresh();

                }
                else
                {
                    //circularProgressBar4.Text = "0";
                }
            }
            catch
            {

            }
            /******************************************* Source Configuration ****************************************************/
            try
            {
                _MDISync.mdiGlobalVar._MasterConfig.GetSourceList(_MDISync.mdiGlobalVar, out iiSuccess, out isReturn, 1, 1);

                if (iiSuccess == 1)
                {
                    int liRowCount;
                    DataTable lDTSource = _MDISync.mdiGlobalVar.GetJSONToDataTable(isReturn);

                    liRowCount = lDTSource.Rows.Count;
                    if (liRowCount > 0)
                    {
                        dgvSourceList.DataSource = lDTSource;
                        dgvSourceList.Columns[0].Width = (dgvSourceList.Width / 100) * 30;
                        dgvSourceList.Columns[1].Width = (dgvSourceList.Width / 100) * 30;
                        dgvSourceList.Columns[2].Width = (dgvSourceList.Width / 100) * 20;
                        dgvSourceList.Columns[3].Width = (dgvSourceList.Width / 100) * 20;

                        foreach (DataGridViewColumn dc in dgvSourceList.Columns)
                        {
                            dc.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                        }
                    }

                    //circularProgressBar4.Text = liRowCount.ToString() + Environment.NewLine + "Source";
                    lDTSource.Dispose();
                    dgvSourceList.Refresh();

                    dgvSourceList.EnableHeadersVisualStyles = false;
                    dgvSourceList.Refresh();

                }
                else
                {
                    //circularProgressBar4.Text = "0";
                }
            }
            catch
            {

            }
            /******************************************* Reports Count & S/W Configuration ****************************************************/
            try
            {
                lReports.Text = _MDISync.mdiGlobalVar.gsReportCount;
                bSoftware.Text = _MDISync.mdiGlobalVar.gsSoftwareName + Environment.NewLine + "[" + _MDISync.mdiGlobalVar.gsSyncType + "]";

                //dgvRows.Columns[0].HeaderText = "Object Name";
                //dgvRows.Columns[1].HeaderText = "Rows";
                //dgvRows.Columns[2].HeaderText = "Size(MB)";
                dgvRows.Columns[3].Visible = false;

                dgvRows.Columns[0].Width = (dgvRows.Width / 100) * 60;
                dgvRows.Columns[1].Width = (dgvRows.Width / 100) * 20;
                dgvRows.Columns[2].Width = (dgvRows.Width / 100) * 20;

                foreach (DataGridViewColumn dc in dgvRows.Columns)
                {
                    dc.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                dgvRows.Columns[0].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dgvRows.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                dgvSourceList.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                dgvAppStat.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            }
            catch
            {

            }
            LogDetails();
            SetImage();
        }

        private void LogDetails()
        {
            try
            {
                _MDISync.mdiGlobalVar._DestinationConfig.AuditLogDetails(_MDISync.mdiGlobalVar, out iiSuccess, out isReturn);
                if (iiSuccess == 1)
                {
                    DataTable lDTLogs = _MDISync.mdiGlobalVar.GetJSONToDataTable(isReturn);

                    lDTLogs.AcceptChanges();

                    var col1 = new DataGridViewTextBoxColumn() { Name = "LogId", HeaderText = "Log ID" };
                    var col2 = new DataGridViewTextBoxColumn() { Name = "objCnt", HeaderText = "Objects" };
                    var col3 = new DataGridViewTextBoxColumn() { Name = "brCount", HeaderText = "Branches" };
                    var col4 = new DataGridViewTextBoxColumn() { Name = "stTime", HeaderText = "Started At" };
                    var col5 = new DataGridViewTextBoxColumn() { Name = "endTime", HeaderText = "Ended In" };
                    var col6 = new DataGridViewTextBoxColumn() { Name = "st", HeaderText = "Status" };
                    var col7 = new DataGridViewImageColumn() { Name = "Status", HeaderText = "Status" };
                    dgvLog.Columns.AddRange(new DataGridViewColumn[] { col1, col2, col3, col4, col5, col6, col7 });

                    dgvLog.RowsAdded += new DataGridViewRowsAddedEventHandler(dgvLog_CellFormatting);

                    foreach (DataRow row in lDTLogs.Rows)
                    {
                        dgvLog.Rows.Add(row.ItemArray);
                    }
                    lDTLogs.Dispose();
                    dgvLog.ClearSelection();
                    dgvLog.Visible = true;

                    dgvLog.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    dgvLog.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvLog.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvLog.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvLog.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvLog.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    dgvLog.Columns[0].Width = (dgvLog.Width / 100) * 30;
                    dgvLog.Columns[1].Width = (dgvLog.Width / 100) * 10;
                    dgvLog.Columns[2].Width = (dgvLog.Width / 100) * 10;
                    dgvLog.Columns[3].Width = (dgvLog.Width / 100) * 20;
                    dgvLog.Columns[4].Width = (dgvLog.Width / 100) * 20;
                    dgvLog.Columns[6].Width = (dgvLog.Width / 100) * 10;

                    dgvLog.Columns[0].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    dgvLog.Columns[1].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvLog.Columns[2].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvLog.Columns[3].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvLog.Columns[4].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvLog.Columns[6].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    dgvLog.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                    dgvLog.Sort(dgvLog.Columns[0], System.ComponentModel.ListSortDirection.Descending);

                    dgvLog.Columns[5].Visible = false;
                }
            }
            catch
            {

            }
        }

        private void dgvLog_CellFormatting(object sender, DataGridViewRowsAddedEventArgs e)
        {
            var loStatus = dgvLog.Rows[e.RowIndex].Cells["st"].Value;
            int liStatus = int.Parse(loStatus.ToString());

            dgvLog.Rows[e.RowIndex].Cells["Status"].Value = liStatus == 1
                ? Properties.Resources.Warning : liStatus == 2 ? Properties.Resources.Failed : Properties.Resources.Success;
            dgvLog.Rows[e.RowIndex].Cells["Status"].ToolTipText = liStatus == 1
                ? "Partially Failed" : liStatus == 2 ? "Failed" : "Success";
        }

        private string getConvertedData(string asType, decimal adData)
        {
            string lsReturn = "0";
            try
            {
                switch (asType)
                {
                    case "rows":
                        if (Decimal.Floor(adData / (1000 * 100 * 100)) > 0) lsReturn = (adData / (1000 * 100 * 100)).ToString("0.00") + " CR";
                        else if (Decimal.Floor(adData / (1000 * 100)) > 0) lsReturn = (adData / (1000 * 100)).ToString("0.00") + " L";
                        else if (Decimal.Floor(adData / 1000) > 0) lsReturn = (adData / 1000).ToString("0.00") + " K";
                        else lsReturn = adData.ToString();
                        break;
                    case "size":
                        if (Decimal.Floor(adData / (1024 * 1024 * 1024)) > 0) lsReturn = (adData / (1024 * 1024 * 1024)).ToString("0.00") + Environment.NewLine + "PB";
                        else if (Decimal.Floor(adData / (1024 * 1024)) > 0) lsReturn = (adData / (1024 * 1024)).ToString("0.00") + Environment.NewLine + "TB";
                        else if (Decimal.Floor(adData / 1024) > 0) lsReturn = (adData / 1024).ToString("0.00") + Environment.NewLine + "GB";
                        else lsReturn = adData.ToString() + Environment.NewLine + "MB";
                        break;
                    default:
                        break;
                }
            }
            catch
            {
                lsReturn = "0";
            }
            return lsReturn;
        }

        private void SetImage()
        {
            //API Calling for Organizational Image
            string lsUrl, lsResult;
            RestAPI restAPI = new RestAPI();
            byte[] bytes = null;
            int i = 0;

            ////Leapsurge Logo
            //LogoContainer.Panel1.BackgroundImage = Properties.Resources.CompanyLogo;
            //LogoContainer.Panel1.BackgroundImageLayout = ImageLayout.Stretch;

            ////Customer's Firm/Company Logo
            //i = 0;
            //lsUrl = _MDISync.mdiGlobalVar.gsContainerLocation + _MDISync.mdiGlobalVar.gsLogoContainer + "/[LSCode]/FirmLogo/[FirmCode].bin";
            ///*try
            //{
            //    lsResult = (restAPI.getAPICalling(lsUrl.Replace("[LSCode]", _MDISync.mdiGlobalVar.custCode)
            //            .Replace("[FirmCode]", _MDISync.mdiGlobalVar.firmCode), out _)).Replace("data:image/png;base64,", "");
            //    //LogoContainer.Panel2.BackgroundImage = (Image)lsResult;
            //    i = 1;
            //}
            //catch
            //{
                
            //}
            //if (i == 1) LogoContainer.Panel2.BackgroundImage = Image.FromStream(new MemoryStream(bytes));
            //else LogoContainer.Panel2.BackgroundImage = Properties.Resources.YourLogo;
            //LogoContainer.Panel2.BackgroundImageLayout = ImageLayout.Stretch;
            //*/
            //try
            //{
            //    lsResult = (restAPI.getAPICalling(lsUrl.Replace("[LSCode]", _MDISync.mdiGlobalVar.custCode)
            //            .Replace("[FirmCode]", _MDISync.mdiGlobalVar.firmCode), out _)).Replace("data:image/png;base64,", "");
            //    bytes = Convert.FromBase64String(lsResult);
            //    i = 1;
            //}
            //catch
            //{
            //    try
            //    {
            //        lsResult = (restAPI.getAPICalling(
            //            lsUrl.Replace("[LSCode]", "LS1000").Replace("[FirmCode]", "default"), out _)).Replace("data:image/png;base64,", "");
            //        bytes = Convert.FromBase64String(lsResult);
            //        i = 1;
            //    }
            //    catch
            //    {

            //    }
            //}
            //if (i == 1) LogoContainer.Panel2.BackgroundImage = Image.FromStream(new MemoryStream(bytes));
            //else LogoContainer.Panel2.BackgroundImage = Properties.Resources.YourLogo;
            //LogoContainer.Panel2.BackgroundImageLayout = ImageLayout.Stretch;

            //Software Logo
            lsUrl = _MDISync.mdiGlobalVar.gsContainerLocation + _MDISync.mdiGlobalVar.gsLogoContainer + "/SWLogo/[SWName].bin";
            try
            {
                bytes = Convert.FromBase64String(restAPI.getAPICalling(lsUrl.Replace("[SWName]", _MDISync.mdiGlobalVar.gsSoftwareName), out _));
                bSoftware.BackgroundImage = Image.FromStream(new MemoryStream(bytes));
                bSoftware.BackgroundImageLayout = ImageLayout.Stretch;
                bSoftware.Text = "";
            }
            catch
            {

            }
            restAPI.Dispose();
        }

        private void dgvLog_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string lsLogId = dgvLog.Rows[e.RowIndex].Cells["LogId"].Value.ToString();
            LogDetails _LogDetails = new LogDetails(lsLogId, _MDISync.mdiGlobalVar);
            _LogDetails.ShowDialog();
        }
    }
}
