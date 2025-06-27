using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Medications;
using Microsoft.Extensions.Logging;

namespace HealthTracker.Client.ApiClient
{
    public class MedicationClient : HealthTrackerClientBase, IMedicationClient
    {
        private const string RouteKey = "Medication";

        public MedicationClient(
            IHealthTrackerHttpClient client,
            IHealthTrackerApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ILogger<MedicationClient> logger)
            : base(client, settings, tokenProvider, logger)
        {
        }

        /// <summary>
        /// Add a new medication to the database
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Medication> AddAsync(string name)
        {
            dynamic template = new
            {
                Name = name
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);
            var medication = Deserialize<Medication>(json);

            return medication;
        }

        /// <summary>
        /// Update an existing medications details
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Medication> UpdateAsync(int id, string name)
        {
            dynamic template = new
            {
                Id = id,
                Name = name
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);
            var medication = Deserialize<Medication>(json);

            return medication;
        }

        /// <summary>
        /// Delete a medication from the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            var baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            var route = $"{baseRoute}/{id}";
            _ = await SendDirectAsync(route, null, HttpMethod.Delete);
        }

        /// <summary>
        /// Return a list of medications
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<Medication>> ListAsync(int pageNumber, int pageSize)
        {
            // Request a list of medications
            var baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            var route = $"{baseRoute}/{pageNumber}/{pageSize}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // The returned JSON will be empty if there are no medications in the database
            List<Medication> medications = Deserialize<List<Medication>>(json);
            return medications;
        }
    }
}
