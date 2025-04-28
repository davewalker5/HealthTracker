using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class ActivityTypeNotFoundException : Exception
    {
        public ActivityTypeNotFoundException()
        {
        }

        public ActivityTypeNotFoundException(string message) : base(message)
        {
        }

        public ActivityTypeNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
