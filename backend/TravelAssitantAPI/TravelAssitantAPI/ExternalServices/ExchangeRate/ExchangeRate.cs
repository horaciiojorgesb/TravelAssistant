using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TravelAssitantAPI.Utils;

namespace TravelAssitantAPI.ExternalServices.ExchangeRate
{
    public class ExchangeRate
    {
        private readonly IConfiguration _config;

        public ExchangeRate(IConfiguration configuration)
        {
            _config = configuration;
        }

        public CurrencyRateResponse getExchangeRate(string countryCode)
        {
            CurrencyRateResponse _currencyRate = new CurrencyRateResponse();

            try
            {
                string apikey = Environment.GetEnvironmentVariable("EXCHANGE_RATE_API_KEY");

                if (string.IsNullOrEmpty(apikey))
                {
                    _currencyRate.ErrorCode = HttpStatusCode.NotFound.ToString();
                    _currencyRate.ErrorMessage = Consts.ApiKeyNotFound;
                }

                string currencySymbol = Consts.CountryCurr.FirstOrDefault(x => x.Key == countryCode).Value;

                if (string.IsNullOrEmpty(currencySymbol))
                {
                    _currencyRate.ErrorCode = HttpStatusCode.NotFound.ToString();
                    _currencyRate.ErrorMessage = Consts.CurrencySymbolNotFound;
                }

                string endpointbuilder = $"{_config["ExternalAPIEndpoints:Exchangeratesapi"]}?access_key={apikey}&symbols={currencySymbol}";

                var client = new RestClient(endpointbuilder);

                var request = new RestRequest
                {
                    Method = Method.Get
                };

                var response = client.Execute(request);

                int statuscode = (int)response.StatusCode;

                if (!response.IsSuccessful)
                {
                    _currencyRate.ErrorCode = statuscode.ToString();
                    _currencyRate.ErrorMessage = $"Failed to retrieve data from exchange rates : {response.StatusCode}";
                    Log.Fatal(_currencyRate.ErrorMessage);

                }

                _currencyRate = JsonConvert.DeserializeObject<CurrencyRateResponse>(response.Content);
                _currencyRate.ErrorCode = statuscode.ToString();


            }
            catch (Exception ex)
            {
                Log.Fatal($"Getforecast - {ex.Message}");
                _currencyRate.ErrorCode = "500";
                _currencyRate.ErrorMessage = $"Unexpected Error occurred while trying to get Exchange Rate - {ex.Message}";
            }

            return _currencyRate;
        }
    }
}
