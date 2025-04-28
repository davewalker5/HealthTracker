using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class ActivityTypeExistsException : Exception
    {
        public ActivityTypeExistsException()
        {
        }

        public ActivityTypeExistsException(string message) : base(message)
        {
        }

        public ActivityTypeExistsException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
