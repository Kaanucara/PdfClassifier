using System;
using System.Collections.Generic;
using Transaction = Classifier3.TransactionG;


namespace Classifier3
{
    public interface IBankStrategy
    {

        // Bankanın adını döndüren özellik
        string BankName { get; }

        // PDF metnini sınıflandırmak için kullanılan metot
        public List<Transaction> ClassifyPdfContent(string filePath);

        // Sayfa metinlerini ayıklamak için isteğe bağlı bir metot
        public List<string> ExtractPageTextsFromPdf(string filePath);

        // Her sayfanın içeriğini sınıflandıran fonksiyon
        public List<Transaction> ClassifyPdfContentByPage(List<string> pageList);
    }
}

