using System;
using System.Collections.Generic;
using System.Text;
using WeatherBuddy.Models;
using Xamarin.Forms;

namespace WeatherBuddy
{
    /// <summary>
    /// Class for programatically building UI components
    /// </summary>
    public static class Components
    {
        public static Frame LocationOverview(Location location)
        {
            Frame frame = new Frame();
            frame.BorderColor = Color.Beige;

            StackLayout stackLayout = new StackLayout();
            stackLayout.Orientation = StackOrientation.Horizontal;
            frame.Content = stackLayout;

            Label cityNameLabel = new Label();
            cityNameLabel.Text = location.name;
            cityNameLabel.FontAttributes = FontAttributes.Bold;
            cityNameLabel.HorizontalOptions = LayoutOptions.StartAndExpand;

            Label cityLocationLabel = new Label();
            cityLocationLabel.Text = location.stateAndCountry;
            cityLocationLabel.HorizontalOptions = LayoutOptions.End;

            stackLayout.Children.Add(cityNameLabel);
            stackLayout.Children.Add(cityLocationLabel);

            return frame;
        }
    }
}
