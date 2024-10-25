using System;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections.Generic;
using System.Transactions;
using Transaction = Classifier3.TransactionG;


namespace Classifier3
{
    public class ZiraatBankStrategy : IBankStrategy
    {
        public string BankName => "Ziraat Bankası";

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

        public List<Transaction> ClassifyPdfContentByPage(List<string> pageList)
        {
            List<Transaction> transactions = new List<Transaction>();
            foreach (var pageText in pageList)
            {
                // Ziraat Bankası'na özgü regex ile tarih, açıklama ve miktarı ayıklıyoruz
                string pattern = @"(?<Date>\d{2}\.\d{2}\.\d{4})\s+(?<Reference>[A-Z]\d{5})\s+(?<Amount>-?\d{1,3}(?:\.\d{3})*,\d{2})\s+(?<Balance>-?\d{1,3}(?:\.\d{3})*,\d{2})\s+(?<Description>[\s\S]+?(?=\d{2}\.\d{2}\.\d{4}|KUVEYT TÜRK KATILIM BANKASI A.Ş.|Sayfa))";
                Regex regex = new Regex(pattern);
                MatchCollection matches = regex.Matches(pageText);
                CultureInfo turkishCulture = new CultureInfo("tr-TR");

                foreach (Match match in matches)
                {
                    try
                    {
                        // Tarih bilgisini işleme
                        var date = DateTime.ParseExact(match.Groups["Date"].Value, "dd.MM.yyyy", null);
                        string reference = match.Groups["Reference"].Value;

                        // Miktar ve bakiye bilgilerini işleme
                        string amountString = match.Groups["Amount"].Value.Replace(".", "").Replace(",", ".");
                        string balanceString = match.Groups["Balance"].Value.Replace(".", "").Replace(",", ".");

                        if (amountString.StartsWith(","))
                        {
                            amountString = "0" + amountString;
                        }

                        if (balanceString.StartsWith(","))
                        {
                            balanceString = "0" + balanceString;
                        }

                        decimal amount = Decimal.Parse(amountString, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
                        decimal balance = Decimal.Parse(balanceString, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);

                        string description = match.Groups["Description"].Value.Trim();

                        // Yeni işlem kaydını listeye ekle
                        transactions.Add(new Transaction
                        {
                            Date = date,
                            Reference = reference,
                            Amount = amount,
                            Balance = balance,
                            Description = description
                        });
                    }
                    catch (Exception ex)
                    {
                        // Hataları loglayabilirsiniz
                        Console.WriteLine($"Hata: {ex.Message}");
                    }
                }
            }


                return transactions;
            }
        }
    }

