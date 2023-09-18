using System.Collections.Generic;
using System.Linq;

namespace GenAIFramework.Test
{
    public enum TemperatureUnit
    {
        Celsius,
        Fahrenheit
    }

    public enum Printer
    {
        HomePrinter,
        OfficePrinter
    }

    public class Utilities
    {
        private static int headcount = 200;
        private static int opex = 500;

        internal static void Reset()
        {
            headcount = 200;
            opex = 500;
        }

        /// <summary>
        /// Adds two numbers
        /// </summary>
        /// <param name="a">First number</param>
        /// <param name="b">Second number</param>
        /// <returns>sum of the two numbers</returns>
        public static double AddNumbers(double a, double b)
        {
            return a + b;
        }

        /// <summary>
        /// Returns addition of the given integers in the list.
        /// </summary>
        /// <param name="list">List of integers</param>
        /// <returns>Sum total of the numbers in the list.</returns>
        public static int Sum(IEnumerable<int> list)
        {
            return list.Aggregate((x, y) => x + y);
        }

        /// <summary>
        /// Returns string length
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Length of the string.</returns>
        public static int GetStringLength(string str)
        {
            return str.Length;
        }

        /// <summary>
        /// Gets average of given numbers
        /// </summary>
        /// <param name="numbers">array of numbers</param>
        /// <returns>Average value</returns>
        public static double Average(double[] numbers)
        {
            return numbers.Average();
        }

        /// <summary>
        /// Get the current weather in a given location
        /// </summary>
        /// <param name="location">The city and state, e.g. San Francisco, CA</param>
        /// <param name="unit"></param>
        /// <returns>Weather condition of the city</returns>
        public static Dictionary<string, object> get_current_weather(string location, TemperatureUnit unit)
        {
            var dict = new Dictionary<string, object>();
            if (location.Contains("Boston"))
            {
                dict.Add("temperature", 22);
                dict.Add("unit", "celsius");
                dict.Add("description", "Sunny");
            }
            else if (location.Contains("San Francisco"))
            {
                dict.Add("current temperature", 18.5);
                dict.Add("unit", "celsius");
                dict.Add("description", "Cloudy");
            }
            
            return dict;
        }

        /// <summary>
        /// Makes an edit to users financial forecast model.
        /// </summary>
        /// <param name="year">The year for which data to edit</param>
        /// <param name="category">The category of data to edit</param>
        /// <param name="amount">Amount of units to edit</param>
        /// <returns></returns>
        public static Dictionary<string, int> EditFinancialForecast(int year, string category, int amount)
        {
            if (category.ToLower().Contains("headcount"))
            {
                headcount += amount;
            }
            else if (category.ToLower().Contains("opex"))
            {
                opex += amount;
            }

            var dict = new Dictionary<string, int>() { { "headcount", headcount }, { "opex", opex } };
            return dict;
        }

        /// <summary>
        /// Sends the financial forecast for print
        /// </summary>
        /// <param name="printer">Printer name</param>
        /// <returns></returns>
        public static string PrintFinancialForecast(Printer printer)
        {
            return $"Printed the forecast to {printer.ToString()}";
        }
    }
}
