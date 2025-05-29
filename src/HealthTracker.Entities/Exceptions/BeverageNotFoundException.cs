using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class BeverageNotFoundException : Exception
    {
        public BeverageNotFoundException()
        {
        }

        public BeverageNotFoundException(string message) : base(message)
        {
        }

        public BeverageNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
