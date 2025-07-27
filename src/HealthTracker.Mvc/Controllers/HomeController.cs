using HealthTracker.Mvc.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Mvc.Interfaces;
using HealthTracker.Mvc.Entities;
using Microsoft.AspNetCore.Diagnostics;

namespace HealthTracker.Mvc.Controllers
{
    [Authorize]
    public class HomeController : HealthTrackerControllerBase
    {
        private readonly IWeightMeasurementClient _weightClient;
        private readonly IExerciseMeasurementClient _exerciseClient;
        private readonly IBeverageConsumptionMeasurementClient _beverageConsumptionMeasurementClient;
        private readonly IHealthTrackerApplicationSettings _settings;
        private readonly IFilterGenerator _filterGenerator;
        private readonly IViewModelBuilder _builder;

        public HomeController(
            IWeightMeasurementClient weightClient,
            IExerciseMeasurementClient exerciseClient,
            IBeverageConsumptionMeasurementClient beverageConsumptionMeasurementClient,
            IHealthTrackerApplicationSettings settings,
            IFilterGenerator filterGenerator,
            IViewModelBuilder builder,
            IPartialViewToStringRenderer renderer,
            ILogger<PersonMedicationController> logger) : base(renderer, logger)
        {
            _weightClient = weightClient;
            _exerciseClient = exerciseClient;
            _beverageConsumptionMeasurementClient = beverageConsumptionMeasurementClient;
            _settings = settings;
            _filterGenerator = filterGenerator;
            _builder = builder;
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

            // Calculate the rolling average weight for the default period
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

            // Calculate the rolling total alcohol consumption for the default period
            var summary = await _beverageConsumptionMeasurementClient.CalculateTotalAlcoholicAsync(model.Filters.PersonId, from, to);
            model.TotalAlcoholConsumption = new BeverageConsumptionSummaryListViewModel()
            {
                Summaries = summary != null ? [summary] : null
            };

            // Calculate the rolling total hydrating drink consumption for the default period
            var measurements = await _beverageConsumptionMeasurementClient.CalculateDailyTotalHydratingAsync(model.Filters.PersonId, from, to);
            if (measurements != null)
            {
                model.HydratingBeverageConsumption = await _builder.CreateBeverageConsumptionListViewModel(model.Filters.PersonId, 0, measurements, from, to, "", ViewFlags.None);
            }

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
            // Construct the error view model
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var model = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                ErrorMessage = exceptionFeature != null ? exceptionFeature.Error.Message : ""
            };

            return View(model);
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
