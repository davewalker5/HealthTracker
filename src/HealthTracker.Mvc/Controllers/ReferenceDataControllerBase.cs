using HealthTracker.Configuration.Interfaces;
using HealthTracker.Mvc.Interfaces;
using HealthTracker.Mvc.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthTracker.Mvc.Controllers
{
    public abstract class ReferenceDataControllerBase<L, M> : HealthTrackerControllerBase
        where L: ListViewModelBase<M>, new()
        where M: class, new()
    {
        protected readonly IHealthTrackerApplicationSettings _settings;

        public ReferenceDataControllerBase(
            IHealthTrackerApplicationSettings settings,
            IPartialViewToStringRenderer renderer,
            ILogger logger) : base(renderer, logger)
        {
            _settings = settings;
        }

        /// <summary>
        /// Helper method to create a list view result
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected IActionResult CreateListResult(M entity, string message)
        {
            // Create the model
            L model = new()
            {
                PageNumber = 1
            };

            // Populate the list of entities with the one of interest, if specified
            if (entity != null)
            {
                model.SetEntities([entity], 1, _settings.ResultsPageSize);
            }

            // Set the message
            model.Message = message;

            // Render the view
            return View("Index", model);
        }
    }
}