using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace LSSyncApp.Template
{
    public class ImageConvertion
    {
        public void ConvertFile()
        {
            try
            {
                string text = "";//Properties.Resources.Happy_birthday_template.ToString();
                
                StartBrowser(text);
                return;
            }
            catch
            {
                return;
            }
        }

        private static void StartBrowser(string source)
        {
            //string lsFile = source;
            //File.AppendAllText("HBD.html", source.Replace("@Name","Gokul Kannan"));

            var th = new Thread(() =>
            {
                var webBrowser = new WebBrowser();
                webBrowser.ScrollBarsEnabled = false;
                webBrowser.IsWebBrowserContextMenuEnabled = true;
                webBrowser.AllowNavigation = true;

                webBrowser.Height = 0;// Properties.Resources.Happy_birthday.Height;
                webBrowser.Width = 0;//Properties.Resources.Happy_birthday.Width;

                webBrowser.DocumentCompleted += webBrowser_DocumentCompleted;
                webBrowser.DocumentText = source.Replace("@Name","Ajith Ji");
                
                Application.Run();
            });
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
            return;
        }

        static void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var webBrowser = (WebBrowser)sender;

            int scrollWidth = 0;//Properties.Resources.Happy_birthday.Width;
            int scrollHeight = 0;//Properties.Resources.Happy_birthday.Height;
            //webBrowser.Height = scrollHeight;
            //webBrowser.Width = scrollWidth;
            using (Bitmap bitmap =
                new Bitmap(
                    scrollWidth,
                    scrollHeight))
            {
                webBrowser
                    .DrawToBitmap(
                    bitmap,
                    new System.Drawing
                        .Rectangle(0, 0, bitmap.Width, bitmap.Height));
                bitmap.Save(@"filename.jpg",
                    System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            return;

        }
    }
}
