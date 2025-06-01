using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Measurements;
using Microsoft.Extensions.Logging;

namespace HealthTracker.Client.ApiClient
{
    public class ReferenceDataClient : HealthTrackerClientBase, IReferenceDataClient
    {
        private const string RouteKey = "ReferenceData";

        public ReferenceDataClient(
            IHealthTrackerHttpClient client,
            IHealthTrackerApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ILogger<ReferenceDataClient> logger)
            : base(client, settings, tokenProvider, logger)
        {
        }

        /// <summary>
        /// Return a list of blood pressure assessment bands
        /// </summary>
        /// <returns></returns>
        public async Task<List<BloodPressureBand>> ListBloodPressureAssessmentBandsAsync()
        {
            // Request a list of activity types
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            var route = $"{baseRoute}/bloodpressurebands";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // The returned JSON will be empty if there are no activity types in the database
            List<BloodPressureBand> bands = !string.IsNullOrEmpty(json) ? Deserialize<List<BloodPressureBand>>(json) : null;
            return bands;
        }

        /// <summary>
        /// Return a list of % SPO2 assessment bands
        /// </summary>
        /// <returns></returns>
        public async Task<List<BloodOxygenSaturationBand>> ListBloodOxygenSaturationAssessmentBandsAsync()
        {
            // Request a list of activity types
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            var route = $"{baseRoute}/spo2bands";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // The returned JSON will be empty if there are no activity types in the database
            List<BloodOxygenSaturationBand> bands = !string.IsNullOrEmpty(json) ? Deserialize<List<BloodOxygenSaturationBand>>(json) : null;
            return bands;
        }

        /// <summary>
        /// Return a list of BMI assessment bands
        /// </summary>
        /// <returns></returns>
        public async Task<List<BMIBand>> ListBMIAssessmentBandsAsync()
        {
            // Request a list of activity types
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            var route = $"{baseRoute}/bmibands";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // The returned JSON will be empty if there are no activity types in the database
            List<BMIBand> bands = !string.IsNullOrEmpty(json) ? Deserialize<List<BMIBand>>(json) : null;
            return bands;
        }
    }
}
