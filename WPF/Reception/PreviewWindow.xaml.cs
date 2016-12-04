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
using Common.Utiles;
using Common.Controls;

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
        PDFViewer pdfReader;

        MainWindow SignatureWindow;

        public int CurrentPageNumber { get { return pdfReader.CurrentPageNumber; } }
        public int PDFPageCount { get { return pdfReader.PageCount; } }

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
                pdfReader.SetZoomLevel(1);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (SignatureWindow == null)
            {
                SignatureWindow = new MainWindow();
                SignatureWindow.ContentWindow = this;
                SignatureWindow.Owner = this;
                SignatureWindow.Show();
            }
            //WinHookerHelper.EnableSpecialKeyboardHook();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //WinHookerHelper.ReleaseSpecialKeyboardHook();
            pdfReader.ClosePDF();
            pdfReader.CleanTempFiles();
            Application.Current.Shutdown();
        }
        

        #region Private Functions

        private void InitUI()
        {
            //设置窗体按比例尺寸
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double h = screenHeight - SystemParameters.CaptionHeight - SystemParameters.MenuBarHeight;
            double w = Math.Floor(Constants.A4Width * h / Constants.A4Height);

            this.SetValue(Window.WidthProperty, w);
            this.SetValue(Window.HeightProperty, h);
            this.SetValue(Window.TopProperty, 0d);
            this.SetValue(Window.LeftProperty, 0d);

            WindowsFormsHost1.SetValue(Canvas.WidthProperty, w);
            WindowsFormsHost1.SetValue(Canvas.HeightProperty, h);

            pdfReader = new PDFViewer();
            WindowsFormsHost1.Child = pdfReader;
        }



        private Size GetA4DisplayAreaSize()
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;

            double w = Constants.A4Height * screenHeight / Constants.A4Width;
            return new Size(w, screenHeight);

        }

        #endregion 

        
        #region Public Functions

        public void NextPage()
        {
            if (pdfReader.PageCount >1 && pdfReader.CurrentPageNumber < pdfReader.PageCount)
            {
                pdfReader.GotoPage(++pdfReader.CurrentPageNumber);
            }
        }

        public void PrePage()
        {
            if (pdfReader.PageCount > 1 && pdfReader.CurrentPageNumber > 1)
            {
                pdfReader.GotoPage(--pdfReader.CurrentPageNumber);
            }
        }

        public void ClosePDF()
        {
            pdfReader.ClosePDF();
        }

        #endregion


    }
}
