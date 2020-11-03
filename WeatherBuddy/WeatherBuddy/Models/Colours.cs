using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Text;
using System.Linq;

namespace WeatherBuddy.Models
{
    public static class Colours
    {
        private static Dictionary<string, Color> colours = new Dictionary<string, Color>()
        {
            // Sky theme
            { "Page-Sky-Light", Color.FromHex("E6F2F8") },
            { "ContentBg-Sky-Light", Color.FromHex("F3F9FC") },
            { "Accent-Sky-Light", Color.FromHex("2196F3") },
            { "Title-Sky-Light", Color.White },
            { "Text-Sky-Light", Color.FromHex("000711") },

            // Sky theme, dark
            { "Page-Sky-Dark", Color.FromHex("1F2437") },
            { "ContentBg-Sky-Dark", Color.FromHex("090B11") },
            { "Accent-Sky-Dark", Color.FromHex("5985A5") },
            { "Title-Sky-Dark", Color.Black },
            { "Text-Sky-Dark", Color.FromHex("F2FCFF") },

            // Sun theme
            { "Page-Sun-Light", Color.FromHex("FDF2D0") },
            { "ContentBg-Sun-Light", Color.FromHex("FEFBEF") },
            { "Accent-Sun-Light", Color.FromHex("F9D461") },
            { "Title-Sun-Light", Color.White },
            { "Text-Sun-Light", Color.FromHex("4A3604") },

            // Sun theme, dark
            { "Page-Sun-Dark", Color.FromHex("8F6900") },
            { "ContentBg-Sun-Dark", Color.FromHex("594100") },
            { "Accent-Sun-Dark", Color.FromHex("D7AC05") },
            { "Title-Sun-Dark", Color.Black },
            { "Text-Sun-Dark", Color.FromHex("FEFBEF") },

            // Grass theme
            { "Page-Grass-Light", Color.FromHex("D3F8E4") },
            { "ContentBg-Grass-Light", Color.FromHex("F2FDF7") },
            { "Accent-Grass-Light", Color.FromHex("25DD76") },
            { "Title-Grass-Light", Color.White },
            { "Text-Grass-Light", Color.FromHex("072815") },

            // Grass theme, dark
            { "Page-Grass-Dark", Color.FromHex("16470C") },
            { "ContentBg-Grass-Dark", Color.FromHex("0D2B07") },
            { "Accent-Grass-Dark", Color.FromHex("40992F") },
            { "Title-Grass-Dark", Color.Black },
            { "Text-Grass-Dark", Color.FromHex("F5FAF4") },

            // Moon theme
            { "Page-Moon-Light", Color.FromHex("EAEAEA") },
            { "ContentBg-Moon-Light", Color.FromHex("F3F3F3") },
            { "Accent-Moon-Light", Color.FromHex("A0A0A0") },
            { "Title-Moon-Light", Color.White },
            { "Text-Moon-Light", Color.DarkSlateGray },

            // Moon theme, dark
            { "Page-Moon-Dark", Color.FromHex("121212") },
            { "ContentBg-Moon-Dark", Color.FromHex("080808") },
            { "Accent-Moon-Dark", Color.FromHex("7A7A7A") },
            { "Title-Moon-Dark", Color.Black },
            { "Text-Moon-Dark", Color.LightSlateGray },
        };

        public static Color GetColor(string type)
        {
            string mode = WeatherCollection.prefs.darkMode ? "Dark" : "Light";
            string key = $"{type}-{WeatherCollection.prefs.theme}-{mode}";
            if (colours.ContainsKey(key)) {
                return colours[key];
            }
            else if (type.Contains("Text"))
            {
                return Color.DarkRed;
            } else
            {
                return Color.Red;
            }
        }
    }
}
