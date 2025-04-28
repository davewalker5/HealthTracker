using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class MedicationNotFoundException : Exception
    {
        public MedicationNotFoundException()
        {
        }

        public MedicationNotFoundException(string message) : base(message)
        {
        }

        public MedicationNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
