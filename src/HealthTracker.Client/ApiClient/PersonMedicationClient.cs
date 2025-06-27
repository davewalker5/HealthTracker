using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Medications;
using Microsoft.Extensions.Logging;

namespace HealthTracker.Client.ApiClient
{
    public class PersonMedicationClient : HealthTrackerClientBase, IPersonMedicationClient
    {
        private const string RouteKey = "PersonMedication";

        public PersonMedicationClient(
            IHealthTrackerHttpClient client,
            IHealthTrackerApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ILogger<PersonMedicationClient> logger)
            : base(client, settings, tokenProvider, logger)
        {
        }

        /// <summary>
        /// Add a new person/medication association to the database
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="medicationId"></param>
        /// <param name="dose"></param>
        /// <param name="stock"></param>
        /// <param name="lastTaken"></param>
        /// <returns></returns>
        public async Task<PersonMedication> AddAsync(int personId, int medicationId, int dose, int stock, DateTime? lastTaken)
        {
            dynamic template = new
            {
                PersonId = personId,
                MedicationId = medicationId,
                DailyDose = dose,
                Stock = stock,
                LastTaken = lastTaken,
                Active = true
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);
            var association = Deserialize<PersonMedication>(json);

            return association;
        }

        /// <summary>
        /// Update an existing person/medication association
        /// </summary>
        /// <param name="id"></param>
        /// <param name="personId"></param>
        /// <param name="medicationId"></param>
        /// <param name="dose"></param>
        /// <param name="stock"></param>
        /// <param name="active"></param>
        /// <param name="lastTaken"></param>
        /// <returns></returns>
        public async Task<PersonMedication> UpdateAsync(int id, int personId, int medicationId, int dose, int stock, bool active, DateTime? lastTaken)
        {
            dynamic template = new
            {
                Id = id,
                PersonId = personId,
                MedicationId = medicationId,
                DailyDose = dose,
                Stock = stock,
                LastTaken = lastTaken,
                Active = active
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);
            var association = Deserialize<PersonMedication>(json);

            return association;
        }

        /// <summary>
        /// Activate a person medication association
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<PersonMedication> ActivateAsync(int id)
            => await SetPersonMedicationStateAsync(id, true);

        /// <summary>
        /// Deactivate a person medication association
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<PersonMedication> DeactivateAsync(int id)
            => await SetPersonMedicationStateAsync(id, false);

        /// <summary>
        /// Delete a person/medication association from the database
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
        /// Retrieve a single measurement given its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<PersonMedication> GetAsync(int id)
        {
            // Request the association with the specified ID
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            string route = $"{baseRoute}/{id}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // Extract the association from the response
            var association = Deserialize<PersonMedication>(json);
            return association;
        }

        /// <summary>
        /// Return a list of person/medication associations for a person
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<PersonMedication>> ListAsync(int personId, int pageNumber, int pageSize)
        {
            // Request a list of medications
            var baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            string route = $"{baseRoute}/{personId}/{pageNumber}/{pageSize}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // The returned JSON will be empty if there are no associations for the person in the database
            List<PersonMedication> associations = Deserialize<List<PersonMedication>>(json);
            return associations;
        }

        /// <summary>
        /// Activate or deactivate a person medication association
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<PersonMedication> SetPersonMedicationStateAsync(int id, bool state)
        {
            // Perpare the payload
            dynamic template = new { Id = id, State = state };
            var data = Serialize(template);

            // Construct the route and send the request
            var baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            string route = $"{baseRoute}/setstate";
            string json = await SendDirectAsync(route, data, HttpMethod.Put);

            // Deseerialise the association from the response and return it
            var association = Deserialize<PersonMedication>(json);
            return association;
        }
    }
}
