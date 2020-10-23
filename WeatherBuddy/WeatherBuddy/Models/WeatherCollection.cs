using Newtonsoft.Json;
using PCLStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherBuddy.Models
{
    /// <summary>
    /// Model for the user's chosen locations and preferences
    /// </summary>
    public class WeatherCollection
    {
        /// <summary>
        /// User preferences
        /// </summary>
        public Prefs prefs { get; private set; } = new Prefs();
        /// <summary>
        /// Locations in the user's collection
        /// </summary>
        public List<Location> locations { get; private set; } = new List<Location>();
        /// <summary>
        /// User's favourite locations
        /// </summary>
        public List<Location> favouriteLocations => locations.Where(location => location.isFavourite).ToList();
        readonly string dataFolderName = "WeatherBuddy";
        readonly string dataFileName = "Locations";

        public WeatherCollection()
        {
        }

        /// <summary>
        /// Asynchronously loads the user's locations from the device's local storage
        /// </summary>
        public async Task LoadLocationsAsync()
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
        /// Asynchronously saves the user's locations to the device's local storage
        /// </summary>
        public async Task SaveLocations()
        {
            IFolder folder = FileSystem.Current.LocalStorage;
            folder = await folder.CreateFolderAsync(dataFolderName, CreationCollisionOption.OpenIfExists);
            IFile file = await folder.CreateFileAsync(dataFileName, CreationCollisionOption.OpenIfExists);
            await file.WriteAllTextAsync(JsonConvert.SerializeObject(locations));
        }
    }
}
