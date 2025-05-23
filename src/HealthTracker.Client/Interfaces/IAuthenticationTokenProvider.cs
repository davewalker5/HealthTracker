namespace HealthTracker.Client.Interfaces
{
    public interface IAuthenticationTokenProvider
    {
        string GetToken();
        void SetToken(string token);
        void ClearToken();
    }
}
