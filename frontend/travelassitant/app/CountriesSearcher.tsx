import React, { ChangeEvent, useEffect, useState } from "react";
import Link from "next/link";
import {CountryList} from "./Country/getCountryList";
import {CountryBasicData} from "./TypeScriptModels/APIDataOperation/Interfaces/CountryBasicData"

const _searcher = () => {
  const [inpusearch_, setInputForsearch] = useState("");
  const [countryList, setCountryList] = useState<CountryBasicData[]>([]);
  const [filteredCountries, setFilteredCountries] = useState<CountryBasicData[]>([]);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const countries =  await CountryList();
        setCountryList(countries);
      } catch (error) {
        console.error("Error fetching countries:", error);
      }
    };

    fetchData();
  }, []);

  useEffect(() => {
    const filterCountries = () => {
      const filtered = countryList.filter((country) =>
        country.name.toLowerCase().includes(inpusearch_.toLowerCase())
      );
      setFilteredCountries(filtered);
    };

    if (inpusearch_.length > 0) {
      filterCountries();
    } else {
      
      setFilteredCountries(countryList);
    }
  }, [inpusearch_, countryList]);

  const handleChange = (e: ChangeEvent<HTMLInputElement>) => {
    setInputForsearch(e.target.value);
  };

  return (
    <div className="flex flex-col items-center">
      <form className="max-w-sm px-4 flex">
        <div className="relative flex-grow">
          <input
            type="text"
            placeholder="Search country"
            onChange={handleChange}
            value={inpusearch_}
            className="w-96 py-3 pl-12 pr-4 border rounded-md outline-none bg-blue-50 focus:bg-white focus:border-indigo-600"
          />
        </div>
      </form>
      {inpusearch_.length > 0 && (
      <div className="block w-full max-w-[18rem] rounded-lg bg-white shadow-[0_2px_15px_-3px_rgba(0,0,0,0.07),0_10px_20px_-2px_rgba(0,0,0,0.04)] dark:bg-neutral-700">
        <ul className="w-full">
          {filteredCountries.map((countryFiltered, index) => (
            <li
              className="w-full border-b-2 border-neutral-100 border-opacity-100 px-4 py-3 dark:border-opacity-50 hover:bg-white"
              key={index}
            >
              <Link href={`/Country/${countryFiltered.iso2Code}`}>{countryFiltered.name}</Link>
            </li>
          ))}
        </ul>
      </div>
      )}
    </div>
  );
};

export default _searcher;
