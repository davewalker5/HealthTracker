using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class StockDateOutOfRangeException : Exception
    {
        public StockDateOutOfRangeException()
        {
        }

        public StockDateOutOfRangeException(string message) : base(message)
        {
        }

        public StockDateOutOfRangeException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
