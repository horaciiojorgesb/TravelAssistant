using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TravelAssitantAPI.ExternalServices.WeatherForecast.Models;
using TravelAssitantAPI.Utils;

namespace TravelAssitantAPI.ExternalServices.WeatherForecast
{
    public class Forecast
    {
        private readonly IConfiguration _config;
        public Forecast(IConfiguration configuration)
        {
            _config = configuration;
        }

        public WeatherResponse GetforecastInfo(double _latitude, double _longitude)
        {
            WeatherResponse _weatherResp = new WeatherResponse();

            try
            {
                string apikey = Environment.GetEnvironmentVariable("WEATHER_FORECAST_API_KEY");

                if (string.IsNullOrEmpty(apikey))
                {
                    _weatherResp.ErrorCode = HttpStatusCode.NotFound.ToString();
                    _weatherResp.ErrorMessage = Consts.ApiKeyNotFound;
                    return _weatherResp;
                }

                string endpointbuilder = $"{_config["ExternalAPIEndpoints:OpenWeatherMap"]}?lat={_latitude}&lon={_longitude}&units=metric&appid={apikey}";

                var client = new RestClient(endpointbuilder);

                var request = new RestRequest
                {
                    Method = Method.Get
                };

                var response = client.Execute(request);

                int statuscode = (int)response.StatusCode;

                if (!response.IsSuccessful)
                {
                    _weatherResp.ErrorCode = statuscode.ToString();
                    _weatherResp.ErrorMessage = $"Failed to retrieve data from Weatherforecast : {response.StatusCode}";
                    Log.Fatal(_weatherResp.ErrorMessage);
                }
                else
                {
                    _weatherResp = JsonConvert.DeserializeObject<WeatherResponse>(response.Content);
                    _weatherResp.ErrorCode = statuscode.ToString();
                }                

            }
            catch (Exception ex)
            {
                Log.Fatal($"Getforecast - {ex.Message}");
                _weatherResp.ErrorCode = "500";
                _weatherResp.ErrorMessage = $"Unexpected Error occurred while trying to Getforecast - {ex.Message}";                
            }

            return _weatherResp;
        }
    }
}
