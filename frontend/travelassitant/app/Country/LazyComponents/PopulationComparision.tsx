import React from 'react';
import { CountryInfo } from '@/app/TypeScriptModels/APIDataOperation/Interfaces/CountryInfo';

interface Props {
  country: CountryInfo;
}

const PopulationComparison: React.FC<Props> = ({ country }) => {
  return (
    <div className="rounded-lg bg-white shadow-md dark:bg-blue-300">
      <h2 className="font-bold text-lg mb-4 mt-4 px-4">Population Comparison from {country?.populationHistoricalData[country?.populationHistoricalData.length - 1].year} to {country?.populationHistoricalData[0].year}</h2>
      <table className="min-w-full bg-white border-white-200 divide-y divide-gray-200">
        <thead>
          <tr>
            <th className="px-6 py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">Year</th>
            <th className="px-6 py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">Total</th>
          </tr>
        </thead>
        <tbody className="bg-white divide-y divide-gray-200">
          {country?.populationHistoricalData.map((populationObj, index) => (
            <tr key={index}>
              <td className="px-6 py-4">{populationObj.year}</td>
              <td className="px-6 py-4">{populationObj.value}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default PopulationComparison;
