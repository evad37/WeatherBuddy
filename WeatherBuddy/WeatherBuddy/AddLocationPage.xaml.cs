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
        private WeatherCollection weatherCollection;
        private JArray availableLocations;
        private string locationFilter = "";
        private bool isLoading = false;
        public AddLocationPage(WeatherCollection weatherCollection)
        {
            InitializeComponent();
            this.weatherCollection = weatherCollection;
        }
        protected async override void OnAppearing()
        {
            await UpdateUIAsync();
        }

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

        private async Task<List<Location>> GetFilteredLocationsAsync()
        {
            List<Location> filteredLocations = new List<Location>();
            if (availableLocations == null) {
                await LoadLocationsAsync();
            } else if (!string.IsNullOrEmpty(locationFilter))
            {
                foreach (JObject item in availableLocations.Children<JObject>())
                {
                    string name = item.Value<string>("name");
                    if (name.Contains(locationFilter))
                    {
                        Location location = new Location();
                        location.id = item.Value<int>("id");
                        location.name = item.Value<string>("name");
                        location.state = item.Value<string>("state");
                        location.country = item.Value<string>("country");
                        filteredLocations.Add(location);
                    }
                }
            }
            return filteredLocations;
        }

        

        /// <summary>
        /// Asynchronously updates the UI based on the current state of the model
        /// </summary>
        private async Task UpdateUIAsync()
        {

            List<Location> filteredLocations = await GetFilteredLocationsAsync();
            // TODO...
            Debug.WriteLine(string.Format("[UpdateUIAsync] there are {0} filteredLocations", filteredLocations.Count));
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
            await UpdateUIAsync();
        }


    }
}