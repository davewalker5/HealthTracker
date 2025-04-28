using HealthTracker.Manager.Entities;

namespace HealthTracker.Manager.Interfaces
{
    public interface ICommandLineParser
    {
        void Add(
            CommandLineOptionType optionType,
            bool isOperation,
            string name,
            string shortName,
            string description,
            string parameterDescription,
            int minimumNumberOfValues,
            int maximumNumberOfValues);

        List<string> GetValues(CommandLineOptionType optionType);
        bool IsPresent(CommandLineOptionType optionType);
        IList<CommandLineOptionType> GetSpecifiedOptions();
        void Help();
        void Parse(IEnumerable<string> args);
    }
}
