using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class MedicationInUseException : Exception
    {
        public MedicationInUseException()
        {
        }

        public MedicationInUseException(string message) : base(message)
        {
        }

        public MedicationInUseException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
