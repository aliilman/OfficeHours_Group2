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

        [HttpGet("live")]
        public async Task<ApiResponse<List<Quote>>> GetExchangeRatesByCurrency([FromQuery] string currency)
        {

            var currencyDataList = GetSupportedCurrencyList();
            if (!currencyDataList.Any(c => c.Code.Equals(currency , StringComparison.OrdinalIgnoreCase)))
            {
                return new ApiResponse<List<Quote>>("Currency is not supported");
            }

            string apiKey = _configuration["Api:ApiKey"];
            string apiUrl = _configuration["Api:LiveUrl"];

            var requestUrl = $"{apiUrl}?access_key={apiKey}&source={currency}";

            using HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();

                JObject jsonObject = JObject.Parse(responseData);

                List<Quote> quoteListesi = jsonObject["quotes"].ToObject<Dictionary<string, double>>()
                    .Select(pair => new Quote { Currency = pair.Key, Value = pair.Value })
                    .ToList();

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
            var currencyDataList = GetSupportedCurrencyList();
            if (!(currencyDataList.Any(c => c.Code.Equals(from , StringComparison.OrdinalIgnoreCase))&&
            currencyDataList.Any(c=>c.Code.Equals(to , StringComparison.OrdinalIgnoreCase))))
            {
                return new ApiResponse<string>("Currency is not supported");
            }
            if(amount<0)
            return new ApiResponse<string>("Amount must be greater than zero");
            
            string apiKey = _configuration["Api:ApiKey"];
            string apiUrl = _configuration["Api:ConvertUrl"];
            var requestUrl = $"{apiUrl}?access_key={apiKey}&from={from}&to={to}&amount={amount}";


            using HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();

                JObject jsonObject = JObject.Parse(responseData);

                return new ApiResponse<string>(data: jsonObject["result"].ToString());
            }
            else
            {
                return new ApiResponse<string>(response.ReasonPhrase);
            }

        }

        [HttpGet("supported-currencies")]
        public async Task<ApiResponse<List<CurrencyData>>> SupportedCurrency()
        {
            var currencyDataList = GetSupportedCurrencyList();

            return new ApiResponse<List<CurrencyData>>(currencyDataList);
        }
        private  List<CurrencyData> GetSupportedCurrencyList()
        {   

            string json = System.IO.File.ReadAllText( _configuration["Files:Supported_Currency"]);

            List<CurrencyData> currencyDataList = JsonSerializer.Deserialize<List<CurrencyData>>(json);

            return currencyDataList;
        }

    }
}