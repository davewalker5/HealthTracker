using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class FoodCategoryExistsException : Exception
    {
        public FoodCategoryExistsException()
        {
        }

        public FoodCategoryExistsException(string message) : base(message)
        {
        }

        public FoodCategoryExistsException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
