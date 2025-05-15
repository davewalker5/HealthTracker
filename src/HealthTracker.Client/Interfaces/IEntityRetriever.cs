namespace HealthTracker.Client.Interfaces
{
    public interface IEntityRetriever<T>
    {
        Task<T> Get(int id);
    }
}