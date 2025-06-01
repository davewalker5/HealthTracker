using HealthTracker.Client.Interfaces;
using HealthTracker.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HealthTracker.Enumerations.Enumerations;

namespace HealthTracker.Mvc.Controllers
{
    [Authorize]
    public class AlcoholController : HealthTrackerControllerBase
    {
        public delegate Task<decimal> UnitCalculator(decimal abv);

        protected readonly Dictionary<TempBeverageMeasure, UnitCalculator> _calculators = new();

        private readonly ILogger<AlcoholController> _logger;

        public AlcoholController(
            IAlcoholUnitCalculationsClient client,
            ILogger<AlcoholController> logger)
        {
            _calculators.Add(TempBeverageMeasure.Pint, client.UnitsPerPint);
            _calculators.Add(TempBeverageMeasure.LargeGlass, client.UnitsPerLargeGlass);
            _calculators.Add(TempBeverageMeasure.MediumGlass, client.UnitsPerMediumGlass);
            _calculators.Add(TempBeverageMeasure.SmallGlass, client.UnitsPerSmallGlass);
            _calculators.Add(TempBeverageMeasure.Shot, client.UnitsPerShot);
            _logger = logger;
        }

        /// <summary>
        /// Serve the alcohol calculation page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            var model = new AlcoholCalculationViewModel()
            {
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
            // If the model's nominally valid, perform some additional checks
            if (ModelState.IsValid)
            {
                // Check the measure isn't the default selection, "None"
                _logger.LogDebug($"Measure = {model.Measure}");
                if (model.Measure == TempBeverageMeasure.None)
                {
                    ModelState.AddModelError("Measure", "You must select a measure");
                }
            }

            // If the model's still valid, proceed with the calculation
            if (ModelState.IsValid)
            {
                // Request the import
                _logger.LogDebug($"Calculating the alcohol content of {model.Quantity} x {model.MeasureName} at {model.ABV} % ABV");
                var unitsPerMeasure = await _calculators[model.Measure](model.ABV);
                var units = model.Quantity * unitsPerMeasure;
                _logger.LogDebug($"Calculated units of alcohol = {units}");

                // Reset the model and set the result message
                ModelState.Clear();
                model.Result = $"{model.Quantity} x {model.MeasureName} at {model.ABV} % ABV contains {units} unit(s) of alcohol";
                model.Measure = TempBeverageMeasure.None;
                model.Quantity = 1;
                model.ABV = 0;
            }
            else
            {
                LogModelStateErrors(_logger);
            }

            return View(model);
        }
    }
}