using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Medications;
using HealthTracker.Mvc.Entities;
using HealthTracker.Mvc.Interfaces;
using HealthTracker.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthTracker.Mvc.Controllers
{
    [Authorize]
    public class PersonMedicationController : MeasurementControllerBase<IPersonMedicationClient, PersonMedicationListViewModel, PersonMedication>
    {
        private readonly ILogger<PersonMedicationController> _logger;

        public PersonMedicationController(
            IPersonClient personClient,
            IPersonMedicationClient measurementClient,
            IHealthTrackerApplicationSettings settings,
            IFilterGenerator filterGenerator,
            ILogger<PersonMedicationController> logger) : base(personClient, measurementClient, settings, filterGenerator)
        {
            _logger = logger;
        }

        /// <summary>
        /// Serve the measurements list page
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(int personId = 0)
        {
            _logger.LogDebug($"Rendering index view: Person ID = {personId}");

            var model = new PersonMedicationListViewModel
            {
                PageNumber = 1,
                Filters = await _filterGenerator.Create(personId),
                Settings = _settings
            };

            return View(model);
        }

        /// <summary>
        /// Handle POST events for page navigation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(PersonMedicationListViewModel model)
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
                    case ControllerActions.ActionAdd:
                        return RedirectToAction("Add", new { personId = model.Filters.PersonId });
                    default:
                        break;
                }

                // Need to clear model state here or the page number that was posted
                // is returned and page navigation doesn't work correctly. So, capture
                // and amend the page number, above, then apply it, below
                ModelState.Clear();

                // Retrieve the matching records
                _logger.LogDebug($"Retrieving page {page} of person/medication associations for person with ID {model.Filters.PersonId}");
                var associations = await _measurementClient.ListPersonMedicationsAsync(model.Filters.PersonId, page, _settings.ResultsPageSize);
                model.SetEntities(associations, page, _settings.ResultsPageSize);

                _logger.LogDebug($"{associations.Count} matching person/medication associations retrieved");
            }
            else
            {
                LogModelStateErrors(_logger);
            }

            // Populate the list of people and render the view
            await _filterGenerator.PopulatePersonList(model.Filters);
            model.Settings = _settings;
            return View(model);
        }
    }
}