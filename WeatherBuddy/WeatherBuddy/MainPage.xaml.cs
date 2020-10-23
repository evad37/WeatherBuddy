using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WeatherBuddy.Models;
using Xamarin.Forms;

namespace WeatherBuddy
{
    public partial class MainPage : ContentPage
    {
        public WeatherCollection weatherCollection { get; private set; } = new WeatherCollection();
        public MainPage()
        {
            InitializeComponent();
            UpdateUI();
        }

        /// <summary>
        /// Updates the UI based on the current state of the model
        /// </summary>
        public void UpdateUI()
        {
            LocationsStackLayout.Children.Clear();
            List<Location> mainPageLocations = weatherCollection.favouriteLocations.Count == 0
                ? weatherCollection.locations
                : weatherCollection.favouriteLocations;
            if (mainPageLocations.Count == 0)
            {
                Label noLocationsLabel = new Label();
                noLocationsLabel.Text = "No locations selected. Go to \"My Locations\" and add some to see the weather.";
                noLocationsLabel.HorizontalOptions = LayoutOptions.Center;
                LocationsStackLayout.Children.Add(noLocationsLabel);
            }
            else
            {
                Frame mainLocationWeather = Components.MainLocationWeather(mainPageLocations[0]);
                LocationsStackLayout.Children.Add(mainLocationWeather);
                foreach (Location location in mainPageLocations.Skip(1))
                {
                    Frame locationWeather = Components.LocationWeather(location);
                    LocationsStackLayout.Children.Add(locationWeather);
                }
            }
        }

        private void LocationsButton_Clicked(object sender, EventArgs e) => OpenLocationsPage();

        private async void OpenLocationsPage()
        {
            LocationsPage locationsPage = new LocationsPage(weatherCollection, UpdateUI);
            await Navigation.PushModalAsync(locationsPage);
        }

        private void PreferencesButton_Clicked(object sender, EventArgs e) => OpenPreferencesPage();
        private async void OpenPreferencesPage()
        {
            PreferencesPage preferencesPage = new PreferencesPage(weatherCollection, UpdateUI);
            await Navigation.PushModalAsync(preferencesPage);
        }
    }
}
