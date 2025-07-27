using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Mvc.Entities;
using HealthTracker.Mvc.Interfaces;
using HealthTracker.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthTracker.Mvc.Controllers
{
    [Authorize]
    public class JobStatusController : HealthTrackerControllerBase
    {
        private readonly IJobStatusClient _client;
        private readonly IHealthTrackerApplicationSettings _settings;

        public JobStatusController(
            IJobStatusClient client,
            IHealthTrackerApplicationSettings settings,
            IPartialViewToStringRenderer renderer,
            ILogger<JobStatusController> logger) : base(renderer, logger)
        {
            _client = client;
            _settings = settings;
        }

        /// <summary>
            /// Serve the empty report page
            /// </summary>
            /// <returns></returns>
            [HttpGet]
        public IActionResult Index()
        {
            JobStatusViewModel model = new JobStatusViewModel
            {
                PageNumber = 1
            };
            return View(model);
        }

        /// <summary>
        /// Respond to a POST event triggering the report generation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(JobStatusViewModel model)
        {
            if (ModelState.IsValid)
            {
                int pageNumber = model.PageNumber;
                switch (model.Action)
                {
                    case ControllerActions.ActionPreviousPage:
                        pageNumber -= 1;
                        break;
                    case ControllerActions.ActionNextPage:
                        pageNumber += 1;
                        break;
                    case ControllerActions.ActionSearch:
                        pageNumber = 1;
                        break;
                    default:
                        break;
                }

                // Need to clear model state here or the page number that was posted
                // is returned and page navigation doesn't work correctly. So, capture
                // and amend the page number, above, then apply it, below
                ModelState.Clear();

                // Get the date and time
                DateTime start = !string.IsNullOrEmpty(model.From) ? DateTime.Parse(model.From) : DateTime.MinValue;
                DateTime end = !string.IsNullOrEmpty(model.To) ? DateTime.Parse(model.To) : DateTime.MaxValue;

                // Retrieve the matching report records
                var records = await _client.ListAsync(start, end, pageNumber, _settings.ResultsPageSize);
                model.SetEntities(records, pageNumber, _settings.ResultsPageSize);
            }
            else
            {
                LogModelState();
            }

            return View(model);
        }
    }
}