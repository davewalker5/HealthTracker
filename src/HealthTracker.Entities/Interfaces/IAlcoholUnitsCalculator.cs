using HealthTracker.Entities.Food;

namespace HealthTracker.Entities.Interfaces
{
    public interface IAlcoholUnitsCalculator
    {

        decimal CalculateUnits(decimal abv, decimal ml);
        void CalculateUnits(IEnumerable<BeverageConsumptionMeasurement> measurements);
    }
}
