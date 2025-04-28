using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Configuration.Entities
{
    [ExcludeFromCodeCoverage]
    public class ApiRoute
    {
        public string Name { get; set; }
        public string Route { get; set; }
    }
}
