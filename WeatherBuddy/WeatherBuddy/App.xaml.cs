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
            MainPage = new MainPage();

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
