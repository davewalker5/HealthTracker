using HealthTracker.Mvc.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Mvc.Interfaces;
using HealthTracker.Mvc.Entities;

namespace HealthTracker.Mvc.Controllers
{
    [Authorize]
    public class HomeController : HealthTrackerControllerBase
    {
        private readonly IWeightMeasurementClient _weightClient;
        private readonly IExerciseMeasurementClient _exerciseClient;
        private readonly IHealthTrackerApplicationSettings _settings;
        private readonly IFilterGenerator _filterGenerator;
        private readonly IViewModelBuilder _builder;
        private readonly ILogger<PersonMedicationController> _logger;

        public HomeController(
            IWeightMeasurementClient weightClient,
            IExerciseMeasurementClient exerciseClient,
            IHealthTrackerApplicationSettings settings,
            IFilterGenerator filterGenerator,
            IViewModelBuilder builder,
            ILogger<PersonMedicationController> logger)
        {
            _weightClient = weightClient;
            _exerciseClient = exerciseClient;
            _settings = settings;
            _filterGenerator = filterGenerator;
            _builder = builder;
            _logger = logger;
        }

        /// <summary>
        /// Serve the home page
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public async Task<IActionResult> Index(int personId = 0)
        {
            var model = new HomeViewModel()
            {
                Filters = await _filterGenerator.Create(personId, ViewFlags.None),
            };
            return View(model);
        }

        /// <summary>
        /// Handle POST events to display the summary
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(HomeViewModel model)
        {
            // Calculate the date range for the last "N" days
            var to = ToDate(DateTime.Today);
            var from = FromDate(to).AddDays(-_settings.DefaultTimePeriodDays);
            _logger.LogDebug($"Retrieving summary data for person with ID {model.Filters.PersonId} from {from} to {to}");

            // Calculate a 7-day rolling average weight and use that as the list of measurements in the weight
            // measurements view model
            var rollingAverageWeight = await _weightClient.CalculateAverageAsync(model.Filters.PersonId, _settings.DefaultTimePeriodDays);
            if (rollingAverageWeight != null)
            {
                model.WeightMeasurements = await _builder.CreateWeightListViewModel(model.Filters.PersonId, 0, [rollingAverageWeight], from, to, "", ViewFlags.None);
            }

            // Retrieve the exercise summary for the period
            model.ExerciseSummaries = new ExerciseSummaryListViewModel()
            {
                Summaries = await _exerciseClient.SummariseAsync(model.Filters.PersonId, 0, from, to)
            };
            
            // Retrieve current medication details
            model.PersonMedications = await _builder.CreatePersonMedicationListViewModel(model.Filters.PersonId, "", ViewFlags.None);

            // Populate the list of people and render the view
            await _filterGenerator.PopulatePersonList(model.Filters);
            return View(model);
        }

        /// <summary>
        /// Serve the error page
        /// </summary>
        /// <returns></returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// Convert a "to" date supplied by the date pickers, and therefore have a time of 00:00:00, to
        /// a date that will capture all data up to midnight on the specified date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private static DateTime FromDate(DateTime date)
            => new(date.Year, date.Month, date.Day, 0, 0, 0);

        /// <summary>
        /// Convert a "to" date supplied by the date pickers, and therefore have a time of 00:00:00, to
        /// a date that will capture all data up to midnight on the specified date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private static DateTime ToDate(DateTime date)
            => new(date.Year, date.Month, date.Day, 23, 59, 59);
    }
}
