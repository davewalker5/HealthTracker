namespace HealthTracker.Mvc.Models
{
    public class ListViewModelBase<T> : PaginatedViewModelBase<T> where T : class
    {
        public string Message { get; set; }

    }
}