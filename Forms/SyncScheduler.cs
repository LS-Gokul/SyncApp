using LSSyncApp.Functions;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace LSSyncApp.Forms
{
    public partial class SyncScheduler : Form
    {
        private DateTimePicker dtp { get; set; }
        private MainForm _MDISync;
        string isReturn;

        public SyncScheduler(MainForm _MainForm)
        {
            try
            {
                InitializeComponent();
                _MDISync = _MainForm;
            }
            catch
            {

            }
        }

        private void SyncScheduler_Load(object sender, EventArgs e)
        {

        }

        private void SyncScheduler_PostLoad(object sender, EventArgs e)
        {
            try
            {
                _MDISync.mdiGlobalVar._fun.Loader(true, _MDISync.wbLoader, _MDISync.mdiGlobalVar.gsTemplatePath,
                    this.Width / 10, this.Height / 10, (this.Width / 2) - ((this.Width / 10) / 2),
                    (this.Height / 2) - ((this.Height / 10) / 2));
                _MDISync.wbLoader.BringToFront();
                FlagButtons(true);

                /////////////////////////////////////////////
                _MDISync.mdiGlobalVar._MasterConfig.GetSchedulerList(_MDISync.mdiGlobalVar, out int liSuccess, out isReturn);
                if (liSuccess == 1 && isReturn != "" && isReturn != null)
                {
                    DataTable dt = (DataTable)JsonConvert.DeserializeObject(isReturn, (typeof(DataTable)));
                    dgvSchedule.DataSource = dt;

                    if (dgvSchedule.Rows.Count > 0)
                    {
                        //Set Visible
                        dgvSchedule.Columns["syncParam"].Visible = false;
                        dgvSchedule.Columns["syncDays"].Visible = false;
                        dgvSchedule.Columns["syncInterval"].Visible = false;

                        //Set Header Text
                        dgvSchedule.Columns["rptName"].HeaderText = "Report Name";
                        dgvSchedule.Columns["startTime"].HeaderText = "Start Time";

                        //Initial Sort
                        dgvSchedule.Sort(dgvSchedule.Columns["rptName"], System.ComponentModel.ListSortDirection.Ascending);

                        /*//Check Box Column
                        DataGridViewCheckBoxColumn curCol = new DataGridViewCheckBoxColumn
                        {
                            HeaderText = "",
                            Name = "Select"
                        };
                        dgvSchedule.Columns.Insert(0, curCol);
                        */
                        //Old Interval Column as Textbox
                        DataGridViewTextBoxColumn intCol = new DataGridViewTextBoxColumn
                        {
                            HeaderText = "Interval",
                            Name = "interval",
                            ReadOnly = true
                        };
                        dgvSchedule.Columns.Insert(1, intCol);

                        //New Interval Column as Dropdown list
                        DataGridViewComboBoxColumn newIntCol = new DataGridViewComboBoxColumn
                        {
                            HeaderText = "New Interval",
                            Name = "newInterval",
                            Items = { "30 Minutes", "1 Hour", "2 Hours", "3 Hours", "4 Hours", "6 Hours", "12 Hours", "24 Hours" },
                            ReadOnly = false
                        };
                        dgvSchedule.Columns.Insert(3, newIntCol);

                        //Select all reports Initially
                        for (int i = 0; i < dgvSchedule.Rows.Count; i++)
                        {
                            //dgvSchedule.Rows[i].Cells["Select"].Value = true;
                            double liSyncInterval = double.Parse(dgvSchedule.Rows[i].Cells["syncInterval"].Value.ToString());
                            if (liSyncInterval == 0) liSyncInterval = 24 * 60;

                            string lsInt = (liSyncInterval / 60).ToString("0.00");

                            string lsInterval = liSyncInterval / 60 >= 1
                                ? lsInt.Substring(0, lsInt.IndexOf(".")) + " Hr" +
                                    (int.Parse(lsInt.Substring(lsInt.IndexOf(".") + 1)) > 0
                                        ? " " + lsInt.Substring(lsInt.IndexOf(".") + 1) + " Mins"
                                        : "")
                                : int.Parse(liSyncInterval.ToString("0")) + " Mins";

                            dgvSchedule.Rows[i].Cells["interval"].Value = lsInterval;
                        }

                        //Set Red only Property
                        dgvSchedule.Columns["rptName"].ReadOnly = true;
                        dgvSchedule.Columns["startTime"].ReadOnly = true;

                        //Set Sort Mode Property
                        dgvSchedule.Columns["rptName"].SortMode = DataGridViewColumnSortMode.NotSortable;
                        dgvSchedule.Columns["startTime"].SortMode = DataGridViewColumnSortMode.NotSortable;
                        //dgvSchedule.Columns["Select"].SortMode = DataGridViewColumnSortMode.NotSortable;
                        dgvSchedule.Columns["interval"].SortMode = DataGridViewColumnSortMode.NotSortable;
                        dgvSchedule.Columns["newInterval"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    }
                }
                //Set Theme Color
                _MDISync.mdiGlobalVar._Theme.SetTheme(this, _MDISync.mdiGlobalVar.giTheme);
                SyncScheduler_Resize(sender, e);
                _MDISync.mdiGlobalVar._fun.Loader(false, _MDISync.wbLoader);
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        private void SyncScheduler_Resize(object sender, EventArgs e)
        {
            try
            {
                splitContainer1.Paint += Panel_Paint;
                int liPanelWidth = (this.Width / 100) * 100, liPanelHeight = (this.Height / 100) * 100;
                mainPanel.Size = new Size(liPanelWidth, liPanelHeight);
                int liWidth = mainPanel.Width / 100, liHeight = mainPanel.Height / 100;

                mainPanel.Location = new Point((this.Width - mainPanel.Width) / 2, (this.Height - mainPanel.Height) / 2);

                int splitDist = (lbEdit.Height) * 2;
                splitContainer1.SplitterDistance = splitDist;
                lbEdit.Location = new Point(splitContainer1.Location.X + (lbEdit.Width / 2), (splitDist - lbEdit.Height) / 2);
                lbSave.Location = new Point((lbEdit.Width + lbEdit.Location.X) + (lbEdit.Width / 2), (splitDist - lbSave.Height) / 2);

                if (dgvSchedule.Columns.Count > 0)
                {
                    //Set Width of each Column
                    liWidth = dgvSchedule.Width / 10;
                    //dgvSchedule.Columns["Select"].Width = liWidth / 3;
                    dgvSchedule.Columns["rptName"].Width = liWidth * 3;
                    dgvSchedule.Columns["startTime"].Width = liWidth * 2;
                    dgvSchedule.Columns["interval"].Width = liWidth * 2;
                    dgvSchedule.Columns["newInterval"].Width = liWidth * 2;
                }
            }
            catch (Exception Ex)
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
        private void SyncSchedulerClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                MenuSettings.EnableMenuItem(this.MdiParent, "timer");
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        private void lbEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvSchedule.Rows.Count > 0)
                    FlagButtons(false);
                else
                    MessageBox.Show("No Reports found");
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        private void Panel_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                ControlPaint.DrawBorder(e.Graphics, e.ClipRectangle, Color.LightGray, ButtonBorderStyle.Solid);

            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        private void FlagButtons(bool abFlag)
        {
            try
            {
                lbEdit.Enabled = abFlag;
                lbSave.Enabled = !abFlag;
                dgvSchedule.Enabled = !abFlag;

            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        private void lbSave_Click(object sender, EventArgs e)
        {
            try
            {


                FlagButtons(true);

            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
            /*
            int rCnt, liDays, liInterval;
            DateTime ldStartDate;
            string lsParam, lsTaskName, lsPath;
            rCnt = dgvSchedule.Rows.Count;
            int i = 0;
            foreach (DataRow dr in dgvSchedule.Rows)
            {
                string lsNewInterval = dr.ItemArray[6].ToString();

                liDays = int.Parse(dr.ItemArray[2].ToString());
                liInterval = int.Parse(dr.ItemArray[3].ToString());
                ldStartDate = DateTime.Now.AddMinutes((i + 1) * 10);
                lsParam = dr.ItemArray[4].ToString();

                lsTaskName = "LS_" + lsParam.Replace("-", "") + "_" + _MDISync.mdiGlobalVar.firmCode;
                lsParam += "~" + _MDISync.mdiGlobalVar.firmCode;
                lsPath = _MDISync.mdiGlobalVar.gsApplPath + "BIEngine.exe";

                if (lsNewInterval != "" && lsNewInterval != null)
                {
                    //"30 Minutes", "1 Hour", "2 Hours", "5 Hours", "24 Hours"
                    liInterval = int.Parse(((lsNewInterval == "30 Minutes" ? 0.5 :
                            lsNewInterval == "1 Hour" ? 1 :
                            lsNewInterval == "2 Hours" ? 2 :
                            lsNewInterval == "3 Hours" ? 3 :
                            lsNewInterval == "4 Hours" ? 4 :
                            lsNewInterval == "6 Hours" ? 6 :
                            lsNewInterval == "12 Hours" ? 12 :
                            lsNewInterval == "24 Hours" ? 24 : 1) * 60).ToString());
                }
                else
                {
                    if (liInterval == 0)
                    {
                        liInterval = 24 * 60;
                    }
                }
                i++;
            }*/
        }

        private void dgvSchedule_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // determine if click was on our date column
            if (e.RowIndex < 0)
            {
                return;
            }
            if (e.ColumnIndex == 1)
            {
                // initialize DateTimePicker
                dtp = new DateTimePicker();
                dtp.Format = DateTimePickerFormat.Custom;
                dtp.CustomFormat = "dd/MM/yyyy HH:mm:ss";
                dtp.Visible = true;
                dtp.Value = DateTime.Parse(dgvSchedule.CurrentCell.Value.ToString());

                // set size and location
                var rect = dgvSchedule.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                dtp.Size = new Size(rect.Width, rect.Height);
                dtp.Location = new Point(rect.X, rect.Y);

                // attach events
                dtp.CloseUp += new EventHandler(dtp_CloseUp);
                dtp.TextChanged += new EventHandler(dtp_OnTextChange);

                dgvSchedule.Controls.Add(dtp);
            }
        }
        // on text change of dtp, assign back to cell
        private void dtp_OnTextChange(object sender, EventArgs e)
        {
            try
            {
                dgvSchedule.CurrentCell.Value = dtp.Text.ToString();
            }
            catch
            {

            }
        }

        // on close of cell, hide dtp
        void dtp_CloseUp(object sender, EventArgs e)
        {
            try
            {
                dtp.Visible = false;
            }
            catch
            {

            }
        }
    }
}