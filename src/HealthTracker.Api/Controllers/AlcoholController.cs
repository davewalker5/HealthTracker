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

        [HttpGet]
        [Route("unitspershot/{abv}")]
        public ActionResult<decimal> UnitsPerShot(decimal abv)
            => _factory.AlcoholUnitsCalculator.UnitsPerShot(abv);

        [HttpGet]
        [Route("unitsperpint/{abv}")]
        public ActionResult<decimal> UnitsPerPint(decimal abv)
            => _factory.AlcoholUnitsCalculator.UnitsPerPint(abv);

        [HttpGet]
        [Route("unitspersmallglass/{abv}")]
        public ActionResult<decimal> UnitsPerSmallGlass(decimal abv)
            => _factory.AlcoholUnitsCalculator.UnitsPerSmallGlass(abv);

        [HttpGet]
        [Route("unitspermediumglass/{abv}")]
        public ActionResult<decimal> UnitsPerMediumGlass(decimal abv)
            => _factory.AlcoholUnitsCalculator.UnitsPerMediumGlass(abv);

        [HttpGet]
        [Route("unitsperlargeglass/{abv}")]
        public ActionResult<decimal> UnitsPerLargeGlass(decimal abv)
            => _factory.AlcoholUnitsCalculator.UnitsPerLargeGlass(abv);
    }
}