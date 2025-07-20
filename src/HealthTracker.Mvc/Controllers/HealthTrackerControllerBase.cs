using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HealthTracker.Mvc.Controllers
{
    public abstract class HealthTrackerControllerBase : Controller
    {
        public static readonly string Version = Assembly.GetExecutingAssembly()
                                                        .GetCustomAttribute<AssemblyFileVersionAttribute>()?
                                                        .Version;

        /// <summary>
        /// Add the version to the view data on each request
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            ViewData["Version"] = Version;
        }

        /// <summary>
        /// Log the contents of model state
        /// </summary>
        /// <param name="logger"></param>
        protected void LogModelState(ILogger logger)
        {
            LogModelStateValues(logger);
            LogModelStateErrors(logger);
        }

        /// <summary>
        /// Log model state keys and values
        /// </summary>
        /// <param name="logger"></param>
        private void LogModelStateValues(ILogger logger)
        {
            foreach (var kvp in ModelState)
            {
                var entry = kvp.Value;
                var attemptedValue = entry?.AttemptedValue;
                var rawValue = entry?.RawValue?.ToString();
                var isValid = entry?.Errors?.Count == 0;

                logger.LogDebug($"Model State Key {kvp.Key}: Attempted = {attemptedValue}, Raw = {rawValue}, Valid =  {isValid}");
            }
        }

        /// <summary>
        /// Log model state errors
        /// </summary>
        /// <param name="logger"></param>
        private void LogModelStateErrors(ILogger logger)
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