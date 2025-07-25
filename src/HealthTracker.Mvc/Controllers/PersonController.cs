using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Identity;
using HealthTracker.Mvc.Entities;
using HealthTracker.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthTracker.Mvc.Controllers
{
    [Authorize]
    public class PersonController : ReferenceDataControllerBase<PersonListViewModel, Person>
    {
        private readonly IPersonClient _client;
        private readonly ILogger<PersonController> _logger;

        public PersonController(
            IPersonClient client,
            IHealthTrackerApplicationSettings settings,
            ILogger<PersonController> logger) : base(settings)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Serve the current list of people
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await CreatePersonListViewModel("");
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

                // Retrieve the matching person records
                var people = await _client.ListAsync(page, _settings.ResultsPageSize);
                model.SetEntities(people, page, _settings.ResultsPageSize);
            }
            else
            {
                LogModelState(_logger);
            }

            return View(model);
        }

        /// <summary>
        /// Serve the page to add a new person
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Add()
        {
            var model = new AddPersonViewModel();
            return View(model);
        }

        /// <summary>
        /// Handle POST events to save new people
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddPersonViewModel model)
        {
            IActionResult result;

            if (model.Action == ControllerActions.ActionCancel)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                _logger.LogDebug(
                    $"Adding person: First Names = {model.Person.FirstNames}, Surname = {model.Person.Surname}, " +
                    $"DoB = {model.Person.DateOfBirth:dd-MMM-yyyy}, Height = {model.Person.Height}, Gender = {model.Person.Gender}");
                var person = await _client.AddAsync(model.Person.FirstNames, model.Person.Surname, model.Person.DateOfBirth, model.Person.Height, model.Person.Gender);

                result = CreateListResult(person, $"{person.Name} successfully added");
            }
            else
            {
                LogModelState(_logger);
                result = View(model);
            }

            return result;
        }

        /// <summary>
        /// Serve the page to edit an existing person
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var people = await _client.ListAsync(1, int.MaxValue);
            var personText = people.Count == 1 ? "person" : "people";
            _logger.LogDebug($"{people.Count} {personText} loaded via the service");

            var person = people.First(x => x.Id == id);
            _logger.LogDebug($"Person with ID {id} identified for editing");

            var model = new EditPersonViewModel();
            model.Person = person;
            return View(model);
        }

        /// <summary>
        /// Handle POST events to update an existing person
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditPersonViewModel model)
        {
            IActionResult result;

            if (model.Action == ControllerActions.ActionCancel)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                _logger.LogDebug(
                    $"Updating person: Id = {model.Person.Id}, First Names = {model.Person.FirstNames}, Surname = {model.Person.Surname}, " +
                    $"DoB = {model.Person.DateOfBirth:dd-MMM-yyyy}, Height = {model.Person.Height}, Gender = {model.Person.Gender}");

                var person = await _client.UpdateAsync(
                    model.Person.Id,
                    model.Person.FirstNames,
                    model.Person.Surname,
                    model.Person.DateOfBirth,
                    model.Person.Height,
                    model.Person.Gender);

                result = CreateListResult(person, $"{person.Name} successfully updated");
            }
            else
            {
                LogModelState(_logger);
                result = View(model);
            }

            return result;
        }

        /// <summary>
        /// Handle POST events to delete an existing medication
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            // Delete the item
            _logger.LogDebug($"Deleting person: ID = {id}");
            await _client.DeleteAsync(id);

            // Return the list view with an empty list of items
            var message = $"Person with ID {id} successfully deleted";
            var model = await CreatePersonListViewModel(message);
            return View("Index", model);
        }

        /// <summary>
        /// Build the person list view model
        /// </summary>
        /// <returns></returns>
        private async Task<PersonListViewModel> CreatePersonListViewModel(string message)
        {
            // Get the list of current people
            var people = await _client.ListAsync(1, _settings.ResultsPageSize);
            var personText = people.Count == 1 ? "person" : "people";
            _logger.LogDebug($"{people.Count} {personText} loaded via the service");


            // Construct the view model and serve the page
            var model = new PersonListViewModel()
            {
                Message = message
            };
            model.SetEntities(people, 1, _settings.ResultsPageSize);
            return model;
        }
    }
}
