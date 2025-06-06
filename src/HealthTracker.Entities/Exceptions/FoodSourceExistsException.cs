using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class FoodSourceExistsException : Exception
    {
        public FoodSourceExistsException()
        {
        }

        public FoodSourceExistsException(string message) : base(message)
        {
        }

        public FoodSourceExistsException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
