using System;
using System.Collections.Generic;
using WeatherBuddy.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WeatherBuddy
{
    /// <summary>
    /// Controller for LocationsPage
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LocationsPage : ContentPage
    {
        /// <summary>
        /// User's weather collection
        /// </summary>
        private WeatherCollection weatherCollection;

        /// <summary>
        /// Action to execute when closing
        /// </summary>
        private Action onClosing;

        /// <summary>
        /// Page for adding a location
        /// </summary>
        private AddLocationPage addLocationPage { get; set; }

        /// <summary>
        /// Device is currently is in landscape orientation
        /// </summary>
        bool isLandscapeOrientation = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="weatherCollection">User's weather collection</param>
        /// <param name="onClosing">Callback to execute when closing</param>
        public LocationsPage(WeatherCollection weatherCollection, Action onClosing)
        {
            InitializeComponent();
            this.weatherCollection = weatherCollection;
            this.onClosing = onClosing;
            this.addLocationPage = new AddLocationPage(weatherCollection, UpdateUI);
        }

        /// <summary>
        /// Updates the UI when the page appears
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            UpdateUI();
        }

        /// <summary>
        /// Updates the UI based on the current state of the model
        /// </summary>
        private void UpdateUI()
        {
            // Update colours
            TitleFrame.BackgroundColor = Colours.GetColor("Accent");
            TitleLabel.TextColor = Colours.GetColor("Title");
            this.BackgroundColor = Colours.GetColor("Page");
            EditHelpTextLabel.TextColor = Colours.GetColor("Text");

            // Update content
            LocationsStackLayout.Children.Clear(); // Remove all existing components in the stack
            if (weatherCollection.locations.Count > 0)
            {
                // Add each location as a LocationWeather component
                foreach (Location location in weatherCollection.locations)
                {
                    Frame frame = Components.LocationWeather(location, weatherCollection.api, isLandscapeOrientation);
                    // Add a tap event handler that will show editing options
                    var locationFrame_tap = new TapGestureRecognizer();
                    locationFrame_tap.Tapped += (s, e) =>
                    {
                        ShowEditOptions(location);
                    };
                    frame.GestureRecognizers.Add(locationFrame_tap);
                    LocationsStackLayout.Children.Add(frame);
                }
            }
            else
            {
                // Tell the user they don't have any locations
                Label noLocationsLabel = new Label();
                noLocationsLabel.Text = "You don't yet have any locations in your collection.";
                noLocationsLabel.TextColor = Colours.GetColor("Text");
                LocationsStackLayout.Children.Add(noLocationsLabel);
            }
        }

        /// <summary>
        /// Handle "Back" button clicks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackButton_Clicked(object sender, EventArgs e) => ClosePage();

        /// <summary>
        /// Asynchronously handles page closing (pops navigation modal and executes the
        /// onClosing action)
        /// </summary>
        private async void ClosePage()
        {
            await Navigation.PopModalAsync();
            onClosing();
        }

        /// <summary>
        /// Handle "New location" button clicks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewLocationButton_Clicked(object sender, EventArgs e) => OpenAddNewLocationPage();

        /// <summary>
        /// Asnychronously opens the Add new location page
        /// </summary>
        private async void OpenAddNewLocationPage()
        {
            await Navigation.PushModalAsync(addLocationPage);
        }

        /// <summary>
        /// Shows the edit options for a location in an ActionSheet, and handles
        /// the selected action.
        /// </summary>
        /// <param name="location">Location to be edited</param>
        private async void ShowEditOptions(Location location)
        {
            List<string> actions = new List<string>();
            int currentIndex = weatherCollection.locations.IndexOf(location);
            if (!location.isFavourite)
            {
                actions.Add("Set as favourite");
            }
            if (currentIndex > 0) // First item can not be moved any higher
            {
                actions.Add("Move to start");
                actions.Add("Move up");
            }
            if (currentIndex < weatherCollection.locations.Count - 1) // Last item can not be moved any lower
            {
                actions.Add("Move down");
                actions.Add("Move to end");
            }

            string selectedAction = await DisplayActionSheet($"Editing {location.name}",
                "Cancel",
                "Delete",
                actions.ToArray()
            );
            // Pass action through to model for handling
            weatherCollection.EditLocation(selectedAction, location);
            // UI needs to be updated to reflect the change
            UpdateUI();
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
            bool isNowLandscapeOrientation = width > height;
            // Check if the orientation has actaully changed
            bool orientationChanged = isLandscapeOrientation != isNowLandscapeOrientation;
            // Update the class property
            isLandscapeOrientation = isNowLandscapeOrientation;
            // Show or hide elements that should only be shown in one orientation
            HorizontalViewBackButton.IsVisible = isLandscapeOrientation;
            HorizontalViewNewLocationButton.IsVisible = isLandscapeOrientation;
            VerticalViewBackButton.IsVisible = !isLandscapeOrientation;
            VerticalViewNewLocationButton.IsVisible = !isLandscapeOrientation;
            if (orientationChanged)
            {
                // Programatticaly generated elements will need updating. And it
                // needs to be on the main thread because the UI will be modified.
                Device.BeginInvokeOnMainThread(UpdateUI);
            }
        }
    }
}