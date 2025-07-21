using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class FoodItemNotFoundException : Exception
    {
        public FoodItemNotFoundException()
        {
        }

        public FoodItemNotFoundException(string message) : base(message)
        {
        }

        public FoodItemNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
