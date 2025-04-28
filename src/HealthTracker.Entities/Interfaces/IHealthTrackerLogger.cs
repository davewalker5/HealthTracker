using HealthTracker.Entities.Logging;

namespace HealthTracker.Entities.Interfaces
{
    public interface IHealthTrackerLogger
    {
        void Initialise(string logFile, Severity minimumSeverityToLog);
        void LogMessage(Severity severity, string message);
        void LogException(Exception ex);
    }
}
