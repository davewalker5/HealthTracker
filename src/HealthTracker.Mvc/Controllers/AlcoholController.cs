using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Measurements;
using HealthTracker.Mvc.Entities;
using HealthTracker.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthTracker.Mvc.Controllers
{
    [Authorize]
    public class AlcoholController : HealthTrackerControllerBase
    {
        public delegate Task<decimal> UnitCalculator(decimal abv);

        protected readonly Dictionary<AlcoholPortionSize, UnitCalculator> _calculators = new();

        private readonly ILogger<AlcoholController> _logger;

        public AlcoholController(
            IAlcoholUnitCalculationsClient client,
            ILogger<AlcoholController> logger)
        {
            _calculators.Add(AlcoholPortionSize.Pint, client.UnitsPerPint);
            _calculators.Add(AlcoholPortionSize.LargeGlass, client.UnitsPerLargeGlass);
            _calculators.Add(AlcoholPortionSize.MediumGlass, client.UnitsPerMediumGlass);
            _calculators.Add(AlcoholPortionSize.SmallGlass, client.UnitsPerSmallGlass);
            _calculators.Add(AlcoholPortionSize.Shot, client.UnitsPerShot);
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
                // Check the portion size isn't the default selection, "None"
                _logger.LogDebug($"Portion size = {model.Portion}");
                if (model.Portion == AlcoholPortionSize.None)
                {
                    ModelState.AddModelError("Portion", "You must select a portion size");
                }
            }

            // If the model's still valid, proceed with the calculation
            if (ModelState.IsValid)
            {
                // Request the import
                _logger.LogDebug($"Calculating the alcohol content of {model.Quantity} x {model.PortionSizeName} at {model.ABV} % ABV");
                var unitsPerPortion = await _calculators[model.Portion](model.ABV);
                var units = model.Quantity * unitsPerPortion;
                _logger.LogDebug($"Calculated units of alcohol = {units}");

                // Reset the model and set the result message
                ModelState.Clear();
                model.Result = $"{model.Quantity} x {model.PortionSizeName} at {model.ABV} % ABV contains {units} unit(s) of alcohol";
                model.Portion = AlcoholPortionSize.None;
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