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
            { "TitleBackground-Sky-Light", Color.FromHex("2196F3") },
            { "TitleText-Sky-Light", Color.White },
            { "HeadingText-Sky-Light", Color.Blue },
            { "NormalText-Sky-Light", Color.DarkBlue },
            { "BoxBorder-Sky-Light", Color.Blue },

            // Sky theme, dark
            { "TitleBackground-Sky-Dark", Color.FromHex("2196F3") },
            { "TitleText-Sky-Dark", Color.Black },
            { "HeadingText-Sky-Dark", Color.FromHex("A8E1FF") },
            { "NormalText-Sky-Dark", Color.FromHex("E5F8FF") },
            { "BoxBorder-Sky-Dark", Color.LightBlue },

            // Sun theme
            { "TitleBackground-Sun-Light", Color.Gold },
            { "TitleText-Sun-Light", Color.White },
            { "HeadingText-Sun-Light", Color.Gold },
            { "NormalText-Sun-Light", Color.DarkGoldenrod },
            { "BoxBorder-Sun-Light", Color.Gold },

            // Sun theme, dark
            { "TitleBackground-Sun-Dark", Color.Gold },
            { "TitleText-Sun-Dark", Color.Black },
            { "HeadingText-Sun-Dark", Color.LightGoldenrodYellow },
            { "NormalText-Sun-Dark", Color.PaleGoldenrod },
            { "BoxBorder-Sun-Dark", Color.LightGoldenrodYellow },

            // Grass theme
            { "TitleBackground-Grass-Light", Color.GreenYellow },
            { "TitleText-Grass-Light", Color.White },
            { "HeadingText-Grass-Light", Color.Green },
            { "NormalText-Grass-Light", Color.DarkGreen },
            { "BoxBorder-SunGrassLight", Color.Green },

            // Grass theme, dark
            { "TitleBackground-Grass-Dark", Color.Green },
            { "TitleText-Grass-Dark", Color.Black },
            { "HeadingText-Grass-Dark", Color.LightGreen },
            { "NormalText-Grass-Dark", Color.PaleGreen },
            { "BoxBorder-Grass-Dark", Color.LightGreen },

            // Moon theme
            { "TitleBackground-Moon-Light", Color.Gray },
            { "TitleText-Moon-Light", Color.White },
            { "HeadingText-Moon-Light", Color.DarkGray },
            { "NormalText-Moon-Light", Color.DarkSlateGray },
            { "BoxBorder-Moon-Light", Color.DarkGray },

            // Moon theme, dark
            { "TitleBackground-Moon-Dark", Color.Gray },
            { "TitleText-Moon-Dark", Color.Black },
            { "HeadingText-Moon-Dark", Color.LightGray },
            { "NormalText-Moon-Dark", Color.LightSlateGray },
            { "BoxBorder-Moon-Dark", Color.LightGray }
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
                return Color.LightSalmon;
            }
        }
    }
}
