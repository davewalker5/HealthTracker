using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Measurements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthTracker.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("[controller]")]
    public class ReferenceDataController : Controller
    {
        private readonly IHealthTrackerFactory _factory;

        public ReferenceDataController(IHealthTrackerFactory factory)
            => _factory = factory;

        /// <summary>
        /// Return a list of blood pressure assessment bands
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("bloodpressurebands")]
        public async Task<ActionResult<IEnumerable<BloodPressureBand>>> ListBloodPressureAssessmentAsync()
        {
            var bands = await _factory.BloodPressureBands.ListAsync(x => true);

            if (bands == null)
            {
                return NoContent();
            }

            return bands;
        }

        /// <summary>
        /// Return a list of % SPO2 assessment bands
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("spo2bands")]
        public async Task<ActionResult<IEnumerable<BloodOxygenSaturationBand>>> ListBloodOxygenSaturationAssessmentAsync()
        {
            var bands = await _factory.BloodOxygenSaturationBands.ListAsync(x => true);

            if (bands == null)
            {
                return NoContent();
            }

            return bands;
        }

        /// <summary>
        /// Return a list of BMI assessment bands
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("bmibands")]
        public async Task<ActionResult<IEnumerable<BMIBand>>> ListBMIAssessmentAsync()
        {
            var bands = await _factory.BMIBands.ListAsync(x => true);

            if (bands == null)
            {
                return NoContent();
            }

            return bands;
        }
    }
}