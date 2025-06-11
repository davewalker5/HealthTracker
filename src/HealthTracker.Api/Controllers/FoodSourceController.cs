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
    public class FoodSourceController : Controller
    {
        private readonly IHealthTrackerFactory _factory;

        public FoodSourceController(IHealthTrackerFactory factory)
            => _factory = factory;

        /// <summary>
        /// Return food source details given an ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<FoodSource>> GetFoodSourceByIdAsync(int id)
        {
            var foodSource = await _factory.FoodSources.GetAsync(x => x.Id == id);

            if (foodSource == null)
            {
                return NotFound();
            }

            return foodSource;
        }

        /// <summary>
        /// Return a list of all food sources in the database
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{pageNumber}/{pageSize}")]
        public async Task<ActionResult<IEnumerable<FoodSource>>> ListAllFoodSourcesAsync(int pageNumber, int pageSize)
        {
            var foodSources = await _factory.FoodSources.ListAsync(x => true, pageNumber, pageSize);

            if (foodSources == null)
            {
                return NoContent();
            }

            return foodSources;
        }

        /// <summary>
        /// Add an food source from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<FoodSource>> AddFoodSourceAsync([FromBody] FoodSource template)
        {
            var foodSource = await _factory.FoodSources.AddAsync(template.Name);
            return foodSource;
        }

        /// <summary>
        /// Update an food source from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        public async Task<ActionResult<FoodSource>> UpdateFoodSourceAsync([FromBody] FoodSource template)
        {
            var foodSource = await _factory.FoodSources.UpdateAsync(template.Id, template.Name);
            return foodSource;
        }

        /// <summary>
        /// Delete an food source
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteFoodSource(int id)
        {
            // Check the source exists, first
            var foodSource = await _factory.FoodSources.GetAsync(x => x.Id == id);
            if (foodSource == null)
            {
                return NotFound();
            }

            // It does, so delete it
            await _factory.FoodSources.DeleteAsync(id);
            return Ok();
        }
    }
}
