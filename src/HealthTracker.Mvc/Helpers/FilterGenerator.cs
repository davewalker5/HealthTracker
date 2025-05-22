using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Mvc.Interfaces;
using HealthTracker.Mvc.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Helpers
{
    public class FilterGenerator : IFilterGenerator
    {
        private readonly IPersonClient _client;
        private readonly IHealthTrackerApplicationSettings _settings;
        private readonly ILogger<FilterGenerator> _logger;

        public FilterGenerator(IPersonClient client, IHealthTrackerApplicationSettings settings, ILogger<FilterGenerator> logger)
        {
            _client = client;
            _settings = settings;
            _logger = logger;
        }

        /// <summary>
        /// Create a filters view model with filter properties selected
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="showAddButton"></param>
        /// <returns></returns>
        public async Task<FiltersViewModel> Create(int personId, DateTime? from, DateTime? to, bool showAddButton)
        {
            // Create a new model and populate the list of people
            var model = new FiltersViewModel()
            {
                PersonId = personId,
                ShowAddButton = showAddButton
            };
            await PopulatePersonList(model);

            // Set the default date range
            model.From = from ?? DateTime.Today.AddDays(-_settings.DefaultTimePeriodDays);
            model.To = to ?? DateTime.Today;

            return model;
        }

        /// <summary>
        /// Create a person filter view model with filter properties selected
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="showAddButton"></param>
        /// <returns></returns>
        public async Task<PersonFilterViewModel> Create(int personId, bool showAddButton)
        {
            // Create a new model and populate the list of people
            var model = new PersonFilterViewModel()
            {
                PersonId = personId,
                ShowAddButton = showAddButton
            };
            await PopulatePersonList(model);

            return model;
        }

        /// <summary>
        /// Populate the list of people in a filters view model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task PopulatePersonList(PersonFilterViewModel model)
        {
            // Load the list of people
            var people = await _client.ListAsync(1, int.MaxValue);
            var personText = people.Count == 1 ? "person" : "people";
            _logger.LogDebug($"{people.Count} {personText} loaded via the service");

            // Create a list of select list items from the list of people. Add an empty entry if there
            // is more than one person. If not, the list will only contain that one person which will
            // be the default selection
            if (people.Count != 1)
            {
                model.People.Add(new SelectListItem() { Text = "", Value = "0" });
            }

            // Now add an entry for each person
            foreach (var person in people)
            {
                model.People.Add(new SelectListItem() { Text = $"{person.Surname}, {person.FirstNames}", Value = person.Id.ToString() });
            }

            // Set the selected person, if there is one
            if (model.PersonId > 0)
            {
                model.SelectedPerson = people.First(x => x.Id == model.PersonId);
            }
        }
    }
}