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
    public class BeverageController : Controller
    {
        private readonly IHealthTrackerFactory _factory;

        public BeverageController(IHealthTrackerFactory factory)
            => _factory = factory;

        /// <summary>
        /// Return beverage details given an ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Beverage>> GetBeverageByIdAsync(int id)
        {
            var beverage = await _factory.Beverages.GetAsync(x => x.Id == id);

            if (beverage == null)
            {
                return NotFound();
            }

            return beverage;
        }

        /// <summary>
        /// Return a list of all beverages in the database
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{pageNumber}/{pageSize}")]
        public async Task<ActionResult<IEnumerable<Beverage>>> ListAllBeveragesAsync(int pageNumber, int pageSize)
        {
            var beverages = await _factory.Beverages.ListAsync(x => true, pageNumber, pageSize);

            if (beverages == null)
            {
                return NoContent();
            }

            return beverages;
        }

        /// <summary>
        /// Add an beverage from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Beverage>> AddBeverageAsync([FromBody] Beverage template)
        {
            var beverage = await _factory.Beverages.AddAsync(
                template.Name,
                template.TypicalABV,
                template.IsHydrating,
                template.IsAlcohol);
            return beverage;
        }

        /// <summary>
        /// Update an beverage from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        public async Task<ActionResult<Beverage>> UpdateBeverageAsync([FromBody] Beverage template)
        {
            var beverage = await _factory.Beverages.UpdateAsync(
                template.Id,
                template.Name,
                template.TypicalABV,
                template.IsHydrating,
                template.IsAlcohol);
            return beverage;
        }

        /// <summary>
        /// Delete an beverage
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteBeverages(int id)
        {
            // Check the activity exists, first
            var beverage = await _factory.Beverages.GetAsync(x => x.Id == id);
            if (beverage == null)
            {
                return NotFound();
            }

            // It does, so delete it
            await _factory.Beverages.DeleteAsync(id);
            return Ok();
        }
    }
}
