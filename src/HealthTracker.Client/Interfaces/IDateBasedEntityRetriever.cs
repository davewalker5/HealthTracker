namespace HealthTracker.Client.Interfaces
{
    public interface IDateBasedEntityRetriever<T>
    {
        Task<T> GetAsync(int id);
        Task<List<T>> ListAsync(int personId, DateTime? from, DateTime? to, int pageNumber, int pageSize);
    }
}