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

namespace SignBoard
{
    /// <summary>
    /// Interaction logic for WebViewWindow.xaml
    /// </summary>
    public partial class WebViewWindow : Window
    {
        private DispatcherTimer dTimer = new DispatcherTimer();


        public WebViewWindow()
        {
            InitializeComponent();

            dTimer.Tick += new EventHandler(dTimer_Tick);
            dTimer.Interval = new TimeSpan(0, 0, 10);

            ShowAD();
        }



        #region Private Functions


        public void ShowAD()
        {
            string url = String.Format("file:///{0}\\Content\\Html\\Welcome.html", Directory.GetCurrentDirectory());
            WebBrowser1.Source = new Uri( url );
        }

        public void ShowThanks()
        {
            string url = String.Format("file:///{0}\\Content\\Html\\ThankYou.html", Directory.GetCurrentDirectory());
            WebBrowser1.Source = new Uri(url);

            //启动 DispatcherTimer对象dTime。
            dTimer.Start(); 
        }

        private void dTimer_Tick(object sender, EventArgs e)
        {
            dTimer.Stop();
            ShowAD();
        }

        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WebBrowser1.Navigate("about:blank");
        }


    }
}
