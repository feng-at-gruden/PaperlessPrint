using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace Common.Controls
{
    public partial class PDFViewer : UserControl
    {
        public int PageCount;
        public int CurrentPageNumber;
        private string PDFName;
        public short Rotate = 0;

        public PDFViewer()
        {
            InitializeComponent();

            foxitReader1.ShowToolBar(false);
            foxitReader1.ShowStatusBar(false);
            foxitReader1.ShowTitleBar(false);
            foxitReader1.ShowNavigationPanels(false);
            foxitReader1.ShowBookmark(false);
            foxitReader1.SetLayoutShowMode(FoxitReaderSDKProLib.BrowseMode.MODE_SINGLE, 1);

            //foxitReader1.UnLockActiveX("license_id","unlock_code");
        }



        public void LoadPDF(string filename)
        {
            PDFName = filename;
            PageCount = GetPdfPageCount(filename);
            if (PageCount > 1)
            {
                for (int i = 1; i <= PageCount; i++)
                {
                    string pn = filename.Replace(".pdf", "_" + i + ".pdf");
                    ExtractPage(filename, pn, i);
                }
                CurrentPageNumber = 1;
                GotoPage(1);
            }
            else
            {
                foxitReader1.OpenFile(filename, null);
                foxitReader1.Rotate = Rotate;
                foxitReader1.ShowNavigationPanels(false);
            }
        }

        public void ClosePDF()
        {
            foxitReader1.CloseFile();
            foxitReader1.Invalidate();
        }

        public void GotoPage(int p)
        {
            ClosePDF();
            CurrentPageNumber = p;
            string pn = PDFName.Replace(".pdf", "_" + p + ".pdf");
            foxitReader1.OpenFile(pn, null);
            foxitReader1.Rotate = Rotate;
            foxitReader1.ShowNavigationPanels(false);
        }

        public void SetZoomLevel(int level)
        {
            foxitReader1.ZoomLevel = level;
        }



        #region Private Funtions

        private int GetPdfPageCount(string filepath)
        {
            int pageCount = 0;
            PdfReader reader = new PdfReader(filepath);
            pageCount = reader.NumberOfPages;
            reader.Close();
            return pageCount;
        }


        ///http://johnatten.com/2013/03/09/splitting-and-merging-pdf-files-in-c-using-itextsharp/#iTextExampleCodeExtractSinglePage
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourcePdfPath"></param>
        /// <param name="outputPdfPath"></param>
        /// <param name="pageNumber"></param>
        /// <param name="password"></param>
        private void ExtractPage(string sourcePdfPath, string outputPdfPath, int pageNumber, string password = "")
        {
            PdfReader reader = null;
            Document document = null;
            PdfCopy pdfCopyProvider = null;
            PdfImportedPage importedPage = null;
            try
            {
                // Intialize a new PdfReader instance with the contents of the source Pdf file:
                reader = new PdfReader(sourcePdfPath);

                // Capture the correct size and orientation for the page:
                document = new Document(reader.GetPageSizeWithRotation(pageNumber));

                // Initialize an instance of the PdfCopyClass with the source 
                // document and an output file stream:
                pdfCopyProvider = new PdfCopy(document, new System.IO.FileStream(outputPdfPath, System.IO.FileMode.Create));
                document.Open();

                // Extract the desired page number:
                importedPage = pdfCopyProvider.GetImportedPage(reader, pageNumber);
                pdfCopyProvider.AddPage(importedPage);
                document.Close();
                reader.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CleanTempFiles()
        {
            for (int i = 1; i <= PageCount; i++)
            {
                string pn = PDFName.Replace(".pdf", "_" + i + ".pdf");
                if (File.Exists(pn))
                {
                    try
                    {
                        File.Delete(pn);
                    }
                    catch { }
                }
            }
        }

        #endregion


    }
}
