namespace HealthTracker.Client.Interfaces
{
    public interface IOperationResultHandler
    {
        public void HandleResults(object output);
        public void HandleMessage(string message);
    }
}