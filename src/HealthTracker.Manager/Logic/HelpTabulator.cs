using HealthTracker.Manager.Entities;
using HealthTracker.Manager.Interfaces;
using Spectre.Console;

namespace HealthTracker.Manager.Logic
{
    public class HelpTabulator : IHelpGenerator
    {
        /// <summary>
        /// Tabulate a collection of available command line options
        /// </summary>
        /// <param name="options"></param>
        public void Generate(IEnumerable<CommandLineOption> options)
        {
            var table = new Table();

            table.AddColumn("Option");
            table.AddColumn("Short Form");
            table.AddColumn("Description");
            table.AddColumn("Parameters");

            foreach (var option in options)
            {
                var rowData = new string[] {
                    GetCellData(option.Name),
                    GetCellData(option.ShortName),
                    GetCellData(option.Description),
                    GetCellData(option.ParameterDescription)
                };

                table.AddRow(rowData);
            }

            AnsiConsole.Write(table);
        }

        private string GetCellData(string value)
            => $"[white]{value}[/]";
    }
}
