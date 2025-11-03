using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace Updates
{
    public partial class Updates : Form
    {
        public static string[] cmdParms;
        public static string gsApplPath = AppDomain.CurrentDomain.BaseDirectory + "\\";

        public Updates(string[] args)
        {
            InitializeComponent();
            cmdParms = args;
        }

        private void Updates_Load(object sender, EventArgs e)
        {
            splitContainer1.SplitterDistance = (this.Width / 10) * 4;
            //label1.MaximumSize = new Size((this.Width / 10) * 5, 0);
            //label1.AutoSize = true;
        }

        private void Post_Load(object sender, EventArgs e)
        {
            Thread t1 = new Thread(RunThread);
            t1.Start();
        }


        private void RunThread()
        {
            int i = 0;
            string lsMessage = "-";
            try
            {
                //Removing Existing Updates
                if (Directory.Exists(gsApplPath + "Updates")) Directory.Delete(gsApplPath + "Updates", true);
                ProgressUpdate("Removing Existing Updates....",0);

                //Downloading New Updates
                using (WebClient web1 = new WebClient())
                {
                    web1.DownloadFile(cmdParms[0], "App.zip");
                }
                ProgressUpdate("Downloading New Updates....", 1);

                //Extracting
                System.IO.Compression.ZipFile.ExtractToDirectory(gsApplPath + "App.zip", gsApplPath + "Updates");
                ProgressUpdate("Extracting....", 2);

                
                //Installing (Copy & Replacing the files to main Directory)
                foreach (string newPath in Directory.GetFiles(gsApplPath + "Updates", "*.*", SearchOption.AllDirectories))
                {
                    File.Copy(newPath, newPath.Replace(gsApplPath + "Updates", gsApplPath), true);
                }
                ProgressUpdate("Installing....", 3);

                //Removing Downloaded Files
                File.Delete(gsApplPath + "App.zip");
                ProgressUpdate("Clearing Unwanted Files....", 0);
                Directory.Delete(gsApplPath + "Updates", true);
                i = 1;
            }
            catch (Exception Ex)
            {
                lsMessage = Ex.Message.Replace(" ", "-");
                if (cmdParms[1] == "0")
                    MessageBox.Show(Ex.Message, "Failed");
            }
            Application.Exit();
            string lsCmds = "";
            if(cmdParms.Length > 2)
                for(int cmd = 2; cmd < cmdParms.Length; cmd++)
                {
                    lsCmds = " " + cmdParms[cmd];
                }
            Process.Start(gsApplPath + "LSEngine.exe","Updates " + (i == 1 ? "Success" : "Failed") 
                + " " + lsMessage + " " + cmdParms[1] + lsCmds);
        }

        private void ProgressUpdate(string asMessage,int aiImage)
        {
            switch(aiImage)
            {
                case 0:
                    pictureBox1.Image = Properties.Resources.UErasing;
                    label1.ForeColor = Color.Black;
                    break;
                case 1:
                    pictureBox1.Image = Properties.Resources.UFileTransfer;
                    label1.ForeColor = Color.DarkGoldenrod;
                    break;
                case 2:
                    pictureBox1.Image = Properties.Resources.UExtract;
                    label1.ForeColor = Color.DeepSkyBlue;
                    break;
                case 3:
                    pictureBox1.Image = Properties.Resources.UInstall;
                    label1.ForeColor = Color.LimeGreen;
                    break;
            }
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            label1.Text = asMessage;
            for (int i = 1; i <= 100; i++)
            {
                progressBar1.Value = i;
                Thread.Sleep(20);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}