using Xamarin.Essentials;

namespace WeatherBuddy.Models
{
    /// <summary>
    /// Class represeting the user's preferences
    /// </summary>
    public class Prefs
    {
        /// <summary>
        /// Temperature unit symbol
        /// </summary>
        public string unit { get; set; } = "C";

        /// <summary>
        /// Temperature unit name
        /// </summary>
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

        /// <summary>
        /// Use dark mode
        /// </summary>
        public bool darkMode { get; set; } = false;

        /// <summary>
        /// Colour theme name
        /// </summary>
        public string theme { get; set; } = "Sky";

        /// <summary>
        /// Constructor. Loads values from device platform's preferences.
        /// </summary>
        public Prefs()
        {
            LoadPreferences();
        }

        /// <summary>
        /// Save preference values to device platform's preferences.
        /// </summary>
        public void SavePreferences()
        {
            Preferences.Set(nameof(unit), unit);
            Preferences.Set(nameof(darkMode), darkMode);
            Preferences.Set(nameof(theme), theme);
        }

        /// <summary>
        /// Load preference values from device platform's preferences. 
        /// </summary>
        public void LoadPreferences()
        {
            // Defaults are assigned when the Prefs object is instantiated,
            // so they are just passed through as the second parameter
            unit = Preferences.Get(nameof(unit), unit);
            darkMode = Preferences.Get(nameof(darkMode), darkMode);
            theme = Preferences.Get(nameof(theme), theme);
        }
    }
}
