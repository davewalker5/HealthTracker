using HealthTracker.Client.Interfaces;
using HealthTracker.Mvc.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Helpers
{
    public class MedicationListGenerator : IMedicationListGenerator
    {
        private readonly IMedicationClient _medicationsClient;
        private readonly IPersonMedicationClient _personMedicationClient;
        private readonly ILogger<FilterGenerator> _logger;

        public MedicationListGenerator(IMedicationClient medicationsClient, IPersonMedicationClient personMedicationClient, ILogger<FilterGenerator> logger)
        {
            _medicationsClient = medicationsClient;
            _personMedicationClient = personMedicationClient;
            _logger = logger;
        }

        /// <summary>
        /// Generate a list of select list items for activity types
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="associationId"></param>
        /// <returns></returns>
        public async Task<IList<SelectListItem>> Create(int personId, int associationId)
        {
            var list = new List<SelectListItem>();

            // Load the list of medications
            var medications = await _medicationsClient.ListMedicationsAsync(1, int.MaxValue);
            var plural = medications.Count == 1 ? "" : "s";
            _logger.LogDebug($"{medications.Count} medication{plural} loaded via the service");

            // If we're editing an existing association, the association ID will be set to a valid
            // value. The editor needs to be able to present a list containing that medication as it's
            // an existing association but all other *associated* medications need to be removed from
            // the list to avoid potentially creating duplicate associations. Similarly with adding a
            // ned association, medications for all existing associations must be removed
            var associations = await _personMedicationClient.ListPersonMedicationsAsync(personId, 1, int.MaxValue);
            var removeMedicationIds = associations.Where(x => x.Id != associationId).Select(x => x.MedicationId).ToList();
            medications.RemoveAll(x => removeMedicationIds.Contains(x.Id));

            // Create a list of select list items from the list of medications. Add an empty entry if there
            // is more than one. If not, the list will only contain that one medication which will be the default
            // selection
            if (medications.Count != 1)
            {
                list.Add(new SelectListItem() { Text = "", Value = "0" });
            }

            // Now add an entry for each person
            foreach (var medication in medications)
            {
                list.Add(new SelectListItem() { Text = medication.Name, Value = medication.Id.ToString() });
            }

            return list;
        }
    }
}