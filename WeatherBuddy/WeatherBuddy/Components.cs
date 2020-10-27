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
            Label cityNameLabel = new Label();
            cityNameLabel.Text = location.name;
            cityNameLabel.FontAttributes = FontAttributes.Bold;
            cityNameLabel.HorizontalOptions = LayoutOptions.StartAndExpand;

            Label cityLocationLabel = new Label();
            cityLocationLabel.Text = location.stateAndCountry;
            cityLocationLabel.HorizontalOptions = LayoutOptions.End;

            StackLayout stackLayout = new StackLayout();
            stackLayout.Orientation = StackOrientation.Horizontal;
            stackLayout.Children.Add(cityNameLabel);
            stackLayout.Children.Add(cityLocationLabel);

            Frame frame = new Frame();
            frame.BorderColor = Color.Beige;
            frame.Content = stackLayout;

            return frame;
        }

        public static Frame LocationWeather(Location location, Api api)
        {
            Label cityNameLabel = new Label();
            cityNameLabel.Text = location.name;
            cityNameLabel.FontAttributes = FontAttributes.Bold;

            Label conditionsLabel = new Label();
            conditionsLabel.Text = "----";

            Label tempLabel = new Label();
            tempLabel.Text = "---";
            tempLabel.HorizontalOptions = LayoutOptions.End;

            StackLayout innerStackLayout = new StackLayout();
            innerStackLayout.Orientation = StackOrientation.Vertical;
            innerStackLayout.HorizontalOptions = LayoutOptions.StartAndExpand;
            innerStackLayout.Children.Add(cityNameLabel);
            innerStackLayout.Children.Add(conditionsLabel);


            StackLayout outerStackLayout = new StackLayout();
            outerStackLayout.Orientation = StackOrientation.Horizontal;
            outerStackLayout.HorizontalOptions = LayoutOptions.FillAndExpand;

            if (location.isFavourite)
            {
                Image favouriteImage = new Image();
                favouriteImage.Source = Device.RuntimePlatform == Device.UWP
                    ? "Images/star.png"
                    : "star.png";
                favouriteImage.WidthRequest = 20;
                favouriteImage.VerticalOptions = LayoutOptions.Start;
                outerStackLayout.Children.Add(favouriteImage);
            }

            outerStackLayout.Children.Add(innerStackLayout);
            outerStackLayout.Children.Add(tempLabel);

            Frame frame = new Frame();
            frame.BorderColor = Color.Beige;
            frame.Content = outerStackLayout;
            // Intentioanlly not awaited, the successHandler will execute when needed
            location.GetWeather(
                api,
                (temp, cond) => {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        tempLabel.Text = $"{location.tempNow}°K";
                        conditionsLabel.Text = location.conditions;
                    });
                },
                (errTitle, errMessage) => {  }
            );


            return frame;
        }

        public static Frame MainLocationWeather(Location location)
        {
            Label cityNameLabel = new Label();
            cityNameLabel.Text = location.name;
            cityNameLabel.VerticalOptions = LayoutOptions.Center;
            cityNameLabel.FontSize = 18;
            cityNameLabel.Margin = 0;
            cityNameLabel.Padding = 0;

            Label tempLabel = new Label();
            tempLabel.Text = $"{location.tempNow}°K";
            tempLabel.FontSize = 80;
            tempLabel.VerticalOptions = LayoutOptions.Center;
            tempLabel.HorizontalOptions = LayoutOptions.CenterAndExpand;
            tempLabel.Margin = 0;
            tempLabel.Padding = 0;

            Label conditionsLabel = new Label();
            conditionsLabel.Text = location.conditions;
            conditionsLabel.FontSize = 16;
            conditionsLabel.VerticalOptions = LayoutOptions.Center;
            conditionsLabel.HorizontalOptions = LayoutOptions.Center;
            conditionsLabel.Margin = 0;
            conditionsLabel.Padding = 0;

            StackLayout stackLayout = new StackLayout();
            stackLayout.Orientation = StackOrientation.Vertical;
            stackLayout.Children.Add(cityNameLabel);
            stackLayout.Children.Add(tempLabel);
            stackLayout.Children.Add(conditionsLabel);

            Frame frame = new Frame();
            frame.BorderColor = Color.Beige;
            frame.Content = stackLayout;
            return frame;
        }
    }
}
