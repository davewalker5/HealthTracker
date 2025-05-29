using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class BeverageInUseException : Exception
    {
        public BeverageInUseException()
        {
        }

        public BeverageInUseException(string message) : base(message)
        {
        }

        public BeverageInUseException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
