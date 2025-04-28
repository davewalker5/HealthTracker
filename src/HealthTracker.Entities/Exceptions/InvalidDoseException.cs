using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class InvalidDoseException : Exception
    {
        public InvalidDoseException()
        {
        }

        public InvalidDoseException(string message) : base(message)
        {
        }

        public InvalidDoseException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
