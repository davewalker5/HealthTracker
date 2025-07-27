using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Food;
using HealthTracker.Mvc.Entities;
using HealthTracker.Mvc.Interfaces;
using HealthTracker.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthTracker.Mvc.Controllers
{
    [Authorize]
    public class BeverageController : ReferenceDataControllerBase<BeverageListViewModel, Beverage>
    {
        private readonly IBeverageClient _client;

        public BeverageController(
            IBeverageClient client,
            IHealthTrackerApplicationSettings settings,
            IPartialViewToStringRenderer renderer,
            ILogger<BeverageController> logger) : base(settings, renderer ,logger)
        {
            _client = client;
        }

        /// <summary>
        /// Serve the current list of beverages
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await CreateBeverageListViewModel("");
            return View(model);
        }

        /// <summary>
        /// Handle POST events for page navigation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(BeverageListViewModel model)
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
                var beverages = await _client.ListAsync(page, _settings.ResultsPageSize);
                model.SetEntities(beverages, page, _settings.ResultsPageSize);
            }
            else
            {
                LogModelState();
            }

            return View(model);
        }
        
        /// <summary>
        /// Serve the page to add a new beverage
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Add()
        {
            var model = new AddBeverageViewModel();
            return View(model);
        }

        /// <summary>
        /// Handle POST events to save new beverages
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddBeverageViewModel model)
        {
            IActionResult result;

            if (model.Action == ControllerActions.ActionCancel)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                _logger.LogDebug($"Adding beverage: Name = {model.Beverage.Name}, Typical ABV % = {model.Beverage.TypicalABV}, Hydrating = {model.Beverage.IsHydrating}, Alcohol = {model.Beverage.IsAlcohol}");
                var beverage = await _client.AddAsync(model.Beverage.Name, model.Beverage.TypicalABV, model.Beverage.IsHydrating, model.Beverage.IsAlcohol);

                result = CreateListResult(beverage, $"{beverage.Name} successfully added");
            }
            else
            {
                LogModelState();
                result = View(model);
            }

            return result;
        }
        
        /// <summary>
        /// Serve the page to edit an existing beverage
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var beverages = await _client.ListAsync(1, int.MaxValue);
            var plural = beverages.Count == 1 ? "" : "s";
            _logger.LogDebug($"{beverages.Count} beverage{plural} loaded via the service");

            var beverage = beverages.First(x => x.Id == id);
            _logger.LogDebug($"Beverage with ID {id} identified for editing");

            var model = new EditBeverageViewModel();
            model.Beverage = beverage;
            return View(model);
        }

        /// <summary>
        /// Handle POST events to update an existing beverage
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditBeverageViewModel model)
        {
            IActionResult result;

            if (model.Action == ControllerActions.ActionCancel)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                _logger.LogDebug($"Updating beverage: Id = {model.Beverage.Id}, Name = {model.Beverage.Name}, Typical ABV % = {model.Beverage.TypicalABV}, Hydrating = {model.Beverage.IsHydrating}, Alcohol = {model.Beverage.IsAlcohol}");
                var beverage = await _client.UpdateAsync(model.Beverage.Id, model.Beverage.Name, model.Beverage.TypicalABV, model.Beverage.IsHydrating, model.Beverage.IsAlcohol);

                result = CreateListResult(beverage, $"{beverage.Name} successfully updated");
            }
            else
            {
                LogModelState();
                result = View(model);
            }

            return result;
        }

        /// <summary>
        /// Handle POST events to delete an existing beverage
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            // Delete the item
            _logger.LogDebug($"Deleting beverage: ID = {id}");
            await _client.DeleteAsync(id);

            // Return the list view with an empty list of items
            var message = $"Beverage with ID {id} successfully deleted";
            var model = await CreateBeverageListViewModel(message);
            return View("Index", model);
        }

        /// <summary>
        /// Build the beverage list view model
        /// </summary>
        /// <returns></returns>
        private async Task<BeverageListViewModel> CreateBeverageListViewModel(string message)
        {
            // Get the list of current beverages
            var beverages = await _client.ListAsync(1, _settings.ResultsPageSize);
            var plural = beverages.Count == 1 ? "" : "s";
            _logger.LogDebug($"{beverages.Count} beverage{plural} loaded via the service");

            // Construct and return the view model
            var model = new BeverageListViewModel()
            {
                Message = message
            };
            model.SetEntities(beverages, 1, _settings.ResultsPageSize);
            return model;
        }
    }
}
