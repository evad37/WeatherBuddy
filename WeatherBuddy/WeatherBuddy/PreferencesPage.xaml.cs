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
    public partial class PreferencesPage : ContentPage
    {
        private WeatherCollection weatherCollection;
        bool isUpdating = false;
        private Action onClosing;
        public PreferencesPage(WeatherCollection weatherCollection, Action onClosing)
        {
            InitializeComponent();
            this.weatherCollection = weatherCollection;
            this.onClosing = onClosing;
            UpdateUI();
        }

        private void UpdateUI()
        {
            isUpdating = true;
            if (weatherCollection.prefs.unit == "C" && !CelciusRadioButton.IsChecked)
            {
                CelciusRadioButton.IsChecked = true;
            }
            else if (weatherCollection.prefs.unit == "F" && !FahrenheitRadioButton.IsChecked)
            {
                FahrenheitRadioButton.IsChecked = true;
            }
            if (DarkModeSwitch.IsToggled != weatherCollection.prefs.darkMode)
            {
                DarkModeSwitch.IsToggled = weatherCollection.prefs.darkMode;
            }
            int themePickerItemIndex = ThemePicker.Items.IndexOf(weatherCollection.prefs.theme);
            if (ThemePicker.SelectedIndex != themePickerItemIndex)
            {
                ThemePicker.SelectedIndex = themePickerItemIndex;
            }
            isUpdating = false;
        }

        private void CelciusRadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (!isUpdating && ((RadioButton)sender).IsChecked)
            {
                weatherCollection.prefs.unit = "C";
                UpdateUI();
            }
        }

        private void FahrenheitRadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (!isUpdating && ((RadioButton)sender).IsChecked)
            {
                weatherCollection.prefs.unit = "F";
                UpdateUI();
            }
        }

        private void DarkModeSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            if (!isUpdating)
            {
                weatherCollection.prefs.darkMode = ((Switch)sender).IsToggled;
                UpdateUI();
            }
        }

        private void ThemePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isUpdating)
            {
                object theme = ((Picker)sender).SelectedItem;
                if (theme != null)
                {
                    weatherCollection.prefs.theme = theme.ToString();
                    UpdateUI();
                }
            }
        }

        private void BackButton_Clicked(object sender, EventArgs e)
        {
            CloseWindow();
        }
        private async void CloseWindow()
        {
            await Navigation.PopModalAsync();
            onClosing();
        }

        private void ResetButton_Clicked(object sender, EventArgs e)
        {
            weatherCollection.prefs.unit = "C";
            weatherCollection.prefs.darkMode = false;
            weatherCollection.prefs.theme = "Sky";
            UpdateUI();
            DisplayAlert("Reset completed", "Your preferences have been reset to the defaults", "OK");
        }

        protected override void OnDisappearing()
        {
            weatherCollection.prefs.SavePreferences();
        }
    }
}