using LSSyncApp.Controllers;
using System;
using System.Windows.Forms;

namespace LSSyncApp.Forms
{
    public partial class CompanyList : Form
    {
        public static TallyODBC _TallyODBC;
        public string ExternalName { get; set; }
        public string ExternalID { get; set; }

        //public CompanyList(TallyODBC tOdbc)
        public CompanyList(TallyODBC tOdbc)
        {
            InitializeComponent();
            _TallyODBC = tOdbc;
        }

        private void dgvCompanyList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex >= 0)
            {
                ExternalID = dgvCompanyList.Rows[e.RowIndex].Cells["guid"].Value.ToString();
                ExternalName = dgvCompanyList.Rows[e.RowIndex].Cells["CompanyName"].Value.ToString();
                this.Close();
            }
        }

        private void CompanyList_Load(object sender, EventArgs e)
        {
            FetchCompany();
            ExternalID = "Failed";
            ExternalName = "Company Not Selected";
        }
        private void CompanyList_PostLoad(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FetchCompany();
        }

        private async void FetchCompany()
        {
            string lsIp = ltIp.Text, lsPort = ltPort.Text;
            dgvCompanyList.Columns[0].Width = (dgvCompanyList.Width / 100) * 98;
            try
            {
                string lsXml = await _TallyODBC.CheckDetails(lsIp,lsPort, "select $Name,$GUID from Company");
                if (lsXml == "Tally not connected")
                {
                    ExternalID = "Failed";
                    ExternalName = lsXml;
                    this.Close();
                }
                else
                {
                    if (lsXml.Contains("<RESULTDATA>"))
                    {
                        lsXml = lsXml.Substring(lsXml.IndexOf("<RESULTDATA>") + 12);
                        lsXml = lsXml.Substring(0, lsXml.IndexOf("</RESULTDATA>"));

                        while (lsXml.Contains("<ROW>"))
                        {
                            string lsExID, lsExName;

                            lsXml = lsXml.Substring(lsXml.IndexOf("<COL>") + 5);
                            lsExName = lsXml.Substring(0, lsXml.IndexOf("</COL>"));

                            lsXml = lsXml.Substring(lsXml.IndexOf("<COL>") + 5);
                            lsExID = lsXml.Substring(0, lsXml.IndexOf("</COL>"));

                            var seq = dgvCompanyList.Rows.Add();
                            dgvCompanyList.Rows[seq].Cells["CompanyName"].Value = lsExName;
                            dgvCompanyList.Rows[seq].Cells["guid"].Value = lsExID;
                        }
                    }
                    else
                    {
                        ExternalID = "Failed";
                        ExternalName = "Company List Not Found";
                        this.Close();
                    }
                }
            }
            catch (Exception Ex)
            {
                ExternalID = "Failed";
                ExternalName = Ex.Message;
                this.Close();
            }
        }

        private void dgvCompanyList_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
