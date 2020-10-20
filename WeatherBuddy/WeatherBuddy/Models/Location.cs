using System;
using System.Collections.Generic;
using System.Text;

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

        public string stateAndCountry
        {
            get => string.IsNullOrEmpty(state)
                ? country
                : string.Format("{0}, {1}", state, country);
        }

        public async void GetWeather(Api api)
        {
            string jsonResponse = await api.FetchData(id.ToString());
            // TODO: parse json response to get temp and conditions
        }
    }
}
