using System;
using Transaction = Classifier3.TransactionG;

namespace Classifier3
{
    public class BankContext
    {
        private readonly IBankStrategy _bankStrategy;

        public BankContext(IBankStrategy bankStrategy)
        {
            _bankStrategy = bankStrategy;
        }

        public List<Transaction> Classify(string filePath)
        {
            return _bankStrategy.ClassifyPdfContent(filePath);
        }

        public string GetBankName()
        {
            return _bankStrategy.BankName;
        }
    }

}

