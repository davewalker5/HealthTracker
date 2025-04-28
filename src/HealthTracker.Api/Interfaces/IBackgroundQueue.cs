using HealthTracker.Api.Entities;

namespace HealthTracker.Api.Interfaces
{
    public interface IBackgroundQueue<T> where T : BackgroundWorkItem
    {
        T Dequeue();
        void Enqueue(T item);
    }
}