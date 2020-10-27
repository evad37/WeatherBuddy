using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WeatherBuddy.Models;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace WeatherBuddy
{
    public partial class MainPage : ContentPage
    {
        public WeatherCollection weatherCollection { get; private set; } = new WeatherCollection();
        private PreferencesPage preferencesPage { get; set; }
        private LocationsPage locationsPage { get; set; }
        public MainPage()
        {
            InitializeComponent();
            Debug.WriteLine("[Main page] Initilaised");
            //weatherCollection.Updated += WeatherCollection_Updated;
            preferencesPage = new PreferencesPage(weatherCollection, UpdateWeatherAndUiAsync);
            locationsPage = new LocationsPage(weatherCollection, UpdateWeatherAndUiAsync);
            UpdateWeatherAndUiAsync();
        }

        private async void UpdateWeatherAndUiAsync()
        {
            // Update each location
            await weatherCollection.LocationsLoaded;
            if (weatherCollection.favouriteLocation != null)
            {
                MainPageActivityIndicator.IsVisible = true;
                MainPageActivityIndicator.IsRunning = true;
                NoLocationsLabel.IsVisible = false;
                MainLocationFrame.IsVisible = true;
                MainLocationNameLabel.Text = weatherCollection.favouriteLocation.name;
                MainLocationTempLabel.Text = "---";
                MainLocationDescriptionLabel.Text = "----";
                await weatherCollection.favouriteLocation.GetWeather(
                    weatherCollection.api,
                    (temp, conditions) =>
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            MainLocationTempLabel.Text = Util.FormatTemp(temp, WeatherCollection.prefs.unit);
                            MainLocationDescriptionLabel.Text = conditions;
                        });
                    },
                    ShowErrorPopup
                   );
                MainPageActivityIndicator.IsVisible = false;
                MainPageActivityIndicator.IsRunning = false;
            }
            else
            {
                MainPageActivityIndicator.IsVisible = false;
                MainPageActivityIndicator.IsRunning = false;
                MainLocationFrame.IsVisible = false;
                NoLocationsLabel.IsVisible = true;
            }
        }

        private void LocationsButton_Clicked(object sender, EventArgs e) => OpenLocationsPage();

        private async void OpenLocationsPage()
        {
            await Navigation.PushModalAsync(locationsPage);
        }

        private void PreferencesButton_Clicked(object sender, EventArgs e) => OpenPreferencesPage();
        private async void OpenPreferencesPage()
        {
            await Navigation.PushModalAsync(preferencesPage);
        }

        internal async void ShowErrorPopup(string title, string message)
        {
            await DisplayAlert(title, message, "OK");
        }
    }
}
