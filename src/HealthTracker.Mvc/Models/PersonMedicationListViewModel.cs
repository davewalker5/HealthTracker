using System.Runtime.CompilerServices;
using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Medications;

namespace HealthTracker.Mvc.Models
{
    public class PersonMedicationListViewModel : FilteredByPersonViewModelBase<PersonMedication>
    {
        public IEnumerable<PersonMedication> Associations => Entities;
        public IHealthTrackerApplicationSettings Settings { get; set; }
        public string SelectedAssociationIds { get; set; }

        /// <summary>
        /// Take one dose of all medications for the selected person
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public async Task TakeAll(ILogger logger, IMedicationTrackingClient client)
        {
            logger.LogDebug($"Taking dose of all medications for person with ID {Filters.PersonId}");
            await client.TakeAllDosesAsync(Filters.PersonId);
        }

        /// <summary>
        /// Untake one dose of all medications for the selected person
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public async Task UntakeAll(ILogger logger, IMedicationTrackingClient client)
        {
            logger.LogDebug($"Un-taking dose of all medications for person with ID {Filters.PersonId}");
            await client.UntakeAllDosesAsync(Filters.PersonId);
        }

        /// <summary>
        /// Skip one dose of all medications for the selected person
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public async Task SkipAll(ILogger logger, IMedicationTrackingClient client)
        {
            logger.LogDebug($"Skipping dose of all medications for person with ID {Filters.PersonId}");
            await client.SkipAllDosesAsync(Filters.PersonId);
        }

        /// <summary>
        /// Take one dose of each selected medication
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="associationClient"></param>
        /// <param name="trackingClient"></param>
        /// <returns></returns>
        public async Task Take(ILogger logger, IPersonMedicationClient associationClient, IMedicationTrackingClient trackingClient)
            => await ActOnSelections(logger, associationClient, trackingClient.TakeDoseAsync);

        /// <summary>
        /// Un-take one dose of each selected medication
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="associationClient"></param>
        /// <param name="trackingClient"></param>
        public async Task Untake(ILogger logger, IPersonMedicationClient associationClient, IMedicationTrackingClient trackingClient)
            => await ActOnSelections(logger, associationClient, trackingClient.UntakeDoseAsync);

        /// <summary>
        /// Skip one dose of each selected medication
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="associationClient"></param>
        /// <param name="trackingClient"></param>
        public async Task Skip(ILogger logger, IPersonMedicationClient associationClient, IMedicationTrackingClient trackingClient)
            => await ActOnSelections(logger, associationClient, trackingClient.SkipDoseAsync);

        /// <summary>
        /// Perform an action, implemented in the callback function, on the selected associations
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="associationClient"></param>
        /// <param name="medicationAction"></param>
        /// <param name="associationAction"></param>
        /// <param name="caller"></param>
        /// <returns></returns>
        private async Task ActOnSelections(
            ILogger logger,
            IPersonMedicationClient associationClient,
            Func<int, int, Task> medicationAction,
            [CallerMemberName] string caller = "")
        {
            // Retrieve the selected associations - the string is a hidden field and may bind as null
            var associationIds = (SelectedAssociationIds ?? "").Split(",", StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse);
            if (associationIds.Any())
            {
                // We need the medication IDs so retrieve the association details for the selected person
                var associations = await associationClient.ListAsync(Filters.PersonId, 1, int.MaxValue);

                // Iterate over the selected association IDs
                foreach (var associationId in associationIds)
                {
                    logger.LogDebug($"{caller} for person/medication association with ID {associationId}");

                    // Get the medication ID
                    var medicationId = associations.First(x => x.Id == associationId).MedicationId;

                    // Now perform the action on the selected association/medication
                    await medicationAction(Filters.PersonId, medicationId);
                }
            }
            else
            {
                logger.LogDebug($"No medications selected to be taken");
            }
        }
    }
}