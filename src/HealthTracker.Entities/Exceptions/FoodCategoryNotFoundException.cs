using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class FoodCategoryNotFoundException : Exception
    {
        public FoodCategoryNotFoundException()
        {
        }

        public FoodCategoryNotFoundException(string message) : base(message)
        {
        }

        public FoodCategoryNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
