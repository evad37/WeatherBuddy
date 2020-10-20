using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace WeatherBuddy.Models
{
    public class Api
    {  
        /// <summary>
        /// HttpClient instance
        /// </summary>
        private readonly HttpClient client = new HttpClient();
        /// <summary>
        /// Formatting string for API calls. "{0}" will be replaced with a city id code.
        /// </summary>
        private string url { get; set; } = "https://api.openweathermap.org/data/2.5/weather?id={0}&appid=" + ApiKey.appId;
        /// <summary>
        /// Cache of repsonses. Keys are base currency codes, values are tuples of the time cached and the response content.
        /// </summary>
        private Dictionary<string, (DateTime, string)> cachedResponses = new Dictionary<string, (DateTime, string)>();
        /// <summary>
        /// Number of hours before a cached response is considered stale.
        /// </summary>
        private int cacheExpriyMinutes = 10;

        /// <summary>
        /// Checks if a response has been cached for a currency code.
        /// </summary>
        /// <param name="baseCode">Currency code to check</param>
        /// <returns></returns>
        private bool IsCached(string baseCode) => cachedResponses.ContainsKey(baseCode);

        /// <summary>
        /// Returns a cached api response if it exists and is not too old
        /// </summary>
        /// <param name="baseCode">Currency code</param>
        /// <returns>Cached api response or null</returns>
        private string CachedResponseIfValid(string baseCode)
        {
            if (IsCached(baseCode))
            {
                (DateTime cachedAtDateTime, string response) = cachedResponses[baseCode];
                TimeSpan cacheAge = DateTime.Now - cachedAtDateTime;
                if (cacheAge.TotalMinutes < cacheExpriyMinutes)
                {
                    return response;
                }
            }
            return null;
        }

        /// <summary>
        /// Fetches weather data from either the api, or a previously cached response
        /// </summary>
        /// <param name="cityId">City id code</param>
        /// <returns>Weather information as JSON string</returns>
        public virtual async Task<string> FetchData(string cityId)
        {
            // Return a cached response if available and valid
            string cachedResponse = CachedResponseIfValid(cityId);
            if (!string.IsNullOrEmpty(cachedResponse))
            {
                return cachedResponse;
            }

            // Throw an error if there's no internet connection
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                throw new NoInternetException();
            }

            // Fetch data from server's api
            var response = await client.GetAsync(string.Format(url, cityId));

            // Check for errors
            if (response.StatusCode != System.Net.HttpStatusCode.OK || response.Content == null)
            {
                throw new BadResponseException(string.Format("Data could not be retrieved from the server (code: {0})", response.StatusCode));
            }

            // Extract and cache the response
            string responseString = await response.Content.ReadAsStringAsync();
            cachedResponses[cityId] = (DateTime.Now, responseString);

            return responseString;
        }
    }
}
