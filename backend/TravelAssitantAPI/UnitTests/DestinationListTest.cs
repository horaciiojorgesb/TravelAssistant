using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System.Collections.Generic;
using TravelAssitantAPI.Controllers;
using TravelAssitantAPI.ExternalServices.Models;
using TravelAssitantAPI.ExternalServices.WorldBank.Models;

namespace UnitTests
{
    class DestinationListTest
    {
        private IConfiguration _configuration;

        [SetUp]
        public void Setup()
        {
            var configBuilder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json");

            _configuration = configBuilder.Build();
        }

        [TestCase("MZ", "Mozambique")]
        [TestCase("SA", "South Africa")]
        public void DestinationExistsOnList(string iso2Code, string name)
        {
            List<CountryProperties> _countryList = new List<CountryProperties>
            {
                new CountryProperties() { Iso2Code = iso2Code, Name = name }
            };

            DestinationsController destinations = new DestinationsController(_configuration);

            var submitRequest = destinations.getDestinations();

            Assert.IsInstanceOf<ObjectResult>(submitRequest);
            var okResult = submitRequest as ObjectResult;

            Assert.IsNotNull(okResult);
            Assert.IsInstanceOf<List<CountryProperties>>(okResult.Value);

            var returnedCountryList = okResult.Value as List<CountryProperties>;

            Assert.AreEqual(_countryList.Count > 0 & _countryList.Exists(x=> x.Iso2Code == iso2Code), returnedCountryList.Count > 0 & returnedCountryList.Exists(x => x.Iso2Code == iso2Code));            

        }
    }
}
