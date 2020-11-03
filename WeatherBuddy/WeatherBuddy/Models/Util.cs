using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherBuddy.Models
{
    /// <summary>
    /// Utility functions
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Formats a temperature as a string, converting from degrees Kelvin to the given unit.
        /// Uses 1 decimal place in the output.
        /// </summary>
        /// <param name="degreesKelvin">Temperature in degrees Kelvin</param>
        /// <param name="unit">Symbol for unit to convert to</param>
        /// <returns>Formatted temperature</returns>
        public static string FormatTemp(double degreesKelvin, string unit)
        {
            if (unit == "C")
            {
                double degreesCelcius = degreesKelvin - 273.15;
                return string.Format("{0:0.0}°C", degreesCelcius);
            }
            else if (unit == "F")
            {
                double degreesFahrenheit = (degreesKelvin - 273.15) * 9 / 5 + 32;
                return string.Format("{0:0.0}°F", degreesFahrenheit);
            }
            else // default to Kelvin
            {
                return string.Format("{0:0.0}°K", degreesKelvin);
            }
        }

        /// <summary>
        /// Formats a temperature as a string, converting from degrees Kelvin to the given unit.
        /// Uses whole numbers in the output.
        /// </summary>
        /// <param name="degreesKelvin">Temperature in degrees Kelvin</param>
        /// <param name="unit">Symbol for unit to convert to</param>
        /// <returns>Formatted temperature</returns>
        public static string FormatTempInteger(double degreesKelvin, string unit)
        {
            if (unit == "C")
            {
                // Convert from Kelvin
                double degreesCelcius = degreesKelvin - 273.15;
                // Round to nearest int (midpoint rounding)
                int degreesCelciusInt = (int)Math.Round(degreesCelcius, 0, MidpointRounding.AwayFromZero);
                return $"{degreesCelciusInt}°C";
            }
            else if (unit == "F")
            {
                // Convert from Kelvin
                double degreesFahrenheit = (degreesKelvin - 273.15) * 9 / 5 + 32;
                // Round to nearest int (midpoint rounding)
                int degreesFahrenheitInt = (int)Math.Round(degreesFahrenheit, 0, MidpointRounding.AwayFromZero);
                return $"{degreesFahrenheitInt}°F";
            }
            else // default to Kelvin
            {
                // Round to nearest int (midpoint rounding)
                int degreesKelvinInt = (int)Math.Round(degreesKelvin, 0, MidpointRounding.AwayFromZero);
                return $"{degreesKelvinInt}°K";
            }
        }
    }
}
