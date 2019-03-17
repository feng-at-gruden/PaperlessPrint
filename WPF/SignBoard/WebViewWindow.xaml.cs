using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Threading;
using System.Diagnostics;
using Microsoft.Win32;

namespace SignBoard
{
    /// <summary>
    /// Interaction logic for WebViewWindow.xaml
    /// </summary>
    public partial class WebViewWindow : Window
    {
        private DispatcherTimer dTimer = new DispatcherTimer();
        private System.Windows.Forms.WebBrowser mWebBrowser;

        public WebViewWindow()
        {
            InitializeComponent();
            SetBrowserCompatibilityMode();

            System.Windows.Forms.Integration.WindowsFormsHost host =
                new System.Windows.Forms.Integration.WindowsFormsHost();

            mWebBrowser = new System.Windows.Forms.WebBrowser();

            host.Child = mWebBrowser;

            this.grid.Children.Add(host);  

            dTimer.Tick += new EventHandler(dTimer_Tick);
            dTimer.Interval = new TimeSpan(0, 0, 10);

            ShowAD();
        }



        #region Private Functions


        public void ShowAD()
        {
            string url = String.Format("file:///{0}\\Content\\Html\\Welcome.html", Directory.GetCurrentDirectory());
            mWebBrowser.Url = new Uri(url);
        }

        public void ShowThanks()
        {
            string url = String.Format("file:///{0}\\Content\\Html\\ThankYou.html", Directory.GetCurrentDirectory());
            mWebBrowser.Url = new Uri(url);

            //启动 DispatcherTimer对象dTime。
            dTimer.Start(); 
        }

        private void dTimer_Tick(object sender, EventArgs e)
        {
            dTimer.Stop();
            ShowAD();
        }


        private void SetBrowserCompatibilityMode()
        {
            // http://msdn.microsoft.com/en-us/library/ee330720(v=vs.85).aspx

            // FeatureControl settings are per-process
            var fileName = System.IO.Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);

            if (String.Compare(fileName, "devenv.exe", true) == 0) // make sure we're not running inside Visual Studio
                return;

            using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION",
                RegistryKeyPermissionCheck.ReadWriteSubTree))
            {
                // Webpages containing standards-based !DOCTYPE directives are displayed in IE10 Standards mode.
                UInt32 mode = 10000; // 10000; 
                key.SetValue(fileName, mode, RegistryValueKind.DWord);
            }

            using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BLOCK_LMZ_SCRIPT",
                RegistryKeyPermissionCheck.ReadWriteSubTree))
            {
                // enable <scripts> in local machine zone
                UInt32 mode = 0;
                key.SetValue(fileName, mode, RegistryValueKind.DWord);
            }

            using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_NINPUT_LEGACYMODE",
                RegistryKeyPermissionCheck.ReadWriteSubTree))
            {
                // disable Legacy Input Model
                UInt32 mode = 0;
                key.SetValue(fileName, mode, RegistryValueKind.DWord);
            }
        }

        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //WebBrowser1.Navigate("about:blank");
        }


    }
}
