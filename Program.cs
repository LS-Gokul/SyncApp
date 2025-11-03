using System;
using Microsoft.Identity.Client;
using System.Windows.Forms;

namespace LSSyncApp
{
    public partial class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (Environment.OSVersion.Version.Major == 6)
                SetProcessDPIAware();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //args = new string[1] { "-CheckStatus~0001" };
            Application.Run(new Login(args));
        }
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
    }
}
