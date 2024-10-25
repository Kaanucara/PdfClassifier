using System;
using Classifier;

namespace Classifier3
{
    public class BankStrategySelector
    {
        // Banka stratejisini seçen metot
        public IBankStrategy? DetermineBankStrategy(string extractedText)
        {
            if (extractedText.Contains("www.ziraatbank.com.tr") || extractedText.Contains("0850 220 00 00"))
            {
                return new ZiraatBankStrategy();
            }
            else if (extractedText.Contains("www.vakifbank.com.tr") || extractedText.Contains("444 0 724"))
            {
                return new VakifBankStrategy();
            }
            else if (extractedText.Contains("TürkiyeİşBankası") || extractedText.Contains("481 005 8590"))
            {
                return new IsBankStrategy();
            }
            else
            {
                return null;  // Tanınmayan bir banka varsa null döner
            }
        }
    }
}
