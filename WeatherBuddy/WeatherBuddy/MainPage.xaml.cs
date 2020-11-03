using System;
using WeatherBuddy.Models;
using Xamarin.Forms;

namespace WeatherBuddy
{
    /// <summary>
    /// Controller for the main page
    /// </summary>
    public partial class MainPage : ContentPage
    {
        /// <summary>
        /// User's weather collection
        /// </summary>
        public WeatherCollection weatherCollection { get; private set; } = new WeatherCollection();

        /// <summary>
        /// Page for setting preferences
        /// </summary>
        private PreferencesPage preferencesPage { get; set; }

        /// <summary>
        /// Page for viewing/editing locations
        /// </summary>
        private LocationsPage locationsPage { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            // Instantiate pages, with callbacks to update this page when they close
            preferencesPage = new PreferencesPage(UpdateWeatherAndUiAsync);
            locationsPage = new LocationsPage(weatherCollection, UpdateWeatherAndUiAsync);
            // Get the latest data and update the UI
            UpdateWeatherAndUiAsync();
        }

        /// <summary>
        /// Updates weather data from the api, and updates the UI accordingly
        /// </summary>
        private async void UpdateWeatherAndUiAsync()
        {
            // Update colours
            TitleFrame.BackgroundColor = Colours.GetColor("Accent");
            TitleLabel.TextColor = Colours.GetColor("Title");
            this.BackgroundColor = Colours.GetColor("Page");
            MainLocationFrame.BackgroundColor = Colours.GetColor("ContentBg");
            MainLocationFrame.BorderColor = Colours.GetColor("Accent");
            foreach (Label label in MainLocationStackLayout.Children)
            {
                label.TextColor = Colours.GetColor("Text");
            }
            NoLocationsLabel.TextColor = Colours.GetColor("Text");

            // Update favourite location
            await weatherCollection.LocationsLoaded;
            if (weatherCollection.favouriteLocation != null)
            {
                // Show loading indicator, since will need to wait for api data 
                MainPageActivityIndicator.IsVisible = true;
                MainPageActivityIndicator.IsRunning = true;

                // There is at least one location
                NoLocationsLabel.IsVisible = false;

                // Update main location details
                MainLocationFrame.IsVisible = true;
                MainLocationNameLabel.Text = weatherCollection.favouriteLocation.name;
                MainLocationTempLabel.Text = "---"; // placeholder until Api returns data
                MainLocationDescriptionLabel.Text = "----"; // placeholder until Api returns data
                // Get weather from Api
                await weatherCollection.favouriteLocation.GetWeather(
                    weatherCollection.api,
                    // Success callback:
                    (temp, conditions) =>
                    {
                        // The UI can only be updated from the main thread
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            // Replace placeholders with actual values
                            MainLocationTempLabel.Text = Util.FormatTemp(temp, WeatherCollection.prefs.unit);
                            MainLocationDescriptionLabel.Text = conditions;
                        });
                    },
                    // Error callback:
                    ShowErrorPopup
                   );
                // Data is now loaded
                MainPageActivityIndicator.IsVisible = false;
                MainPageActivityIndicator.IsRunning = false;
            }
            else // No favourite location
            {
                // Not currently laoding data
                MainPageActivityIndicator.IsVisible = false;
                MainPageActivityIndicator.IsRunning = false;
                // No location data to display 
                MainLocationFrame.IsVisible = false;
                // Show explanatory note instead
                NoLocationsLabel.IsVisible = true;
            }
        }

        /// <summary>
        /// Handles "My locations" button clicks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LocationsButton_Clicked(object sender, EventArgs e) => OpenLocationsPage();

        /// <summary>
        /// Asynchronously opens the locations page
        /// </summary>
        private async void OpenLocationsPage()
        {
            await Navigation.PushModalAsync(locationsPage);
        }

        /// <summary>
        /// Handle "Preferences" button clicks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreferencesButton_Clicked(object sender, EventArgs e) => OpenPreferencesPage();

        /// <summary>
        /// Asynchronously opens the preferences page
        /// </summary>
        private async void OpenPreferencesPage()
        {
            await Navigation.PushModalAsync(preferencesPage);
        }

        /// <summary>
        /// Shows an error message to the user in a popup. Also makes the button
        /// for reloading data appear.
        /// </summary>
        /// <param name="title">Error titler</param>
        /// <param name="message">Error message</param>
        internal async void ShowErrorPopup(string title, string message)
        {
            await DisplayAlert(title, message, "OK");
            ReloadButton.IsVisible = true;
        }

        /// <summary>
        /// Adjusts the interface when the page orientation, or size, changes
        /// </summary>
        /// <param name="width">Page width</param>
        /// <param name="height">Page height</param>
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            // Check if the new size means the device is in landscape orientation
            bool isLandscapeOrientation = width > height;
            // Adjust the layout based on the orientation
            MainLocationStackLayout.Orientation = isLandscapeOrientation ? StackOrientation.Horizontal : StackOrientation.Vertical;
            PageButtonsStackLayout.Orientation = isLandscapeOrientation ? StackOrientation.Horizontal : StackOrientation.Vertical;
        }

        /// <summary>
        /// Handles "Reload data" button clicks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReloadButton_Clicked(object sender, EventArgs e)
        {
            // Hide the button while data loads 
            ReloadButton.IsVisible = false;
            // Update the weather data (and the UI, based on that)
            UpdateWeatherAndUiAsync();
        }
    }
}
