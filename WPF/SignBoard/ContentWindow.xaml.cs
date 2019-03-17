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
using Common;
using Common.Utiles;
using Common.Controls;

namespace SignBoard
{
    /// <summary>
    /// Interaction logic for ContentWindow.xaml
    /// </summary>
    public partial class ContentWindow : Window
    {
        MainWindow SignatureWindow;
        WebViewWindow adWindow = new WebViewWindow();
        PDFViewer pdfViewer;
        string currentPDF;

        public int CurrentPageNumber { get { return pdfViewer.CurrentPageNumber; } }
        public int PDFPageCount { get { return pdfViewer.PageCount; } }

        public ContentWindow()
        {
            InitializeComponent();
            InitUI();
            //WinHookerHelper.EnableSpecialKeyboardHook();
        }

        #region UI Events

        private void Window_Closed(object sender, EventArgs e)
        {
            //WinHookerHelper.ReleaseSpecialKeyboardHook();
            CleanTempFiles();
            Application.Current.Shutdown();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ShowSignWindow();
            ShowAD();
        }

        #endregion


        #region Private Functions

        public void LoadPDF(string filename)
        {
            currentPDF = filename;
            if (pdfViewer != null)
            {
                pdfViewer.LoadPDF(filename);

            }
                
        }

        public void ClosePDF()
        {
            if (pdfViewer != null)
            {
                pdfViewer.ClosePDF();
                pdfViewer.CleanTempFiles();
            }
            CleanTempFile(currentPDF);
        }

        public void NextPage()
        {
            if (pdfViewer.PageCount > 1 && pdfViewer.CurrentPageNumber < pdfViewer.PageCount)
            {
                pdfViewer.GotoPage(++pdfViewer.CurrentPageNumber);
            }
        }

        public void PrePage()
        {
            if (pdfViewer.PageCount > 1 && pdfViewer.CurrentPageNumber > 1)
            {
                pdfViewer.GotoPage(--pdfViewer.CurrentPageNumber);
            }
        }


        private void CleanTempFiles()
        {
            if (!Directory.Exists(Constants.TempFileFolder))
            {
                return;
            }
            //Clean local temp files
            foreach (string d in Directory.GetFileSystemEntries(Constants.TempFileFolder))
            {
                if (File.Exists(d))
                {
                    try
                    {
                        File.Delete(d);
                    }
                    catch { }
                }
            }
        }

        private void CleanTempFile(string filename)
        {
            if (File.Exists(filename))
            {
                try
                {
                    File.Delete(filename);
                }
                catch { }
            }
        }

        private void InitUI()
        {
            Size contentSize = UtilsHelper.GetPDFDisplayAreaSize();
            WindowsFormsHost1.SetValue(Canvas.WidthProperty, contentSize.Width);
            WindowsFormsHost1.SetValue(Canvas.HeightProperty, contentSize.Height);

            pdfViewer = new PDFViewer();
            pdfViewer.Rotate = 3;
            WindowsFormsHost1.Child = pdfViewer;
        }

        private void ShowSignWindow()
        {
            SignatureWindow = new MainWindow();
            SignatureWindow.ContentWindow = this;
            SignatureWindow.Owner = this;
            SignatureWindow.Show();
        }

        public void ShowAD()
        {
            ClosePDF();
            if (adWindow != null && adWindow.IsActive)
                adWindow.Close();

            adWindow = new WebViewWindow();
            adWindow.ShowAD();
            adWindow.Owner = this;
            adWindow.Show();
        }

        public void ShowThanks()
        {
            ClosePDF();
            if (adWindow != null && adWindow.IsActive)
                adWindow.Close();

            adWindow = new WebViewWindow();
            adWindow.ShowThanks();
            adWindow.Owner = this;
            adWindow.Show();
        }

        public void CloseADWindow()
        {
            adWindow.Close();
            //adWindow = null;
        }

        #endregion

        

    }
}
