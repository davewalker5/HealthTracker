using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Mvc.Entities;
using HealthTracker.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthTracker.Mvc.Controllers
{
    [Authorize]
    public class PersonController : Controller
    {
        private readonly IPersonClient _client;
        private readonly IHealthTrackerApplicationSettings _settings;
        private readonly ILogger<PersonController> _logger;

        public PersonController(IPersonClient client, IHealthTrackerApplicationSettings settings, ILogger<PersonController> logger)
        {
            _client = client;
            _settings = settings;
            _logger = logger;
        }

        /// <summary>
        /// Serve the current list of people
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var people = await _client.ListPeopleAsync(1, _settings.ResultsPageSize);
            _logger.LogDebug($"{people.Count} people loaded via the service");

            var model = new PersonListViewModel();
            model.SetEntities(people, 1, _settings.ResultsPageSize);
            return View(model);
        }

        /// <summary>
        /// Handle POST events for page navigation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(PersonListViewModel model)
        {
            if (ModelState.IsValid)
            {
                int page = model.PageNumber;
                switch (model.Action)
                {
                    case ControllerActions.ActionPreviousPage:
                        page -= 1;
                        break;
                    case ControllerActions.ActionNextPage:
                        page += 1;
                        break;
                    default:
                        break;
                }

                // Need to clear model state here or the page number that was posted
                // is returned and page navigation doesn't work correctly. So, capture
                // and amend the page number, above, then apply it, below
                ModelState.Clear();

                // Retrieve the matching airport records
                var people = await _client.ListPeopleAsync(page, _settings.ResultsPageSize);
                model.SetEntities(people, page, _settings.ResultsPageSize);
            }

            return View(model);
        }
    }
}
