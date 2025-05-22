namespace HealthTracker.Mvc.Interfaces
{
    public interface IFiltersViewModel : IPersonFilterViewModel
    {
        DateTime From { get; set; }
        DateTime To { get; set; }
    }
}