using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelAssitantAPI.ExternalServices.Models;
using TravelAssitantAPI.ExternalServices.WorldBank.Models;
using TravelAssitantAPI.Utils;

namespace TravelAssitantAPI.ExternalServices.WorldBank
{
    public class WorldBankOperations
    {
        private readonly IConfiguration _config;
        public WorldBankOperations(IConfiguration configuration)
        {
            _config = configuration;
        }


        public List<CountryProperties> GetCountryPropertyList()
        {
            var client = new RestClient(_config["ExternalAPIEndpoints:WorldBank"]);
            List<CountryProperties> _countries = new List<CountryProperties>();
            string errMsg;

            var request = new RestRequest
            {
                Method = Method.Get
            };

            try
            {                               
                int pageNbr = 1;
                int pages;

                request.AddParameter("format", "json");

                do
                {
                    request.AddParameter("page", pageNbr);

                    var response = client.Execute(request);

                    if (!response.IsSuccessful)
                    {
                        errMsg = $"Failed to retrieve data from World Bank API. Status Code: {response.StatusCode})";
                        Log.Fatal(errMsg);
                        return _countries;
                    }

                    List<object> jsonResponse = JsonConvert.DeserializeObject<List<object>>(response.Content);

                    var pagingInfo = JsonConvert.DeserializeObject<PageInfo>(jsonResponse[0].ToString());

                    pages = pagingInfo.Pages;

                    var countriesOnPage = JsonConvert.DeserializeObject<List<CountryProperties>>(jsonResponse[1].ToString());

                    _countries.AddRange(countriesOnPage);

                    request.Parameters.RemoveParameter("page");

                    pageNbr++;
                }
                while (pageNbr <= pages);
                
            }
            catch (Exception ex)
            {
                Log.Fatal(string.Format("FetchCountriesFromWorldBankApi - {0}", ex.Message));                
            }

            return _countries;

        }

        public CountryCoordinates getCountryCoordinates(string countryCode)
        {
            CountryCoordinates _countryCoordinates = new CountryCoordinates();

            List<CountryCoordinates> countryList = new List<CountryCoordinates>();

            try
            {
                string endpointbuilder = $"{_config["ExternalAPIEndpoints:WorldBank"]}/{countryCode}?format=json";

                var client = new RestClient(endpointbuilder);

                var request = new RestRequest
                {
                    Method = Method.Get
                };

                var response = client.Execute(request);

                int statuscode = (int)response.StatusCode;

                if (!response.IsSuccessful)
                {
                    _countryCoordinates.ErrorCode = statuscode.ToString();
                    _countryCoordinates.ErrorMessage = $"Failed to retrieve country coordinates from Worldbank API : {response.StatusCode}";
                    Log.Fatal(_countryCoordinates.ErrorMessage);

                    return _countryCoordinates;
                }

                var objJson = JsonConvert.DeserializeObject<List<object>>(response.Content);

                countryList = JsonConvert.DeserializeObject<List<CountryCoordinates>>(objJson[1].ToString());

                if (countryList.Count > 0)
                {
                    _countryCoordinates = countryList.FirstOrDefault();
                    _countryCoordinates.ErrorCode = statuscode.ToString();                    
                }
                else
                {
                    _countryCoordinates.ErrorCode = statuscode.ToString();
                    _countryCoordinates.ErrorMessage = "No content returned by WorldBank";
                    Log.Fatal(_countryCoordinates.ErrorMessage);
                }


            }
            catch (Exception ex)
            {
                Log.Fatal($"Getforecast - {ex.Message}");
                _countryCoordinates.ErrorCode = "500";
                _countryCoordinates.ErrorMessage = $"Unexpected Error occurred while trying to Get Country Coordinates - {ex.Message}";
            }

            return _countryCoordinates;

        }

        public CountryInfo getCountryPopulationAndGDP(string countryCode)
        {
            CountryInfo _countryInfo = new CountryInfo();            

            try
            {
                _countryInfo = GetPopulationData(countryCode);
                _countryInfo = GetPopulationGDP(countryCode, _countryInfo);
                _countryInfo = GetPopulationHistoricalData(countryCode, _countryInfo);
                _countryInfo = GetPopulationHistoricalGDP(countryCode, _countryInfo);
                
            }
            catch (Exception ex)
            {
                Log.Fatal($"Getforecast - {ex.Message}");
                _countryInfo.ErrorCode = "500";
                _countryInfo.ErrorMessage = $"Unexpected Error occurred while trying to Get Population and Gdp - {ex.Message}";
            }

            return _countryInfo;
        }

        #region Helpers
        public CountryInfo GetPopulationData(string countryCode)
        {
            CountryInfo _countryInfo = new CountryInfo();
            _countryInfo.populationTotal = "0";
            _countryInfo.countryName = "";
            List<CountryPopulation> _countryPopulationList = new List<CountryPopulation>();

            try
            {
                string endpointbuilder = $"{_config["ExternalAPIEndpoints:WorldBank"]}/{countryCode}/indicator/SP.POP.TOTL?date={_config["CountryPopulationDataYearFilter"]}&format=json";

                var client = new RestClient(endpointbuilder);

                var request = new RestRequest
                {
                    Method = Method.Get
                };

                var response = client.Execute(request);

                int statuscode = (int)response.StatusCode;

                if (!response.IsSuccessful)
                {
                    _countryInfo.ErrorCode = statuscode.ToString();
                    _countryInfo.ErrorMessage = $"Failed to retrieve country Population data from Worldbank API : {response.StatusCode}";
                    Log.Error(_countryInfo.ErrorMessage);
                }
                else
                {
                    Log.Information($"GetPopulationHistoricalData - {response.Content}");

                    var objJson = JsonConvert.DeserializeObject<List<object>>(response.Content);

                    _countryPopulationList = JsonConvert.DeserializeObject<List<CountryPopulation>>(objJson[1].ToString());

                    if (_countryPopulationList.Count > 0)
                    {

                        if (!string.IsNullOrEmpty(_countryPopulationList.ElementAt(0).value))
                        {
                            _countryInfo.populationTotal = _countryPopulationList.ElementAt(0).value;
                            _countryInfo.countryName = _countryPopulationList.ElementAt(0).country.value;
                            _countryInfo.ErrorCode = statuscode.ToString();
                        }
                    }
                    else
                    {
                        _countryInfo.ErrorCode = statuscode.ToString();
                        _countryInfo.ErrorMessage = "No content returned by WorldBank";
                        Log.Error(_countryInfo.ErrorMessage);
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Fatal($"GetPopulationData - {ex.Message}");
                _countryInfo.ErrorCode = "500";                
                _countryInfo.ErrorMessage = $"Unexpected Error occurred while trying to GetPopulationData - {ex.Message}";
            }

            return _countryInfo;
        }

        public CountryInfo GetPopulationHistoricalData(string countryCode, CountryInfo _countryInf)
        {
            CountryInfo _countryInfo = _countryInf;
            _countryInfo.PopulationHistoricalData = new List<PopulationHist>();

            List<CountryPopulation> _countryPopulationList = new List<CountryPopulation>();            

            try
            {
                string[] desiredyears = _config["DesiredYearsForCountryPopulationAndGDPHistoricalComparision"].Split(";");

                string endpointbuilder = $"{_config["ExternalAPIEndpoints:WorldBank"]}/{countryCode}/indicator/SP.POP.TOTL?date={desiredyears[0]}:{desiredyears[1]}&format=json";

                var client = new RestClient(endpointbuilder);

                var request = new RestRequest
                {
                    Method = Method.Get
                };

                var response = client.Execute(request);

                int statuscode = (int)response.StatusCode;

                if (!response.IsSuccessful)
                {
                    _countryInfo.ErrorCode = statuscode.ToString();
                    _countryInfo.ErrorMessage = $"Failed to retrieve country Population data from Worldbank API : {response.StatusCode}";
                    Log.Error(_countryInfo.ErrorMessage);
                }
                else
                {
                    Log.Information($"GetPopulationHistoricalData - {response.Content}");

                    var objJson = JsonConvert.DeserializeObject<List<object>>(response.Content);

                    _countryPopulationList = JsonConvert.DeserializeObject<List<CountryPopulation>>(objJson[1].ToString());

                    if (_countryPopulationList.Count > 0)
                    {                        
                        foreach (CountryPopulation pop in _countryPopulationList.OrderByDescending(x => x.date).Take(5))
                        {
                            pop.value = string.IsNullOrEmpty(pop.value) ? "N/A" : pop.value;

                            _countryInfo.PopulationHistoricalData.Add(new PopulationHist() { value = pop.value, year = pop.date });
                        }
                    }
                    else
                    {
                        Log.Information("GetPopulationHistoricalData - No content returned by WorldBank");
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Fatal($"Getforecast - {ex.Message}");
                _countryInfo.ErrorCode = "500";
                _countryInfo.ErrorMessage = $"Unexpected Error occurred while trying to GetPopulationHistoricalData - {ex.Message}";
            }

            return _countryInfo;
        }

        public CountryInfo GetPopulationGDP(string countryCode, CountryInfo _countryInf)
        {
            List<CountryGDP> _countryGDPList = new List<CountryGDP>();
            CountryInfo _countryInfo = _countryInf;
            _countryInfo.gdpvalue = "0";

            try
            {
                string endpointbuilder = $"{_config["ExternalAPIEndpoints:WorldBank"]}/{countryCode}/indicator/NY.GDP.MKTP.CD?date={_config["CountryPopulationDataYearFilter"]}&format=json";

                var client = new RestClient(endpointbuilder);

                var request = new RestRequest
                {
                    Method = Method.Get
                };

                var response = client.Execute(request);

                int statuscode = (int)response.StatusCode;

                if (!response.IsSuccessful)
                {
                    _countryInfo.ErrorCode = statuscode.ToString();
                    _countryInfo.ErrorMessage = $"Failed to retrieve country Population gdp from Worldbank API : {response.StatusCode}";
                    Log.Error(_countryInfo.ErrorMessage);
                }
                else
                {
                    Log.Information($"GetPopulationGDP - {response.Content}");

                    var objJson = JsonConvert.DeserializeObject<List<object>>(response.Content);

                    _countryGDPList = JsonConvert.DeserializeObject<List<CountryGDP>>(objJson[1].ToString());                                        

                    if (_countryGDPList.Count > 0)
                    {

                        if (!string.IsNullOrEmpty(_countryGDPList.ElementAt(0).value))
                        {
                            _countryInfo.gdpvalue = _countryGDPList.ElementAt(0).value;
                            _countryInfo.ErrorCode = statuscode.ToString();
                        }
                        else {
                            _countryInfo.gdpvalue = "N/A";
                        }
                    }
                    else
                    {
                        _countryInfo.ErrorCode = statuscode.ToString();
                        _countryInfo.ErrorMessage = "No content returned by WorldBank";
                        Log.Error(_countryInfo.ErrorMessage);
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Fatal($"GetPopulationGDP - {ex.Message}");
                _countryInfo.ErrorCode = "500";
                _countryInfo.ErrorMessage = $"Unexpected Error occurred while trying to GetPopulationGDP - {ex.Message}";
            }

            return _countryInfo;
        }

        public CountryInfo GetPopulationHistoricalGDP(string countryCode, CountryInfo _countryInf)
        {
            CountryInfo _countryInfo = _countryInf;
            _countryInfo.GdpHistoricalData = new List<GdpHist>();
            List<CountryGDP> _countryGDPList = new List<CountryGDP>();

            try
            {
                string[] desiredyears = _config["DesiredYearsForCountryPopulationAndGDPHistoricalComparision"].Split(";");

                string endpointbuilder = $"{_config["ExternalAPIEndpoints:WorldBank"]}/{countryCode}/indicator/NY.GDP.MKTP.CD?date={desiredyears[0]}:{desiredyears[1]}&format=json";

                var client = new RestClient(endpointbuilder);

                var request = new RestRequest
                {
                    Method = Method.Get
                };

                var response = client.Execute(request);

                int statuscode = (int)response.StatusCode;

                if (!response.IsSuccessful)
                {
                    _countryInfo.ErrorCode = statuscode.ToString();
                    _countryInfo.ErrorMessage = $"Failed to retrieve country Population data from Worldbank API : {response.StatusCode}";
                    Log.Error(_countryInfo.ErrorMessage);
                }
                else
                {
                    Log.Information($"GetPopulationHistoricalGDP - {response.Content}");

                    var objJson = JsonConvert.DeserializeObject<List<object>>(response.Content);

                    _countryGDPList = JsonConvert.DeserializeObject<List<CountryGDP>>(objJson[1].ToString());

                    if (_countryGDPList.Count > 0)
                    {                        
                        foreach (CountryGDP gdp in _countryGDPList.OrderByDescending(x=> x.date).Take(5))
                        {
                            gdp.value = string.IsNullOrEmpty(gdp.value) ? "N/A" : gdp.value;

                            _countryInfo.GdpHistoricalData.Add(new GdpHist() { value = gdp.value, year = gdp.date });
                        }
                    }
                    else
                    {
                        Log.Information("GetPopulationHistoricalGDP - No content returned by WorldBank");
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Fatal($"Getforecast - {ex.Message}");
                _countryInfo.ErrorCode = "500";
                _countryInfo.ErrorMessage = $"Unexpected Error occurred while trying to GetPopulationHistoricalGDP - {ex.Message}";
            }

            return _countryInfo;
        }

        #endregion



    }
}
