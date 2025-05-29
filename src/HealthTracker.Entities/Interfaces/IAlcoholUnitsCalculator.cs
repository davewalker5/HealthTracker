using HealthTracker.Entities.Measurements;
using System.Linq.Expressions;

namespace HealthTracker.Entities.Interfaces
{
    public interface IAlcoholUnitsCalculator
    {
        decimal CalculateUnits(decimal abv, decimal ml);
        decimal UnitsPerShot(decimal abv);
        decimal UnitsPerPint(decimal abv);
        decimal UnitsPerSmallGlass(decimal abv);
        decimal UnitsPerMediumGlass(decimal abv);
        decimal UnitsPerLargeGlass(decimal abv);
        void CalculateUnits(IEnumerable<AlcoholConsumptionMeasurement> measurements);
    }
}
