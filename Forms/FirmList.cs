using Microsoft.Win32;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LSSyncApp.Forms
{
    public partial class FirmList : Form
    {
        private GlobalVariable _gblVariable;
        public FirmList(GlobalVariable _globalVariable)
        {
            InitializeComponent();
            _gblVariable = _globalVariable;
        }

        private void FirmList_Load(object sender, EventArgs e)
        {
            _gblVariable._fun.Loader(true, wbLoader, _gblVariable.gsTemplatePath, Width, Height);
        }

        private void FirmList_PostLoad(object sender, EventArgs e)
        {
            dataGridView1.Columns[0].Width = dataGridView1.Width;
            _gblVariable._Theme.SetTheme(this, _gblVariable.giTheme);
            _gblVariable._MasterConfig.GetFirmList(_gblVariable, out int liSuccess, out string lsReturn);
            if(liSuccess == 1)
            {
                JsonElement ljeFirmList = _gblVariable.createJsonElement(lsReturn);
                for (int i = 0; i < ljeFirmList.EnumerateArray().Count(); i++)
                {
                    int liRow = dataGridView1.Rows.Add();
                    dataGridView1.Rows[liRow].Cells[0].Value = ljeFirmList[i].GetProperty("firmName").ToString();
                    dataGridView1.Rows[liRow].Cells[1].Value = ljeFirmList[i].GetProperty("firmCode").ToString();
                    if(ljeFirmList[i].GetProperty("firmCode").ToString() == _gblVariable.firmCode)
                    {
                        dataGridView1.Rows[liRow].Selected = true;
                    }
                    else
                    {
                        dataGridView1.Rows[liRow].Selected = false;
                    }
                }
            }
            _gblVariable._fun.Loader(false, wbLoader);
            
        }

        private async void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            _gblVariable._fun.Loader(true, wbLoader, _gblVariable.gsTemplatePath, Width, Height);
            wbLoader.BringToFront();
            await ChangeParamAsync(e.RowIndex);
            _gblVariable._fun.Loader(false, wbLoader);
            this.Close();
        }

        Task ChangeParamAsync(int aiRow)
        {
            return Task.Run(() => ChangeParams(aiRow));
        }

        private void ChangeParams(int aiRow)
        {
            try
            {
                if (aiRow >= 0)
                {
                    _gblVariable.firmCode = dataGridView1.Rows[aiRow].Cells[1].Value.ToString();

                    _gblVariable._MasterConfig.GetUserConfigDetails(_gblVariable, _gblVariable.gsAADUserId, 2, out int iiSuccess, out string lsReturn);
                    if (iiSuccess == 0)
                    {
                        MessageBox.Show(lsReturn, "Exception");
                    }
                    else
                    {
                        _gblVariable._fun.custDBConfig(_gblVariable, _gblVariable.custCode, _gblVariable.firmCode, _gblVariable.gsAADUserId);


                        _gblVariable._fun.SetSettingsVariable(_gblVariable, lsReturn);

                        int liDBExists = _gblVariable._fun.CheckDBExists(_gblVariable, out iiSuccess, out lsReturn);
                        if (iiSuccess == 0)
                        {
                            //setStatusValue("status", lsReturn);
                            MessageBox.Show(lsReturn, "Exception");
                        }
                        else
                        {
                            if (liDBExists == 0)
                            {
                                //setStatusValue("status", "We are Allocating a private space for you.....");
                                _gblVariable._fun.CreateDB(_gblVariable, out iiSuccess, out lsReturn);
                                if (iiSuccess == 0)
                                {
                                    //setStatusValue("status", "");
                                    MessageBox.Show(lsReturn, "Exception");
                                    _gblVariable.setMessageLog(_gblVariable.firmCode + "_" + _gblVariable.gsLogFileName, lsReturn, _gblVariable.giCmd);
                                }
                            }
                            else
                            {
                                //Registry.SetValue(_gblVariable.regPath, "FC", _gblVariable.firmCode);
                            }
                        }
                    }
                }
            }
            catch
            {

            }
            Thread.Sleep(5000);
        }

        private void bClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
