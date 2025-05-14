namespace HealthTracker.Client.Interfaces
{
    public interface IMeasurementRetriever<T>
    {
        Task<T> GetMeasurement(int id);
    }
}