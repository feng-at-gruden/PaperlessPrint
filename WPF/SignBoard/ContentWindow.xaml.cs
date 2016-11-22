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
        public ContentWindow()
        {
            InitializeComponent();

            PDFViewer pdfViewer1 = new PDFViewer();
            WindowsFormsHost1.Child = pdfViewer1;

            pdfViewer1.LoadPDF("D:\test.pdf");

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            ShowSignWindow();
        }



        #region Private Functions


        private void ShowSignWindow()
        {
            SignatureWindow = new MainWindow();
            SignatureWindow.Show();
        }


        #endregion

    }
}
