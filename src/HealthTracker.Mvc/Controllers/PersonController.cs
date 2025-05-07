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
        /// <param name="personId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(int personId = 0)
        {
            // Get the list of current people
            var people = await _client.ListPeopleAsync(1, _settings.ResultsPageSize);
            var personText = people.Count == 1 ? "person" : "people";
            _logger.LogDebug($"{people.Count} {personText} loaded via the service");

            // If the peson ID is specified, filter out only that individual
            if (personId > 0)
            {
                _logger.LogDebug($"Filtering results for person with ID {personId}");
                var person = people.First(x => x.Id == personId);
                people = [person];
            }

            // Construct the view model and serve the page
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
            if (ModelState.IsValid)
            {
                var name = model.Person.Name();
                _logger.LogDebug(
                    $"Adding person: First Names = {model.Person.FirstNames}, Surname = {model.Person.Surname}, " +
                    $"DoB = {model.Person.DateOfBirth:dd-MMM-yyyy}, Height = {model.Person.Height}, Gender = {model.Person.Gender}");
                await _client.AddPersonAsync(model.Person.FirstNames, model.Person.Surname, model.Person.DateOfBirth, model.Person.Height, model.Person.Gender);
                ModelState.Clear();
                model.Clear();
                model.Message = $"'{name}' added successfully";
            }

            return View(model);
        }
        
        /// <summary>
        /// Serve the page to edit an existing person
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var people = await _client.ListPeopleAsync(1, int.MaxValue);
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

            if (ModelState.IsValid)
            {
                _logger.LogDebug(
                    $"Updating person: Id = {model.Person.Id}, First Names = {model.Person.FirstNames}, Surname = {model.Person.Surname}, " +
                    $"DoB = {model.Person.DateOfBirth:dd-MMM-yyyy}, Height = {model.Person.Height}, Gender = {model.Person.Gender}");

                await _client.UpdatePersonAsync(
                    model.Person.Id,
                    model.Person.FirstNames,
                    model.Person.Surname,
                    model.Person.DateOfBirth,
                    model.Person.Height,
                    model.Person.Gender);

                result = RedirectToAction("Index", new { personId = model.Person.Id });
            }
            else
            {
                result = View(model);
            }

            return result;
        }
    }
}
