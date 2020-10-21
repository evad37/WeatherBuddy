using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using WeatherBuddy.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WeatherBuddy
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddLocationPage : ContentPage
    {
        /// <summary>
        /// Main model for the app
        /// </summary>
        private WeatherCollection weatherCollection;
        /// <summary>
        /// JSON data for available locations
        /// </summary>
        private JArray availableLocations;
        /// <summary>
        /// Text to filter available locations (by name)
        /// </summary>
        private string locationFilter = "";
        /// <summary>
        /// Data is currently be loaded
        /// </summary>
        private bool isLoading = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="weatherCollection">User's collection of weather locations</param>
        public AddLocationPage(WeatherCollection weatherCollection)
        {
            InitializeComponent();
            this.weatherCollection = weatherCollection;
        }

        /// <summary>
        /// Async tasks to do when the page appears.
        /// </summary>
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await Task.Delay(500); // Gives time for the page to actually be displayed.
            await LoadLocationsAsync().ContinueWith(_ =>
            {
                // Switch to main thread so UI can be modified
                Device.BeginInvokeOnMainThread(() =>
                {
                    UpdateUI();
                });
            });
        }

        /// <summary>
        /// Load list of (and data for) available locations from json file
        /// </summary>
        /// <returns></returns>
        private async Task LoadLocationsAsync()
        {
            isLoading = true;
            try
            {
                var assembly = IntrospectionExtensions.GetTypeInfo(typeof(AddLocationPage)).Assembly;
                Stream stream = assembly.GetManifestResourceStream("WeatherBuddy.Data.citylist.json");
                string text = "";
                using (var reader = new System.IO.StreamReader(stream))
                {
                    text = await reader.ReadToEndAsync();
                    availableLocations = JsonConvert.DeserializeObject<JArray>(text);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            finally
            {
                isLoading = false;
            }
        }

        /// <summary>
        /// Get a list of Location objects by filtering the available locations
        /// with the locationFilter. Limited to a maximum of 20 results.
        /// </summary>
        /// <returns>List of matching locations</returns>
        private List<Location> GetFilteredLocations()
        {
            // Limit to no more than 20 results, to prevent adding thousands of 
            // locations for when only a few letters have been typed
            const int resultCountLimit = 20;

            List<Location> filteredLocations = new List<Location>();

            if (availableLocations == null || string.IsNullOrEmpty(locationFilter)) {
                // No result to giver as either still loading, or the filter is empty
                return filteredLocations;
            }

            // Add each matching location as a Location object
            foreach (JObject item in availableLocations.Children<JObject>())
            {
                string name = item.Value<string>("name");
                if (name.ToLower().Contains(locationFilter.ToLower()))
                {
                    Location location = new Location();
                    location.id = item.Value<int>("id");
                    location.name = item.Value<string>("name");
                    location.state = item.Value<string>("state");
                    location.country = item.Value<string>("country");
                    filteredLocations.Add(location);
                    // Retrun early if the limit has now been reached
                    if (filteredLocations.Count == resultCountLimit)
                    {
                        return filteredLocations;
                    }
                }
            }
            return filteredLocations;
        }        

        /// <summary>
        /// Asynchronously updates the UI based on the current state of the model
        /// </summary>
        private void UpdateUI()
        {
            // Activity indicator
            LocationsLoadingActivityIndicator.IsRunning = isLoading;
            LocationsLoadingActivityIndicator.IsVisible = isLoading;

            // Filter results
            FilterResultsStackLayout.Children.Clear();
            List<Location> filteredLocations = GetFilteredLocations();
            foreach (Location location in filteredLocations)
            {
                Frame locationFrame = Components.LocationOverview(location);
                FilterResultsStackLayout.Children.Add(locationFrame);
            }
        }

        private void BackButton_Clicked(object sender, EventArgs e) => CloseWindow();
        private async void CloseWindow()
        {
            await Navigation.PopModalAsync();
        }

        private void LocationFilterEntry_TextChanged(object sender, TextChangedEventArgs e) => FilterChanged(((Entry)sender).Text);

        private async void FilterChanged(string filterText) {
            if (filterText.Trim() == locationFilter)
            {
                // No effective difference
                return;
            }
            locationFilter = filterText.Trim();
            UpdateUI();
        }


    }
}