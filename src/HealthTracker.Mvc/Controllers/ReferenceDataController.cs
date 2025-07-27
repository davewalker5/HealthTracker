using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Measurements;
using HealthTracker.Mvc.Interfaces;
using HealthTracker.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthTracker.Mvc.Controllers
{
    [Authorize]
    public class ReferenceDataController : ReferenceDataControllerBase<ActivityTypeListViewModel, ActivityType>
    {
        private readonly IReferenceDataClient _client;

        public ReferenceDataController(
            IReferenceDataClient client,
            IHealthTrackerApplicationSettings settings,
            IPartialViewToStringRenderer renderer,
            ILogger<ReferenceDataController> logger)
            : base(settings, renderer, logger)
        {
            _client = client;
        }

        /// <summary>
        /// Serve list of blood pressure assessment bands
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> BloodPressureBands()
        {
            // Get the list of bands
            var bands = await _client.ListBloodPressureAssessmentBandsAsync();
            var plural = bands.Count == 1 ? "" : "s";
            _logger.LogDebug($"{bands.Count} blood pressure assessment band{plural} loaded via the service");

            // Construct the view model and serve the page
            var model = new BloodPressureBandListViewModel() { Bands = bands };
            return View(model);
        }

        /// <summary>
        /// Serve list of blood pressure assessment bands
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> BloodOxygenSaturationBands()
        {
            // Get the list of bands
            var bands = await _client.ListBloodOxygenSaturationAssessmentBandsAsync();
            var plural = bands.Count == 1 ? "" : "s";
            _logger.LogDebug($"{bands.Count} % SPO2 assessment band{plural} loaded via the service");

            // Construct the view model and serve the page
            var model = new BloodOxygenSaturationBandListViewModel() { Bands = bands };
            return View(model);
        }

        /// <summary>
        /// Serve list of BMI assessment bands
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> BMIBands()
        {
            // Get the list of bands
            var bands = await _client.ListBMIAssessmentBandsAsync();
            var plural = bands.Count == 1 ? "" : "s";
            _logger.LogDebug($"{bands.Count} BMI assessment band{plural} loaded via the service");

            // Construct the view model and serve the page
            var model = new BMIBandListViewModel() { Bands = bands };
            return View(model);
        }
    }
}