using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class PlannedMealExistsException : Exception
    {
        public PlannedMealExistsException()
        {
        }

        public PlannedMealExistsException(string message) : base(message)
        {
        }

        public PlannedMealExistsException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
