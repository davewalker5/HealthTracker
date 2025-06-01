using HealthTracker.Entities.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthTracker.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("[controller]")]
    public class AlcoholController : Controller
    {
        private readonly IHealthTrackerFactory _factory;

        public AlcoholController(IHealthTrackerFactory factory)
            => _factory = factory;

        [HttpGet]
        [Route("units/{abv}/{volume}")]
        public ActionResult<decimal> CalculateUnits(decimal abv, decimal volume)
            => _factory.AlcoholUnitsCalculator.CalculateUnits(abv, volume);
    }
}