using System;
using System.Collections.Generic;
using System.Linq;
using WeatherBuddy.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WeatherBuddy
{
    /// <summary>
    /// Controller for preferences page
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PreferencesPage : ContentPage
    {
        /// <summary>
        /// UI is currently being updated
        /// </summary>
        bool isUpdating = false;

        /// <summary>
        /// Action to execute when closing this page
        /// </summary>
        private Action onClosing;

        /// <summary>
        /// Frame elements in the view
        /// </summary>
        private List<Frame> frames;

        /// <summary>
        /// Label elements in the view
        /// </summary>
        private List<Label> labels;

        /// <summary>
        /// Picker elements in the view
        /// </summary>
        private List<Picker> pickers;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="onClosing"></param>
        public PreferencesPage(Action onClosing)
        {
            InitializeComponent();
            this.onClosing = onClosing;
            frames = new List<Frame>() { UnitsFrame, DarkModeFrame, ThemeFrame };
            labels = new List<Label>() { UnitsLabel, DarkModeLabel, ThemeLabel };
            pickers = new List<Picker>() { UnitPicker, ThemePicker };
            UpdateUI();
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
        /// Updates the UI based on current model states
        /// </summary>
        private void UpdateUI()
        {
            isUpdating = true;

            // Update colours
            TitleFrame.BackgroundColor = Colours.GetColor("Accent");
            TitleLabel.TextColor = Colours.GetColor("Title");
            this.BackgroundColor = Colours.GetColor("Page");
            foreach (Frame frame in frames)
            {
                frame.BackgroundColor = Colours.GetColor("ContentBg");
                frame.BorderColor = Colours.GetColor("Accent");
            }
            foreach (Label label in labels)
            {
                label.TextColor = Colours.GetColor("Text");
            }
            foreach (Picker picker in pickers)
            {
                picker.TextColor = Colours.GetColor("Text");
            }
            HorizontalViewBackButton.TextColor = Colours.ButtonText;
            HorizontalViewBackButton.BackgroundColor = Colours.ButtonBackground;
            HorizontalViewResetButton.TextColor = Colours.ButtonText;
            HorizontalViewResetButton.BackgroundColor = Colours.ButtonBackground;
            VerticalViewBackButton.TextColor = Colours.ButtonText;
            VerticalViewBackButton.BackgroundColor = Colours.ButtonBackground;
            VerticalViewResetButton.TextColor = Colours.ButtonText;
            VerticalViewResetButton.BackgroundColor = Colours.ButtonBackground;

            // Update controls:

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

        /// <summary>
        /// Handles toggle on the dark mode switch
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DarkModeSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            if (!isUpdating) // Make sure this is a user-generated event
            {
                WeatherCollection.prefs.darkMode = ((Switch)sender).IsToggled;
                UpdateUI();
            }
        }

        /// <summary>
        /// Handles selection changes in the unit picker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UnitPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isUpdating) // Make sure this is a user-generated event
            {
                object units = ((Picker)sender).SelectedItem;
                // Pickers allow the selected item to be null, check for this to prevent errors
                if (units != null)
                {
                    // Unit code is the first letter of the unit name
                    WeatherCollection.prefs.unit = units.ToString().First().ToString();
                }
            }
        }

        /// <summary>
        /// Handles selection changes in the theme picker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ThemePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isUpdating) // Make sure this is a user-generated event
            {
                object theme = ((Picker)sender).SelectedItem;
                // Pickers allow the selected item to be null, check for this to prevent errors
                if (theme != null)
                {
                    WeatherCollection.prefs.theme = theme.ToString();
                    UpdateUI();
                }
            }
        }

        /// <summary>
        /// Handles "Back" button clicks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackButton_Clicked(object sender, EventArgs e) => CloseWindow();

        /// <summary>
        /// Asynchronously handles page closing (pops navigation modal and executes the
        /// onClosing action)
        /// </summary>
        private async void CloseWindow()
        {
            await Navigation.PopModalAsync();
            onClosing();
        }

        /// <summary>
        /// Handles "Reset" button clicks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetButton_Clicked(object sender, EventArgs e)
        {
            // Reset model's values to default values
            WeatherCollection.prefs.Reset();
            UpdateUI();
            DisplayAlert("Reset completed", "Your preferences have been reset to the defaults", "OK");
        }

        /// <summary>
        /// When page disappears, saves the preferences
        /// </summary>
        protected override void OnDisappearing()
        {
            WeatherCollection.prefs.SavePreferences();
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
            // Show or hide elements that should only be shown in one orientation
            HorizontalViewBackButton.IsVisible = isLandscapeOrientation;
            HorizontalViewResetButton.IsVisible = isLandscapeOrientation;
            VerticalViewBackButton.IsVisible = !isLandscapeOrientation;
            VerticalViewResetButton.IsVisible = !isLandscapeOrientation;
        }
    }
}