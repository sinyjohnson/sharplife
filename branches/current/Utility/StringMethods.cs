#region Apache License 2.0
/*
   Copyright 2010 SF Games

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/
#endregion

using System;

namespace Utility
{
    /// <summary>
    /// Miscellaneous string and char methods
    /// </summary>
    public static class StringMethods
    {
        /// <summary>
        /// Returns an integer from the given string or -1 if no numbers exist
        /// and the number of digits in the found integer
        /// 
        /// string may contain numbers and letters and the method will return
        /// an integer from the first run of integers found in the string
        /// 
        /// Example
        /// "67abc88" will return 67
        /// </summary>
        /// <param name="str"></param>
        /// <param name="numberOfDigits"></param>
        /// <returns></returns>
        public static int StringToInteger(string str, out int numberOfDigits)
        {
            string numbers = string.Empty;
            numberOfDigits = 0;

            foreach (char c in str)
            {
                if (char.IsDigit(c))
                {
                    numbers += c;
                    numberOfDigits++;
                }
                else
                {
                    // Stop when first non numeric character is found
                    break;
                }
            }

            if (!String.IsNullOrEmpty(numbers))
                return Convert.ToInt32(numbers);

            return -1;
        }
    }
}