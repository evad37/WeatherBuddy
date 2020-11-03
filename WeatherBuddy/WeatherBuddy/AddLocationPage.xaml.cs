using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        private JArray availableLocations = new JArray();
        /// <summary>
        /// Text to filter available locations (by name)
        /// </summary>
        private string locationFilter = "";
        /// <summary>
        /// Data is currently be loaded
        /// </summary>
        private bool isLoading = false;
        /// <summary>
        /// Callback to execute when page is closing
        /// </summary>
        private Action onClosing;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="weatherCollection">User's collection of weather locations</param>
        /// <param name="onClosing">Callback to execute when page is closing</param>
        public AddLocationPage(WeatherCollection weatherCollection, Action onClosing)
        {
            InitializeComponent();
            this.weatherCollection = weatherCollection;
            this.onClosing = onClosing;
            // Load locations asynchronosuly in the background
            Task.Run(LoadLocationsAsync);
        }

        /// <summary>
        /// Async tasks to do when the page appears.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Reset the filter entry, which trigger UI to update
            LocationFilterEntry.Text = string.Empty;
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
                availableLocations = new JArray();
                using (var streamReader = new System.IO.StreamReader(stream))
                using (JsonTextReader reader = new JsonTextReader(streamReader))
                {
                    var serializer = new JsonSerializer();
                    while (await reader.ReadAsync())
                    {
                        if (reader.TokenType == JsonToken.StartObject)
                        {
                            availableLocations.Add(serializer.Deserialize<JObject>(reader));
                        }
                        // Very short delay to avoid blocking thread
                        long delayTicks = 1;
                        await Task.Delay(new TimeSpan(delayTicks));
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            finally
            {
                isLoading = false;
                Device.BeginInvokeOnMainThread(this.UpdateUI); // Main thread requiref for UI modifications
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
                int id = item.Value<int>("id");
                bool alreadyInCollection = weatherCollection.HasLocation(id);
                string name = item.Value<string>("name");
                bool isMatch = name.ToLower().Contains(locationFilter.ToLower());
                if (isMatch && !alreadyInCollection)
                {
                    Location location = new Location();
                    location.id = item.Value<int>("id");
                    location.name = item.Value<string>("name");
                    location.state = item.Value<string>("state");
                    location.country = item.Value<string>("country");
                    // Check it isn't already there, as the data list contains some duplicates.
                    bool isDuplicate = filteredLocations.FindIndex(l => l.name == location.name && l.state == location.state && l.country == location.country) >= 0;
                    if (!isDuplicate)
                    {
                        filteredLocations.Add(location);
                        // Return early if the limit has now been reached
                        if (filteredLocations.Count == resultCountLimit)
                        {
                            return filteredLocations;
                        }
                    }
                }
            }
            // Sort locations that start with the filtered text ahead of other matches
            return filteredLocations.OrderBy(location => location.name.IndexOf(locationFilter) == 0
                ? $"AAAA{location.name}"
                : location.name
            ).ToList();
        }        

        /// <summary>
        /// Asynchronously updates the UI based on the current state of the model
        /// </summary>
        private void UpdateUI()
        {
            // Update colours
            TitleFrame.BackgroundColor = Colours.GetColor("Accent");
            TitleLabel.TextColor = Colours.GetColor("Title");
            this.BackgroundColor = Colours.GetColor("Page");
            FilterLabel.TextColor = Colours.GetColor("Text");
            LocationFilterEntry.TextColor = Colours.GetColor("Text");

            // Activity indicator
            LocationsLoadingActivityIndicator.IsRunning = isLoading;
            LocationsLoadingActivityIndicator.IsVisible = isLoading;

            // Filter results
            FilterResultsStackLayout.Children.Clear();
            List<Location> filteredLocations = GetFilteredLocations();
            foreach (Location location in filteredLocations)
            {
                Frame locationFrame = Components.LocationOverview(location);
                var locationFrame_tap = new TapGestureRecognizer();
                locationFrame_tap.Tapped += async (s, e) =>
                {
                    LocationFilterEntry.Unfocus();
                    weatherCollection.AddLocation(location);
                    bool finished = await DisplayAlert("Location added", $"{location.name} has been added!", "Finished", "Add another");
                    if (finished)
                    {
                        CloseWindow();
                    } else
                    {
                        LocationFilterEntry.Text = string.Empty;
                        UpdateUI();
                    }
                };
                locationFrame.GestureRecognizers.Add(locationFrame_tap);
                FilterResultsStackLayout.Children.Add(locationFrame);
            }
        }

        private void BackButton_Clicked(object sender, EventArgs e) => CloseWindow();
        private async void CloseWindow()
        {
            LocationFilterEntry.Unfocus();
            await Navigation.PopModalAsync();
            onClosing();
        }

        private void LocationFilterEntry_TextChanged(object sender, TextChangedEventArgs e) {
            string filterText = ((Entry)sender).Text.Trim();
            if (filterText == locationFilter)
            {
                // No effective difference (just whitespace)
                return;
            }
            locationFilter = filterText;
            UpdateUI();
        }
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            bool isLandscapeOrientation = width > height;
            HorizontalViewBackButton.IsVisible = isLandscapeOrientation;
            VerticalViewBackButton.IsVisible = !isLandscapeOrientation;
        }


    }
}