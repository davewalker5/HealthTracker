using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.DataExchange.Entities
{
    [ExcludeFromCodeCoverage]
    public class ExportableEntityBase
    {
        public const string DateTimeFormat = "dd/MM/yyyy";
        public const string TimestampFormat = "yyyy-MM-dd HH:mm:ss";
    }
}
