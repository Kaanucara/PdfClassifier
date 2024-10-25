using System;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System.Collections.Generic;
using System.Text;

namespace Classifier3
{
    public static class PdfHelper
    {
        // PDF sayfa metinlerini döndüren fonksiyon
        public static List<string> ExtractPageTextsFromPdf(string url)
        {
            //var stringBuilder = new StringBuilder();
            List<string> pageList = new List<string>();
            using var pdfReader = new PdfReader(url);
            using var pdfDoc = new PdfDocument(pdfReader);
          // var strategy = new SimpleTextExtractionStrategy();
            for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
            {
                var pageText = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i));
                //stringBuilder.Append(pageText); // Sayfa metinlerini birleştir
                pageList.Add(pageText);
            }
            
            //return stringBuilder.ToString(); // Tek bir string olarak döner
            return pageList; // Tek bir string olarak döner
        }

    }
}
