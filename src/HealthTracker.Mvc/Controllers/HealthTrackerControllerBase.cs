using Microsoft.AspNetCore.Mvc;

namespace HealthTracker.Mvc.Controllers
{
    public abstract class HealthTrackerControllerBase : Controller
    {
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
    }
}