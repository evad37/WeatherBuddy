using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherBuddy.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WeatherBuddy
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LocationsPage : ContentPage
    {
        private WeatherCollection weatherCollection;
        private Action onClosing;
        public LocationsPage(WeatherCollection weatherCollection, Action onClosing)
        {
            InitializeComponent();
            this.weatherCollection = weatherCollection;
            this.onClosing = onClosing;
            UpdateUI();
        }

        /// <summary>
        /// Updates the UI based on the current state of the model
        /// </summary>
        private void UpdateUI()
        {
            LocationsStackLayout.Children.Clear();
            if (weatherCollection.locations.Count > 0)
            {
                foreach (Location location in weatherCollection.locations)
                {
                    Frame frame = Components.LocationWeather(location);
                    LocationsStackLayout.Children.Add(frame);
                }
            }
            else
            {
                Label noLocationsLabel = new Label();
                noLocationsLabel.Text = "You don't yet have any locations in your collection.";
                LocationsStackLayout.Children.Add(noLocationsLabel);
            }
        }

        private void BackButton_Clicked(object sender, EventArgs e) => ClosePage();

        private async void ClosePage()
        {
            await Navigation.PopModalAsync();
            onClosing();
        }

        private void NewLocationButton_Clicked(object sender, EventArgs e) => OpenAddNewLocationPage();
        private async void OpenAddNewLocationPage()
        {
            AddLocationPage addLocationPage = new AddLocationPage(weatherCollection, UpdateUI);
            await Navigation.PushModalAsync(addLocationPage);
        }
    }
}