using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class DuplicatePersonMedicationException : Exception
    {
        public DuplicatePersonMedicationException()
        {
        }

        public DuplicatePersonMedicationException(string message) : base(message)
        {
        }

        public DuplicatePersonMedicationException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
