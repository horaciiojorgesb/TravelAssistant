"use client";

import React, { useState, useEffect, Suspense } from "react";

import {CountryInfo} from "../../TypeScriptModels/APIDataOperation/Interfaces/CountryInfo"

import { useUser } from '@auth0/nextjs-auth0/client';

const LazyGdp = React.lazy(() => import("../LazyComponents/GdpComparision"));
 const LazyPopulation = React.lazy(() => import("../LazyComponents/PopulationComparision"));

 const _countryDetail = ({params: { Countryiso2Code },}: {params: { Countryiso2Code: string };}) => {

 const [country, setCountry] = useState<CountryInfo>();
 const {user} = useUser();
 const [_isloaded, setisLoaded] = useState(false);
 

    useEffect(()  => {
        const getDestinationCountry = async() => {
                    
          try {            
            const response = await fetch(`${process.env.NEXT_PUBLIC_DESTINATION_DETAILS_API}=${Countryiso2Code}`,{
            method: 'GET',
            headers: {
              'ApiKey':`${process.env.NEXT_PUBLIC_API_KEY}`,
              'Content-Type': 'application/json'
            }            
          });  
                  
    
          if (response.ok) {
            const result = await response.json();
            console.log(result);
            setCountry(result)
          }
    
        }
          catch(error) {
            alert(error)
          }
        };

        getDestinationCountry();    
      },[]);

      return (        
        <div className="flex flex-col items-center min-h-screen p-3 w-full">

        <h1 className="text-center font-extrabold text-white shadow-lg text-4xl mb-8">{country?.countryName}</h1>
              
                
        {
          user !=null ?          
          <a href="/api/auth/logout" className="absolute top-0 right-0 mt-4 mr-4 px-6 py-3 text-white bg-indigo-600 rounded-lg">LogOut</a> 
          : <a href="/api/auth/login" className="absolute top-0 right-0 mt-4 mr-4 md:w-auto px-6 py-3 text-white bg-indigo-600 rounded-lg text-center">Want to see more? Click to Login/Sign Up!</a>
          
        }

{
          user ==null ?          
          <div className="mt-8 md:max-w-6xl">
          
        </div> : ""
        }     
        
      
        <div className="mt-8 w-full md:max-w-2xl">
          <div className="rounded-lg bg-white shadow-md light:bg-neutral-700">
          <h2 className="font-bold text-lg mb-4 mt-4 px-4">Weather forecast</h2>             
          {country?.weather.map((weatherObj, index) => (
              <div key={index} className="flex items-center text-center mb-4 px-4">
                <p className="mr-2">{weatherObj.date}</p>
                <img src={weatherObj.img} width={42} height={42} alt="weather" className="mr-2" />
                <p className="text-sm">{weatherObj.min} / {weatherObj.max} °C ({weatherObj.description})</p>
              </div>
            ))}       
          </div>
        </div>


        <div className="mt-8 w-full md:max-w-6xl">
          <div className={`rounded-lg bg-white shadow-md light:bg-neutral-700 ${user != null ? '' : 'filter blur-sm'}`}>
          <h2 className="font-bold text-lg mb-4 mt-4 px-4">Country Info</h2>             
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4 px-4">
              <div>
                <h3 className="text-lg">Population</h3>
                <p>{country?.populationTotal}</p>
              </div>
              <div>
                <h3 className="text-lg">GDP</h3>
                <p>{country?.gdpvalue} USD</p>
              </div>
              <div>
                <h3 className="text-lg">Exchange rate</h3>
                <p>1 Unit / {country?.exchangeRate} USD</p>
              </div>
            </div>  
          </div>
          
        </div>
        <Suspense fallback={<div>Loading population and gdp statistics...</div>}>
        <div className="flex flex-wrap justify-center w-full md:max-w-6xl">          
          <div className={`mt-8 w-full md:max-w-6xl ${user != null || _isloaded ? '' : 'filter blur-sm'} md:w-1/2`}>
                    {country && <LazyPopulation country={country} />}
                </div>

                <div className={`mt-8 w-full md:max-w-6xl ${user != null || _isloaded ? '' : 'filter blur-sm'} md:w-1/2`}>
                    {country && <LazyGdp country={country} />}
                </div>          
        </div>
        </Suspense>
             
              
      </div>
      
        );
      };
      export default _countryDetail;
