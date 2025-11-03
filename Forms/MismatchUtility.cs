using System;
using System.Data;
using System.Linq;
using System.Drawing;
using Newtonsoft.Json;
using System.Text.Json;
using LSSyncApp.Functions;
using System.Windows.Forms;
using LSSyncApp.Controllers;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.IO;

namespace LSSyncApp.Forms
{
    public partial class MismatchUtility : Form
    {
        private MainForm _MDISync;
        private string isReturn, isSourceList, isTableList;
        private int iiSuccess, iiSource = 0, iiSourceCount = 0, iiTableCount = 0, iiSelCount = 0;
        private JsonElement ljeSourceList, ljeTableList;
        private ODBCSyncParam _SyncParam = new ODBCSyncParam();
        private string IsTblCode, isRptParam; //src, dest, tblName, columnList, 

        public MismatchUtility(MainForm _MainForm)
        {
            InitializeComponent();
            _MDISync = _MainForm;
        }

        private void cbxSourceList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        private async void dgvTableList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0) return;
                IsTblCode = dgvTableList.Rows[e.RowIndex].Cells[1].Value.ToString();
                isRptParam = dgvTableList.Rows[e.RowIndex].Cells[5].Value.ToString();

                _MDISync.mdiGlobalVar._fun.Loader(true, wbLoader, _MDISync.mdiGlobalVar.gsTemplatePath, Width, Height);
                await LoadDataAsync(((KeyValuePair<string, string>)cbxSourceList.SelectedItem).Value,
                    dgvTableList.Rows[e.RowIndex].Cells[2].Value.ToString(),
                    dgvTableList.Rows[e.RowIndex].Cells[3].Value.ToString(),
                    dgvTableList.Rows[e.RowIndex].Cells[4].Value.ToString());
                
                _MDISync.mdiGlobalVar._fun.Loader(false, wbLoader);

            }
            catch(Exception Ex)
            {
                _MDISync.mdiGlobalVar._fun.Loader(false, wbLoader);
                MessageBox.Show(Ex.Message);
            }
        }

        private void dgvMismatchList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            dgvMismatchList.CommitEdit(DataGridViewDataErrorContexts.Commit);
            
        }

        private void dgvMismatchList_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                if (Convert.ToBoolean(dgvMismatchList.Rows[e.RowIndex].Cells[0].Value) == true)
                    iiSelCount += 1;
                else
                    iiSelCount -= 1;
                EnableButton();
            }
        }

        private void EnableButton()
        {
            MessageBox.Show(iiSelCount.ToString());
            if (iiSelCount > 0)
                lbSync.Enabled = true;
            else
                lbSync.Enabled = false;
        }

        private void lbSync_Click(object sender, EventArgs e)
        {
            try
            {
                string lsBrList = "", lsSource = cbxSourceList.Text, lsDateList = "";
                int j = 0;
                if (iiSelCount > 0)
                {
                    if (dgvMismatchList.Columns[1].Name == "Branch")
                    {
                        for (int i = 0; i < dgvMismatchList.Rows.Count; i++)
                        {
                            string dateString = dgvMismatchList.Rows[2].Cells[1].Value.ToString();
                            DateTime dt = DateTime.ParseExact(dateString, "MMMM yyyy", CultureInfo.InvariantCulture);

                            string lsLastDate = dt.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
                            string lsFirstDate = dt.ToString("yyyy-MM-dd");

                            if (Convert.ToBoolean(dgvMismatchList.Rows[i].Cells[0].Value) == true)
                            {
                                lsBrList += (j == 0 ? "" : ",") + "{\"brCode\":\"" + dgvMismatchList.Rows[i].Cells[1].Value.ToString() + "\"}";
                                lsDateList += (j == 0 ? "" : ",") + "{\"brCode\":\"" + dgvMismatchList.Rows[i].Cells[1].Value.ToString()
                                    + "\",\"fromDate\":\"" + lsFirstDate + "\",\"toDate\":\"" + lsLastDate + "\"}";
                                j++;
                            }
                            lsBrList = "[" + lsBrList + "]";
                            lsDateList = "[" + lsDateList + "]";
                        }
                    }
                    dgvMismatchList.Enabled = false;
                    var progress = new Progress<int>(v => { lpbGlobalPB.Value = v; });
                    _SyncParam.sourceConfiguration(isRptParam, progress, _MDISync.mdiGlobalVar, 2, lsSource, IsTblCode, lsDateList, lsBrList);
                    dgvMismatchList.Enabled = true;
                }
            }
            catch
            {
                dgvMismatchList.Enabled = true;
            }
        }

        private void RefreshData(string aiSource, string asSource, string asDest, string asColumns)
        {
            try
            {
                if (asColumns == "" || asColumns == null || !asColumns.Contains("|")) return;

                string lsCommonColumn1 = "", lsCommonColumn2 = "", lsCommonColumn3 = "", lsCommonColumn4 = "", lsCommonColumn5 = "";
                string lsCalcColumn1 = "", lsCalcColumn2 = "", lsCalcColumn3 = "", lsCalcColumn4 = "", lsCalcColumn5 = "";
                string lsCommonColumnH1 = "", lsCommonColumnH2 = "", lsCommonColumnH3 = "", lsCommonColumnH4 = "", lsCommonColumnH5 = "";
                string lsCalcColumnH1 = "", lsCalcColumnH2 = "", lsCalcColumnH3 = "", lsCalcColumnH4 = "", lsCalcColumnH5 = "";
                string lsCommonColumns, lsComputeColumns;
                DataTable ldtDest, ldtSource;
                int liSrcRows, liColCount = 0, liCommonInc = 1, liComputeInc = 1;

                lsCommonColumns = asColumns.Substring(0, asColumns.IndexOf("|")) + ",";
                lsComputeColumns = asColumns.Substring(asColumns.IndexOf("|") + 1) + ",";

                //Common Columns
                while(lsCommonColumns.Contains(","))
                {
                    switch(liCommonInc)
                    {
                        case 1:
                            lsCommonColumnH1 = lsCommonColumns.Substring(0, lsCommonColumns.IndexOf(",")).Trim();
                            lsCommonColumn1 = lsCommonColumnH1.Replace(" ","_");
                            break;
                        case 2:
                            lsCommonColumnH2 = lsCommonColumns.Substring(0, lsCommonColumns.IndexOf(",")).Trim();
                            lsCommonColumn2 = lsCommonColumnH2;
                            break;
                        case 3:
                            lsCommonColumnH3 = lsCommonColumns.Substring(0, lsCommonColumns.IndexOf(",")).Trim();
                            lsCommonColumn3 = lsCommonColumnH3;
                            break;
                        case 4:
                            lsCommonColumnH4 = lsCommonColumns.Substring(0, lsCommonColumns.IndexOf(",")).Trim();
                            lsCommonColumn4 = lsCommonColumnH4;
                            break;
                        case 5:
                            lsCommonColumnH5 = lsCommonColumns.Substring(0, lsCommonColumns.IndexOf(",")).Trim();
                            lsCommonColumn5 = lsCommonColumnH5;
                            break;
                    }
                    lsCommonColumns = lsCommonColumns.Substring(lsCommonColumns.IndexOf(",") + 1);
                    liCommonInc += 1;
                }

                //Compute Columns
                while (lsComputeColumns.Contains(","))
                {
                    switch(liComputeInc)
                    {
                        case 1:
                            lsCalcColumnH1 = lsComputeColumns.Substring(0, lsComputeColumns.IndexOf(",")).Trim();
                            lsCalcColumn1 = lsCalcColumnH1.Replace(" ","_");
                            break;
                        case 2:
                            lsCalcColumnH2 = lsComputeColumns.Substring(0, lsComputeColumns.IndexOf(",")).Trim();
                            lsCalcColumn2 = lsCalcColumnH2;
                            break;
                        case 3:
                            lsCalcColumnH3 = lsComputeColumns.Substring(0, lsComputeColumns.IndexOf(",")).Trim();
                            lsCalcColumn3 = lsCalcColumnH3;
                            break;
                        case 4:
                            lsCalcColumnH4 = lsComputeColumns.Substring(0, lsComputeColumns.IndexOf(",")).Trim();
                            lsCalcColumn4 = lsCalcColumnH4;
                            break;
                        case 5:
                            lsCalcColumnH5 = lsComputeColumns.Substring(0, lsComputeColumns.IndexOf(",")).Trim();
                            lsCalcColumn5 = lsCalcColumnH5;
                            break;
                    }
                    lsComputeColumns = lsComputeColumns.Substring(lsComputeColumns.IndexOf(",") + 1);
                    liComputeInc += 1;
                }

                //Source Table Data Fetch
                isReturn = SourceDataFetch(asSource, aiSource, out iiSuccess);
                _SyncParam.odbcGlobalVar.logFile("Test.Txt", isReturn, 1);
                if (iiSuccess == 0)
                {
                    return;
                }

                //isReturn = File.ReadAllText("C:\\Git\\Test.txt");
                dynamic jsonDict = JsonConvert.DeserializeObject(isReturn);
                var jarrayString = JsonConvert.SerializeObject(jsonDict["root"]["xmlData"]); ;
                ldtSource = JsonConvert.DeserializeObject<DataTable>(jarrayString);
                //ldtSource = (DataTable)JsonConvert.DeserializeObject(isReturn, (typeof(DataTable)));

                //Destination Table Data Fetch
                _MDISync.mdiGlobalVar._DestinationConfig.ExecuteRawQuery(_MDISync.mdiGlobalVar,
                    asDest, 0, out iiSuccess, out isReturn);

                //Calculate Both Source & Destination
                if(iiSuccess == 1)
                {
                    ldtDest = (DataTable)JsonConvert.DeserializeObject(isReturn, (typeof(DataTable)));
                    
                    var dt = (from table1 in ldtSource.AsEnumerable()
                        join table2 in ldtDest.AsEnumerable() on table1.Field<string>(lsCommonColumn1) equals table2.Field<string>(lsCommonColumn1)
                        //where table1.Field<string>("billDt") == table2.Field<string>("billDt")
                        select new {
                            T1 = table1,
                            T2 = table2
                        }).Where(t => (lsCommonColumn2 != "" ? t.T1[lsCommonColumn2].ToString() : "") == (lsCommonColumn2 != "" ? t.T2[lsCommonColumn2].ToString() : "")
                            && (lsCommonColumn3 != "" ? t.T1[lsCommonColumn3].ToString() : "") == (lsCommonColumn3 != "" ? t.T2[lsCommonColumn3].ToString() : "")
                            && (lsCommonColumn4 != "" ? t.T1[lsCommonColumn4].ToString() : "") == (lsCommonColumn4 != "" ? t.T2[lsCommonColumn4].ToString() : "")
                            && (lsCommonColumn5 != "" ? t.T1[lsCommonColumn5].ToString() : "") == (lsCommonColumn5 != "" ? t.T2[lsCommonColumn5].ToString() : "")
                        ).ToList();

                    liSrcRows = dt.Count();
                    
                    if (liSrcRows > 0)
                    {
                        int colCnt = ((liCommonInc - 1) + ((liComputeInc - 1) * 3)), j = 0;
                        DataGridViewColumn[] cols = new DataGridViewColumn[colCnt];

                        DataGridViewCheckBoxColumn lcbxCol = new DataGridViewCheckBoxColumn()
                        {
                            HeaderText = "Select",
                            Name = "Select",
                            DefaultCellStyle = new DataGridViewCellStyle() { Alignment = DataGridViewContentAlignment.MiddleCenter, NullValue = false },
                            
                        };
                        //j++;
                        liColCount++;

                        for (int i = 0; i < liCommonInc - 1; i++)
                        {
                            cols[j] = new DataGridViewTextBoxColumn()
                            {
                                Name = (i == 0 ? lsCommonColumn1 : (i == 1 ? lsCommonColumn2 : (i == 2 ? lsCommonColumn3 : (i == 3 ? lsCommonColumn4 : lsCommonColumn5)))),
                                HeaderText = (i == 0 ? lsCommonColumnH1 : (i == 1 ? lsCommonColumnH2 : (i == 2 ? lsCommonColumnH3 : (i == 3 ? lsCommonColumnH4 : lsCommonColumnH5)))),
                                ReadOnly = true
                            };
                            j++;
                            liColCount++;
                        }
                        for(int i = 0; i < (liComputeInc - 1); i++)
                        {
                            cols[j + (i * 3)] = new DataGridViewTextBoxColumn()
                            {
                                HeaderText = "Source " + (i == 0 ? lsCalcColumnH1 : (i == 1 ? lsCalcColumnH2 : (i == 2 ? lsCalcColumnH3 : (i == 3 ? lsCalcColumnH4 : lsCalcColumnH5)))),
                                Name = "Src_" + (i == 0 ? lsCalcColumn1 : (i == 1 ? lsCalcColumn2 : (i == 2 ? lsCalcColumn3 : (i == 3 ? lsCalcColumn4 : lsCalcColumn5)))),
                                ReadOnly = true,
                                DefaultCellStyle = new DataGridViewCellStyle() { Alignment = DataGridViewContentAlignment.MiddleRight },
                                HeaderCell = new DataGridViewColumnHeaderCell { Style = new DataGridViewCellStyle() { Alignment = DataGridViewContentAlignment.MiddleRight } }
                            };
                            liColCount++;
                            cols[j + (i * 3) + 1] = new DataGridViewTextBoxColumn()
                            {
                                HeaderText = "Dest " + (i == 0 ? lsCalcColumnH1 : (i == 1 ? lsCalcColumnH2 : (i == 2 ? lsCalcColumnH3 : (i == 3 ? lsCalcColumnH4 : lsCalcColumnH5)))),
                                Name = "Dest_" + (i == 0 ? lsCalcColumn1 : (i == 1 ? lsCalcColumn2 : (i == 2 ? lsCalcColumn3 : (i == 3 ? lsCalcColumn4 : lsCalcColumn5)))),
                                ReadOnly = true,
                                DefaultCellStyle = new DataGridViewCellStyle() { Alignment = DataGridViewContentAlignment.MiddleRight },
                                HeaderCell = new DataGridViewColumnHeaderCell { Style = new DataGridViewCellStyle() { Alignment = DataGridViewContentAlignment.MiddleRight } }
                            };
                            liColCount++;
                            cols[j + (i * 3) + 2] = new DataGridViewTextBoxColumn()
                            {
                                HeaderText = "Mismatch " + (i == 0 ? lsCalcColumnH1 : (i == 1 ? lsCalcColumnH2 : (i == 2 ? lsCalcColumnH3 : (i == 3 ? lsCalcColumnH4 : lsCalcColumnH5)))),
                                Name = "Mismatch_" + (i == 0 ? lsCalcColumn1 : (i == 1 ? lsCalcColumn2 : (i == 2 ? lsCalcColumn3 : (i == 3 ? lsCalcColumn4 : lsCalcColumn5)))),
                                ReadOnly = true,
                                DefaultCellStyle = new DataGridViewCellStyle (){ Alignment = DataGridViewContentAlignment.MiddleRight },
                                HeaderCell = new DataGridViewColumnHeaderCell{Style =  new DataGridViewCellStyle() { Alignment = DataGridViewContentAlignment.MiddleRight } }
                            };
                            liColCount++;
                        }

                        DataGridViewProgressColumn lbProgress = new DataGridViewProgressColumn
                        {
                            Name = "Progress",
                            HeaderText = "Progress",
                            ReadOnly = true
                        };
                        liColCount++;

                        DataGridViewDisableButtonColumn lbButton = new DataGridViewDisableButtonColumn
                        {
                            Name = "Sync",
                            HeaderText = "",
                            ReadOnly = true,
                            
                        };
                        liColCount++;

                        this.Invoke(new MethodInvoker(delegate () {
                            dgvMismatchList.Columns.Add(lcbxCol);
                            dgvMismatchList.Columns.AddRange(cols);
                            dgvMismatchList.Columns.Add(lbProgress);
                            dgvMismatchList.Columns.Add(lbButton);
                        }));

                        for (int i = 0; i < liSrcRows; i++)
                        {
                            Int64 liMisMatchCount = 0;
                            int compCols = 1, row = 0;
                            this.Invoke(new MethodInvoker(delegate () {
                                row = dgvMismatchList.Rows.Add(); 
                            }));

                            //this.Invoke(new MethodInvoker(delegate () {
                            //    ((DataGridViewCheckBoxCell)dgvMismatchList.Rows[row].Cells[0]).Selected = false;
                            //}));

                            for (int comm = 0; comm < liCommonInc - 1; comm++)
                            {
                                this.Invoke(new MethodInvoker(delegate () {
                                    dgvMismatchList.Rows[row].Cells[comm + 1].Value = dt[i].T1[comm].ToString();
                                }));
                                compCols++;
                            }
                            for (int comp = 0; comp < liComputeInc - 1; comp++)
                            {
                                Int64 liSource, liDest;
                                liSource = Int64.Parse(dt[i].T1[(compCols - 1) + comp].ToString());
                                liDest= Int64.Parse(dt[i].T2[(compCols - 1) + comp].ToString());
                                liMisMatchCount += (liSource - liDest);
                                this.Invoke(new MethodInvoker(delegate () {
                                    dgvMismatchList.Rows[row].Cells[compCols + (comp * 3)].Value = liSource;
                                    dgvMismatchList.Rows[row].Cells[compCols + (comp * 3) + 1].Value = liDest;
                                    dgvMismatchList.Rows[row].Cells[compCols + (comp * 3) + 2].Value = liSource - liDest;
                                }));
                            }
                            this.Invoke(new MethodInvoker(delegate ()
                            {
                                ((DataGridViewProgressCell)dgvMismatchList.Rows[row].Cells["Progress"]).Red = 89;
                                ((DataGridViewProgressCell)dgvMismatchList.Rows[row].Cells["Progress"]).Green = 250;
                                ((DataGridViewProgressCell)dgvMismatchList.Rows[row].Cells["Progress"]).Blue = 159;
                                dgvMismatchList.Rows[row].Cells["Progress"].Style = new DataGridViewCellStyle { ForeColor = Color.FromArgb(41, 41, 75) };


                                ((DataGridViewDisableButtonCell)dgvMismatchList.Rows[row].Cells["Sync"]).Value = "Sync";
                                ((DataGridViewDisableButtonCell)dgvMismatchList.Rows[row].Cells["Sync"]).FlatStyle = FlatStyle.Flat;
                                dgvMismatchList.Rows[row].Cells["Sync"].Style = new DataGridViewCellStyle
                                {
                                    ForeColor = Color.FromArgb(41, 41, 75),
                                    BackColor = Color.FromArgb(250, 202, 89),
                                    Font = new Font("Times New Roman", 9, FontStyle.Italic | FontStyle.Bold)
                                };
                            }));
                            if (liMisMatchCount == 0)
                            {
                                this.Invoke(new MethodInvoker(delegate () {
                                    //((DataGridViewCheckBoxCell)dgvMismatchList.Rows[row].Cells[0]).ReadOnly = true;
                                    ((DataGridViewDisableButtonCell)dgvMismatchList.Rows[row].Cells[liColCount - 1]).Enabled = false;
                                }));
                            }
                        }
                        int liWidth = dgvMismatchList.Width / (liColCount + 2);
                        for (int i = 0; i < liColCount; i++)
                        {
                            this.Invoke(new MethodInvoker(delegate() {
                                dgvMismatchList.Columns[i].Width = liWidth;
                            }));
                        }
                    }
                    
                }
                
                
            }
            catch (Exception Ex)
            {
                _MDISync.mdiGlobalVar._fun.Loader(false, wbLoader);
                MessageBox.Show(Ex.Message);
            }
        }

        private async void Post_Load(object sender, EventArgs e)
        {
            try
            {
                _MDISync.mdiGlobalVar._fun.Loader(true, wbLoader, _MDISync.mdiGlobalVar.gsTemplatePath, Width, Height);

                int liPanelWidth = (this.Width / 100) * 100, liPanelHeight = (this.Height / 100) * 100;
                mainPanel.Size = new Size(liPanelWidth, liPanelHeight);
                int liWidth = mainPanel.Width / 100, liHeight = mainPanel.Height / 100;

                mainPanel.Location = new Point((this.Width - mainPanel.Width) / 2, (this.Height - mainPanel.Height) / 2);

                splitContainer1.SplitterDistance = liWidth * 30;

                mainPanel.Region = Region.FromHrgn(_MDISync.mdiGlobalVar.createRoundRect(0, 0, mainPanel.Width, mainPanel.Height, 50, 50));
                _MDISync.mdiGlobalVar._Theme.SetTheme(this, _MDISync.mdiGlobalVar.giTheme);
                
                await LoadBackGroundAsync();

                if (iiSource == 1)
                {
                    ljeSourceList = _MDISync.mdiGlobalVar.createJsonElement(isSourceList);
                    iiSourceCount = ljeSourceList.EnumerateArray().Count();
                    Dictionary<string, string> cbItems = new Dictionary<string, string>();
                    for (int i = 0; i < iiSourceCount; i++)
                    {
                        cbItems.Add(ljeSourceList[i].GetProperty("seq").ToString(),
                            ljeSourceList[i].GetProperty("serverName").ToString());
                    }

                    if (iiSourceCount > 0)
                    {
                        cbxSourceList.DataSource = new BindingSource(cbItems, null);
                        this.cbxSourceList.SelectedIndexChanged += new EventHandler(this.cbxSourceList_SelectedIndexChanged);
                        cbxSourceList.DisplayMember = "Value";
                        cbxSourceList.ValueMember = "Key";

                        //Load Table List
                        if (iiTableCount == 1)
                        {
                            ljeTableList = _MDISync.mdiGlobalVar.createJsonElement(isTableList);
                            int liTblCount = ljeTableList.EnumerateArray().Count(), liRow;
                            dgvTableList.Columns[0].Width = dgvTableList.Width;
                            for (int i = 0; i < liTblCount; i++)
                            {
                                liRow = dgvTableList.Rows.Add();
                                dgvTableList.Rows[liRow].Cells[0].Value = ljeTableList[i].GetProperty("tblName").ToString();
                                dgvTableList.Rows[liRow].Cells[1].Value = ljeTableList[i].GetProperty("tblCode").ToString();
                                dgvTableList.Rows[liRow].Cells[2].Value = ljeTableList[i].GetProperty("src").ToString();
                                dgvTableList.Rows[liRow].Cells[3].Value = ljeTableList[i].GetProperty("dest").ToString();
                                dgvTableList.Rows[liRow].Cells[4].Value = ljeTableList[i].GetProperty("columnList").ToString();
                                dgvTableList.Rows[liRow].Cells[5].Value = ljeTableList[i].GetProperty("rptParam").ToString();
                            }
                        }
                    }
                }

                //PostLoadBackGround();
                _MDISync.mdiGlobalVar._fun.Loader(false, wbLoader);
            }
            catch(Exception Ex)
            {
                _MDISync.mdiGlobalVar._fun.Loader(false, wbLoader);
                MessageBox.Show(Ex.Message);
            }
        }

        Task LoadBackGroundAsync()
        {
            return Task.Run(() => LoadBackGround());
        }

        Task LoadDataAsync(string aiSource, string asSource, string asDest, string asColumns)
        {
            return Task.Run(() => RefreshData(aiSource, asSource, asDest, asColumns));
        }

        private void LoadBackGround()
        {
            //Load Source List
            try
            {
                _MDISync.mdiGlobalVar._MasterConfig.GetSourceList(_MDISync.mdiGlobalVar, out iiSource, out isSourceList);
                _MDISync.mdiGlobalVar._MasterConfig.GetMismatchTableList(_MDISync.mdiGlobalVar, out iiTableCount, out isTableList);
            }
            catch
            {
            }
        }


        private void MisMatchClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                MenuSettings.EnableMenuItem(this.MdiParent, "mismatchutility");
            }
            catch(Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
            
        }

        private void lbCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        private string SourceDataFetch(string asQuery, string asSource, out int aiSuccess)
        {
            aiSuccess = 0;
            try
            {
                string lsReturn = "", lsRet = "";
                int liSourceCnt;
                JsonElement ljeSourceConfig;
                
                /************************************************************************************/
                //Source Configuration
                _MDISync.mdiGlobalVar._MasterConfig.GetSourceList(_MDISync.mdiGlobalVar, out int liStatus, out lsRet, 1, 0, asSource);
                if (liStatus == 1 && lsRet != null && lsRet != "")
                {
                    ljeSourceConfig = _MDISync.mdiGlobalVar.createJsonElement(lsRet);
                    liSourceCnt = ljeSourceConfig.EnumerateArray().Count();

                    string ip = ljeSourceConfig[0].GetProperty("host").ToString();
                    string port = ljeSourceConfig[0].GetProperty("port").ToString();
                    int iiCY = int.Parse(ljeSourceConfig[0].GetProperty("curYear").ToString());
                    string isODBCServer = ljeSourceConfig[0].GetProperty("serverName").ToString();
                    string odbcDBName = ljeSourceConfig[0].GetProperty("dbName").ToString();
                    string isODBCUID = ljeSourceConfig[0].GetProperty("uID").ToString();
                    string isODBCPwd = ljeSourceConfig[0].GetProperty("pwd").ToString();
                    string isSyncType = ljeSourceConfig[0].GetProperty("syncType").ToString();
                    string isSoftwareName = ljeSourceConfig[0].GetProperty("stName").ToString();
                    string lsFinYear = ljeSourceConfig[0].GetProperty("finYear").ToString();
                    string lsMinTimeFY = ljeSourceConfig[0].GetProperty("minTime").ToString();
                    string isSeq = ljeSourceConfig[0].GetProperty("seq").ToString();
                    string isDbTypeName = ljeSourceConfig[0].GetProperty("dbTypeName").ToString();
                    int iiAuthType = int.Parse(ljeSourceConfig[0].GetProperty("authType").ToString());

                    if (lsFinYear != null && lsFinYear != "" && lsFinYear.Contains("FY"))
                    {
                        lsFinYear = lsFinYear.Replace("FY", "").Trim();
                        if (lsMinTimeFY == "" || lsMinTimeFY == null)
                        {
                            _MDISync.mdiGlobalVar.defTime = "20" + lsFinYear.Substring(0, 2) + "-04-01 00:00:00";
                        }
                        else
                        {
                            _MDISync.mdiGlobalVar.defTime = lsMinTimeFY;
                        }
                        _MDISync.mdiGlobalVar.gsFinYear = lsFinYear.Substring(0, 2);
                        string isFinYear = _MDISync.mdiGlobalVar.gsFinYear;
                    }
                    else
                    {
                        return "";
                    }

                    switch (isSoftwareName)
                    {
                        case "LOGIC ERP":

                            break;
                        case "Busy":
                            break;
                        case "Bizom":
                            break;
                        case "Tally":
                            break;
                        default:
                            switch (isSyncType)
                            {
                                case "API":
                                    break;
                                case "ODBC":
                                    //_SyncParam.odbcGlobalVar.logFile("Test.Txt", "Select * From (" + asQuery + ") as xmlData For XML Auto,Elements", 1);
                                    lsReturn = _MDISync.mdiGlobalVar.odbcConn.srcDBExecQueryRetOne( "Select * From (" + asQuery + ") as xmlData For XML Auto,Elements", isODBCServer, isODBCUID, isODBCPwd);

                                    //_SyncParam.odbcGlobalVar.logFile("Test.Txt", lsReturn, 1);
                                    if (lsReturn == "" || lsReturn == null || lsReturn.Contains("Failed"))
                                    {
                                        GC.Collect();
                                        return "";
                                    }
                                    lsReturn = "<root>" + lsReturn.Replace("&quot;", "") + "</root>";
                                    XmlDocument doc = new XmlDocument();
                                    doc.LoadXml(lsReturn);

                                    string lsJson = JsonConvert.SerializeXmlNode(doc);//,Formatting.None, true);
                                    lsReturn = _SyncParam.odbcGlobalVar.replaceSpecialCharacters(lsJson);
                                    aiSuccess = 1;
                                    break;
                                default:
                                    break;
                            }
                            break;
                    }
                }
                return lsReturn;
            }
            catch
            {
                return "";
            }
        }
    }
}
