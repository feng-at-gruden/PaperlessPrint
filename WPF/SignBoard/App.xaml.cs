using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SignBoard
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        System.Threading.Mutex mutex; 
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            bool ret;
            mutex = new System.Threading.Mutex(true, "Paperless Print SignBoard", out ret);

            if (!ret)
            {
                MessageBox.Show("程序已经在运行中,请关闭重试！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }
        }
    }
}
