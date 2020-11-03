using Newtonsoft.Json;
using PCLStorage;
using System.Collections.Generic;
using System.Linq;
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
        public static Prefs prefs { get; private set; } = new Prefs();

        /// <summary>
        /// Locations in the user's collection
        /// </summary>
        public List<Location> locations { get; private set; } = new List<Location>();

        /// <summary>
        /// User's favourite location (or null if there is no favourite location)
        /// </summary>
        public Location favouriteLocation => locations.FirstOrDefault(location => location.isFavourite);

        /// <summary>
        /// Name of folder (within local storage) in which to store locations
        /// </summary>
        readonly string dataFolderName = "WeatherBuddy";

        /// <summary>
        /// Name of file to use to store locations
        /// </summary>
        readonly string dataFileName = "Locations";

        /// <summary>
        /// Api object instance for retrieving weather data
        /// </summary>
        public Api api { get; } = new Api();

        /// <summary>
        /// Task for loading locations
        /// </summary>
        public Task LocationsLoaded { get; private set; }

        /// <summary>
        /// Constructor. Asynchronously loads locations. 
        /// </summary>
        public WeatherCollection()
        {
            LocationsLoaded = LoadLocationsAsync();
        }

        /// <summary>
        /// Asynchronously loads the user's locations from the device's local storage.
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

        /// <summary>
        /// Edit or delete a location
        /// </summary>
        /// <param name="action">Action to peform</param>
        /// <param name="location">Location to be edited</param>
        public void EditLocation(string action, Location location)
        {
            // Get the current position within the list
            int index = locations.IndexOf(location);

            if (action == "Delete")
            {
                locations.Remove(location);
                // If the user no longer has a favourite location, make the first remaining
                // location the favourite location.
                if (favouriteLocation == null && locations.Count > 0)
                {
                    locations[0].isFavourite = true;
                }
            }
            else if (action == "Set as favourite")
            {
                // Unset previous favourite, if any
                if (favouriteLocation != null)
                {
                    favouriteLocation.isFavourite = false;
                }
                // Can now set the location as favourite
                location.isFavourite = true;
            }
            else if (action == "Move up" && index > 0) // Can't move the first entry up any further
            {
                // Remove, then reinsert at index that is one higher
                locations.Remove(location);
                locations.Insert(index - 1, location);
            }
            else if (action == "Move down" && index < locations.Count - 1) // Can't move the last entry down any further
            {
                // Remove, then reinsert at index that is one lower
                locations.Remove(location);
                locations.Insert(index + 1, location);
            }
            else if (action == "Move to start" && index > 0) // Not first entry, is already at start
            {
                // Remove, then reinsert at index 0
                locations.Remove(location);
                locations.Insert(0, location);
            }
            else if (action == "Move to end" && index < locations.Count - 1)  // Not last entry (count - 1), is already at end
            {
                // Remove, then reinsert at the last index
                locations.Remove(location);
                locations.Insert(locations.Count - 1, location);
            }
        }

        /// <summary>
        /// Checks if the collection contains a location
        /// </summary>
        /// <param name="id">ID of location to check</param>
        /// <returns>Collection contains the location</returns>
        public bool HasLocation(int id)
        {
            // If it is in the list, then the index will be 0 or higher
            return locations.FindIndex(location => location.id == id) >= 0;
        }

        /// <summary>
        /// Add a location to the list of locations. If there are no favourite
        /// locations, it will be set as the favourite.
        /// </summary>
        /// <param name="location">Location to be added</param>
        public void AddLocation(Location location)
        {
            locations.Add(location);
            if (favouriteLocation == null)
            {
                location.isFavourite = true;
            }
        }
    }
}
