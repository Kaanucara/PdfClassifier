using System;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections.Generic;
using Transaction = Classifier3.TransactionG;

namespace Classifier3
{
    public class VakifBankStrategy : IBankStrategy
    {
        public string BankName => "Vakıfbank";

        public List<Transaction> ClassifyPdfContent(string filePath)
        {
            // PDF sayfa metinlerini ayıklıyoruz
            var pageTexts = ExtractPageTextsFromPdf(filePath);

            // Sayfa metinlerini sınıflandırıyoruz
            return ClassifyPdfContentByPage(pageTexts);
        }

        // PdfHelper'daki ortak fonksiyonu kullanarak PDF'den metin çıkarma
        public List<string> ExtractPageTextsFromPdf(string pdfFilePath)
        {
            return PdfHelper.ExtractPageTextsFromPdf(pdfFilePath);
        }

        public List<Transaction> ClassifyPdfContentByPage(List<string> pageList)
        {
            List<Transaction> transactions = new List<Transaction>();
            foreach (var pageText in pageList)
        {
           string pattern = @"(?<Date>\d{2}\.\d{2}\.\d{4})\s+(?<Time>\d{2}:\d{2})\s+(?<TransactionNo>\d+)\s+(?<Amount>-?\d{1,3}(?:\.\d{3})*,\d{2})\s+(?<Balance>-?\d{1,3}(?:\.\d{3})*,\d{2})\s+(?<TransactionName>[^\n]+)\s*\n(?<Description>[\s\S]+?(?=\d{2}\.\d{2}\.\d{4}|\Z))";
           Regex regex = new Regex(pattern);
           MatchCollection matches = regex.Matches(pageText);

    
            foreach (Match match in matches)
             {
                try
                {
                    // Tarih ve saat bilgilerini işleme
                    var date = DateTime.ParseExact(match.Groups["Date"].Value + " " + match.Groups["Time"].Value, "dd.MM.yyyy HH:mm", null);
                    string transactionNo = match.Groups["TransactionNo"].Value;

                    // Miktar ve bakiye bilgilerini işleme
                    string amountString = match.Groups["Amount"].Value.Replace(".", "").Replace(",", ".");
                    string balanceString = match.Groups["Balance"].Value.Replace(".", "").Replace(",", ".");

                    decimal amount = Decimal.Parse(amountString, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
                    decimal balance = Decimal.Parse(balanceString, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);

                    // İşlem adı ve açıklama
                    string transactionName = match.Groups["TransactionName"].Value.Trim();
                    string description = match.Groups["Description"].Value.Trim();

                    // Yeni işlem kaydını listeye ekle
                    transactions.Add(new Transaction
                    {
                        Date = date,
                        Reference = transactionNo,
                        Amount = amount,
                        Balance = balance,
                        Description = description,
                        TransactionName = transactionName
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Hata: {ex.Message}");
                }
                
              }
            }
            return transactions;
        }
    }
}
