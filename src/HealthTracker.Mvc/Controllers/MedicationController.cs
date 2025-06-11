using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Medications;
using HealthTracker.Mvc.Entities;
using HealthTracker.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthTracker.Mvc.Controllers
{
    [Authorize]
    public class MedicationController : ReferenceDataControllerBase<MedicationListViewModel, Medication>
    {
        private readonly IMedicationClient _client;

        private readonly ILogger<MedicationController> _logger;

        public MedicationController(
            IMedicationClient client,
            IHealthTrackerApplicationSettings settings,
            ILogger<MedicationController> logger) : base(settings)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Serve the current list of medications
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Get the list of current medications
            var medications = await _client.ListAsync(1, _settings.ResultsPageSize);
            var plural = medications.Count == 1 ? "" : "s";
            _logger.LogDebug($"{medications.Count} medication{plural} loaded via the service");

            // Construct the view model and serve the page
            var model = new MedicationListViewModel();
            model.SetEntities(medications, 1, _settings.ResultsPageSize);
            return View(model);
        }

        /// <summary>
        /// Handle POST events for page navigation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(MedicationListViewModel model)
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
                var medications = await _client.ListAsync(page, _settings.ResultsPageSize);
                model.SetEntities(medications, page, _settings.ResultsPageSize);
            }
            else
            {
                LogModelStateErrors(_logger);
            }

            return View(model);
        }
        
        /// <summary>
        /// Serve the page to add a new medication
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Add()
        {
            var model = new AddMedicationViewModel();
            return View(model);
        }

        /// <summary>
        /// Handle POST events to save new medications
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddMedicationViewModel model)
        {
            IActionResult result;

            if (model.Action == ControllerActions.ActionCancel)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                _logger.LogDebug($"Adding medication: Name = {model.Medication.Name}");
                var medication = await _client.AddAsync(model.Medication.Name);

                result = CreateListResult(medication, $"{medication.Name} successfully added");
            }
            else
            {
                LogModelStateErrors(_logger);
                result = View(model);
            }

            return result;
        }
        
        /// <summary>
        /// Serve the page to edit an existing medication
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var medications = await _client.ListAsync(1, int.MaxValue);
            var plural = medications.Count == 1 ? "" : "s";
            _logger.LogDebug($"{medications.Count} medication{plural} loaded via the service");

            var medication = medications.First(x => x.Id == id);
            _logger.LogDebug($"Medication with ID {id} identified for editing");

            var model = new EditMedicationViewModel();
            model.Medication = medication;
            return View(model);
        }

        /// <summary>
        /// Handle POST events to update an existing medication
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditMedicationViewModel model)
        {
            IActionResult result;

            if (model.Action == ControllerActions.ActionCancel)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                _logger.LogDebug($"Updating medication: Id = {model.Medication.Id}, Name = {model.Medication.Name}");
                var medication = await _client.UpdateAsync(model.Medication.Id, model.Medication.Name);

                result = CreateListResult(medication, $"{medication.Name} successfully updated");
            }
            else
            {
                LogModelStateErrors(_logger);
                result = View(model);
            }

            return result;
        }
    }
}
