using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class BeverageMeasureExistsException : Exception
    {
        public BeverageMeasureExistsException()
        {
        }

        public BeverageMeasureExistsException(string message) : base(message)
        {
        }

        public BeverageMeasureExistsException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
