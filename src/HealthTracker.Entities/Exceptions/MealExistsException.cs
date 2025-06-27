using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class MealExistsException : Exception
    {
        public MealExistsException()
        {
        }

        public MealExistsException(string message) : base(message)
        {
        }

        public MealExistsException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
