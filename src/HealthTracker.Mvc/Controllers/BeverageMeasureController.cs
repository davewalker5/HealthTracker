using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Food;
using HealthTracker.Mvc.Entities;
using HealthTracker.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthTracker.Mvc.Controllers
{
    [Authorize]
    public class BeverageMeasureController : ReferenceDataControllerBase<BeverageMeasureListViewModel, BeverageMeasure>
    {
        private readonly IBeverageMeasureClient _client;

        private readonly ILogger<BeverageMeasureController> _logger;

        public BeverageMeasureController(
            IBeverageMeasureClient client,
            IHealthTrackerApplicationSettings settings,
            ILogger<BeverageMeasureController> logger) : base(settings)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Serve the current list of beverage measures
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await CreateBeverageMeasureListViewModel("");
            return View(model);
        }

        /// <summary>
        /// Handle POST events for page navigation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(BeverageMeasureListViewModel model)
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
                var measures = await _client.ListAsync(page, _settings.ResultsPageSize);
                model.SetEntities(measures, page, _settings.ResultsPageSize);
            }
            else
            {
                LogModelState(_logger);
            }

            return View(model);
        }
        
        /// <summary>
        /// Serve the page to add a new beverage measure
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Add()
        {
            var model = new AddBeverageMeasureViewModel();
            return View(model);
        }

        /// <summary>
        /// Handle POST events to save new beverage measures
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddBeverageMeasureViewModel model)
        {
            IActionResult result;

            if (model.Action == ControllerActions.ActionCancel)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                _logger.LogDebug($"Adding beverage measure: Name = {model.Measure.Name}, Volume = {model.Measure.Volume}");
                var measure = await _client.AddAsync(model.Measure.Name, model.Measure.Volume);

                result = CreateListResult(measure, $"{measure.Name} successfully added");
            }
            else
            {
                LogModelState(_logger);
                result = View(model);
            }

            return result;
        }
        
        /// <summary>
        /// Serve the page to edit an existing beverage measure
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var measures = await _client.ListAsync(1, int.MaxValue);
            var plural = measures.Count == 1 ? "" : "s";
            _logger.LogDebug($"{measures.Count} beverage measure{plural} loaded via the service");

            var measure = measures.First(x => x.Id == id);
            _logger.LogDebug($"Activity type with ID {id} identified for editing");

            var model = new EditBeverageMeasureViewModel();
            model.Measure = measure;
            return View(model);
        }

        /// <summary>
        /// Handle POST events to update an existing beverage measure
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditBeverageMeasureViewModel model)
        {
            IActionResult result;

            if (model.Action == ControllerActions.ActionCancel)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                _logger.LogDebug($"Updating beverage measure: Id = {model.Measure.Id}, Name = {model.Measure.Name}, Volume = {model.Measure.Volume}");
                var measure = await _client.UpdateAsync(model.Measure.Id, model.Measure.Name, model.Measure.Volume);

                result = CreateListResult(measure, $"{measure.Name} successfully updated");
            }
            else
            {
                LogModelState(_logger);
                result = View(model);
            }

            return result;
        }

        /// <summary>
        /// Handle POST events to delete an existing beverage measure
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            // Delete the item
            _logger.LogDebug($"Deleting beverage measure: ID = {id}");
            await _client.DeleteAsync(id);

            // Return the list view with an empty list of items
            var message = $"Beverage measure with ID {id} successfully deleted";
            var model = await CreateBeverageMeasureListViewModel(message);
            return View("Index", model);
        }

        /// <summary>
        /// Build the beverage measure list view model
        /// </summary>
        /// <returns></returns>
        private async Task<BeverageMeasureListViewModel> CreateBeverageMeasureListViewModel(string message)
        {
            // Get the list of current beverage measures
            var measures = await _client.ListAsync(1, _settings.ResultsPageSize);
            var plural = measures.Count == 1 ? "" : "s";
            _logger.LogDebug($"{measures.Count} beverage measure{plural} loaded via the service");

            // Construct the view model and serve the page
            var model = new BeverageMeasureListViewModel()
            {
                Message = message
            };
            model.SetEntities(measures, 1, _settings.ResultsPageSize);
            return model;
        }
    }
}
