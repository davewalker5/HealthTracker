using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class MealNotFoundException : Exception
    {
        public MealNotFoundException()
        {
        }

        public MealNotFoundException(string message) : base(message)
        {
        }

        public MealNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
