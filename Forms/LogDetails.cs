using System;
using System.Data;
using System.Windows.Forms;

namespace LSSyncApp.Forms
{
    public partial class LogDetails : Form
    {
        private string isLogId, isReturn, isText = "";
        private GlobalVariable igvGblVar;
        private int iiSuccess;

        private void rtbLogDetails_TextChanged(object sender, EventArgs e)
        {

        }

        public LogDetails(string asLogId, GlobalVariable agVar)
        {
            InitializeComponent();
            isLogId = asLogId;
            igvGblVar = agVar;
        }

        private void lbCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LogDetails_Load(object sender, EventArgs e)
        {
            try
            {
                label1.Text = isLogId;
                igvGblVar._Theme.SetTheme(this, igvGblVar.giTheme);

                igvGblVar._DestinationConfig.ExecuteRawQuery(igvGblVar, "", 2, out iiSuccess, out isReturn);
                if (iiSuccess == 0)
                {
                    this.Close();
                    return;
                }

                igvGblVar._DestinationConfig.LogDetails(igvGblVar, isLogId,out iiSuccess, out isReturn);
                if (iiSuccess == 1)
                {
                    DataTable lDTLogDetails = igvGblVar.GetJSONToDataTable(isReturn);
                    foreach (DataRow dtR in lDTLogDetails.Rows)
                    {
                        isText += dtR.ItemArray[0].ToString();
                    }

                    rtbLogDetails.Text = isText.Replace("~~~~~", Environment.NewLine)
                        .Replace("!$!", Environment.NewLine + Environment.NewLine)
                        .Replace("$", Environment.NewLine).Replace("*+*+*+*", "	");
                }
            }
            catch(Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }
    }
}
