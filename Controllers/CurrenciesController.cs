using System.Text.Json;

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OfficeHours_Group2.Dtos;
using OfficeHours_Group2.Response;

namespace OfficeHours_Group2.Controllers
{
    [Route("[controller]")]
    public class CurrenciesController(IConfiguration configuration) : Controller
    {

        private readonly IConfiguration _configuration = configuration;

        //döviz tiplerini .json şeklinde tutulacak
        //

        //seçilen kura göre bilgilerini getiren api

        [HttpGet("live")]
        public async Task<ApiResponse<List<Quote>>> GetExchangeRatesByCurrency([FromQuery] string currency)
        {
            string apiKey = _configuration["Api:ApiKey"];

            string apiUrl = "http://api.currencylayer.com/live";

            var format = "1";

            //string queryString = $"?access_key={apiKey}";
            var requestUrl = $"{apiUrl}?access_key={apiKey}&source={currency}&format={format}";

            // HttpClient kullanarak isteği oluşturun
            using HttpClient client = new();
            // İsteği gerçekleştirin
            HttpResponseMessage response = await client.GetAsync(requestUrl);

            // Yanıtı kontrol edin
            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response content: " + responseData);

                // JSON verisini JObject'e çevirme
                JObject jsonObje = JObject.Parse(responseData);

                // "quotes" alanındaki her bir veriyi alıp bir liste içine atma
                List<Quote> quoteListesi = jsonObje["quotes"].ToObject<Dictionary<string, double>>()
                    .Select(pair => new Quote { Currency = pair.Key, Value = pair.Value })
                    .ToList();

                // Liste içindeki verileri yazdırma
                foreach (var quote in quoteListesi)
                {
                    Console.WriteLine($"Currency: {quote.Currency}, Value: {quote.Value}");
                }

                return new ApiResponse<List<Quote>>(data: quoteListesi);
            }
            else
            {
                return new ApiResponse<List<Quote>>(response.ReasonPhrase);
            }
        }

        [HttpGet("convert")]
        public async Task<ApiResponse<string>> ConvertCurrency([FromQuery] string from, string to, decimal amount)
        {
            //https://api.currencylayer.com/convert?from=EUR&to=GBP&amount=100
            string apiKey = _configuration["Api:ApiKey"];

            string apiUrl = "http://api.currencylayer.com/convert";

            var requestUrl = $"{apiUrl}?access_key={apiKey}&from={from}&to={to}&amount={amount}";

            // HttpClient kullanarak isteği oluşturun
            using HttpClient client = new();
            // İsteği gerçekleştirin
            HttpResponseMessage response = await client.GetAsync(requestUrl);

            // Yanıtı kontrol edin
            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();
                //Console.WriteLine("Response content: " + responseData);

                JObject jsonObje = JObject.Parse(responseData);
                string resultValue = jsonObje["result"].ToString();
                Console.WriteLine(resultValue);

                return new ApiResponse<string>(data: resultValue);
            }
            else
            {
                return new ApiResponse<string>(response.ReasonPhrase);
            }

        }

        [HttpGet("SupportedCurrency")]
        public async Task<ApiResponse<List<CurrencyData>>> SupportedCurrency()
        {
            var currencyDataList = GetSupportedCurrencyList();

            return new ApiResponse<List<CurrencyData>>(currencyDataList);
        }

        private static List<CurrencyData> GetSupportedCurrencyList()
        {
            string json = System.IO.File.ReadAllText("supported_currency.json");

            //List<CurrencyData> currencyDataList = JsonConvert.DeserializeObject<List<CurrencyData>>(json);
            List<CurrencyData> currencyDataList = JsonSerializer.Deserialize<List<CurrencyData>>(json);

            return currencyDataList;
        }

        [HttpGet("GetCurrency")]
        public async Task GetCurrency()
        {
            string apiKey = _configuration["Api:ApiKey"];

            string apiUrl = "http://api.currencylayer.com/live";

            var currencies = "EUR,GBP,CAD,PLN";
            var sourceCurrency = "TRY";

            var format = "1";

            //string queryString = $"?access_key={apiKey}";
            var requestUrl = $"{apiUrl}?access_key={apiKey}&currencies={currencies}&source={sourceCurrency}&format={format}";

            // HttpClient kullanarak isteği oluşturun
            using HttpClient client = new();
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