using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class FoodCategoryInUseException : Exception
    {
        public FoodCategoryInUseException()
        {
        }

        public FoodCategoryInUseException(string message) : base(message)
        {
        }

        public FoodCategoryInUseException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
