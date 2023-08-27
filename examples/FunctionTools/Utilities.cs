using System.Collections.Generic;

namespace FunctionTools
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

    internal class Utilities
    {
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
        /// <param name="amount">Amoun of units to edit</param>
        /// <returns></returns>
        public static Dictionary<string, int> EditFinancialForecast(int year, string category, int amount)
        {
            int headcount = 200;
            int opex = 500;
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
        /// <param name="printer">Name of the printer that forecast should be sent for print</param>
        /// <returns></returns>
        public static string PrintFinancialForecast(Printer printer)
        {
            return $"Printed the forecast to {printer.ToString()}";
        }
    }
}
