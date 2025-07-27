using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class PlannedMealNotFoundException : Exception
    {
        public PlannedMealNotFoundException()
        {
        }

        public PlannedMealNotFoundException(string message) : base(message)
        {
        }

        public PlannedMealNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
