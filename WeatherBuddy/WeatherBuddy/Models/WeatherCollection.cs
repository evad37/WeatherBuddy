using Newtonsoft.Json;
using PCLStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeatherBuddy.Models
{
    /// <summary>
    /// Model for the user's chosen locations and preferences
    /// </summary>
    class WeatherCollection
    {
        Prefs prefs = new Prefs();
        List<Location> locations = new List<Location>();
        readonly string dataFolderName = "WeatherBuddy";
        readonly string dataFileName = "Locations";

        public WeatherCollection()
        {
            LoadLocations();
        }

        /// <summary>
        /// Loads the user's locations from the device's local storage
        /// </summary>
        public void LoadLocations()
        {
            LoadLocationsAsync();
        }

        /// <summary>
        /// Asynchronously loads the user's locations from the device's local storage
        /// </summary>
        private async void LoadLocationsAsync()
        {
            IFolder folder = FileSystem.Current.LocalStorage;
            folder = await folder.CreateFolderAsync(dataFolderName, CreationCollisionOption.OpenIfExists);
            IFile file = await folder.CreateFileAsync(dataFileName, CreationCollisionOption.OpenIfExists);
            string serializedData = await file.ReadAllTextAsync();
            List<Location> savedLocations = JsonConvert.DeserializeObject<List<Location>>(serializedData);
            if (savedLocations != null)
            {
                locations = savedLocations;
            }
        }

        /// <summary>
        /// Saves the user's locations from the device's local storage
        /// </summary>
        public async void SaveLocations()
        {
            IFolder folder = FileSystem.Current.LocalStorage;
            folder = await folder.CreateFolderAsync(dataFolderName, CreationCollisionOption.OpenIfExists);
            IFile file = await folder.CreateFileAsync(dataFileName, CreationCollisionOption.OpenIfExists);
            await file.WriteAllTextAsync(JsonConvert.SerializeObject(locations));
        }
    }
}
