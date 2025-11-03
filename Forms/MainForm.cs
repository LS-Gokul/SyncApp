using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Text.Json;
using LSSyncApp.Functions;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using LSSyncApp.Models;

namespace LSSyncApp.Forms
{
    public partial class MainForm : Form
    {
        public GlobalVariable mdiGlobalVar = new GlobalVariable();
        public static UpdateVersion updateVersion = new UpdateVersion();
        public static string isURL;
        private DashboardNew _DashboardNew;
        private About _About;
        private MismatchUtility _MismatchUtility;
        private SyncData _Sync;
        private SyncScheduler _SyncScheduler;

        public MainForm(GlobalVariable MDIgbl)
        {
            try
            {
                InitializeComponent();
                mdiGlobalVar = MDIgbl;
                ConstructLoader();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        private void ShowNewForm(object sender, EventArgs e)
        {
            try
            {
                dataSync.Enabled = false;
                _Sync = new SyncData(this)
                {
                    MdiParent = this,
                    FormBorderStyle = FormBorderStyle.None,
                    Dock = DockStyle.Fill
                };
                _Sync.Show();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        private void MainForm_Load()
        {
            try
            {
                //API Calling for User Image
                RestAPI restAPI = new RestAPI();
                byte[] bytes = null;
                int i = 0;
                try
                {
                    string lsUrl = mdiGlobalVar.gsContainerLocation + mdiGlobalVar.gsLogoContainer + "/[LSCode]/UserProfileImages/[UserCode].bin";
                    bytes = Convert.FromBase64String(restAPI.getAPICalling(
                        lsUrl.Replace("[LSCode]", mdiGlobalVar.custCode).Replace("[UserCode]", mdiGlobalVar.gsUserId), out _));
                    i = 1;
                }
                catch
                {

                }
                if (i == 1) pUserIcon.BackgroundImage = Image.FromStream(new MemoryStream(bytes));
                else pUserIcon.BackgroundImage = Properties.Resources.UserIcon;
                pUserIcon.BackgroundImageLayout = ImageLayout.Stretch;
                restAPI.Dispose();

                //Notifications for update
                isURL = updateVersion.getUpdateURL();
                if (isURL.Contains("https://") && mdiGlobalVar.giCmd == 0)
                {
                    toolStripStatusLabel5.Image = Properties.Resources.Bell1;
                    lUpdate.Text = "Latest Version of App is Available...";
                    bUpdate.FlatStyle = FlatStyle.Flat;
                    bUpdate.FlatAppearance.BorderSize = 0;
                    bUpdate.FlatAppearance.BorderColor = Color.FromArgb(0, 255, 255, 255);
                }
                else
                {
                    lUpdate.Text = "No Updates Available...";
                    bUpdate.Visible = false;
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        Task LoadBackGroundAsync()
        {
            return Task.Run(() => MainForm_Load());
            //return Task.Run(() => MainForm_PostLoad());
        }

        private async void MainForm_PostLoad(object sender, EventArgs e)
        {
            try
            {
                mdiGlobalVar._fun.Loader(true, wbLoader, mdiGlobalVar.gsTemplatePath, Width, Height);
                
                popupPanel.Region = Region.FromHrgn(mdiGlobalVar.createRoundRect(0, 0, popupPanel.Width, popupPanel.Height, 50, 50));
                //notPanel.Region = Region.FromHrgn(mdiGlobalVar.createRoundRect(0, 0, notPanel.Width, notPanel.Height, 50, 50));
                popupPanel.BackColor = Color.FromArgb(0, 255, 255, 255);
                bLogout.FlatStyle = FlatStyle.Flat;
                bLogout.FlatAppearance.BorderSize = 0;
                bLogout.FlatAppearance.BorderColor = Color.FromArgb(0, 255, 255, 255);

                int liWidth = this.Width / 20, liImageWidth, liMarginWidth;
                toolStrip.Size = new Size(liWidth, toolStrip.Height);

                liImageWidth = (liWidth / 5) * 3;
                liMarginWidth = liImageWidth / 2;
                toolStrip.ImageScalingSize = new Size(liImageWidth, liImageWidth);

                logo.Size = new Size(toolStrip.Size.Width, logo.Height);
                logo.Padding = new Padding((logo.Width / 10) * 2, 7, (logo.Width / 10) * 2, 7);

                label1.Left = logo.Right;
                label2.Left = logo.Right;

                foreach (ToolStripItem items in toolStrip.Items)
                {
                    items.Size = new Size(liImageWidth, liImageWidth);
                    items.ImageScaling = ToolStripItemImageScaling.SizeToFit;
                    items.Margin = new Padding(liMarginWidth, liMarginWidth, liMarginWidth, liMarginWidth / 10);
                    items.Padding = new Padding(13, 7, 13, 7);
                }

                this.WindowState = mdiGlobalVar.winState;
                label1.Text = mdiGlobalVar.custName;
                label2.Text = mdiGlobalVar.firmName;
                rUserDetails.Text = "";
                rUserDetails.SelectionFont = new Font("Ebrima", 12, FontStyle.Bold);
                //rUserDetails.SelectionColor = Color.White;
                rUserDetails.SelectedText = mdiGlobalVar.gsUserName;
                rUserDetails.SelectionFont = new Font("Ebrima", 7, FontStyle.Bold);
                //rUserDetails.SelectionColor = Color.Silver;
                rUserDetails.SelectedText = Environment.NewLine + mdiGlobalVar.gsAADUserId;
                rUserDetails.SelectionFont = new Font("Ebrima", 7, FontStyle.Bold);
                //rUserDetails.SelectionColor = Color.Silver;
                rUserDetails.SelectedText = Environment.NewLine + mdiGlobalVar.gsUserRoleName;

                updateVersion.versionCheck(mdiGlobalVar);
                toolStrip.Renderer = new MySR();

                toolStripStatusLabel1.Text = mdiGlobalVar.gsSystemUser + " | ";
                toolStripStatusLabel2.Text = mdiGlobalVar.gsSystemName + " | ";
                toolStripStatusLabel3.Text = mdiGlobalVar.gsSystemIp + " | ";

                if (checkInternet() == "Success")
                {
                    toolStripStatusLabel4.ForeColor = Color.FromArgb(89, 250, 159);
                    toolStripStatusLabel4.Text = "Internet Connected";
                    toolStripStatusLabel4.Image = Properties.Resources.wifi;
                }
                else
                {
                    toolStripStatusLabel4.ForeColor = Color.FromArgb(250, 89, 89);
                    toolStripStatusLabel4.Text = "Internet Not Connected";
                    toolStripStatusLabel4.Image = Properties.Resources.nowifi;
                }

                bLight.FlatStyle = FlatStyle.Flat;
                bLight.FlatAppearance.BorderSize = 0;
                bLight.FlatAppearance.BorderColor = Color.FromArgb(0, 255, 255, 255);

                bDark.FlatStyle = FlatStyle.Flat;
                bDark.FlatAppearance.BorderSize = 0;
                bDark.FlatAppearance.BorderColor = Color.FromArgb(0, 255, 255, 255);

                themePanel.Size = new Size((popupPanel.Width / 10) * 9, themePanel.Height);
                themePanel.Location = new Point((popupPanel.Width - themePanel.Width) / 2, themePanel.Location.Y);
                splitContainer3.SplitterDistance = themePanel.Width / 2;

                themeBackPanel.Size = new Size(bLight.Width + splitContainer3.SplitterWidth, themeBackPanel.Height);
                themeBackPanel.Left = themePanel.Left;

                Text = Text + " [Version - " + mdiGlobalVar.gsAppVersion + "]";

                await LoadBackGroundAsync();

                ////////////////////////////Post Load
                _DashboardNew = new DashboardNew(this)
                {
                    MdiParent = this,
                    FormBorderStyle = FormBorderStyle.None,
                    Dock = DockStyle.Fill
                };
                _DashboardNew.Show();

                wbLoader.BringToFront();

                wbNotPanel.Height = (this.Height / 5) * 4;
                wbNotPanel.Width = this.Width / 5;
                WBNotificationPanel();
                wbNotification.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(browser_DocumentCompleted);

                bLight_Click(sender, e);

                //this.bgNotifications.DoWork += new DoWorkEventHandler(this.bgNotifications_DoWork);
                mdiGlobalVar._fun.Loader(false, wbLoader);

                bgNotifications.RunWorkerAsync();
            }
            catch
            {
                mdiGlobalVar._fun.Loader(false, wbLoader);
            }
        }

        private void About_Clicked(object sender, EventArgs e)
        {
            try
            {
                about.Enabled = false;
                _About = new About(this)
                {
                    MdiParent = this,
                    FormBorderStyle = FormBorderStyle.None,
                    Dock = DockStyle.Fill
                };
                _About.Show();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (Form childForm in MdiChildren)
                {
                    childForm.Close();
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        //private async void saveToolStripButton_Click(object sender, EventArgs e)
        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                logout();
                this.Close();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        //private async Task<bool> logout()
        private bool logout()
        {
            //Registry.SetValue(mdiGlobalVar.regPath, "CC", "");
            //Registry.SetValue(mdiGlobalVar.regPath, "FC", "");
            //Registry.SetValue(mdiGlobalVar.regPath, "UE", "");
            /*
            /////////////////////////////////////////////////////
            IPublicClientApplication PublicClientApp;
            ///////////////////////AAD Authentication//////////////////////////////
            PublicClientApp = PublicClientApplicationBuilder.Create(mdiGlobalVar.ClientId)
                            .WithRedirectUri(mdiGlobalVar.RedirectUrl)
                            .WithB2CAuthority(mdiGlobalVar.Authority)
                            .Build();
            TokenCacheHelper.EnableSerialization(PublicClientApp.UserTokenCache);

            var accounts = await PublicClientApp.GetAccountsAsync();
            if (accounts.Any())
            {
                try
                {
                    await PublicClientApp.RemoveAsync(accounts.FirstOrDefault());
                }
                catch (MsalException ex)
                {
                    MessageBox.Show($"Error signing-out user: {ex.Message}");
                    return false;
                }
            }*/
            try
            {
                DeserializeJWT _DeserializeJWT = new DeserializeJWT();
                _DeserializeJWT.SignOut(out int liSuccess);
                return (liSuccess == 1 ? true : false);
            }
            catch
            {
                return false;
            }
        }

        private bool MouseIsOverControl(Control ctrl) => ctrl == null ? false : ctrl.ClientRectangle.Contains(ctrl.PointToClient(Cursor.Position));

        private string checkInternet()
        {
            try
            {
                Ping myPing = new Ping();
                string host = "google.com";
                byte[] buffer = new byte[32];
                int timeout = 1000;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                if (reply.Status == IPStatus.Success)
                {
                    return "Success";
                }
                return "Failed";
            }
            catch (Exception)
            {
                return "Failed";
            }
        }

        //private async void aBCToolStripMenuItem_Click(object sender, EventArgs e)
        private void aBCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                logout();
                this.Close();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        private void pUserIcon_Click(object sender, EventArgs e)
        {
            try
            {
                int xLoc = this.Right - popupPanel.Width;
                popupPanel.Location = new Point(xLoc, pUserIcon.Height);
                popupPanel.Visible = true;
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        private void pUserIcon_Click_Leave(object sender, EventArgs e)
        {
            try
            {
                if (MouseIsOverControl(popupPanel) || MouseIsOverControl(pUserIcon) || MouseIsOverControl(shape1)
                    || MouseIsOverControl(panel2) || MouseIsOverControl(bLogout)
                    || MouseIsOverControl(pbLogout) || MouseIsOverControl(bLight) || MouseIsOverControl(bDark)
                    || MouseIsOverControl(themeBackPanel)) return;
                popupPanel.Visible = false;
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        private void Notification_Enter(object sender, EventArgs e)
        {
            try
            {
                //AppUpdate 
                mdiGlobalVar._MasterConfig.GetAppLatstVersion(mdiGlobalVar, out _, out _);

                //Triggers
                mdiGlobalVar._fun.Triggers(mdiGlobalVar);

                int xLoc = this.Right - wbNotPanel.Width;
                wbNotPanel.Location = new Point(xLoc - 20, statusStrip1.Location.Y + 10 - wbNotPanel.Height);
                wbNotPanel.Visible = true;
                /*
                int xLoc = this.Right - notPanel.Width;
                notPanel.Location = new Point(xLoc, statusStrip1.Location.Y + 10 - notPanel.Height);
                notPanel.Visible = true;
                */
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        private void Notification_Leave(object sender, EventArgs e)
        {
            try
            {
                if (MouseIsOverControl(notificationPanel) || MouseIsOverControl(wbNotPanel)
                    || MouseIsOverControl(bUpdate) || MouseIsOverControl(lUpdate)) return;
                wbNotPanel.Visible = false;
                /*
                if (MouseIsOverControl(notificationPanel) || MouseIsOverControl(notPanel)
                    || MouseIsOverControl(bUpdate) || MouseIsOverControl(lUpdate)) return;
                notPanel.Visible = false;*/
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        //private async void panel2_Paint(object sender, EventArgs e)
        private void panel2_Paint(object sender, EventArgs e)
        {
            try
            {
                logout();
                this.Close();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        private void bLight_Click(object sender, EventArgs e)
        {
            try
            {
                logo.Image = Properties.Resources.AppLogoWOB;

                mdiGlobalVar.giTheme = 0;
                themeBackPanel.BackColor = Color.Gainsboro;
                themeBackPanel.Left = themePanel.Left;
                SetThemeForActiveForms();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        private void bDark_Click(object sender, EventArgs e)
        {
            try
            {
                logo.Image = Properties.Resources.AppLogoWOBW;

                mdiGlobalVar.giTheme = 1;
                themeBackPanel.BackColor = Color.Black;
                themeBackPanel.Left = themePanel.Left + splitContainer3.SplitterDistance;
                SetThemeForActiveForms();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        //Set Theme Color
        private void SetThemeForActiveForms()
        {
            try
            {
                foreach (Form frm in Application.OpenForms)
                {
                    mdiGlobalVar._Theme.SetTheme(frm, mdiGlobalVar.giTheme);
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        private void bUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                updateVersion.appUpdate(isURL);
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                wbNotPanel.Visible = false;
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        //////////////////////////////////////
        /// Web Browser Components
        /////////////////////////////////////
        private void WBNotificationPanel()
        {
            string lsTemplate, lsBodyTemplate, lsButtonsTemplate, lsButtonContainerTemplate, lsUpdateButtonsTemplate;
            string lsBody, lsButtons, lsButtonContainer, lsClearButtonTemplate, lsClearButton;
            try
            {

                lsTemplate = "<!DOCTYPE html>" + Environment.NewLine
                    + "<html lang=\"en\">" + Environment.NewLine
                    + "<head>" + Environment.NewLine
                    + "    <meta charset=\"UTF-8\">" + Environment.NewLine
                    + "    <meta http-equiv=\"X-UA-Compatible\" content=\"chrome=1,IE=edge\">" + Environment.NewLine
                    + "    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">" + Environment.NewLine
                    + "    <title>Document</title>" + Environment.NewLine
                    + "    <style>" + Environment.NewLine
                    + "        body {background-color: azure;display: flex;justify-content: center;align-items: center;flex-direction:column;font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;}" + Environment.NewLine
                    + "        .card {margin: 0.25rem;border-radius: 16px;height: auto;background-color: #FFFFFF;width: 100%;display: flex;justify-content: center;align-items: flex-start;flex-direction: column;overflow: hidden;position: relative;box-shadow: 0px 0px 12px 8px rgba(0,0,0,0.15);max-width:100%;-webkit-box-shadow: 0px 0px 12px 8px rgba(0,0,0,0.15);-moz-box-shadow: 0px 0px 12px 8px rgba(0,0,0,0.15);}" + Environment.NewLine
                    + "        .card-icon {max-width: 80px;max-height: 80px;overflow: hidden;border-radius: 50%;}" + Environment.NewLine
                    + "        .card-icon img {height: 100%;width: 100%;}" + Environment.NewLine
                    + "        .card-body {display: flex;justify-content: center;align-items: flex-start;flex-direction: column;}" + Environment.NewLine
                    + "        .userName{font-size: 1.3rem;font-weight: 700;}" + Environment.NewLine
                    + "        .msg-content {font-size: .975rem;font-weight: 400;margin: 0; max-width: 70% }" + Environment.NewLine
                    + "        .card-footer {display: flex;justify-content: center;align-items: flex-start;flex-direction: row;width: 100%;border-top: 1px solid #ccc ;}" + Environment.NewLine
                    + "        .card-footer button:first-child {border-right: 1px solid #ccc;}" + Environment.NewLine
                    + "        .card-footer button:last-child {border-left: 1px solid #ccc;}" + Environment.NewLine
                    + "        .card-footer button {background-color: transparent;outline: 0;border: 0;width: 100%;padding: .5rem;font-size: 1rem;font-weight: 500;letter-spacing: 1px;color: rgb(86, 86, 86);transition: .2s cubic-bezier(.2,.2,.38,.9);}" + Environment.NewLine
                    + "        .card-footer button:hover, .card-footer button:focus  {background-color: #e5e5e5;color: #4958ff;}" + Environment.NewLine
                    + "        .dnone{display:none;}"
                    + "        .main-content{padding: 1rem;display: flex;justify-content: center;align-items: flex-start;flex-direction: row;gap: 1rem;}" + Environment.NewLine
                    + "        .clear {position: absolute;right: 10px;float: right;top: 10px;border: 0;outline: 0;padding: 0;background-color: transparent;text-decoration: underline;color: #4958ff;}" + Environment.NewLine
                    + "    </style>" + Environment.NewLine
                    + "</head>" + Environment.NewLine
                    + "<body>@body</body>" + Environment.NewLine
                    + "</html>";

                lsBodyTemplate = "    <div id=\"@id\" class=\"card\" style=\"\">" + Environment.NewLine + "@clearButton" + Environment.NewLine
                    + "        <div class=\"main-content\">" + Environment.NewLine
                    + "            <div class=\"card-icon\">" + Environment.NewLine
                    + "                <img src=\"https://leapsurgebi.blob.core.windows.net/domain/favicon_io/leapsurgebi.com/android-chrome-512x512.png\" alt=\"\" >" + Environment.NewLine
                    + "            </div>" + Environment.NewLine
                    + "            <div class=\"card-body\">" + Environment.NewLine
                    + "                <div class=\"userName\">@title</div>" + Environment.NewLine
                    + "                <p class=\"msg-content\">" + Environment.NewLine
                    + "                    @message" + Environment.NewLine
                    + "                </p>" + Environment.NewLine
                    + "            </div>" + Environment.NewLine
                    + "        </div>" + Environment.NewLine
                    + "        @buttons"
                    + "    </div>";
                lsButtonContainerTemplate = "<div class=\"card-footer\">@buttons</div>";
                lsButtonsTemplate = "<button onclick=\"window.open('microsoft-edge:@buttonLink','_blank');\" id=\"@id\">@buttonName</button>";
                lsUpdateButtonsTemplate = "<button id=\"AppUpdate\" appUrl=\"@url\">@buttonName</button>";
                lsClearButtonTemplate = "		<button type=\"button\" class=\"clear\" id=\"@id\" name=\"clear\">Clear</button>" + Environment.NewLine;
                
                //App Updates
                lsBody = "";
                lNotifications.Text = "No Notifications";
                if (mdiGlobalVar.gsAppUpdateList.Contains("{"))
                {
                    JsonElement ljeUpdates = mdiGlobalVar.createJsonElement(mdiGlobalVar.gsAppUpdateList);
                    if(ljeUpdates.EnumerateArray().Count() > 0)
                    {
                        lNotifications.Text = "Notifications";
                        string lsMessage, lsTitle, lsUrl;
                        lsButtons = "";
                        lsButtonContainer = "";

                        lsTitle = "Version Updates";
                        lsMessage = "Latest Version of the Software is Avaliable.<br>Current Version : " + mdiGlobalVar.gsVersion 
                            + "<br>Latest Version : " + ljeUpdates[0].GetProperty("ver").ToString();
                        lsUrl = ljeUpdates[0].GetProperty("appUrl").ToString();

                        lsButtons = lsUpdateButtonsTemplate.Replace("@url", lsUrl).Replace("@buttonName", "Update");
                        lsButtonContainer = lsButtonContainerTemplate.Replace("@buttons", lsButtons);

                        lsBody += lsBodyTemplate.Replace("@id", "AppUpdate").Replace("@message", lsMessage)
                            .Replace("@buttons", lsButtonContainer).Replace("@title", lsTitle).Replace("@clearButton", "");
                    }
                }

                //Notifications
                if (mdiGlobalVar.gsNotificationList.Contains("{"))
                {
                    JsonElement ljeNotification = mdiGlobalVar.createJsonElement(mdiGlobalVar.gsNotificationList);
                    for (int i = 0; i < ljeNotification.EnumerateArray().Count(); i++)
                    {
                        lNotifications.Text = "Notifications";
                        string lsTriggerId, lsParam, lsMessage, lsTitle = "";
                        int liTriggerType, liActionTaken, liStatus;
                        lsButtons = "";
                        lsButtonContainer = "";
                        lsClearButton = "";

                        liTriggerType = int.Parse(ljeNotification[i].GetProperty("triggerType").ToString());
                        liActionTaken = int.Parse(ljeNotification[i].GetProperty("actionTaken").ToString());
                        liStatus = int.Parse(ljeNotification[i].GetProperty("notStatus").ToString());

                        lsTriggerId = ljeNotification[i].GetProperty("triggerId").ToString();
                        lsParam = ljeNotification[i].GetProperty("param").ToString();
                        lsMessage = ljeNotification[i].GetProperty("cMessage").ToString();
                        lsTitle = (lsParam == "" || lsParam == null ? "Leapsurge" : lsParam);

                        switch (liTriggerType)
                        {
                            case 0:         //High Priority Push Notification
                            case 3:         //Low Priority Push Notification
                                break;
                            case 1:         //Trigger Report to sync
                                lsTitle = lsParam.Replace("-", "");
                                lsMessage = (liStatus == 0 ? "No Action Taken" : (liStatus == 1 ? "Running" :
                                    (liStatus == 2 ? "Success" : (liStatus == 3 ? "Partially Failed - " + lsMessage :
                                    (liStatus == 4 ? "Failed - " + lsMessage : "")))));
                                break;
                            case 2:         //Exe Update
                                break;
                            case 4:         //High Priority Notification With Buttons
                            case 5:         //Low Priority Notification With Buttons
                                string lsButton1, lsValue1, lsButton2, lsValue2, lsButton3, lsValue3;
                                lsButton1 = ljeNotification[i].GetProperty("buttonName1").ToString();
                                lsValue1 = ljeNotification[i].GetProperty("buttonValue1").ToString();
                                lsButton2 = ljeNotification[i].GetProperty("buttonName2").ToString();
                                lsValue2 = ljeNotification[i].GetProperty("buttonValue2").ToString();
                                lsButton3 = ljeNotification[i].GetProperty("buttonName3").ToString();
                                lsValue3 = ljeNotification[i].GetProperty("buttonValue3").ToString();

                                lsButtons = (lsButton1 != "" && lsButton1 != null && lsValue1 != "" && lsValue1 != null ?
                                    lsButtonsTemplate.Replace("@buttonLink", lsValue1).Replace("@buttonName", lsButton1) : "");
                                lsButtons += (lsButton2 != "" && lsButton2 != null && lsValue2 != "" && lsValue2 != null ?
                                    lsButtonsTemplate.Replace("@buttonLink", lsValue2).Replace("@buttonName", lsButton2) : "");
                                lsButtons += (lsButton3 != "" && lsButton3 != null && lsValue3 != "" && lsValue3 != null ?
                                    lsButtonsTemplate.Replace("@buttonLink", lsValue3).Replace("@buttonName", lsButton3) : "");
                                lsButtonContainer = lsButtonContainerTemplate.Replace("@buttons", lsButtons)
                                    .Replace("@id", lsTriggerId).Replace("@name", lsTriggerId);
                                break;
                        }
                        lsClearButton = lsClearButtonTemplate.Replace("@id", lsTriggerId);
                        lsBody += lsBodyTemplate.Replace("@id", lsTriggerId).Replace("@message", lsMessage)
                            .Replace("@buttons", lsButtonContainer).Replace("@title", lsTitle).Replace("@clearButton", lsClearButton);
                    }

                }
                lsTemplate = lsTemplate.Replace("@body", lsBody);
                mdiGlobalVar.TemplateFile("Notifications.html", lsTemplate, 2);
                wbNotification.Url = new Uri(String.Format("file:///{0}Notifications.html", mdiGlobalVar.gsTemplatePath));
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        private void browser_DocumentCompleted(Object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                this.wbNotification.Document.Body.MouseDown += new HtmlElementEventHandler(Body_MouseDown);
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }
        void Body_MouseDown(Object sender, HtmlElementEventArgs e)
        {
            try
            {
                string lsId, lsName, lsUrl, lsQuery;
                switch (e.MouseButtonsPressed)
                {
                    case MouseButtons.Left:
                        HtmlElement element = this.wbNotification.Document.GetElementFromPoint(e.ClientMousePosition);
                        if (element != null)
                        {
                            lsId = element.Id;
                            lsName = element.Name;
                            switch (lsId)
                            {
                                case "AppUpdate":
                                    lsUrl = element.GetAttribute("appUrl");
                                    updateVersion.appUpdate(lsUrl);
                                    break;
                                default:
                                    if (lsName == "clear")
                                    {
                                        lsQuery = "Update LS_Trigger " + Environment.NewLine
                                            + " Set [Action] = 3 " + Environment.NewLine
                                            + $" Where [application_code] = '{mdiGlobalVar.gsAppCode}' And " + Environment.NewLine
                                            + $"    [LSCode] = '{mdiGlobalVar.custCode}' And [firm_code] = '{mdiGlobalVar.firmCode}' And [Id] = '{lsId}'";
                                        mdiGlobalVar._MasterConfig.ExecuteRawQuery(mdiGlobalVar, lsQuery, 1, out _, out _, 0, 2);
                                        element.Parent.Style = "display:none";

                                    }
                                    break;
                            }
                        }
                        break;
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        private void bClearNot_Click(object sender, EventArgs e)
        {
            try
            {
                string lsQuery = "Update LS_Trigger " + Environment.NewLine
                    + " Set [Action] = 3 " + Environment.NewLine
                    + $" Where [application_code] = '{mdiGlobalVar.gsAppCode}' And " + Environment.NewLine
                    + $"    [LSCode] = '{mdiGlobalVar.custCode}' And [firm_code] = '{mdiGlobalVar.firmCode}'";
                mdiGlobalVar._MasterConfig.ExecuteRawQuery(mdiGlobalVar, lsQuery, 1, out _, out _, 0, 2);
                mdiGlobalVar.gsNotificationList = "";
                wbNotPanel.Visible = false;
                WBNotificationPanel();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
            
        }

        private void ConstructLoader()
        {
            try
            {
                string lsLoaderTemplate = "<!DOCTYPE html>"
                    + "<html lang=\"en\">"
                    + "<head>"
                    + "    <meta charset=\"UTF-8\">"
                    + "    <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">"
                    + "    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">"
                    + "    <title>Document</title>"
                    + "    <style>   "
                    + "		#preloader {  position: fixed;  top: 0;  left: 0;  right: 0;  bottom: 0;  z-index: 9999;  overflow: hidden;  background: white;}"
                    + "		#preloader:before {  content: \"\";  position: fixed;  top: calc(50% - 30px);  left: calc(50% - 30px);  border: 6px solid pink;  border-top-color: blue;  border-radius: 50%;  width: 60px;  height: 60px;  -webkit-animation: animate-preloader 1s linear infinite;  animation: animate-preloader 1s linear infinite;}"
                    + "		@-webkit-keyframes animate-preloader {  0% {    transform: rotate(0deg);  }  100% {    transform: rotate(360deg);  }}"
                    + "		@keyframes animate-preloader {  0% {    transform: rotate(0deg);  }  100% {    transform: rotate(360deg);  }}"
                    + "    </style>"
                    + "</head>"
                    + "<body>"
                    + "    <div id=\"preloader\"></div>"
                    + "</body>"
                    + "</html>";
                mdiGlobalVar.TemplateFile("Loader.html", lsLoaderTemplate, 2);
            }
            catch
            {

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string lsFirm = mdiGlobalVar.firmCode;

                FirmList _FirmList = new FirmList(mdiGlobalVar);
                _FirmList.ShowDialog();

                if (lsFirm != mdiGlobalVar.firmCode)
                {
                    //Statistics
                    mdiGlobalVar._fun.UpdateDBStats(mdiGlobalVar, out _, out _);

                    //Forms Closing
                    _DashboardNew.Close();
                    if (dataSync.Enabled == false) _Sync.Close();
                    if (about.Enabled == false) _About.Close();
                    if (mismatchutility.Enabled == false) _MismatchUtility.Close();

                    //Reload Main Form
                    MainForm_PostLoad(sender, e);
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        ///////////////////////////////////////////
        // Background process in this Form
        ///////////////////////////////////////////
        /*private void bgNotifications_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!bgNotifications.CancellationPending)
            {
                Thread.Sleep(10000);

                //AppUpdate 
                mdiGlobalVar._MasterConfig.GetAppLatstVersion(mdiGlobalVar,out _, out _);

                //Triggers
                mdiGlobalVar._fun.Triggers(mdiGlobalVar);

                WBNotificationPanel();
                if (bgNotifications.IsBusy != true) bgNotifications.RunWorkerAsync();
            }
        }*/

        private void mismatchutility_Click(object sender, EventArgs e)
        {
            try
            {
                mismatchutility.Enabled = false;
                _MismatchUtility = new MismatchUtility(this)
                {
                    MdiParent = this,
                    FormBorderStyle = FormBorderStyle.None,
                    Dock = DockStyle.Fill
                };
                _MismatchUtility.Show();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void pUserIcon_Click_1(object sender, EventArgs e)
        {

        }

        private void timer_Click(object sender, EventArgs e)
        {
            try
            {
                timer.Enabled = false;
                _SyncScheduler = new SyncScheduler(this)
                {
                    MdiParent = this,
                    FormBorderStyle = FormBorderStyle.None,
                    Dock = DockStyle.Fill
                };
                _SyncScheduler.Show();
            }
            catch(Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }
    }

    public class MySR : ToolStripSystemRenderer
    {
        public MySR() { }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
        }
    }
}