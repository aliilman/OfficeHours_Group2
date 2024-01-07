using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeHours_Group2.Dtos;
using OfficeHours_Group2.Response;

namespace OfficeHours_Group2.Controllers
{
    [Route("[controller]")]
    public class CurrencyController(IConfiguration configuration) : Controller
    {

        private readonly IConfiguration _configuration = configuration;

        //döviz tiplerini .json şeklinde tutulacak
        //

        //seçilen kura göre bilgilerini getiren api

        [HttpGet("live")]
        public async Task GetExchangeRatesByCurrency([FromQuery]string currency)
        {
            
        }

        [HttpGet("convert")]
        public async Task ConvertCurrency([FromQuery] string from , string to, decimal amount)
        {


        }

        [HttpGet("SupportedCurrency")]
        public async Task<ApiResponse<List<CurrencyData>>> SupportedCurrency()
        {
            var currencyDataList = GetSupportedCurrencyList();

            return new ApiResponse<List<CurrencyData>>(currencyDataList);
        }

        private List<CurrencyData> GetSupportedCurrencyList()
        {
            string json = System.IO.File.ReadAllText("supported_currency.json");

            List<CurrencyData> currencyDataList = JsonConvert.DeserializeObject<List<CurrencyData>>(json);

            return currencyDataList;
        }

        [HttpGet("GetCurrency")]
        public async Task GetCurrency()
        {

            string apiKey = _configuration["Api:ApiKey"];

            string apiUrl = "http://api.currencylayer.com/live";

            //string apiKey = "b8f1f77b143b59ab66bae4c2cde550f0"; // appsetting

            var currencies = "EUR,GBP,CAD,PLN";

            var sourceCurrency = "TRY";

            var format = "1";

            //string queryString = $"?access_key={apiKey}";
            var requestUrl = $"{apiUrl}?access_key={apiKey}&currencies={currencies}&source={sourceCurrency}&format={format}";

            // HttpClient kullanarak isteği oluşturun
            using (HttpClient client = new HttpClient())
            {
                // İsteği gerçekleştirin
                HttpResponseMessage response = await client.GetAsync(requestUrl);

                // Yanıtı kontrol edin
                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Response content: " + responseData);
                }
                else
                {
                    Console.WriteLine("Error: " + response.ReasonPhrase);
                }
            }

        }


        // [HttpGet()]
        // public async Task GetCurrency1()
        // {
        //     // CurrencyLayer API'nin endpoint'i
        //     string apiUrl = "http://api.currencylayer.com/live";

        //     // CurrencyLayer API Key
        //     string apiKey = "b8f1f77b143b59ab66bae4c2cde550f0"; // Bu kısmı kendi API anahtarınızla değiştirin

        //     // RestSharp kullanarak isteği oluşturun
        //     var client = new RestClient(apiUrl);
        //     var request = new RestRequest("Method.Get");

        //     // API Key parametresini ekleyin
        //     request.AddQueryParameter("access_key", apiKey);

        //     // İsteği gerçekleştirin
        //     var response = client.Execute(request);

        //     // Yanıtı kontrol edin
        //     if (response.IsSuccessful)
        //     {
        //         Console.WriteLine("Response content: " + response.Content);
        //     }
        //     else
        //     {
        //         Console.WriteLine("Error: " + response.ErrorMessage);
        //     }
        // }

        // girilen değer için conversion yapan
        // desteklenen currency tipler

        // "live" - get the most recent exchange rate data

        // https://api.currencylayer.com/live

        // // "historical" - get historical rates for a specific day  
        // https://api.currencylayer.com/historical?date=YYYY-MM-DD

        // // "convert" - convert one currency to another  
        // https://api.currencylayer.com/convert?from=EUR&to=GBP&amount=100

        // // "timeframe" - request exchange rates for a specific period of time 
        // https://api.currencylayer.com/timeframe?start_date=2015-01-01&end_date=2015-05-01

        // // "change" - request any currency's change parameters (margin, percentage) 
        // https://api.currencylayer.com/change?currencies=USD,EUR
    }
}