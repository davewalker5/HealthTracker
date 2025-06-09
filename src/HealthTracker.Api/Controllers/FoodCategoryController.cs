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
    public class FoodCategoryController : Controller
    {
        private readonly IHealthTrackerFactory _factory;

        public FoodCategoryController(IHealthTrackerFactory factory)
            => _factory = factory;

        /// <summary>
        /// Return food category details given an ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<FoodCategory>> GetFoodCategoryByIdAsync(int id)
        {
            var foodSource = await _factory.FoodCategories.GetAsync(x => x.Id == id);

            if (foodSource == null)
            {
                return NotFound();
            }

            return foodSource;
        }

        /// <summary>
        /// Return a list of all food categories in the database
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{pageNumber}/{pageSize}")]
        public async Task<ActionResult<IEnumerable<FoodCategory>>> ListAllFoodCategoriesAsync(int pageNumber, int pageSize)
        {
            var foodSources = await _factory.FoodCategories.ListAsync(x => true, pageNumber, pageSize);

            if (foodSources == null)
            {
                return NoContent();
            }

            return foodSources;
        }

        /// <summary>
        /// Add a food category from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<FoodCategory>> AddFoodCategoryAsync([FromBody] FoodCategory template)
        {
            var foodSource = await _factory.FoodCategories.AddAsync(template.Name);
            return foodSource;
        }

        /// <summary>
        /// Update a food category from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        public async Task<ActionResult<FoodCategory>> UpdateFoodCategoryAsync([FromBody] FoodCategory template)
        {
            var foodSource = await _factory.FoodCategories.UpdateAsync(template.Id, template.Name);
            return foodSource;
        }

        /// <summary>
        /// Delete a food category
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteFoodCategory(int id)
        {
            // Check the category exists, first
            var foodSource = await _factory.FoodCategories.GetAsync(x => x.Id == id);
            if (foodSource == null)
            {
                return NotFound();
            }

            // It does, so delete it
            await _factory.FoodCategories.DeleteAsync(id);
            return Ok();
        }
    }
}
