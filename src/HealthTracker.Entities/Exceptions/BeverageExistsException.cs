using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class BeverageExistsException : Exception
    {
        public BeverageExistsException()
        {
        }

        public BeverageExistsException(string message) : base(message)
        {
        }

        public BeverageExistsException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
