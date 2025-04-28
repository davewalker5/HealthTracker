namespace HealthTracker.Api.Entities
{
    public class BloodOxygenSaturationMeasurementExportWorkItem : ExportWorkItem
    {
        public int PersonId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
