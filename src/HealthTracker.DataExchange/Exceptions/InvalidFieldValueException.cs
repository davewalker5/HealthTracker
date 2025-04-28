using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.DataExchange.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class InvalidFieldValueException : Exception
    {
        public InvalidFieldValueException()
        {
        }

        public InvalidFieldValueException(string message) : base(message)
        {
        }

        public InvalidFieldValueException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
