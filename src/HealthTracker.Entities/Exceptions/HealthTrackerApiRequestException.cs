using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class HealthTrackerApiRequestException : Exception
    {
        public HealthTrackerApiRequestException()
        {
        }

        public HealthTrackerApiRequestException(string message) : base(message)
        {
        }

        public HealthTrackerApiRequestException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
