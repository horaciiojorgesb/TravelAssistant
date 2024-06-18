import React from 'react';
import { CountryInfo } from '@/app/TypeScriptModels/APIDataOperation/Interfaces/CountryInfo';

interface Propos
{
    country: CountryInfo;
}

const GdpComparison: React.FC<Propos> = ({ country }) => {
  return (
    <div className="rounded-lg bg-white shadow-md dark:bg-neutral-700">
      <h2 className="font-bold text-lg mb-4 mt-4 px-4">GDP Comparison from {country?.gdpHistoricalData[country?.gdpHistoricalData.length - 1].year} to {country?.gdpHistoricalData[0].year}</h2>
      <table className="min-w-full bg-white border-gray-200 divide-y divide-gray-200">
        <thead>
          <tr>
            <th className="px-6 py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">Year</th>
            <th className="px-6 py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">Total</th>
          </tr>
        </thead>
        <tbody className="bg-white divide-y divide-gray-200">
          {country?.gdpHistoricalData.map((gdpobj, index) => (
            <tr key={index}>
              <td className="px-6 py-4">{gdpobj.year}</td>
              <td className="px-6 py-4">{gdpobj.value}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default GdpComparison;
