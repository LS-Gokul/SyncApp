using LSSyncApp.Forms;
using LSSyncApp.Functions;
using LSSyncApp.Models;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LSSyncApp
{
    public partial class About : Form
    {
        public static MainForm _MDiSync;
        public static UpdateVersion updateVersion = new UpdateVersion();
        public static AboutPage _aboutPage = new AboutPage();
        public static double idCurrent = 0.0, idLatest = 0.0;
        public static int iiUpdate = 0;
        public static string isUpdateUrl;

        public About(MainForm mdi)
        {
            InitializeComponent();
            _MDiSync = mdi;
            //this.WindowState = FormWindowState.Minimized;
        }

        Task LoadBackGroundAsync()
        {
            return Task.Run(() => About_PostLoad());
            //return Task.Run(() => MainForm_PostLoad());
        }

        private void About_PostLoad()
        {
            string lsDesc;

            //Descreption
            rDesc.Text = "This Application is used for establishing consistency among data from a source to a "
                + "target data storage and the continuous harmonization of the data over time.";
            rLatestVersion.Text = "";
            rCurrentVersion.Text = "";
            //Current Version
            _aboutPage = updateVersion.updateVersion(1, _MDiSync.mdiGlobalVar);
            if (_aboutPage.Status == "Failed")
            {
                lsDesc = _aboutPage.Message;
                rCurrentVersion.Text = lsDesc;
            }
            else
            {
                string lsReleaseDate, lsUpdatedOn, lsReleaseNotes;
                idCurrent = _aboutPage.Version;
                lsReleaseDate = _aboutPage.ReleaseDate.ToString() ?? "-";
                lsUpdatedOn = _aboutPage.UpdatedOn.ToString() ?? "-";
                lsReleaseNotes = _aboutPage.ReleaseNotes.ToString() ?? "-";

                rCurrentVersion.SelectionFont = new Font("Ebrima", 8, FontStyle.Bold);
                rCurrentVersion.SelectionColor = Color.FromArgb(159, 159, 255);
                rCurrentVersion.SelectedText = "Version :";
                rCurrentVersion.SelectionFont = new Font("Ebrima", 8, FontStyle.Regular);
                rCurrentVersion.SelectionColor = Color.White;
                rCurrentVersion.SelectedText = "	" + _aboutPage.Version.ToString("0.00") + Environment.NewLine;
                rCurrentVersion.SelectionFont = new Font("Ebrima", 8, FontStyle.Bold);
                rCurrentVersion.SelectionColor = Color.FromArgb(159, 159, 255);
                rCurrentVersion.SelectedText = "Relesed Date :";
                rCurrentVersion.SelectionFont = new Font("Ebrima", 8, FontStyle.Regular);
                rCurrentVersion.SelectionColor = Color.White;
                rCurrentVersion.SelectedText = "	" + lsReleaseDate + Environment.NewLine;
                rCurrentVersion.SelectionFont = new Font("Ebrima", 8, FontStyle.Bold);
                rCurrentVersion.SelectionColor = Color.FromArgb(159, 159, 255);
                rCurrentVersion.SelectedText = "Updated On :";
                rCurrentVersion.SelectionFont = new Font("Ebrima", 8, FontStyle.Regular);
                rCurrentVersion.SelectionColor = Color.White;
                rCurrentVersion.SelectedText = "	" + lsUpdatedOn + Environment.NewLine;
                rCurrentVersion.SelectionFont = new Font("Ebrima", 8, FontStyle.Bold);
                rCurrentVersion.SelectionColor = Color.FromArgb(159, 159, 255);
                rCurrentVersion.SelectedText = "Relese Notes :";
                rCurrentVersion.SelectionFont = new Font("Ebrima", 8, FontStyle.Regular);
                rCurrentVersion.SelectionColor = Color.White;
                rCurrentVersion.SelectedText = Environment.NewLine + "      " + lsReleaseNotes + Environment.NewLine;
            }


            //Latest Version
            _aboutPage = updateVersion.updateVersion(2, _MDiSync.mdiGlobalVar);
            if (_aboutPage.Status == "Failed")
            {
                lsDesc = _aboutPage.Message;
                rLatestVersion.Text = lsDesc;
            }
            else
            {
                idLatest = _aboutPage.Version;

                rLatestVersion.SelectionFont = new Font("Ebrima", 8, FontStyle.Bold);
                rLatestVersion.SelectionColor = Color.FromArgb(159, 159, 255);
                rLatestVersion.SelectedText = "Version :";
                rLatestVersion.SelectionFont = new Font("Ebrima", 8, FontStyle.Regular);
                rLatestVersion.SelectionColor = Color.White;
                rLatestVersion.SelectedText = "	" + _aboutPage.Version.ToString("0.00") + Environment.NewLine;
                rLatestVersion.SelectionFont = new Font("Ebrima", 8, FontStyle.Bold);
                rLatestVersion.SelectionColor = Color.FromArgb(159, 159, 255);
                rLatestVersion.SelectedText = "Relesed Date :";
                rLatestVersion.SelectionFont = new Font("Ebrima", 8, FontStyle.Regular);
                rLatestVersion.SelectionColor = Color.White;
                rLatestVersion.SelectedText = "	" + _aboutPage.ReleaseDate + Environment.NewLine;
                rLatestVersion.SelectionFont = new Font("Ebrima", 8, FontStyle.Bold);
                rLatestVersion.SelectionColor = Color.FromArgb(159, 159, 255);
                rLatestVersion.SelectedText = "Relese Notes :";
                rLatestVersion.SelectionFont = new Font("Ebrima", 8, FontStyle.Regular);
                rLatestVersion.SelectionColor = Color.White;
                rLatestVersion.SelectedText = Environment.NewLine + _aboutPage.ReleaseNotes + Environment.NewLine;
            }

            isUpdateUrl = updateVersion.getUpdateURL();
            if (idLatest > idCurrent && isUpdateUrl.Contains("https://"))
            {
                iiUpdate = 0;
                lbUpdate.Visible = true;
            }
            else if (idLatest > idCurrent)
            {
                iiUpdate = 1;
                lbUpdate.Visible = true;
            }
            else if (isUpdateUrl.Contains("https://"))
            {
                iiUpdate = 2;
                lbUpdate.Visible = true;
            }
            else
            {
                lbUpdate.Visible = false;
            }
            rDesc.ReadOnly = true;
            rCurrentVersion.ReadOnly = true;
            rLatestVersion.ReadOnly = true;
            //this.WindowState = FormWindowState.Maximized;

            mainPanel.Region = Region.FromHrgn(_MDiSync.mdiGlobalVar.createRoundRect(0, 0, mainPanel.Width, mainPanel.Height, 50, 50));
            rCurrentVersionBack.Region = Region.FromHrgn(_MDiSync.mdiGlobalVar.createRoundRect(0, 0, rCurrentVersionBack.Width, rCurrentVersionBack.Height, 50, 50));
            rLatestVersionBack.Region = Region.FromHrgn(_MDiSync.mdiGlobalVar.createRoundRect(0, 0, rLatestVersionBack.Width, rLatestVersionBack.Height, 50, 50));
            rCurrentVersionB1.Region = Region.FromHrgn(_MDiSync.mdiGlobalVar.createRoundRect(0, 0, rCurrentVersionB1.Width, rCurrentVersionB1.Height, 50, 50));
            rLatestVersionB1.Region = Region.FromHrgn(_MDiSync.mdiGlobalVar.createRoundRect(0, 0, rLatestVersionB1.Width, rLatestVersionB1.Height, 50, 50));

            lbCancel.FlatStyle = FlatStyle.Flat;
            lbCancel.FlatAppearance.BorderSize = 0;
            lbCancel.FlatAppearance.BorderColor = Color.FromArgb(0, 255, 255, 255);

            lbUpdate.FlatStyle = FlatStyle.Flat;
            lbUpdate.FlatAppearance.BorderSize = 0;
            lbUpdate.FlatAppearance.BorderColor = Color.FromArgb(0, 255, 255, 255);

            //Set Theme Color
            _MDiSync.mdiGlobalVar._Theme.SetTheme(this, _MDiSync.mdiGlobalVar.giTheme);
        }

        private void About_Load(object sender, EventArgs e)
        {
            
        }  

        private async void ResizeEvent(object sender, EventArgs e)
        {
            await LoadBackGroundAsync();
            int liHeight, liWidth, liX, liY;
            liWidth = this.Width / 2;
            liHeight = this.Height / 2;

            liX = liWidth - (mainPanel.Width / 2);
            liY = liHeight - (mainPanel.Height / 2);

            mainPanel.Location = new System.Drawing.Point(liX, liY);
        }

        private void lbCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lbUpdate_Click(object sender, EventArgs e)
        {
            /*DialogResult res = MessageBox.Show("Latest Version is now Avaliable..\r\n\r\nCurrent Version : " + idCurrent.ToString("0.00")
                                + "\r\nLatest Version: " + idLatest.ToString("0.00") + "\r\n\r\nDo you want to update", "Version", MessageBoxButtons.YesNo);
            */
            DialogResult res = MessageBox.Show("Latest Version is now Avaliable..\r\n\r\nDo you want to update",
                                        "Version", MessageBoxButtons.YesNo);
            
            if (res == DialogResult.Yes)
            {
                if(iiUpdate == 0 || iiUpdate == 1)
                {
                    pVersionUpdate.Visible = true;
                    pVersionUpdate.Maximum = 100;
                    pVersionUpdate.Step = 1;

                    var progress = new Progress<int>(v =>
                    {
                        pVersionUpdate.Value = v;
                    });
                    _aboutPage = updateVersion.updateVersion(idCurrent.ToString("0.00"), _MDiSync.mdiGlobalVar.giCmd, idLatest.ToString("0.00"), progress);
                    _MDiSync.mdiGlobalVar.gsVersion = _aboutPage.Version.ToString("0.00");
                    pVersionUpdate.Visible = false;
                    About_Load(sender, e);
                }
                if (iiUpdate == 0 || iiUpdate == 2)
                {
                    _aboutPage = updateVersion.appUpdate(isUpdateUrl);
                    if (_aboutPage.Status == "Failed")
                    {
                        MessageBox.Show(_aboutPage.Message);
                    }
                }
                    
            }
        }

        private void AboutClosing(object sender, FormClosingEventArgs e)
        {
            MenuSettings.EnableMenuItem(this.MdiParent, "about");
        }

    }

}
