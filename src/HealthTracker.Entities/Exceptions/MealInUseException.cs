using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class MealInUseException : Exception
    {
        public MealInUseException()
        {
        }

        public MealInUseException(string message) : base(message)
        {
        }

        public MealInUseException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
