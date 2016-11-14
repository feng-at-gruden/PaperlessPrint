using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PaperlessPrint
{
    static class Program
    {
        

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            bool createNew;
            using (System.Threading.Mutex mutex = new System.Threading.Mutex(true, Application.ProductName, out createNew))
            {
                if (createNew)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    if (args.Length == 0)
                        Application.Run(new MainForm());
                    else
                        Application.Run(new MainForm(args));
                }
                else
                {
                    MessageBox.Show("程序已经在运行中,请关闭重试！");
                    System.Threading.Thread.Sleep(500);
                    System.Environment.Exit(1);
                }
            }
        }
    }
}
