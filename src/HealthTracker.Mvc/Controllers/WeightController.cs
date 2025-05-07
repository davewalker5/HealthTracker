using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Mvc.Entities;
using HealthTracker.Mvc.Interfaces;
using HealthTracker.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthTracker.Mvc.Controllers
{
    [Authorize]
    public class WeightController : Controller
    {
        private readonly IWeightMeasurementClient _client;
        private readonly IHealthTrackerApplicationSettings _settings;
        private readonly IFilterGenerator _filterGenerator;
        private readonly ILogger<WeightController> _logger;

        public WeightController(
            IWeightMeasurementClient client,
            IHealthTrackerApplicationSettings settings,
            IFilterGenerator filterGenerator,
            ILogger<WeightController> logger)
        {
            _client = client;
            _settings = settings;
            _filterGenerator = filterGenerator;
            _logger = logger;
        }

        /// <summary>
        /// Serve the weight measurements page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new WeightListViewModel
            {
                PageNumber = 1,
                Filters = await _filterGenerator.Create()
            };
            return View(model);
        }

        /// <summary>
        /// Handle POST events for page navigation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(WeightListViewModel model)
        {
            if (ModelState.IsValid)
            {
                int page = model.PageNumber;
                switch (model.Action)
                {
                    case ControllerActions.ActionPreviousPage:
                        page -= 1;
                        break;
                    case ControllerActions.ActionNextPage:
                        page += 1;
                        break;
                    default:
                        break;
                }

                // Need to clear model state here or the page number that was posted
                // is returned and page navigation doesn't work correctly. So, capture
                // and amend the page number, above, then apply it, below
                ModelState.Clear();

                // Retrieve the matching weight records
                _logger.LogDebug(
                    $"Retrieving page {page} of weight measurements for person with ID {model.Filters.PersonId}" +
                    $" in the date range {model.Filters.From:dd-MMM-yyyy} to {model.Filters.To:dd-MMM-yyyy}");

                var measurements = await _client.ListWeightMeasurementsAsync(
                    model.Filters.PersonId, model.Filters.From, model.Filters.To, page, _settings.ResultsPageSize);
                model.SetEntities(measurements, page, _settings.ResultsPageSize);

                _logger.LogDebug($"{measurements.Count} matching weight measurements retrieved");
            }

            // Populate the list of people and render the view
            await _filterGenerator.PopulatePersonList(model.Filters);
            return View(model);
        }
    }
}