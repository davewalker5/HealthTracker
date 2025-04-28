using HealthTracker.Configuration.Entities;
using HealthTracker.Configuration.Interfaces;

namespace HealthTracker.Configuration.Logic
{
    public class HealthTrackerConfigReader : ConfigReader<HealthTrackerApplicationSettings>, IConfigReader<HealthTrackerApplicationSettings>
    {
        /// <summary>
        /// Load and return the application settings from the named JSON-format application settings file
        /// </summary>
        /// <param name="jsonFileName"></param>
        /// <returns></returns>
        public override HealthTrackerApplicationSettings Read(string jsonFileName)
            => base.Read(jsonFileName);
    }
}
