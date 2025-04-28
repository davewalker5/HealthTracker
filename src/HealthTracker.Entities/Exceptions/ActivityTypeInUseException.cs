using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class ActivityTypeInUseException : Exception
    {
        public ActivityTypeInUseException()
        {
        }

        public ActivityTypeInUseException(string message) : base(message)
        {
        }

        public ActivityTypeInUseException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
