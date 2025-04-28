using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class InvalidMeasurementFormatException : Exception
    {
        public InvalidMeasurementFormatException()
        {
        }

        public InvalidMeasurementFormatException(string message) : base(message)
        {
        }

        public InvalidMeasurementFormatException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
