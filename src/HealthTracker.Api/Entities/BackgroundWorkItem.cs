namespace HealthTracker.Api.Entities
{
    public class BackgroundWorkItem
    {
        public string JobName { get; set; }

        public override string ToString()
        {
            return $"JobName = {JobName}";
        }
    }
}
