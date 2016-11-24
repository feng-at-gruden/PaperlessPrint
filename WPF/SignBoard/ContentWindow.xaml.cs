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

namespace SignBoard
{
    /// <summary>
    /// Interaction logic for ContentWindow.xaml
    /// </summary>
    public partial class ContentWindow : Window
    {
        MainWindow SignatureWindow;
        PDFViewer pdfViewer;


        public ContentWindow()
        {
            InitializeComponent();
            InitUI();
            ShowSignWindow();
            //LoadPDF("C:\\Tools\\1.pdf");
        }

        #region UI Events

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            
        }

        #endregion


        #region Private Functions

        public void LoadPDF(string filename)
        {
            if (pdfViewer != null)
                pdfViewer.LoadPDF(filename);
        }

        public void ClosePDF()
        {
            if (pdfViewer != null)
                pdfViewer.ClosePDF();
        }

        private void InitUI()
        {
            Size contentSize = UtilsHelper.GetPDFDisplayAreaSize();
            WindowsFormsHost1.SetValue(Canvas.WidthProperty, contentSize.Width);
            WindowsFormsHost1.SetValue(Canvas.HeightProperty, contentSize.Height);

            pdfViewer = new PDFViewer();
            WindowsFormsHost1.Child = pdfViewer;

            //PDFReader pdfReader = new PDFReader();
            //WindowsFormsHost1.Child = pdfReader;
        }

        private void ShowSignWindow()
        {
            SignatureWindow = new MainWindow();
            SignatureWindow.ContentWindow = this;
            SignatureWindow.Show();
        }


        #endregion

    }
}
