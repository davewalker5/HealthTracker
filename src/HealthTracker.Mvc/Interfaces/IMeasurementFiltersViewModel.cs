namespace HealthTracker.Mvc.Interfaces
{
    public interface IMeasurementFiltersViewModel
    {
        string PersonName { get; set; }
        DateTime From { get; set; }
        DateTime To { get; set; }
    }
}