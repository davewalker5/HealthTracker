namespace HealthTracker.DataExchange.Interfaces
{
    public interface IOmronConnectBloodPressureImporter
    {
        Task ImportAsync(string encoded, int personId);
        Task ImportAsync(byte[] content, int personId);
    }
}
