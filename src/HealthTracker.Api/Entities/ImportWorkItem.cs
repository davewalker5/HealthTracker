namespace HealthTracker.Api.Entities
{
    public class ImportWorkItem : BackgroundWorkItem
    {
        public string Content { get; set; }
        public override string ToString()
        {
            return $"{base.ToString()}, Content Length = {Content.Length}";
        }
    }
}
