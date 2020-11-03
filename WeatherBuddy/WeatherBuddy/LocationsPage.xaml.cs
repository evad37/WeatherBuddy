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
        private AddLocationPage addLocationPage { get; set; }
        bool isLandscapeOrientation = false;

        public LocationsPage(WeatherCollection weatherCollection, Action onClosing)
        {
            InitializeComponent();
            this.weatherCollection = weatherCollection;
            this.onClosing = onClosing;
            this.addLocationPage = new AddLocationPage(weatherCollection, UpdateUI);
            //UpdateUI();
        }

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
            LocationsStackLayout.Children.Clear();
            if (weatherCollection.locations.Count > 0)
            {
                foreach (Location location in weatherCollection.locations)
                {
                    Frame frame = Components.LocationWeather(location, weatherCollection.api, isLandscapeOrientation);
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
                Label noLocationsLabel = new Label();
                noLocationsLabel.Text = "You don't yet have any locations in your collection.";
                noLocationsLabel.TextColor = Colours.GetColor("Text");
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
            await Navigation.PushModalAsync(addLocationPage);
        }

        private async void ShowEditOptions(Location location)
        {
            List<string> actions = new List<string>();
            int currentIndex = weatherCollection.locations.IndexOf(location);
            if (!location.isFavourite)
            {
                actions.Add("Set as favourite");
            }
            if (currentIndex > 0)
            {
                actions.Add("Move to start");
                actions.Add("Move up");
            }
            if (currentIndex < weatherCollection.locations.Count-1)
            {
                actions.Add("Move down");
                actions.Add("Move to end");
            }

            string selectedAction = await DisplayActionSheet($"Editing {location.name}",
                "Cancel",
                "Delete",
                actions.ToArray()
            );
            weatherCollection.EditLocation(selectedAction, location);
            UpdateUI();
        }
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            bool isNowLandscapeOrientation = width > height;
            bool orientationChanged = isLandscapeOrientation != isNowLandscapeOrientation;
            isLandscapeOrientation = isNowLandscapeOrientation;
            HorizontalViewBackButton.IsVisible = isLandscapeOrientation;
            HorizontalViewNewLocationButton.IsVisible = isLandscapeOrientation;
            VerticalViewBackButton.IsVisible = !isLandscapeOrientation;
            VerticalViewNewLocationButton.IsVisible = !isLandscapeOrientation;
            if (orientationChanged)
            {
                Device.BeginInvokeOnMainThread(UpdateUI);
            }
            //Task.Delay(100).ContinueWith(_ =>
            //{
            //    Device.BeginInvokeOnMainThread(UpdateUI);
            //});
            //if (hasLoadedLocations)
            //{
           //     // Recreate the locationWeather components with the correct oreintation
           //     UpdateUI();
            //} 
        }
    }
}