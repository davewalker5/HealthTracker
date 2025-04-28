using HealthTracker.Entities.Logging;
using HealthTracker.Configuration.Entities;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Configuration.Interfaces
{
    public interface IHealthTrackerApplicationSettings
    {
        string Secret { get; set; }
        string ApiUrl { get; set; }
        string ApiDateFormat { get; set; }
        int TokenLifespanMinutes { get; set; }
        Severity MinimumLogLevel { get; set; }
        string LogFile { get; set; }
        string TokenFile { get; set; }
        List<ApiRoute> ApiRoutes { get; set; }
        string ExportPath { get ; set; }
        int DefaultTimePeriodDays { get; set; }
        string OmronDateColumnTitle { get; set; }
        string OmronSystolicColumnTitle { get; set; }
        string OmronDiastolicColumnTitle { get; set; }
        int MedicationOrderLeadTimeDays { get; set; }
        int MedicationDaysRemainingWarningDays { get; set; }
        int MedicationDaysRemainingCriticalDays { get; set; }
        decimal ShotSize { get; set; }
        decimal SmallGlassSize { get; set; }
        decimal MediumGlassSize { get; set; }
        decimal LargeGlassSize { get; set; }
    }
}
