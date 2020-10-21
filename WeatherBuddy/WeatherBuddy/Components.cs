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

        public static Frame LocationWeather(Location location)
        {
            Frame frame = new Frame();
            frame.BorderColor = Color.Beige;

            StackLayout outerStackLayout = new StackLayout();
            outerStackLayout.Orientation = StackOrientation.Horizontal;
            outerStackLayout.HorizontalOptions = LayoutOptions.FillAndExpand;
            frame.Content = outerStackLayout;

            StackLayout innerStackLayout = new StackLayout();
            innerStackLayout.Orientation = StackOrientation.Vertical;
            innerStackLayout.HorizontalOptions = LayoutOptions.StartAndExpand;
            outerStackLayout.Children.Add(innerStackLayout);

            Label cityNameLabel = new Label();
            cityNameLabel.Text = location.name;
            cityNameLabel.FontAttributes = FontAttributes.Bold;

            Label conditionsLabel = new Label();
            conditionsLabel.Text = location.conditions;

            Label tempLabel = new Label();
            tempLabel.Text = $"{location.tempNow}°K";
            tempLabel.HorizontalOptions = LayoutOptions.End;

            innerStackLayout.Children.Add(cityNameLabel);
            innerStackLayout.Children.Add(conditionsLabel);
            outerStackLayout.Children.Add(tempLabel);

            return frame;
        }
    }
}
