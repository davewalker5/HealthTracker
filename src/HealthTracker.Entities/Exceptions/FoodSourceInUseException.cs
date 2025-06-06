using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class FoodSourceInUseException : Exception
    {
        public FoodSourceInUseException()
        {
        }

        public FoodSourceInUseException(string message) : base(message)
        {
        }

        public FoodSourceInUseException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
