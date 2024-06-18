using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelAssitantAPI.Models;

namespace TravelAssitantAPI.ExternalServices.Models
{
    public class CountryInfo : ApiError
    {
        public string countryName { get; set; }
        public string populationTotal { get; set; }
        public string gdpvalue { get; set; }
        public string exchangeRate { get; set; }

        public List<Weather> Weather { get; set; }
        public List<GdpHist> GdpHistoricalData { get; set; }
        public List<PopulationHist> PopulationHistoricalData { get; set; }

    }

    public class Weather
    { 
        public string date { get; set; }
        public double min { get; set; }
        public double max { get; set; }
        public string description { get; set; }
        public string img { get; set; }
    }

    public class GdpHist
    {
        public string year { get; set; }
        public string value { get; set; }
    }

    public class PopulationHist
    {
        public string year { get; set; }
        public string value { get; set; }
    }
}
