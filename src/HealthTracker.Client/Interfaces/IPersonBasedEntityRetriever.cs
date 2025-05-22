namespace HealthTracker.Client.Interfaces
{
    public interface IPersonBasedEntityRetriever<T>
    {
        Task<T> GetAsync(int id);
        Task<List<T>> ListAsync(int personId, int pageNumber, int pageSize);
    }
}