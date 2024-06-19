# TravelAssistant
 Travel assistant web application that allows users to search for a destination city and obtain detailed information including population, GDP per capita, weather forecast, and currency exchange rates.
The frontend for this project was built with React/NextJS + Tailwind and for the backend C# was used with .NET Core Framework.

Aditionally Auth0 was used to handle user authentication (https://auth0.com/).

Frontend:
	* runs in dev at http://localhost:3000/
BackEnd:	
	Destination Info:

		Runs in dev at: http://localhost:14319/api/Destinations/information?countryCode={countryCode}  or
				https://localhost:44320/api/worldbank/getCountryCoordinatesByCode?countryCode={countryCode} in which {countryCode} represents the ISO 3166-1 alpha-2 code of the queried country. 
	Destination List List:
		Runs in dev at: http://localhost:14319/api/Destinations/getDestinations  or
				https://localhost:44320/api/Destinations/getDestinations