using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class NutritionalValueNotFoundException : Exception
    {
        public NutritionalValueNotFoundException()
        {
        }

        public NutritionalValueNotFoundException(string message) : base(message)
        {
        }

        public NutritionalValueNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
