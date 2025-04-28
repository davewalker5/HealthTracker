using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Manager.Entities
{
    [ExcludeFromCodeCoverage]
    public class CommandLineOptionValue
    {
        public CommandLineOption Option { get; set; }
        public List<string> Values { get; private set; } = [];
    }
}
