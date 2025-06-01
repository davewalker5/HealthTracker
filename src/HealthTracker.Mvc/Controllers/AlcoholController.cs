using HealthTracker.Client.Interfaces;
using HealthTracker.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HealthTracker.Enumerations.Enumerations;
using HealthTracker.Mvc.Interfaces;

namespace HealthTracker.Mvc.Controllers
{
    [Authorize]
    public class AlcoholController : HealthTrackerControllerBase
    {
        private readonly IAlcoholUnitCalculationsClient _client;
        private readonly IBeverageMeasureListGenerator _listGenerator;
        private readonly ILogger<AlcoholController> _logger;

        public AlcoholController(
            IAlcoholUnitCalculationsClient client,
            IBeverageMeasureListGenerator listGenerator,
            ILogger<AlcoholController> logger)
        {
            _client = client;
            _listGenerator = listGenerator;
            _logger = logger;
        }

        /// <summary>
        /// Serve the alcohol calculation page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new AlcoholCalculationViewModel()
            {
                Measures = await _listGenerator.Create(),
                Volume = 1,
                Quantity = 1,
                ABV = 0
            };
            return View(model);
        }

        /// <summary>
        /// Handle POST events to perform the calculation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(AlcoholCalculationViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Request the import
                _logger.LogDebug($"Calculating the alcohol content of {model.Quantity} x {model.Volume} ml at {model.ABV} % ABV");
                var units = await _client.CalculateUnitsAsync(model.ABV, model.Quantity * model.Volume);
                _logger.LogDebug($"Calculated units of alcohol = {units}");

                // Set the result message
                model.Result = $"{model.Quantity} x {model.Volume} ml at {model.ABV} % ABV contains {units} unit(s) of alcohol";
            }
            else
            {
                LogModelStateErrors(_logger);
            }

            // Populate the measures and render the view
            model.Measures = await _listGenerator.Create();
            return View(model);
        }
    }
}