using HealthTracker.Entities.Food;
using HealthTracker.Entities.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthTracker.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("[controller]")]
    public class NutritionalValueController : Controller
    {
        private readonly IHealthTrackerFactory _factory;

        public NutritionalValueController(IHealthTrackerFactory factory)
            => _factory = factory;

        /// <summary>
        /// Return nutritional value details given an ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<NutritionalValue>> GetNutritionalValueByIdAsync(int id)
        {
            var nutritionalvalue = await _factory.NutritionalValues.GetAsync(x => x.Id == id);

            if (nutritionalvalue == null)
            {
                return NotFound();
            }

            return nutritionalvalue;
        }

        /// <summary>
        /// Add a nutritional value from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<NutritionalValue>> AddNutritionalValueAsync([FromBody] NutritionalValue template)
        {
            var nutritionalvalue = await _factory.NutritionalValues.AddAsync(
                template.Calories,
                template.Fat,
                template.SaturatedFat,
                template.Protein,
                template.Carbohydrates,
                template.Sugar,
                template.Fibre
            );

            return nutritionalvalue;
        }

        /// <summary>
        /// Update a nutritional value from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        public async Task<ActionResult<NutritionalValue>> UpdateNutritionalValueAsync([FromBody] NutritionalValue template)
        {
            var nutritionalvalue = await _factory.NutritionalValues.UpdateAsync(
                template.Id,
                template.Calories,
                template.Fat,
                template.SaturatedFat,
                template.Protein,
                template.Carbohydrates,
                template.Sugar,
                template.Fibre
            );

            return nutritionalvalue;
        }

        /// <summary>
        /// Delete a nutritional value
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteNutritionalValue(int id)
        {
            // Check the nutritionalvalue exists, first
            var nutrition = await _factory.NutritionalValues.GetAsync(x => x.Id == id);
            if (nutrition == null)
            {
                return NotFound();
            }

            // It does, so delete it
            await _factory.NutritionalValues.DeleteAsync(id);
            return Ok();
        }
    }
}
