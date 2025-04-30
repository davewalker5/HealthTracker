using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Medications;

namespace HealthTracker.Client.ApiClient
{
    public class MedicationTrackingClient : HealthTrackerClientBase, IMedicationTrackingClient
    {
        private const string RouteKey = "MedicationTracking";

        public MedicationTrackingClient(IHealthTrackerHttpClient client, IHealthTrackerApplicationSettings settings, IAuthenticationTokenProvider tokenProvider)
            : base(client, settings, tokenProvider)
        {
        }

        /// <summary>
        /// Add stock to a person/medication association
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="medicationId"></param>
        /// <param name="tablets"></param>
        /// <returns></returns>
        public async Task<PersonMedication> AddMedicationStockAsync(int personId, int medicationId, int tablets)
            => await SendStockRequest(personId, medicationId, tablets, "addstock");

        /// <summary>
        /// Set the stock level for a person/medication association
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="medicationId"></param>
        /// <param name="tablets"></param>
        /// <returns></returns>
        public async Task<PersonMedication> SetMedicationStockAsync(int personId, int medicationId, int tablets)
            => await SendStockRequest(personId, medicationId, tablets, "setstock");

        /// <summary>
        /// Set the daily dose for a person/medication association
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="medicationId"></param>
        /// <param name="tablets"></param>
        /// <returns></returns>
        public async Task<PersonMedication> SetMedicationDoseAsync(int personId, int medicationId, int tablets)
            => await SendStockRequest(personId, medicationId, tablets, "setdose");

        /// <summary>
        /// Record taking a dose of a medication
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="medicationId"></param>
        /// <returns></returns>
        public async Task<PersonMedication> TakeDoseAsync(int personId, int medicationId)
            => await SendSingleMedicationTrackingRequest(personId, medicationId, "take");

        /// <summary>
        /// Record taking a dose of all medications
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public async Task TakeAllDosesAsync(int personId)
            => await SendAllMedicationsTrackingRequest(personId, "takeall");

        /// <summary>
        /// Record un-taking a dose of a medication
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="medicationId"></param>
        /// <returns></returns>
        public async Task<PersonMedication> UntakeDoseAsync(int personId, int medicationId)
            => await SendSingleMedicationTrackingRequest(personId, medicationId, "untake");

        /// <summary>
        /// Record un-taking a dose of all medications
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public async Task UntakeAllDosesAsync(int personId)
            => await SendAllMedicationsTrackingRequest(personId, "untakeall");

        /// <summary>
        /// Fast-forward recording of doses taken for a medication
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="medicationId"></param>
        /// <returns></returns>
        public async Task<PersonMedication> FastForwardAsync(int personId, int medicationId)
            => await SendSingleMedicationTrackingRequest(personId, medicationId, "fastforward");

        /// <summary>
        /// Fast-forward recording of doses taken for all medications for a person
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public async Task FastForwardAllAsync(int personId)
            => await SendAllMedicationsTrackingRequest(personId, "fastforwardall");

        /// <summary>
        /// Record skipping a dose of a medication
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="medicationId"></param>
        /// <returns></returns>
        public async Task<PersonMedication> SkipDoseAsync(int personId, int medicationId)
            => await SendSingleMedicationTrackingRequest(personId, medicationId, "skip");

        /// <summary>
        /// Record skipping a dose of all medications
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public async Task SkipAllDosesAsync(int personId)
            => await SendAllMedicationsTrackingRequest(personId, "skipall");

        /// <summary>
        /// Send a stock or dose management request
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="medicationId"></param>
        /// <param name="tablets"></param>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        private async Task<PersonMedication> SendStockRequest(int personId, int medicationId, int tablets, string endpoint)
        {
            dynamic template = new
            {
                PersonId = personId,
                MedicationId = medicationId,
                Tablets = tablets
            };

            return await SendTrackingRequest(template, endpoint);
        }

        /// <summary>
        /// Send a tracking request for a single person/medication association
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="medicationId"></param>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        private async Task<PersonMedication> SendSingleMedicationTrackingRequest(int personId, int medicationId, string endpoint)
        {
            dynamic template = new
            {
                PersonId = personId,
                MedicationId = medicationId
            };

            return await SendTrackingRequest(template, endpoint);
        }

        /// <summary>
        /// Send a tracking request for all person/medication associations for a person
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        private async Task SendAllMedicationsTrackingRequest(int personId, string endpoint)
        {
            dynamic template = new
            {
                PersonId = personId
            };

            var data = Serialize(template);
            var baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            var route = $"{baseRoute}/{endpoint}";

            _ = await SendDirectAsync(route, data, HttpMethod.Put);
        }

        /// <summary>
        /// Send a request that returns a person medication object
        /// </summary>
        /// <param name="template"></param>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        private async Task<PersonMedication> SendTrackingRequest(object template, string endpoint)
        {
            var data = Serialize(template);
            var baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            var route = $"{baseRoute}/{endpoint}";

            string json = await SendDirectAsync(route, data, HttpMethod.Put);
            var association = Deserialize<PersonMedication>(json);

            return association;
        }
    }
}
