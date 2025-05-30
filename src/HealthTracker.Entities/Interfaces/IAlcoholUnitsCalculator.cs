using HealthTracker.Entities.Measurements;
using HealthTracker.Enumerations.Enumerations;
using System.Linq.Expressions;

namespace HealthTracker.Entities.Interfaces
{
    public interface IAlcoholUnitsCalculator
    {
        decimal GetVolume(BeverageMeasure measure);
        decimal CalculateVolume(BeverageMeasure measure, int quantity);
        decimal CalculateUnits(decimal abv, decimal ml);
        decimal UnitsPerShot(decimal abv);
        decimal UnitsPerPint(decimal abv);
        decimal UnitsPerHalfPint(decimal abv);
        decimal UnitsPerSmallGlass(decimal abv);
        decimal UnitsPerMediumGlass(decimal abv);
        decimal UnitsPerLargeGlass(decimal abv);
        void CalculateUnits(IEnumerable<BeverageConsumptionMeasurement> measurements);
    }
}
