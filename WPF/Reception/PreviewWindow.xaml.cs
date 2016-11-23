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
using Common;

namespace Reception
{
    /// <summary>
    /// Interaction logic for PreviewWindow.xaml
    /// </summary>
    public partial class PreviewWindow : Window
    {

        #region Field
        private string[] args = null;
        private String currentFileName;
        PDFReader pdfReader;

        MainWindow SignatureWindow;

        #endregion

        public PreviewWindow()
        {
            InitializeComponent();
            InitUI();

            this.args = (Application.Current as App).args;

            if (this.args != null && this.args.Length > 0)
            {
                currentFileName = this.args[0];
                pdfReader.LoadPDF(currentFileName);
            }
        }




        #region Private Functions

        private void InitUI()
        {
            Size contentSize = GetA4DisplayAreaSize();
            WindowsFormsHost1.SetValue(Canvas.WidthProperty, contentSize.Width);
            WindowsFormsHost1.SetValue(Canvas.HeightProperty, contentSize.Height);

            pdfReader = new PDFReader();
            WindowsFormsHost1.Child = pdfReader;

            //设置窗体按比例尺寸
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double h = screenHeight - SystemParameters.CaptionHeight - SystemParameters.MenuBarHeight;
            double w = Math.Floor(Constants.A4Width * h/ Constants.A4Height);
            //double w = Math.Floor(billImageW * h / billImageH);

            this.SetValue(Window.WidthProperty, w);
            this.SetValue(Window.HeightProperty, h);
            //this.SetValue(Window.TopProperty, 0d);
            this.SetValue(Window.LeftProperty, -50d);

            //PDFReader pdfReader = new PDFReader();
            //WindowsFormsHost1.Child = pdfReader;
        }



        private Size GetA4DisplayAreaSize()
        {

            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;

            double w = Constants.A4Height * screenHeight / Constants.A4Width;

            return new Size(w, screenHeight);

        }
        #endregion 

        private void Window_Activated(object sender, EventArgs e)
        {
            if (SignatureWindow == null)
            {
                SignatureWindow = new MainWindow();
                //SignatureWindow.ContentWindow = this;
                SignatureWindow.Show();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

    }
}
