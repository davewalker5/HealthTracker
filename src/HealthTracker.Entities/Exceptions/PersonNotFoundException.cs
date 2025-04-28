using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class PersonNotFoundException : Exception
    {
        public PersonNotFoundException()
        {
        }

        public PersonNotFoundException(string message) : base(message)
        {
        }

        public PersonNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
