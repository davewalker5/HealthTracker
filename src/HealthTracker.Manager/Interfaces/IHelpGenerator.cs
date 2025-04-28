using HealthTracker.Manager.Entities;

namespace HealthTracker.Manager.Interfaces
{
    public interface IHelpGenerator
    {
        void Generate(IEnumerable<CommandLineOption> options);
    }
}
