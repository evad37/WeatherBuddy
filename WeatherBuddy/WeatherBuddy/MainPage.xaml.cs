using System;
using WeatherBuddy.Models;
using Xamarin.Forms;

namespace WeatherBuddy
{
    public partial class MainPage : ContentPage
    {
        private WeatherCollection weatherCollection = new WeatherCollection();
        public MainPage()
        {
            InitializeComponent();
            UpdateUI();
        }

        /// <summary>
        /// Updates the UI based on the current state of the model
        /// </summary>
        private void UpdateUI()
        {
            // TODO
        }

        private void LocationsButton_Clicked(object sender, EventArgs e) => OpenLocationsPage();

        private async void OpenLocationsPage()
        {
            LocationsPage locationsPage = new LocationsPage(weatherCollection);
            await Navigation.PushModalAsync(locationsPage);
        }

        private void PreferencesButton_Clicked(object sender, EventArgs e) => OpenPreferencesPage();
        private async void OpenPreferencesPage()
        {
            PreferencesPage preferencesPage = new PreferencesPage(weatherCollection);
            await Navigation.PushModalAsync(preferencesPage);
        }
    }
}
