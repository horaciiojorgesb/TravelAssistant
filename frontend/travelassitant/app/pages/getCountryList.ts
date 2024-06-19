
import { CountryBasicData } from "../TypeScriptModels/APIDataOperation/Interfaces/CountryBasicData";

export const CountryList = async (): Promise<CountryBasicData[]> => {
  try {
      const response = await fetch(process.env.NEXT_PUBLIC_DESTINATION_API || '', {
          method: 'GET',
          headers: {
              'ApiKey': process.env.NEXT_PUBLIC_API_KEY || '',
              'Content-Type': 'application/json'
          }
      });

      if (!response.ok) {
          throw new Error(`Error fetching countries: ${response.statusText}`);
      }

      const data: CountryBasicData[] = await response.json();
      console.log("response:", data);
      return data;
  } catch (error) {
      console.error("Error fetching countries:", error);
      return [];
  }
};