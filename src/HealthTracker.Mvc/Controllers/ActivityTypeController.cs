using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Measurements;
using HealthTracker.Mvc.Entities;
using HealthTracker.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthTracker.Mvc.Controllers
{
    [Authorize]
    public class ActivityTypeController : ReferenceDataControllerBase<ActivityTypeListViewModel, ActivityType>
    {
        private readonly IActivityTypeClient _client;

        private readonly ILogger<ActivityTypeController> _logger;

        public ActivityTypeController(
            IActivityTypeClient client,
            IHealthTrackerApplicationSettings settings,
            ILogger<ActivityTypeController> logger) : base(settings)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Serve the current list of activity types
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await CreateActivityTypeListViewModel("");
            return View(model);
        }

        /// <summary>
        /// Handle POST events for page navigation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ActivityTypeListViewModel model)
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

                // Retrieve the matching records
                var activityTypes = await _client.ListAsync(page, _settings.ResultsPageSize);
                model.SetEntities(activityTypes, page, _settings.ResultsPageSize);
            }
            else
            {
                LogModelState(_logger);
            }

            return View(model);
        }

        /// <summary>
        /// Serve the page to add a new activity type
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Add()
        {
            var model = new AddActivityTypeViewModel();
            return View(model);
        }

        /// <summary>
        /// Handle POST events to save new activity types
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddActivityTypeViewModel model)
        {
            IActionResult result;

            if (model.Action == ControllerActions.ActionCancel)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                _logger.LogDebug($"Adding activity type: Description = {model.ActivityType.Description}, Distance Based = {model.ActivityType.DistanceBased}");
                var activityType = await _client.AddAsync(model.ActivityType.Description, model.ActivityType.DistanceBased);

                result = CreateListResult(activityType, $"{activityType.Description} successfully added");
            }
            else
            {
                LogModelState(_logger);
                result = View(model);
            }

            return result;
        }

        /// <summary>
        /// Serve the page to edit an existing activity type
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var activityTypes = await _client.ListAsync(1, int.MaxValue);
            var plural = activityTypes.Count == 1 ? "" : "s";
            _logger.LogDebug($"{activityTypes.Count} activity type{plural} loaded via the service");

            var activityType = activityTypes.First(x => x.Id == id);
            _logger.LogDebug($"Activity type with ID {id} identified for editing");

            var model = new EditActivityTypeViewModel();
            model.ActivityType = activityType;
            return View(model);
        }

        /// <summary>
        /// Handle POST events to update an existing activity type
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditActivityTypeViewModel model)
        {
            IActionResult result;

            if (model.Action == ControllerActions.ActionCancel)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                _logger.LogDebug($"Updating activity type: Id = {model.ActivityType.Id}, Description = {model.ActivityType.Description}, Distance Based = {model.ActivityType.DistanceBased}");
                var activityType = await _client.UpdateAsync(model.ActivityType.Id, model.ActivityType.Description, model.ActivityType.DistanceBased);

                result = CreateListResult(activityType, $"{activityType.Description} successfully updated");
            }
            else
            {
                LogModelState(_logger);
                result = View(model);
            }

            return result;
        }

        /// <summary>
        /// Handle POST events to delete an existing activity type
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            // Delete the item
            _logger.LogDebug($"Deleting activity type: ID = {id}");
            await _client.DeleteAsync(id);

            // Return the list view with an empty list of items
            var message = $"Activity type with ID {id} successfully deleted";
            var model = await CreateActivityTypeListViewModel(message);
            return View("Index", model);
        }

        /// <summary>
        /// Build the activity type list view model
        /// </summary>
        /// <returns></returns>
        private async Task<ActivityTypeListViewModel> CreateActivityTypeListViewModel(string message)
        {
            // Get the list of current activity types
            var activityTypes = await _client.ListAsync(1, _settings.ResultsPageSize);
            var plural = activityTypes.Count == 1 ? "" : "s";
            _logger.LogDebug($"{activityTypes.Count} activity type{plural} loaded via the service");

            // Construct the view model and serve the page
            var model = new ActivityTypeListViewModel()
            {
                Message = message
            };
            model.SetEntities(activityTypes, 1, _settings.ResultsPageSize);
            return model;
        }
    }
}
