using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Measurements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace HealthTracker.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("[controller]")]
    public class BeverageConsumptionMeasurementController : Controller
    {
        private const string DateTimeFormat = "yyyy-MM-dd H:mm:ss";
        private readonly IHealthTrackerFactory _factory;
        private readonly ILogger<BeverageConsumptionMeasurementController> _logger;

        public BeverageConsumptionMeasurementController(IHealthTrackerFactory factory, ILogger<BeverageConsumptionMeasurementController> logger)
        {
            _factory = factory;
            _logger = logger;
        }

        /// <summary>
        /// Return a single measurement given its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<BeverageConsumptionMeasurement>> Get(int id)
        {
            var measurements = await _factory.BeverageConsumptionMeasurements.ListAsync(x => x.Id == id, 1, int.MaxValue);
            await PopulateAncillaryProperties(measurements);
            return measurements.First();
        }

        /// <summary>
        /// Return a list of all measurements for a person
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{personId}/{from}/{to}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<IEnumerable<BeverageConsumptionMeasurement>>> ListBeverageConsumptionMeasurementsForPersonAsync(int personId, string from, string to, int pageNumber, int pageSize)
        {
            // Decode the start and end date and convert them to dates
            DateTime fromDate = DateTime.ParseExact(HttpUtility.UrlDecode(from), DateTimeFormat, null);
            DateTime toDate = DateTime.ParseExact(HttpUtility.UrlDecode(to), DateTimeFormat, null);

            // Retrieve matching results
            var measurements = await _factory.BeverageConsumptionMeasurements.ListAsync(
                x => (x.PersonId == personId) && (x.Date >= fromDate) && (x.Date <= toDate),
                pageNumber,
                pageSize);

            if (measurements == null)
            {
                return NoContent();
            }

            // Populate ancillary properties on each measurement
            await PopulateAncillaryProperties(measurements);
            return measurements;
        }

        /// <summary>
        /// Add a measurement from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<BeverageConsumptionMeasurement>> AddBeverageConsumptionMeasurementAsync([FromBody] BeverageConsumptionMeasurement template)
        {
            // Calculate the volume, if it's not specified
            var volume = template.Volume <= 0 ?
                _factory.AlcoholUnitsCalculator.CalculateVolume(template.Measure, template.Quantity) :
                template.Volume;

            // Add the measurement
            var measurement = await _factory.BeverageConsumptionMeasurements.AddAsync(
                template.PersonId,
                template.BeverageId,
                template.Date,
                template.Measure,
                template.Quantity,
                volume,
                template.ABV
            );

            // Populate ancillary properties on the measurement
            await PopulateAncillaryProperties([measurement]);
            return measurement;
        }

        /// <summary>
        /// Update a measurement from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        public async Task<ActionResult<BeverageConsumptionMeasurement>> UpdateBeverageConsumptionMeasurementAsync([FromBody] BeverageConsumptionMeasurement template)
        {
            // Calculate the volume, if it's not specified
            var volume = template.Volume <= 0 ?
                _factory.AlcoholUnitsCalculator.CalculateVolume(template.Measure, template.Quantity) :
                template.Volume;

            // Update the measurement
            var measurement = await _factory.BeverageConsumptionMeasurements.UpdateAsync(
                template.Id,
                template.PersonId,
                template.BeverageId,
                template.Date,
                template.Measure,
                template.Quantity,
                volume,
                template.ABV
            );

            // Populate ancillary properties on the measurement
            await PopulateAncillaryProperties([measurement]);
            return measurement;
        }

        /// <summary>
        /// Delete a measurement
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteBeverageConsumptionMeasurements(int id)
        {
            // Check the measurement exists, first
            var measurements = await _factory.BeverageConsumptionMeasurements.ListAsync(x => x.Id == id, 1, int.MaxValue);
            if (!measurements.Any())
            {
                return NotFound();
            }

            // It does, so delete it
            await _factory.BeverageConsumptionMeasurements.DeleteAsync(id);
            return Ok();
        }

        /// <summary>
        /// Populate ancillary properties on a set of measurements
        /// </summary>
        /// <param name="id"></param>
        private async Task PopulateAncillaryProperties(IEnumerable<BeverageConsumptionMeasurement> measurements)
        {
            // Calculate the units of alcohol for each record
            _factory.AlcoholUnitsCalculator.CalculateUnits(measurements);

            // Retrieve a list of beverages and populate the beverage name for each record
            var beverages = await _factory.Beverages.ListAsync(x => true, 1, int.MaxValue);
            foreach (var measurement in measurements)
            {
                measurement.Beverage = beverages.FirstOrDefault(x => x.Id == measurement.BeverageId)?.Name ?? "";
            }
        }
    }
}
