using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tablet
{
    /// <summary>
    /// https://msdn.microsoft.com/en-us/windows/uwp/input-and-devices/pen-and-stylus-interactions
    /// http://www.codeproject.com/Articles/17895/Handling-Touch-Pen-or-Mouse-Digitizer-input-in-you
    /// http://stackoverflow.com/questions/40330046/capture-stylus-writings-from-c-sharp-panel
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool createNew;
            using (System.Threading.Mutex mutex = new System.Threading.Mutex(true, Application.ProductName, out createNew))
            {
                if (createNew)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new MainForm());
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
