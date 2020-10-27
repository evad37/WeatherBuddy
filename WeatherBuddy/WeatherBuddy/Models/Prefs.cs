using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace WeatherBuddy.Models
{
    public class Prefs
    {

        public string unit { get; set; } = "C";
        public string unitName
        {
            get
            {
                if (unit == "C")
                {
                    return "Celcius";
                }
                else if (unit == "F")
                {
                    return "Fahrenheit";
                }
                else
                {
                    return "Kelvin";
                }
                
            }
        }
        public bool darkMode { get; set; } = false;
        public string theme { get; set; } = "Sky";

        public Prefs()
        {
            LoadPreferences();
        }

        public void SavePreferences()
        {
            Preferences.Set(nameof(unit), unit);
            Preferences.Set(nameof(darkMode), darkMode);
            Preferences.Set(nameof(theme), theme);
        }

        public void LoadPreferences()
        {
            unit = Preferences.Get(nameof(unit), unit);
            darkMode = Preferences.Get(nameof(darkMode), darkMode);
            theme = Preferences.Get(nameof(theme), theme);
        }
    }
}
