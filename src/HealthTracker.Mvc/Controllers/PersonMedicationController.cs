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
        private readonly IMedicationTrackingClient _medicationTrackingClient;
        private readonly IMedicationListGenerator _medicationListGenerator;

        public PersonMedicationController(
            IPersonClient personClient,
            IPersonMedicationClient measurementClient,
            IMedicationTrackingClient medicationTrackingClient,
            IHealthTrackerApplicationSettings settings,
            IFilterGenerator filterGenerator,
            IMedicationListGenerator medicationListGenerator,
            ILogger<PersonMedicationController> logger) : base(personClient, measurementClient, settings, filterGenerator)
        {
            _logger = logger;
            _medicationTrackingClient = medicationTrackingClient;
            _medicationListGenerator = medicationListGenerator;
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
            var model = await CreatePersonMedicationListViewModel(personId, "");
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
                    case ControllerActions.ActionAdd:
                        return RedirectToAction("Add", new { personId = model.Filters.PersonId });
                    case ControllerActions.ActionTake:
                        await model.Take(_logger, _measurementClient, _medicationTrackingClient);
                        break;
                    case ControllerActions.ActionUnTake:
                        await model.Untake(_logger, _measurementClient, _medicationTrackingClient);
                        break;
                    case ControllerActions.ActionSkip:
                        await model.Skip(_logger, _measurementClient, _medicationTrackingClient);
                        break;
                    case ControllerActions.ActionTakeAll:
                        await model.TakeAll(_logger, _medicationTrackingClient);
                        break;
                    case ControllerActions.ActionUnTakeAll:
                        await model.UntakeAll(_logger, _medicationTrackingClient);
                        break;
                    case ControllerActions.ActionSkipAll:
                        await model.SkipAll(_logger, _medicationTrackingClient);
                        break;
                    default:
                        break;
                }

                // Generate a model containing the associations for the selected person
                model = await CreatePersonMedicationListViewModel(model.Filters.PersonId, "");
            }
            else
            {
                LogModelStateErrors(_logger);
                await _filterGenerator.PopulatePersonList(model.Filters);
                model.Settings = _settings;
            }

            // Populate the list of people and render the view
            return View(model);
        }

        /// <summary>
        /// Serve the page to add a new association
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Add(int personId)
        {
            _logger.LogDebug($"Rendering add view: Person ID = {personId}");

            var model = new AddPersonMedicationViewModel();
            model.Association.PersonId = personId;
            model.Medications = await _medicationListGenerator.Create(personId, 0);
            await SetFilterDetails(model, personId, DateTime.Now, DateTime.Now);
            return View(model);
        }

        /// <summary>
        /// Handle POST events to save new associations
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddPersonMedicationViewModel model)
        {
            if (model.Action == ControllerActions.ActionCancel)
            {
                return RedirectToAction("Index", new { personId = model.PersonId });
            }

            if (ModelState.IsValid)
            {
                // Add the measurement
                _logger.LogDebug(
                    $"Adding new person/medication association: Person ID = {model.Association.PersonId}, " +
                    $"Medication ID = {model.Association.MedicationId}, " +
                    $"Daily Dose = {model.Association.DailyDose}, " +
                    $"Stock = {model.Association.Stock}, " +
                    $"Active = {model.Association.Active}, " +
                    $"LastTaken = {model.Association.LastTaken}");

                var measurement = await _measurementClient.AddPersonMedicationAsync(
                    model.Association.PersonId,
                    model.Association.MedicationId,
                    model.Association.DailyDose,
                    model.Association.Stock,
                    model.Association.LastTaken);

                // Return the measurement list view with a confirmation message
                var listModel = await CreatePersonMedicationListViewModel(model.Association.PersonId, "Association successfully added");
                return View("Index", listModel);
            }
            else
            {
                LogModelStateErrors(_logger);
            }

            return View(model);
        }

        /// <summary>
        /// Serve the page to edit an existing associations
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogDebug($"Rendering edit view: Association ID = {id}");

            // Load the measurement to edit
            var association = await _measurementClient.Get(id);

            // Construct the view model
            var model = new EditPersonMedicationViewModel
            {
                Association = association,
                Medications = await _medicationListGenerator.Create(association.PersonId, id)
            };

            await SetFilterDetails(model, association.PersonId, DateTime.Now, DateTime.Now);
            return View(model);
        }

        /// <summary>
        /// Handle POST events to save updated associations
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditPersonMedicationViewModel model)
        {
            IActionResult result;

            if (model.Action == ControllerActions.ActionCancel)
            {
                return RedirectToAction("Index", new { personId = model.Association.PersonId, start = model.From, end = model.To });
            }

            if (ModelState.IsValid)
            {
                // Update the measurement
                _logger.LogDebug(
                    $"Updating person/medication association: ID = {model.Association.Id}, " +
                    $"Person ID = {model.Association.PersonId}, " +
                    $"Medication ID = {model.Association.MedicationId}, " +
                    $"Daily Dose = {model.Association.DailyDose}, " +
                    $"Stock = {model.Association.Stock}, " +
                    $"Active = {model.Association.Active}, " +
                    $"LastTaken = {model.Association.LastTaken}");

                await _measurementClient.UpdatePersonMedicationAsync(
                    model.Association.Id,
                    model.Association.PersonId,
                    model.Association.MedicationId,
                    model.Association.DailyDose,
                    model.Association.Stock,
                    model.Association.Active,
                    model.Association.LastTaken);

                // Return the measurement list view with a confirmation message
                var listModel = await CreatePersonMedicationListViewModel(model.Association.PersonId, "Association successfully updated");
                return View("Index", listModel);
            }
            else
            {
                LogModelStateErrors(_logger);
                result = View(model);
            }

            return result;
        }

        /// <summary>
        /// Handle POST events to delete an existing association
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            // Retrieve the measurement and capture the person
            _logger.LogDebug($"Retrieving exercise measurement: ID = {id}");
            var measurement = await _measurementClient.Get(id);
            var personId = measurement.PersonId;

            // Delete the measurement
            _logger.LogDebug($"Deleting blood glucose measurement: ID = {id}");
            await _measurementClient.DeletePersonMedicationAsync(id);

            // Return the list view with an empty list of measurements
            var model = await CreatePersonMedicationListViewModel(personId, "Association successfully deleted");
            return View("Index", model);
        }

        /// <summary>
        /// Helper method to create a list view result when editing or deleting an association
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected async Task<PersonMedicationListViewModel> CreatePersonMedicationListViewModel(int personId, string message)
        {
            // Create the model
            var model = new PersonMedicationListViewModel
            {
                Filters = await _filterGenerator.Create(personId),
                Settings = _settings,
                SelectedAssociationIds = "",
                Message = message
            };

            // Retrieve and set the medication associations for the specified person, if specified
            if (personId > 0)
            {
                _logger.LogDebug($"Retrieving person/medication associations for person with ID {model.Filters.PersonId}");
                var associations = await _measurementClient.ListPersonMedicationsAsync(personId, 1, int.MaxValue);
                model.SetEntities(associations, 1, int.MaxValue);
                _logger.LogDebug($"{associations.Count} matching person/medication associations retrieved");
            }

            // Return the model
            return model;
        }
    }
}