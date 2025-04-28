namespace HealthTracker.Api.Entities
{
    public class BloodPressureMeasurementExportWorkItem : ExportWorkItem
    {
        public int PersonId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
