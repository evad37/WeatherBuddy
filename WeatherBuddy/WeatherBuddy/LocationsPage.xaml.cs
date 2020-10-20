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
        public LocationsPage(WeatherCollection weatherCollection)
        {
            InitializeComponent();
            this.weatherCollection = weatherCollection;
            UpdateUI();
        }

        /// <summary>
        /// Updates the UI based on the current state of the model
        /// </summary>
        private void UpdateUI()
        {
            // TODO
        }

        private void BackButton_Clicked(object sender, EventArgs e) => ClosePage();

        private async void ClosePage()
        {
            await Navigation.PopModalAsync();
        }

        private void NewLocationButton_Clicked(object sender, EventArgs e) => OpenAddNewLocationPage();
        private async void OpenAddNewLocationPage()
        {
            AddLocationPage addLocationPage = new AddLocationPage(weatherCollection);
            await Navigation.PushModalAsync(addLocationPage);
        }
    }
}