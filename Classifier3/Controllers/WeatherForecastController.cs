using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Classifier3;

namespace Classifier.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly BankStrategySelector _bankStrategySelector;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
            _bankStrategySelector = new BankStrategySelector();  // BankStrategySelector'ı başlatıyoruz
        }

        [HttpGet("process")]
        public IActionResult ProcessPdf([FromQuery] string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return BadRequest("Dosya yolu geçersiz veya dosya bulunamadı.");
            }

            try
            {
                List<string> extractedTextList = PdfHelper.ExtractPageTextsFromPdf(url);  // pdf içeriği çıkar
                string extractedText = string.Join(" ", extractedTextList);

                // Banka stratejisini belirlemek için yeni sınıfı kullanıyoruz
                var bankStrategy = _bankStrategySelector.DetermineBankStrategy(extractedText);
                if (bankStrategy == null)
                {
                    return BadRequest("PDF içeriğinde tanınmayan bir banka bulundu.");
                }

                // Seçilen stratejiyi kullanarak işlemleri yap
                var context = new BankContext(bankStrategy);
                var transactions = context.Classify(url);

                return Ok(new { Bank = context.GetBankName(), Transactions = transactions });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bir hata oluştu: {ex.Message}");
            }
        }
    }
}
