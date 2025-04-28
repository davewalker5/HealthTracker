using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class DuplicateMedicationException : Exception
    {
        public DuplicateMedicationException()
        {
        }

        public DuplicateMedicationException(string message) : base(message)
        {
        }

        public DuplicateMedicationException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
