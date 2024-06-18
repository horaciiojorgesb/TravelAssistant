using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TravelAssitantAPI.Utils
{
    public class BasicFunctions
    {
        public  bool StringContainsNumbers(string input)
        {
            return input.Any(char.IsDigit);
        }
    }
}
