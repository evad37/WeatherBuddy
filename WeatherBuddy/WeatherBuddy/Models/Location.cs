using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace WeatherBuddy.Models
{
    /// <summary>
    /// Class representing a location and it's weather data.
    /// </summary>
    public class Location
    {
        /// <summary>
        /// OpenWeatherMap ID
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Name of location
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 2-letter state code for location
        /// </summary>
        public string state { get; set; }

        /// <summary>
        /// 2-letter country code for location
        /// </summary>
        public string country { get; set; }

        /// <summary>
        /// Location is one of the user's favourites
        /// </summary>
        public bool isFavourite { get; set; } = false;

        /// <summary>
        /// Current temperature in degrees Kelvin
        /// </summary>
        /// 
        public double tempNow { get; private set; }

        /// <summary>
        /// Short description of current conditions
        /// </summary>
        public string conditions { get; private set; }

        /// <summary>
        /// Tuple of error message heading and body
        /// </summary>
        public (string, string) errorMessage = ("", "");

        /// <summary>
        /// Name of country if available, otherwise the country code
        /// </summary>
        string countryName
        {
            get
            {
                try
                {
                    return (new RegionInfo(country)).EnglishName;
                }
                catch (Exception)
                {
                    // RegionInfo couldn't get the name for the code, so just return
                    // the code itself.
                    return country;
                }
            }
        }
        /// <summary>
        /// The location's state and country, or just country, depending on whether state has a value
        /// </summary>
        public string stateAndCountry => string.IsNullOrEmpty(state)
                ? countryName
                : string.Format("{0}, {1}", state, countryName);

        /// <summary>
        /// Gets and parses current weather for this location from the api  
        /// </summary>
        /// <param name="api">Api object to use for request</param>
        /// <param name="successHandler">Callback for success retrieving weather. Is passed </param>
        /// <param name="errorHandler">Callback for when there is an error retrieving weather</param>
        /// <returns>Task that is completed when weather has been retreived and parsed</returns>
        internal async Task GetWeather(Api api, Action<double, string> successHandler, Action<string, string> errorHandler)
        {
            try
            {
                // Fetch info from api
                string responseString = await api.FetchData(id.ToString());
                // Parse relevant data out of the json response string
                JObject response = JsonConvert.DeserializeObject<JObject>(responseString);
                JObject responseMain = response.Value<JObject>("main");
                tempNow = responseMain.Value<double>("temp");
                JArray responseWeatherArray = response.Value<JArray>("weather");
                JObject responseWeather = (JObject)responseWeatherArray.First;
                conditions = responseWeather.Value<string>("description");
                // Pass temperature and conditions to a callback function, to prevent race condition on
                // properties being accessed before they are actually updating.
                successHandler(tempNow, conditions);
            }
            // Catch errors and pass a heading and message to a callback function, so the caller
            // can decide what to do with the error.
            catch (NoInternetException) // User doesn't have an internet connection
            {
                errorHandler("No internet connection", "Please connect to the internet and try again.");
            }
            catch (HttpRequestException) // Error in internet connection, or in api url
            {
                errorHandler("Connection error", "Weather information could not be retrieved.");
            }
            catch (BadResponseException e) // Error from api returning a bad response 
            {
                errorHandler("Server error", e.Message);
            }
            catch (FormatException e) // Error in parsing data
            {
                errorHandler("Data error", e.Message);
            }
            catch (Exception e) // Any other errors
            {
                errorHandler("Error", e.Message);
            }
        }
    }
}
