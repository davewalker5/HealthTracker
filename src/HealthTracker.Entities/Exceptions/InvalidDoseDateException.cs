using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class InvalidDoseDateException : Exception
    {
        public InvalidDoseDateException()
        {
        }

        public InvalidDoseDateException(string message) : base(message)
        {
        }

        public InvalidDoseDateException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
