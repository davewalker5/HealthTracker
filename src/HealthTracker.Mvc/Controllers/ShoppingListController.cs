using HealthTracker.Client.Interfaces;
using HealthTracker.Enumerations.Enumerations;
using HealthTracker.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthTracker.Mvc.Controllers
{
    [Authorize]
    public class ShoppingListController : Controller
    {
        private readonly IPersonClient _personClient;
        private readonly IPlannedMealClient _client;
        private readonly ILogger<ShoppingListController> _logger;

        public ShoppingListController(
            IPersonClient personClient,
            IPlannedMealClient client,
            ILogger<ShoppingListController> logger)
        {
            _personClient = personClient;
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Serve the current list of activity types
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(int personId, DateTime from, DateTime to)
        {
            var model = await CreateShoppingListViewModel(personId, from, to, "");
            return View(model);
        }

        /// <summary>
        /// Build the shoppoing list view model
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task<ShoppingListViewModel> CreateShoppingListViewModel(int personId, DateTime from, DateTime to, string message)
        {
            _logger.LogDebug($"Creating shopping list model for person with ID {personId} for the date range {from} to {to}");

            // Retrieve a list of people
            var people = await _personClient.ListAsync(1, int.MaxValue);

            // Construct and return the view model
            var model = new ShoppingListViewModel()
            {
                PersonId = personId,
                PersonName = people.First(x => x.Id == personId).Name,
                From = from,
                To = to,
                Message = message,
                Items = await _client.GetShoppingListAsync(personId, from, to)
            };

            return model;
        }
    }
}
