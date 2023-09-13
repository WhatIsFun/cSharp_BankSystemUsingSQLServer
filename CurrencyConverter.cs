using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using Newtonsoft.Json;

namespace cSharp_BankSystemUsingSQLServer
{
    internal class CurrencyConverter
    {
        private readonly HttpClient httpClient;

        public CurrencyConverter()
        {
            httpClient = new HttpClient();
        }

        public async Task<CurrencyConverterData> GetExCurrencyConverterAsync(string Base, string exchangeTo)
        {
            string CurrencyConverterApiUrl = $"https://v6.exchangerate-api.com/v6/2d8754c1bf6d68b8bbea954d/pair/{Base}/{exchangeTo}";

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(CurrencyConverterApiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    CurrencyConverterData CurrencyConverterData = JsonConvert.DeserializeObject<CurrencyConverterData>(responseBody);
                    return CurrencyConverterData;
                }
                else
                {
                    Console.WriteLine($"API request failed with status code: {response.StatusCode}");
                    return null;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Failed to retrieve data from the API: {ex.Message}");
                return null;
            }
        }

    }

    public class CurrencyConverterData
    {
        public string base_code { get; set; }
        public string target_code { get; set; }
        public string conversion_rate { get; set; }
    }
}

