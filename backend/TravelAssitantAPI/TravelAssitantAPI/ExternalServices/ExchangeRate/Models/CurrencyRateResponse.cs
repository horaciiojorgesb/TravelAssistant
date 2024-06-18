using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TravelAssitantAPI.Models;

namespace TravelAssitantAPI.ExternalServices.ExchangeRate
{
    public class CurrencyRateResponse : ApiError
    {

        public bool sucess { get; set; }
        public long timestamp { get; set; }

        [JsonPropertyName("base")]
        public string Base { get; set; }

        public Dictionary<string, decimal> rates { get; set; }

    }
}