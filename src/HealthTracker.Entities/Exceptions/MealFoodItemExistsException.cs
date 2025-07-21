using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class MealFoodItemExistsException : Exception
    {
        public MealFoodItemExistsException()
        {
        }

        public MealFoodItemExistsException(string message) : base(message)
        {
        }

        public MealFoodItemExistsException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
