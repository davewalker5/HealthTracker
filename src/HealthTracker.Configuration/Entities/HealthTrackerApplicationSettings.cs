using HealthTracker.Entities.Logging;
using HealthTracker.Configuration.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Configuration.Entities
{
    [ExcludeFromCodeCoverage]
    public class HealthTrackerApplicationSettings : IHealthTrackerApplicationSettings
    {
        public string Secret { get; set; }
        public string ApiUrl { get; set; }
        public string ApiDateFormat { get; set; }
        public int TokenLifespanMinutes { get; set; }
        public Severity MinimumLogLevel { get; set; }
        public string LogFile { get; set; }
        public string TokenFile { get; set; }
        public List<ApiRoute> ApiRoutes { get; set; }
        public string ExportPath { get ; set; }
        public int DefaultTimePeriodDays { get; set; }
        public string OmronDateColumnTitle { get; set; }
        public string OmronSystolicColumnTitle { get; set; }
        public string OmronDiastolicColumnTitle { get; set; }
        public int MedicationOrderLeadTimeDays { get; set; }
        public int MedicationDaysRemainingWarningDays { get; set; }
        public int MedicationDaysRemainingCriticalDays { get; set; }
        public decimal ShotSize { get; set; }
        public decimal SmallGlassSize { get; set; }
        public decimal MediumGlassSize { get; set; }
        public decimal LargeGlassSize { get; set; }
        public int ResultsPageSize { get; set; }
    }
}
