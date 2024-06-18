export interface CountryInfo {
  countryName: string
  populationTotal: number
  gdpvalue: number
  exchangeRate: number
  weather: weather_[]
  gdpHistoricalData : gdpHistoricalData_[]
  populationHistoricalData: populationHistoricalData_[]
}

interface weather_ {
  date: string  
  min: number
  max: number
  description: string
  img: string  
}

interface gdpHistoricalData_
{
  year: string
  value: string
}

interface populationHistoricalData_
{
  year: string
  value: string
}