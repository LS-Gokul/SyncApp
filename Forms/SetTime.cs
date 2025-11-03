using System;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace LSSyncApp.Forms
{
    public partial class SetTime : Form
    {
        private GlobalVariable gblVar;
        private int iiType, iiSuccess;
        private string isParam, isReturn, isSqlString;
        public string MinDate { get; set; }
        public int iiRet { get; set; }

        public SetTime(GlobalVariable agblVar, string asParam, int aiType)
        {
            InitializeComponent();
            gblVar = agblVar;
            isParam = asParam;
            iiType = aiType;
        }

        private void SetTime_Load(object sender, EventArgs e)
        {
            dtpMinDate.MaxDate = DateTime.Now;
            dtpMinDate.Value = DateTime.Now.AddDays(-1);
            
            dgvTableList.Columns[0].Width = (dgvTableList.Width / 40) * 3;
            dgvTableList.Columns[1].Width = (dgvTableList.Width / 10) * 9;

            gblVar._Theme.SetTheme(this, gblVar.giTheme);

        }

        private void SetTime_PostLoad(object sender, EventArgs e)
        {
            gblVar._MasterConfig.GetFetchTableList(gblVar, iiType, "'" + isParam + "'", out iiSuccess, out isReturn);
            if(iiSuccess == 1)
            {
                JsonElement ljeTableList = gblVar.createJsonElement(isReturn);
                int liCnt = ljeTableList.EnumerateArray().Count();
                for (int i = 0; i < liCnt; i++)
                {
                    int llRow = dgvTableList.Rows.Add();
                    dgvTableList.Rows[i].Cells[0].Value = true;
                    dgvTableList.Rows[llRow].Cells[1].Value = ljeTableList[i].GetProperty("tblShort").ToString();
                    dgvTableList.Rows[llRow].Cells[2].Value = ljeTableList[i].GetProperty("tableName").ToString();
                }

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MinDate = dtpMinDate.Value.ToString("yyyy-MM-dd");
            iiRet = UpdateMinTime();
            this.Close();
        }

        private int UpdateMinTime()
        {
            int liRet = 0, j = 0;
            string lsDate = MinDate, lsTblName, lsTblList = "";
            try
            {
                for(int i = 0; i < dgvTableList.Rows.Count; i++)
                {
                    if (bool.Parse(dgvTableList.Rows[i].Cells[0].Value.ToString()) == true)
                    {
                        lsTblName = dgvTableList.Rows[i].Cells[2].Value.ToString();
                        lsTblList += (j == 0 ? "" : ",") + "'" + lsTblName + "'";
                        j++;
                    }
                }
                if (j > 0)
                {
                    isSqlString = "Update LS_MaxTime Set [Max Time] = '" + lsDate + "' Where [Table Name] in (" + lsTblList + ");";
                    gblVar._DestinationConfig.ExecuteRawQuery(gblVar, "", 2, out iiSuccess, out isReturn);
                    if (iiSuccess == 1)
                    {
                        gblVar._DestinationConfig.ExecuteRawQuery(gblVar, isSqlString, 1, out iiSuccess, out isReturn);
                        if (iiSuccess == 0)
                        {
                            MessageBox.Show(isReturn);
                        }
                        else
                        {
                            liRet = 1;
                        }
                    }
                    else
                    {
                        MessageBox.Show(isReturn);
                    }
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
            return liRet;
        }
    }
}
