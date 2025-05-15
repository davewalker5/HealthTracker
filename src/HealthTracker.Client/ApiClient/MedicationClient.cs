using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Medications;

namespace HealthTracker.Client.ApiClient
{
    public class MedicationClient : HealthTrackerClientBase, IMedicationClient
    {
        private const string RouteKey = "Medication";

        public MedicationClient(IHealthTrackerHttpClient client, IHealthTrackerApplicationSettings settings, IAuthenticationTokenProvider tokenProvider)
            : base(client, settings, tokenProvider)
        {
        }

        /// <summary>
        /// Add a new medication to the database
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Medication> AddMedicationAsync(string name)
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
        public async Task<Medication> UpdateMedicationAsync(int id, string name)
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
        public async Task DeleteMedicationAsync(int id)
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
        public async Task<List<Medication>> ListMedicationsAsync(int pageNumber, int pageSize)
        {
            // Request a list of medications
            var baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            var route = $"{baseRoute}/{pageNumber}/{pageSize}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // The returned JSON will be empty if there are no medications in the database
            List<Medication> medications = !string.IsNullOrEmpty(json) ? Deserialize<List<Medication>>(json) : null;
            return medications;
        }
    }
}
