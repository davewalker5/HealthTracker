using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Food;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthTracker.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("[controller]")]
    public class BeverageMeasureController : Controller
    {
        private readonly IHealthTrackerFactory _factory;

        public BeverageMeasureController(IHealthTrackerFactory factory)
            => _factory = factory;

        /// <summary>
        /// Return activity type details given an ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<BeverageMeasure>> GetBeverageMeasureByIdAsync(int id)
        {
            var beverageMeasure = await _factory.BeverageMeasures.GetAsync(x => x.Id == id);

            if (beverageMeasure == null)
            {
                return NotFound();
            }

            return beverageMeasure;
        }

        /// <summary>
        /// Return a list of all activity types in the database
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{pageNumber}/{pageSize}")]
        public async Task<ActionResult<IEnumerable<BeverageMeasure>>> ListAllBeverageMeasuresAsync(int pageNumber, int pageSize)
        {
            var beverageMeasures = await _factory.BeverageMeasures.ListAsync(x => true, pageNumber, pageSize);

            if (beverageMeasures == null)
            {
                return NoContent();
            }

            return beverageMeasures;
        }

        /// <summary>
        /// Add an activity type from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<BeverageMeasure>> AddBeverageMeasureAsync([FromBody] BeverageMeasure template)
        {
            var beverageMeasure = await _factory.BeverageMeasures.AddAsync(template.Name, template.Volume);
            return beverageMeasure;
        }

        /// <summary>
        /// Update an activity type from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        public async Task<ActionResult<BeverageMeasure>> UpdateBeverageMeasureAsync([FromBody] BeverageMeasure template)
        {
            var beverageMeasure = await _factory.BeverageMeasures.UpdateAsync(template.Id, template.Name, template.Volume);
            return beverageMeasure;
        }

        /// <summary>
        /// Delete an activity type
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteBeverageMeasures(int id)
        {
            // Check the measure exists, first
            var beverageMeasure = await _factory.BeverageMeasures.GetAsync(x => x.Id == id);
            if (beverageMeasure == null)
            {
                return NotFound();
            }

            // It does, so delete it
            await _factory.BeverageMeasures.DeleteAsync(id);
            return Ok();
        }
    }
}
