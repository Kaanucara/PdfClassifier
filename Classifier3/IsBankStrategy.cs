using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Classifier3;
using System.Transactions;
using Transaction = Classifier3.TransactionG;




namespace Classifier
{
    public class IsBankStrategy : IBankStrategy
    {
        public string BankName => "Türkiye İş Bankası";

        public List<Transaction> ClassifyPdfContent(string filePath)
        {
            // PDF sayfa metinlerini ayıklıyoruz
            var pageTexts = ExtractPageTextsFromPdf(filePath);

            // Sayfa metinlerini sınıflandırıyoruz
            return ClassifyPdfContentByPage(pageTexts);
        }

        public List<string> ExtractPageTextsFromPdf(string pdfFilePath)
        {
            // PDF sayfa metinlerini alıyoruz
            return PdfHelper.ExtractPageTextsFromPdf(pdfFilePath);
           
        }

        // Her sayfanın içeriğini sınıflandıran fonksiyon
        public List<Transaction> ClassifyPdfContentByPage(List<string> pageList)
        {
            List<Transaction> transactions = new List<Transaction>();

            foreach (var pageText in pageList)
            {
                // Regex ile işlemleri ayıklamak için mevcut kodunuzu kullanabilirsiniz
                string pattern = @"(?<Date>\d{2}/\d{2}/\d{4}-\d{2}:\d{2}:\d{2})\s+(?<Branch>[A-Za-zÇĞİÖŞÜçğöşü\s]+)\s+(?<Amount>-?\d{1,3}(?:\.\d{3})*,\d{2})\s+(?<Balance>-?\d{1,3}(?:\.\d{3})*,\d{2})\s+(?<AdditionalBalance>-?\d{1,3}(?:\.\d{3})*,\d{2})\s+(?<Transaction>SF|FA)\s+(?<TransactionType>POS|FAST)\s+(?<Description>(?:(?!\d{2}/\d{2}/\d{4}-\d{2}:\d{2}:\d{2}|Sayfa:).)*)";
                Regex regex = new Regex(pattern, RegexOptions.Singleline);
                MatchCollection matches = regex.Matches(pageText);

                foreach (Match match in matches)
                {
                    try
                    {
                        var date = DateTime.ParseExact(match.Groups["Date"].Value, "dd/MM/yyyy-HH:mm:ss", CultureInfo.InvariantCulture);
                        string branch = match.Groups["Branch"].Value.Trim();
                        string transaction = match.Groups["Transaction"].Value.Trim();
                        string transactionType = match.Groups["TransactionType"].Value.Trim();

                        string amountString = match.Groups["Amount"].Value;
                        string balanceString = match.Groups["Balance"].Value;
                        string additionalBalanceString = match.Groups["AdditionalBalance"].Value;

                        amountString = amountString.Replace(".", "").Replace(",", ".");
                        balanceString = balanceString.Replace(".", "").Replace(",", ".");
                        additionalBalanceString = additionalBalanceString.Replace(".", "").Replace(",", ".");

                        decimal amount = Decimal.Parse(amountString, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
                        decimal balance = Decimal.Parse(balanceString, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
                        decimal additionalBalance = Decimal.Parse(additionalBalanceString, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);

                        string description = match.Groups["Description"].Value.Trim();

                        transactions.Add(new Transaction
                        {
                            Date = date,
                            Branch = branch,
                            Amount = amount,
                            Balance = balance,
                            AdditionalBalance = additionalBalance,
                            Transaction = transaction,
                            TransactionType = transactionType,
                            Description = description
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
