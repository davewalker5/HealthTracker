namespace HealthTracker.Api.Entities
{
    public class ExportWorkItem : BackgroundWorkItem
    {
        public string FileName { get; set; }
        public override string ToString()
        {
            return $"{base.ToString()}, FileName = {FileName}";
        }
    }
}
