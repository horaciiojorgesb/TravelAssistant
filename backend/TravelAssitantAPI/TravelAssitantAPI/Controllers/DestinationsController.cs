using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TravelAssitantAPI.ExternalServices.ExchangeRate;
using TravelAssitantAPI.ExternalServices.Models;
using TravelAssitantAPI.ExternalServices.WeatherForecast;
using TravelAssitantAPI.ExternalServices.WeatherForecast.Models;
using TravelAssitantAPI.ExternalServices.WorldBank;
using TravelAssitantAPI.ExternalServices.WorldBank.Models;
using TravelAssitantAPI.Utils;

namespace TravelAssitantAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DestinationsController : Controller
    {
        private readonly string _countriesPath;
        private readonly IConfiguration _config;
        WorldBankOperations _countryInfo;

        public DestinationsController(IConfiguration configuration)
        {
            _config = configuration;
            _countriesPath = _config["FilePaths:CountriesData"];
            _countryInfo = new WorldBankOperations(configuration);
        }
        [Route("getDestinations")]
        [HttpGet]
        [Authorize]
        [ResponseCache(Duration = 60)]
        public IActionResult getDestinations()
        {
            
            try
            {
                List<CountryProperties> _countryList = new List<CountryProperties>();
                bool isUnitTestRunning = bool.Parse(_config["isUnitTestRunning"]);

                if (!isUnitTestRunning)
                {
                    if (System.IO.File.Exists(_countriesPath))
                    {
                        string[] _countries = System.IO.File.ReadAllLines(_countriesPath);

                        if (_countries.Length > 0 && _countries[0] == DateTime.Today.ToString("yyyy-MM-dd"))
                        {

                            foreach (string _country in _countries.Skip(1))
                            {
                                _countryList.Add(new CountryProperties() { Name = _country.Split(";").ElementAt(0), Iso2Code = _country.Split(";").ElementAt(1) });
                            }

                            return Ok(_countryList);
                        }
                    }

                }

                _countryList = _countryInfo.GetCountryPropertyList();


                List<string> linesToWrite = new List<string>
                {
                    DateTime.Today.ToString("yyyy-MM-dd")
                };

                if (_countryList.Count() > 0)
                {
                    if (!isUnitTestRunning)
                    {
                        linesToWrite.AddRange(_countryList.Select(c => $"{c.Name};{c.Iso2Code}"));

                        System.IO.File.WriteAllText(_countriesPath, string.Join(Environment.NewLine, linesToWrite));
                    }                    

                    return Ok(_countryList);
                }
                else
                {
                    return NotFound("No countries found!");
                }

            }
            catch (Exception ex)
            {
                Log.Fatal("GetCountries - " + ex.Message);
                return StatusCode(500, $"Unexpected Error occurred while trying to GetCountries - {ex.Message}");
            }

        }

        [Route("information/")]
        [HttpGet]
        [Authorize]
        [ResponseCache(Duration = 60, VaryByQueryKeys = new[] { "countryCode" })]
        public IActionResult getCountryDetails(string countryCode)
        {
            CountryInfo _countryInfo = new CountryInfo();

            try
            {
                WorldBankOperations worldBankOperations = new WorldBankOperations(_config);
                Forecast forecast = new Forecast(_config);
                ExchangeRate exchange = new ExchangeRate(_config);

                CountryCoordinates countryCoordinates = worldBankOperations.getCountryCoordinates(countryCode);
                
                if (countryCoordinates.ErrorCode != Consts.OKStatus)
                {
                    return StatusCode(int.Parse(countryCoordinates.ErrorCode), new CountryInfo() { ErrorCode = countryCoordinates.ErrorCode, ErrorMessage = countryCoordinates.ErrorMessage });
                }

                _countryInfo = worldBankOperations.getCountryPopulationAndGDP(countryCode);

                if (_countryInfo.ErrorCode != Consts.OKStatus)
                {
                    return StatusCode(int.Parse(_countryInfo.ErrorCode), new CountryInfo() { ErrorCode = _countryInfo.ErrorCode, ErrorMessage = _countryInfo.ErrorMessage });
                }

                WeatherResponse _forecast = forecast.GetforecastInfo(double.Parse(countryCoordinates.longitude), double.Parse(countryCoordinates.longitude));

                if (_forecast.ErrorCode != Consts.OKStatus)
                {
                    _countryInfo.ErrorMessage = $"Weather Forecast Error: {_forecast.ErrorCode} - {_forecast.ErrorMessage}";
                    return StatusCode(int.Parse(_countryInfo.ErrorCode), _countryInfo);
                }

                _countryInfo.Weather = new List<ExternalServices.Models.Weather>();

                foreach (WeatherForecast record in _forecast.List.Take(6))
                {
                    _countryInfo.Weather.Add(new ExternalServices.Models.Weather()
                    {
                        date = record.dt_txt,
                        description = record.weather.ElementAt(0).description,
                        img = record.weather.ElementAt(0).IconUrl,
                        max = record.main.temp_max,
                        min = record.main.temp_min
                    });
                }


                CurrencyRateResponse exchangeRate = exchange.getExchangeRate(countryCode);

                if (exchangeRate.ErrorCode != Consts.OKStatus)
                {
                    _countryInfo.ErrorMessage = $"Exchange Rate Error: {exchangeRate.ErrorCode} - {exchangeRate.ErrorMessage}";
                    return StatusCode(int.Parse(_countryInfo.ErrorCode), _countryInfo);
                }

                _countryInfo.exchangeRate = exchangeRate.rates.FirstOrDefault().Value.ToString();

                return Ok(_countryInfo);
            }
            catch (Exception ex)
            {
                Log.Fatal($"Error on GetCountries - {ex.Message} ");
                _countryInfo.ErrorCode = HttpStatusCode.InternalServerError.ToString();
                _countryInfo.ErrorMessage = $"Unexpected Error occurred while trying to GetCountries - {ex.Message}";
                return StatusCode(500, _countryInfo);
            }

            
        }
    }
}
