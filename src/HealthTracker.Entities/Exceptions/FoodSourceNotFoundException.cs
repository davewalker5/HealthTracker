using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class FoodSourceNotFoundException : Exception
    {
        public FoodSourceNotFoundException()
        {
        }

        public FoodSourceNotFoundException(string message) : base(message)
        {
        }

        public FoodSourceNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
