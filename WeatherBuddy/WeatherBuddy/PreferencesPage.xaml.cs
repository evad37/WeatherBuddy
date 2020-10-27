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
        protected override void OnAppearing()
        {
            base.OnAppearing();
            UpdateUI();
        }

        private void UpdateUI()
        {
            isUpdating = true;
            // Units
            int unitPickerIndex = UnitPicker.Items.IndexOf(WeatherCollection.prefs.unitName);
            if (UnitPicker.SelectedIndex != unitPickerIndex)
            {
                UnitPicker.SelectedIndex = unitPickerIndex;
            }
            // Dark mode
            if (DarkModeSwitch.IsToggled != WeatherCollection.prefs.darkMode)
            {
                DarkModeSwitch.IsToggled = WeatherCollection.prefs.darkMode;
            }
            // Theme
            int themePickerItemIndex = ThemePicker.Items.IndexOf(WeatherCollection.prefs.theme);
            if (ThemePicker.SelectedIndex != themePickerItemIndex)
            {
                ThemePicker.SelectedIndex = themePickerItemIndex;
            }
            isUpdating = false;
        }


        private void DarkModeSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            if (!isUpdating)
            {
                WeatherCollection.prefs.darkMode = ((Switch)sender).IsToggled;
                UpdateUI();
            }
        }

        private void UnitPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isUpdating)
            {
                object units = ((Picker)sender).SelectedItem;
                if (units != null)
                {
                    WeatherCollection.prefs.unit = units.ToString().First().ToString();
                }
            }
        }

        private void ThemePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isUpdating)
            {
                object theme = ((Picker)sender).SelectedItem;
                if (theme != null)
                {
                    WeatherCollection.prefs.theme = theme.ToString();
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
            WeatherCollection.prefs.unit = "C";
            WeatherCollection.prefs.darkMode = false;
            WeatherCollection.prefs.theme = "Sky";
            UpdateUI();
            DisplayAlert("Reset completed", "Your preferences have been reset to the defaults", "OK");
        }

        protected override void OnDisappearing()
        {
            WeatherCollection.prefs.SavePreferences();
        }

    }
}