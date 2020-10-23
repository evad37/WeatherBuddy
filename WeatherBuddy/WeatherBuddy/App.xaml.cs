using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WeatherBuddy
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Device.SetFlags(new string[] { "RadioButton_Experimental" });
            MainPage = new MainPage();

        }

        protected async override void OnStart()
        {
            await ((MainPage)MainPage).weatherCollection.LoadLocationsAsync();
            ((MainPage)MainPage).UpdateUI();
        }

        protected async override void OnSleep()
        {
            await ((MainPage)MainPage).weatherCollection.SaveLocations();
        }

        protected override void OnResume()
        {
        }
    }
}
