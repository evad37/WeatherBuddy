using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WeatherBuddy.Models
{
    class Location
    {
        int id;
        string name;
        string state;
        string country;
        double tempNow;
        string conditions;
        /// <summary>
        /// Tuple of error message headring and body
        /// </summary>
        (string, string) errorMessage = ("", "");

        public string stateAndCountry
        {
            get => string.IsNullOrEmpty(state)
                ? country
                : string.Format("{0}, {1}", state, country);
        }

        public async Task<bool> GetWeather(Api api)
        {
            bool isLoaded = false;
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
                errorMessage = ("", "");
                isLoaded = true;
            }
            catch (NoInternetException)
            {
                errorMessage = ("No internet connection", "Please connect to the internet and try again.");
            }
            catch (HttpRequestException) // Error in internet connection, or in api url
            {
                errorMessage = ("Connection error", "Conversion rates could not be retrieved.");
            }
            catch (BadResponseException e) // Error from api returning a bad response 
            {
                errorMessage = ("Server error", e.Message);
            }
            catch (FormatException e) // Error in parsing data
            {
                errorMessage = ("Data error", e.Message);
            }
            catch (Exception e) // Any other errors
            {
                errorMessage = ("Error", e.Message);
            }

            return isLoaded;
        }
    }
}
