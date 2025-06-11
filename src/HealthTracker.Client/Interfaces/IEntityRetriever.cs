namespace HealthTracker.Client.Interfaces
{
    public interface IEntityRetriever<T>
    {
        Task<T> GetAsync(int id);
    }
}