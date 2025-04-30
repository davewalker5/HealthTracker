using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Medications;

namespace HealthTracker.Client.ApiClient
{
    public class PersonMedicationClient : HealthTrackerClientBase, IPersonMedicationClient
    {
        private const string RouteKey = "PersonMedication";

        public PersonMedicationClient(IHealthTrackerHttpClient client, IHealthTrackerApplicationSettings settings, IAuthenticationTokenProvider tokenProvider)
            : base(client, settings, tokenProvider)
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
        public async Task<PersonMedication> AddPersonMedicationAsync(int personId, int medicationId, int dose, int stock, DateTime? lastTaken)
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
        public async Task<PersonMedication> UpdatePersonMedicationAsync(int id, int personId, int medicationId, int dose, int stock, bool active, DateTime? lastTaken)
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
        public async Task<PersonMedication> ActivatePersonMedicationAsync(int id)
            => await SetPersonMedicationStateAsync(id, true);

        /// <summary>
        /// Deactivate a person medication association
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<PersonMedication> DeactivatePersonMedicationAsync(int id)
            => await SetPersonMedicationStateAsync(id, false);

        /// <summary>
        /// Delete a person/medication association from the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeletePersonMedicationAsync(int id)
        {
            var baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            var route = $"{baseRoute}/{id}";
            _ = await SendDirectAsync(route, null, HttpMethod.Delete);
        }

        /// <summary>
        /// Return a list of person/medication associations for a person
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public async Task<List<PersonMedication>> ListPersonMedicationsAsync(int personId)
        {
            // Request a list of medications
            var baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            string route = $"{baseRoute}/{personId}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // The returned JSON will be empty if there are no associations for the person in the database
            List<PersonMedication> associations = !string.IsNullOrEmpty(json) ? Deserialize<List<PersonMedication>>(json) : null;
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
