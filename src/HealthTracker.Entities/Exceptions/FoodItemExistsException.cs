using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class FoodItemExistsException : Exception
    {
        public FoodItemExistsException()
        {
        }

        public FoodItemExistsException(string message) : base(message)
        {
        }

        public FoodItemExistsException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
