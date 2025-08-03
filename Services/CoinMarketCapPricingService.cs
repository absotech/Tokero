using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tokero.Services
{
    public class CoinMarketCapPricingService
    {
        private readonly HttpClient httpClient;
        private const string ApiBaseUrl = "https://pro-api.coinmarketcap.com/";
        public CoinMarketCapPricingService(string apiKey)
        {
            httpClient = new HttpClient
            {
                BaseAddress = new Uri(ApiBaseUrl)
            };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", apiKey);
        }

        public async Task<double> GetCurrentPriceAsync(string symbol)
        {
            var requestUri = $"/v2/cryptocurrency/quotes/latest?symbol={symbol.ToUpper()}";
            try
            {
                var response = await httpClient.GetAsync(requestUri);
                if (!response.IsSuccessStatusCode) return 0;

                var jsonString = await response.Content.ReadAsStringAsync();
                var quotesResponse = JsonSerializer.Deserialize<CmcLatestResponse>(jsonString);

                if (quotesResponse?.Data != null && quotesResponse.Data.TryGetValue(symbol.ToUpper(), out var cryptoList))
                {
                    return cryptoList.FirstOrDefault()?.Quote?["USD"]?.Price ?? 0;
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching current price for {symbol}: {ex.Message}");
                return 0;
            }
        }

        //mock api because the real API is paywalled
        public Task<decimal> GetHistoricalPriceAsync(string symbol, DateTime date)
        {
            decimal basePrice = date.Month switch
            {
                1 => 42000.50m,
                2 => 45500.75m,
                3 => 51000.20m,
                4 => 68000.00m,
                5 => 65200.90m,
                6 => 69500.45m,
                7 => 71300.60m,
                8 => 72000.30m,
                _ => 60000m
            };
            decimal finalPrice = symbol.ToUpper() switch
            {
                "BTC" => basePrice,
                "ETH" => basePrice * 0.075m,
                "SOL" => basePrice * 0.003m,
                _ => basePrice * 0.001m
            };

            return Task.FromResult(finalPrice);
        }
    }

    public class CmcLatestResponse
    {
        [JsonPropertyName("data")]
        public Dictionary<string, List<CmcCrypto>> Data { get; set; }
    }

    public class CmcCrypto
    {
        [JsonPropertyName("quote")]
        public Dictionary<string, CmcQuote> Quote { get; set; }
    }

    public class CmcQuote
    {
        [JsonPropertyName("price")]
        public double? Price { get; set; }
    }
}