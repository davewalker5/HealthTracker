using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class FoodItemInUseException : Exception
    {
        public FoodItemInUseException()
        {
        }

        public FoodItemInUseException(string message) : base(message)
        {
        }

        public FoodItemInUseException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
