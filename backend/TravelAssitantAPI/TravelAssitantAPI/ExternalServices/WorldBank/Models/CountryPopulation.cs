using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelAssitantAPI.ExternalServices.WorldBank.Models
{
    public class CountryPopulation
    {

        public _country country { get; set; }
        public string value { get; set; }
        public string date { get; set; }
    }

    public class _country
    {
        public string id { get; set; }
        public string value { get; set; }
    }
}
