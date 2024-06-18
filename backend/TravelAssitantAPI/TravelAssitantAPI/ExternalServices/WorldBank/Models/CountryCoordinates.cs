using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelAssitantAPI.Models;

namespace TravelAssitantAPI.ExternalServices.WorldBank.Models
{
    public class CountryCoordinates : ApiError
    {
        public string capitalCity { get; set; }
        public string longitude { get; set; }
        public string latitude { get; set; }
    }
}
