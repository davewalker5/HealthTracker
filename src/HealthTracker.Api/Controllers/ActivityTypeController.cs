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
    public class ActivityTypeController : Controller
    {
        private readonly IHealthTrackerFactory _factory;

        public ActivityTypeController(IHealthTrackerFactory factory)
            => _factory = factory;

        /// <summary>
        /// Return activity type details given an ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<ActivityType>> GetActivityTypeByIdAsync(int id)
        {
            var activityType = await _factory.ActivityTypes.GetAsync(x => x.Id == id);

            if (activityType == null)
            {
                return NotFound();
            }

            return activityType;
        }

        /// <summary>
        /// Return a list of all activity types in the database
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{pageNumber}/{pageSize}")]
        public async Task<ActionResult<IEnumerable<ActivityType>>> ListAllActivityTypesAsync(int pageNumber, int pageSize)
        {
            var activityTypes = await _factory.ActivityTypes.ListAsync(x => true, pageNumber, pageSize);

            if (activityTypes == null)
            {
                return NoContent();
            }

            return activityTypes;
        }

        /// <summary>
        /// Add an activity type from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<ActivityType>> AddActivityTypeAsync([FromBody] ActivityType template)
        {
            var activityType = await _factory.ActivityTypes.AddAsync(template.Description, template.DistanceBased);
            return activityType;
        }

        /// <summary>
        /// Update an activity type from a template contained in the request body
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        public async Task<ActionResult<ActivityType>> UpdateActivityTypeAsync([FromBody] ActivityType template)
        {
            var activityType = await _factory.ActivityTypes.UpdateAsync(template.Id, template.Description, template.DistanceBased);
            return activityType;
        }

        /// <summary>
        /// Delete an activity type
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteActivityTypes(int id)
        {
            // Check the activity exists, first
            var activityType = await _factory.ActivityTypes.GetAsync(x => x.Id == id);
            if (activityType == null)
            {
                return NotFound();
            }

            // It does, so delete it
            await _factory.ActivityTypes.DeleteAsync(id);
            return Ok();
        }
    }
}
