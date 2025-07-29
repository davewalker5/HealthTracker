namespace HealthTracker.Api.Entities
{
    public class ShoppingListExportWorkItem : ExportWorkItem
    {
        public int PersonId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
