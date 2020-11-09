using WeatherBuddy.Models;
using Xamarin.Forms;

namespace WeatherBuddy
{
    /// <summary>
    /// Class for programatically building UI components
    /// </summary>
    public static class Components
    {
        /// <summary>
        /// A component containing location's name, country, and state (if applicable)
        /// within a frame.
        /// </summary>
        /// <param name="location">Location to be displayed</param>
        /// <returns>Frame containing location overview</returns>
        public static Frame LocationOverview(Location location)
        {
            Label cityNameLabel = new Label();
            cityNameLabel.Text = location.name;
            cityNameLabel.FontAttributes = FontAttributes.Bold;
            cityNameLabel.HorizontalOptions = LayoutOptions.StartAndExpand;
            cityNameLabel.TextColor = Colours.GetColor("Text");

            Label cityLocationLabel = new Label();
            cityLocationLabel.Text = location.stateAndCountry;
            cityLocationLabel.HorizontalOptions = LayoutOptions.End;
            cityLocationLabel.TextColor = Colours.GetColor("Text");

            StackLayout stackLayout = new StackLayout();
            stackLayout.Orientation = StackOrientation.Horizontal;
            stackLayout.Children.Add(cityNameLabel);
            stackLayout.Children.Add(cityLocationLabel);

            Frame frame = new Frame();
            frame.BorderColor = Colours.GetColor("Accent");
            frame.BackgroundColor = Colours.GetColor("ContentBg");
            frame.Content = stackLayout;

            return frame;
        }

        /// <summary>
        /// A component containing location's name and weather information, within
        /// a frame
        /// </summary>
        /// <param name="location">Location to be displayed</param>
        /// <param name="api">API object for retreiving weather information</param>
        /// <param name="isLandscapeOrientation">Format for landscape viwing instead of portrait veiwing</param>
        /// <returns>Frame containing location's weather</returns>
        public static Frame LocationWeather(Location location, Api api, bool isLandscapeOrientation)
        {
            Label cityNameLabel = new Label();
            cityNameLabel.Text = location.name;
            cityNameLabel.FontAttributes = FontAttributes.Bold;
            cityNameLabel.TextColor = Colours.GetColor("Text");

            Label conditionsLabel = new Label();
            conditionsLabel.Text = "----";
            conditionsLabel.HorizontalOptions = isLandscapeOrientation ? LayoutOptions.EndAndExpand : LayoutOptions.Start;
            conditionsLabel.TextColor = Colours.GetColor("Text");

            Label tempLabel = new Label();
            tempLabel.Text = "---";
            tempLabel.HorizontalOptions = LayoutOptions.End;
            tempLabel.TextColor = Colours.GetColor("Text");

            StackLayout innerStackLayout = new StackLayout();
            innerStackLayout.Orientation = isLandscapeOrientation ? StackOrientation.Horizontal : StackOrientation.Vertical;
            innerStackLayout.HorizontalOptions = isLandscapeOrientation ? LayoutOptions.FillAndExpand : LayoutOptions.StartAndExpand;
            innerStackLayout.Children.Add(cityNameLabel);
            innerStackLayout.Children.Add(conditionsLabel);

            StackLayout outerStackLayout = new StackLayout();
            outerStackLayout.Orientation = StackOrientation.Horizontal;
            outerStackLayout.HorizontalOptions = LayoutOptions.FillAndExpand;

            if (location.isFavourite)
            {
                Image favouriteImage = new Image();
                string filename = WeatherCollection.prefs.darkMode ? "starwhite.png" : "star.png";
                if (Device.RuntimePlatform == Device.UWP)
                {
                    filename = "Images/" + filename;
                }
                favouriteImage.Source = filename;
                favouriteImage.WidthRequest = 20;
                favouriteImage.VerticalOptions = LayoutOptions.Start;
                outerStackLayout.Children.Add(favouriteImage);
            }

            outerStackLayout.Children.Add(innerStackLayout);
            outerStackLayout.Children.Add(tempLabel);

            Frame frame = new Frame();
            frame.BorderColor = Colours.GetColor("Accent");
            frame.BackgroundColor = Colours.GetColor("ContentBg");
            frame.Content = outerStackLayout;

            // Async call intentioanlly not awaited, as the successHandler will execute when needed. In the mean time,
            // the frame can be returned and the caller can attach it to the UI.
#pragma warning disable CS4014 // Suppress warning "Because this call is not awaited, execution of the current method continues before the call is completed"
            location.GetWeather(
                api,
                // Success callback:
                (temp, conditions) =>
                {
                    // The UI can only be updated from the main thread
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        // Replace placeholders with actual values
                        tempLabel.Text = Util.FormatTempInteger(temp, WeatherCollection.prefs.unit);
                        conditionsLabel.Text = conditions;
                    });
                },
                // Ignore errors, to avaoid flooding the user with many popups/warnings (potenitally
                // one for each location in there collection, which could be very large).
                // The error state will be noticeable as labels will just be dashes, and
                // will be explictly stated if/when they return to the main page.
                (_errTitle, _errMessage) => { }
            );
#pragma warning restore CS4014 // End of supressing "call is not awaited" warning

            return frame;
        }
    }
}
