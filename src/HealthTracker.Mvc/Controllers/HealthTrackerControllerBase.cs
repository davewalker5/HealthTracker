using HealthTracker.Client.Interfaces;
using HealthTracker.Mvc.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HealthTracker.Mvc.Controllers
{
    public class HealthTrackerControllerBase : Controller
    {
        protected readonly IPersonClient _personClient;

        public HealthTrackerControllerBase()
            => _personClient = null;

        public HealthTrackerControllerBase(IPersonClient personClient)
            => _personClient = personClient;

        /// <summary>
        /// Log model state errors
        /// </summary>
        /// <param name="logger"></param>
        protected void LogModelStateErrors(ILogger logger)
        {
            foreach (var modelState in ViewData.ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    logger.LogDebug(error.ErrorMessage);
                }
            }
        }

        /// <summary>
        /// Set person details on the view model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="personId"></param>
        /// <returns></returns>
        protected async Task SetPersonDetails(IMeasurementPersonViewModel model, int personId)
        {
            // Retrieve the person with whom the new measurement is to be associated
            var people = await _personClient.ListPeopleAsync(1, int.MaxValue);
            var person = people.First(x => x.Id == personId);

            // Set the person details on the model
            model.PersonName = person.Name;
        }
    }
}