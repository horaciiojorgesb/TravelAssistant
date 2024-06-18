"use client";

import React from "react";
import CountriesSearcher_ from "./CountriesSearcher";

export default function Home() {
  return (
    <div className="flex flex-col items-center my-8">
      <h1 className="font-extrabold text-[#222328 text-[32px] text-white">
        Travel Assistant
      </h1>
      <p className="mb-4 font-bold text-white shadow-lg">Search for destinations that you're planing to Go!</p>
      <CountriesSearcher_ />
    </div>
  );
};

