using System;
using System.Linq;
using System.Windows.Forms;

namespace Updates
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (Environment.OSVersion.Version.Major == 6)
                SetProcessDPIAware();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Count() <= 0) 
                Application.Exit();
            else
                Application.Run(new Updates(args));
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
    }
}
