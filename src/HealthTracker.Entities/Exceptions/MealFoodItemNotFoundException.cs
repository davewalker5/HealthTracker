using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class MealFoodItemNotFoundException : Exception
    {
        public MealFoodItemNotFoundException()
        {
        }

        public MealFoodItemNotFoundException(string message) : base(message)
        {
        }

        public MealFoodItemNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
