using HealthTracker.Client.Interfaces;
using HealthTracker.Mvc.Entities;
using HealthTracker.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HealthTracker.Enumerations.Enumerations;

namespace HealthTracker.Mvc.Controllers
{
    [Authorize]
    public class ExportController : DataExchangeControllerBase
    {
        private readonly IPersonClient _personClient;

        public ExportController(
            IPersonClient personClient,
            IBloodGlucoseMeasurementClient bloodGlucoseMeasurementClient,
            IBloodOxygenSaturationMeasurementClient bloodOxygenSaturationMeasurementClient,
            IBloodPressureMeasurementClient bloodPressurementMeasurementClient,
            IExerciseMeasurementClient exerciseMeasurementClient,
            IWeightMeasurementClient weightMeasurementClient,
            IBeverageConsumptionMeasurementClient beverageConsumptionMeasurementClient,
            IFoodItemClient foodItemClient,
            IMealClient mealClient,
            IMealConsumptionMeasurementClient mealConsumptionMeasurementClient,
            ILogger<WeightController> logger) : base(
                bloodGlucoseMeasurementClient,
                bloodOxygenSaturationMeasurementClient,
                bloodPressurementMeasurementClient,
                exerciseMeasurementClient,
                weightMeasurementClient,
                beverageConsumptionMeasurementClient,
                foodItemClient,
                mealClient,
                mealConsumptionMeasurementClient,
                logger
            )
        {
            _personClient = personClient;
        }

        /// <summary>
        /// Serve the measurements export page
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="exportType"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ExportMeasurements(int personId, DateTime start, DateTime end, DataExchangeType exportType)
        {
            _logger.LogDebug($"Rendering export view: Person ID = {personId}, From = {start}, To = {end}, Type = {exportType}");

            var people = await _personClient.ListAsync(1, int.MaxValue);

            var model = new ExportMeasurementsViewModel
            {
                PersonId = personId,
                From = FromDate(start),
                To = ToDate(end),
                DataExchangeType = exportType,
                Message = "",
                PersonName = people.First(x => x.Id == personId).Name
            };

            return View(model);
        }

        /// <summary>
        /// Handle POST events on the measurements export page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExportMeasurements(ExportMeasurementsViewModel model)
        {
            if (model.Action == ControllerActions.ActionCancel)
            {
                var controllerName = ControllerName(model.DataExchangeType);
                _logger.LogDebug($"Cancelling export and returning to the {controllerName} Index action");
                return RedirectToAction("Index", controllerName, new
                    {
                        personId = model.PersonId,
                        start = model.From,
                        end = model.To
                    });
            }

            if (ModelState.IsValid)
            {
                _logger.LogDebug(
                    $"Requesting export of data: Person ID = {model.PersonId}, " +
                    $"From = {model.From}, " +
                    $"To = {model.To}, " +
                    $"Type = {model.DataExchangeType}, " +
                    $"File Name = {model.FileName}");
                await ExportAsync(model.DataExchangeType, model.PersonId, model.From, model.To, model.FileName);
                ModelState.Clear();
                model.Message = $"Data export to {model.FileName} has been requested";
                model.FileName = "";
                model.BackButtonLabel = "< Back";
            }
            else
            {
                LogModelState(_logger);
            }

            // Populate the person name and render the view
            var people = await _personClient.ListAsync(1, int.MaxValue);
            model.PersonName = people.First(x => x.Id == model.PersonId).Name;
            return View(model);
        }

        /// <summary>
        /// Serve the generic data export page
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="exportType"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Export(DataExchangeType exportType)
        {
            _logger.LogDebug($"Rendering export view: Export Type = {exportType}");

            var model = new ExportViewModel
            {
                DataExchangeType = exportType,
                Message = ""
            };

            return View(model);
        }

        /// <summary>
        /// Handle POST events on the generic data export page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Export(ExportViewModel model)
        {
            if (model.Action == ControllerActions.ActionCancel)
            {
                var controllerName = ControllerName(model.DataExchangeType);
                _logger.LogDebug($"Cancelling export and returning to the {controllerName} Index action");
                return RedirectToAction("Index", controllerName);
            }

            if (ModelState.IsValid)
            {
                _logger.LogDebug(
                    $"Requesting export of data: Type = {model.DataExchangeType}, File Name = {model.FileName}");
                await ExportAsync(model.DataExchangeType, model.FileName);
                ModelState.Clear();
                model.Message = $"Data export to {model.FileName} has been requested";
                model.FileName = "";
                model.BackButtonLabel = "< Back";
            }
            else
            {
                LogModelState(_logger);
            }

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