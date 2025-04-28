using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class PersonInUseException : Exception
    {
        public PersonInUseException()
        {
        }

        public PersonInUseException(string message) : base(message)
        {
        }

        public PersonInUseException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
