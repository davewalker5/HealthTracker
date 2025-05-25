using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace HealthTracker.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("[controller]")]
    public class JobStatusController : Controller
    {
        private const string DateTimeFormat = "yyyy-MM-dd H:mm:ss";

        private readonly IHealthTrackerFactory _factory;

        public JobStatusController(IHealthTrackerFactory factory)
            => _factory = factory;

        /// <summary>
        /// Return a list of job statuses
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{start}/{end}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<JobStatus>>> ListJobStatusesAsync(string start, string end, int pageNumber, int pageSize)
        {
            // Decode the start and end date and convert them to dates
            DateTime startDate = DateTime.ParseExact(HttpUtility.UrlDecode(start), DateTimeFormat, null);
            DateTime endDate = DateTime.ParseExact(HttpUtility.UrlDecode(end), DateTimeFormat, null);

            // Get the report content
            var results = await _factory.JobStatuses
                                        .ListAsync(x => (x.Start >= startDate) && ((x.End == null) || (x.End <= endDate)),
                                                   pageNumber,
                                                   pageSize);

            if (!results.Any())
            {
                return NoContent();
            }

            // Return the results
            return results;
        }
    }
}