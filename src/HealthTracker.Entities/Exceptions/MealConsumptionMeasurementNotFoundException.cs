using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class MealConsumptionMeasurementNotFoundException : Exception
    {
        public MealConsumptionMeasurementNotFoundException()
        {
        }

        public MealConsumptionMeasurementNotFoundException(string message) : base(message)
        {
        }

        public MealConsumptionMeasurementNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
