using Newtonsoft.Json;
using System;
using System.Data;
using System.Windows.Forms;

namespace LSSyncApp.Forms
{
    public partial class Schedulers : Form
    {
        private GlobalVariable gblVar;
        private string isData;
        public DataTable _SchedulerData { get; set; }
        public int _Status { get; set; }
        //public DataTable _DataTable { get; set };

        public Schedulers(GlobalVariable agblVar, string asData)
        {
            InitializeComponent();
            gblVar = agblVar;
            _SchedulerData = new DataTable();
            _Status = 0;
            isData = asData;
        }

        private void Scheduler_Load(object sender, EventArgs e)
        {
            try
            {   
                DataTable dt = (DataTable)JsonConvert.DeserializeObject(isData, (typeof(DataTable)));
                dgvScheduleList.DataSource = dt;

                if (dgvScheduleList.Rows.Count > 0)
                {
                    //Set Visible
                    dgvScheduleList.Columns["syncParam"].Visible = false;
                    dgvScheduleList.Columns["syncDays"].Visible = false;
                    dgvScheduleList.Columns["syncInterval"].Visible = false;

                    //Set Header Text
                    dgvScheduleList.Columns["rptName"].HeaderText = "Report Name";
                    dgvScheduleList.Columns["startTime"].HeaderText = "Start Time";

                    //Initial Sort
                    dgvScheduleList.Sort(dgvScheduleList.Columns["rptName"], System.ComponentModel.ListSortDirection.Ascending);

                    //Check Box Column
                    DataGridViewCheckBoxColumn curCol = new DataGridViewCheckBoxColumn
                    {
                        HeaderText = "",
                        Name = "Select"
                    };
                    dgvScheduleList.Columns.Insert(0, curCol);

                    //Old Interval Column as Textbox
                    DataGridViewTextBoxColumn intCol = new DataGridViewTextBoxColumn
                    {
                        HeaderText = "Interval",
                        Name = "interval",
                        ReadOnly = true
                    };
                    dgvScheduleList.Columns.Insert(3, intCol);

                    //New Interval Column as Dropdown list
                    DataGridViewComboBoxColumn newIntCol = new DataGridViewComboBoxColumn
                    {
                        HeaderText = "New Interval",
                        Name = "newInterval",
                        Items = {"30 Minutes", "1 Hour", "2 Hours", "5 Hours", "24 Hours" },
                        ReadOnly = false
                    };
                    dgvScheduleList.Columns.Insert(4, newIntCol);

                    gblVar._Theme.SetTheme(this, gblVar.giTheme);

                    //Select all reports Initially
                    for (int i = 0; i < dgvScheduleList.Rows.Count; i++)
                    {
                        dgvScheduleList.Rows[i].Cells["Select"].Value = true;
                        double liSyncInterval = double.Parse(dgvScheduleList.Rows[i].Cells["syncInterval"].Value.ToString());
                        if (liSyncInterval == 0) liSyncInterval = 24 * 60;

                        string lsInt = (liSyncInterval / 60).ToString("0.00");

                        string lsInterval = liSyncInterval / 60 >= 1
                            ? lsInt.Substring(0, lsInt.IndexOf(".")) + " Hr" + 
                                (int.Parse(lsInt.Substring(lsInt.IndexOf(".") + 1)) > 0 
                                    ? " " + lsInt.Substring(lsInt.IndexOf(".") + 1) + " Mins"
                                    : "")
                            : int.Parse(liSyncInterval.ToString("0")) + " Mins";

                        dgvScheduleList.Rows[i].Cells["interval"].Value = lsInterval;
                    }

                    //Set Red only Property
                    dgvScheduleList.Columns["rptName"].ReadOnly = true;
                    dgvScheduleList.Columns["startTime"].ReadOnly = true;

                    //Set Width of each Column
                    int liWidth = dgvScheduleList.Width / 10;
                    dgvScheduleList.Columns["Select"].Width = liWidth;
                    dgvScheduleList.Columns["rptName"].Width = liWidth * 3;
                    dgvScheduleList.Columns["startTime"].Width = liWidth * 2;
                    dgvScheduleList.Columns["interval"].Width = liWidth * 2;
                    dgvScheduleList.Columns["newInterval"].Width = liWidth * 2;

                    //Create Columns to Data Table
                    foreach (DataGridViewColumn dc in dgvScheduleList.Columns)
                    {
                        if (dc.Name != "Select")
                            _SchedulerData.Columns.Add(dc.Name, typeof(string));
                    }


                    //Set Sort Mode Property
                    dgvScheduleList.Columns["rptName"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dgvScheduleList.Columns["startTime"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dgvScheduleList.Columns["Select"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dgvScheduleList.Columns["interval"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dgvScheduleList.Columns["newInterval"].SortMode = DataGridViewColumnSortMode.NotSortable;

                }
                else
                {
                    _Status = 0;
                    this.Close();
                }
            }
            catch
            {
                this.Close();
                _Status = 0;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int i = 0;
                for (i = 0; i < dgvScheduleList.Rows.Count; i++)
                {
                    if (dgvScheduleList.Rows[i].Cells["Select"].Value.ToString() == "true" ||
                            dgvScheduleList.Rows[i].Cells["Select"].Value.ToString() == "True")
                    {
                        _SchedulerData.Rows.Add(dgvScheduleList.Rows[i].Cells["rptName"].Value,
                            dgvScheduleList.Rows[i].Cells["startTime"].Value,
                            dgvScheduleList.Rows[i].Cells["syncDays"].Value,
                            dgvScheduleList.Rows[i].Cells["syncInterval"].Value,
                            dgvScheduleList.Rows[i].Cells["syncParam"].Value,
                            dgvScheduleList.Rows[i].Cells["interval"].Value,
                            dgvScheduleList.Rows[i].Cells["newInterval"].Value
                            );
                        //_SchedulerData.Rows[j].ItemArray = ((DataTable)dgvScheduleList.DataSource).Rows[i].ItemArray;
                        //j++;
                    }
                }

                /********Check Status**********/
                _SchedulerData.Rows.Add("Check Status", (DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00"),
                    "1", "10", "-CheckStatus", "10 Minutes", "");

                _SchedulerData.AcceptChanges();
                _Status = 1;
            }
            catch
            {
                _Status = 0;
            }
            this.Close();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
