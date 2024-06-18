using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using TravelAssitantAPI.Controllers;
using TravelAssitantAPI.ExternalServices.Models;

namespace UnitTests
{
    public class CountryInfoTest
    {
        private IConfiguration _configuration;

        [SetUp]
        public void Setup()
        {            
            var configBuilder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json");

            _configuration = configBuilder.Build();
        }

        [TestCase("MZ")]
        public void GetcountryInfo(string countryCode)
        {
            CountryInfo expectedResult = new CountryInfo()
            {
                countryName = "Mozambique",
                populationTotal = "32969518",
                gdpvalue = "18406835954.6695"
            };

            DestinationsController destinations = new DestinationsController(_configuration);

            var submitRequest = destinations.getCountryDetails(countryCode);


            Assert.IsInstanceOf<ObjectResult>(submitRequest);
            var okResult = submitRequest as ObjectResult;

            Assert.IsNotNull(okResult);
            Assert.IsInstanceOf<CountryInfo>(okResult.Value);

            var returnedCountryInfo = okResult.Value as CountryInfo;
            Assert.AreEqual(expectedResult.countryName, returnedCountryInfo.countryName);
            Assert.AreEqual(expectedResult.populationTotal, returnedCountryInfo.populationTotal);
            Assert.AreEqual(expectedResult.gdpvalue, returnedCountryInfo.gdpvalue);

        }
    }
}