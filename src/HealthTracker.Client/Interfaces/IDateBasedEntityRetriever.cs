namespace HealthTracker.Client.Interfaces
{
    public interface IDateBasedEntityRetriever<T> : IEntityRetriever<T>
    {
        Task<List<T>> ListAsync(int personId, DateTime? from, DateTime? to, int pageNumber, int pageSize);
    }
}