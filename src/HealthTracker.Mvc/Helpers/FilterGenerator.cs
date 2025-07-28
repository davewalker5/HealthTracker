using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Identity;
using HealthTracker.Mvc.Entities;
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
        /// <param name="flags"></param>
        /// <returns></returns>
        public async Task<FiltersViewModel> Create(int personId, DateTime? from, DateTime? to, ViewFlags flags)
        {
            // Create a new model and populate the list of people
            var model = new FiltersViewModel()
            {
                PersonId = personId,
                ShowAddButton = flags.HasFlag(ViewFlags.Add),
                ShowExportButton = flags.HasFlag(ViewFlags.Export)
            };
            await PopulatePersonList(model);

            // Set the default date range
            if (flags.HasFlag(ViewFlags.FutureDates))
            {
                model.From = from ?? DateTime.Today;
                model.To = to ?? DateTime.Today.AddDays(_settings.DefaultTimePeriodDays);
            }
            else
            {
                model.From = from ?? DateTime.Today.AddDays(-_settings.DefaultTimePeriodDays);
                model.To = to ?? DateTime.Today;
            }

            return model;
        }

        /// <summary>
        /// Create a person filter view model with filter properties selected
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public async Task<PersonFilterViewModel> Create(int personId, ViewFlags flags)
        {
            // Create a new model and populate the list of people
            var model = new PersonFilterViewModel()
            {
                PersonId = personId,
                ShowAddButton = flags.HasFlag(ViewFlags.Add),
                ShowExportButton = flags.HasFlag(ViewFlags.Export)
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
            // Load the list of people and create the select list
            var people = await LoadPeople();
            model.People = CreatePersonSelectList(people);

            // Set the selected person, if there is one
            if (model.PersonId > 0)
            {
                model.SelectedPerson = people.First(x => x.Id == model.PersonId);
            }
        }

        /// <summary>
        /// Create and return a list of select list items for the current people in the database
        /// </summary>
        /// <returns></returns>
        public async Task<IList<SelectListItem>> CreatePersonSelectList()
        {
            var people = await LoadPeople();
            var list = CreatePersonSelectList(people);
            return list;
        }

        /// <summary>
        /// Load the current list of people
        /// </summary>
        /// <returns></returns>
        private async Task<IList<Person>> LoadPeople()
        {
            var people = await _client.ListAsync(1, int.MaxValue);
            var personText = people.Count == 1 ? "person" : "people";
            _logger.LogDebug($"{people.Count} {personText} loaded via the service");

            return people;
        }

        /// <summary>
        /// Given a collection of people, create a list of select list items for chosing one of them
        /// </summary>
        /// <param name="people"></param>
        /// <returns></returns>
        private IList<SelectListItem> CreatePersonSelectList(IList<Person> people)
        {
            IList<SelectListItem> list = [];

            // Create a list of select list items from the list of people. Add an empty entry if there
            // is more than one person. If not, the list will only contain that one person which will
            // be the default selection
            if (people.Count != 1)
            {
                list.Add(new SelectListItem() { Text = "", Value = "0" });
            }

            // Now add an entry for each person
            foreach (var person in people)
            {
                list.Add(new SelectListItem() { Text = $"{person.Surname}, {person.FirstNames}", Value = person.Id.ToString() });
            }

            return list;
        }
    }
}