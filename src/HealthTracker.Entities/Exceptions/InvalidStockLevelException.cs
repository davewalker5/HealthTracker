using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class InvalidStockLevelException : Exception
    {
        public InvalidStockLevelException()
        {
        }

        public InvalidStockLevelException(string message) : base(message)
        {
        }

        public InvalidStockLevelException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
