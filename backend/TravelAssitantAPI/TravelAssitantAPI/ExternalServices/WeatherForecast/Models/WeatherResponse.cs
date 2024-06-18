using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelAssitantAPI.Models;

namespace TravelAssitantAPI.ExternalServices.WeatherForecast.Models
{
    public class WeatherResponse : ApiError
    {
        public string cod { get; set; }
        public string message { get; set; }
        public List<WeatherForecast> List { get; set; }
    }

    public class WeatherForecast
    {
        public Main main { get; set; }
        public List<Weather> weather { get; set; }
        public string dt_txt { get; set; }
    }

    public class Main
    {
        public double temp_min { get; set; }
        public double temp_max { get; set; }
    }

    public class Weather
    {
        public string description { get; set; }
        public string icon { get; set; }
        [JsonIgnore]
        public string IconUrl => $"http://openweathermap.org/img/wn/{icon}.png";
    }
}
