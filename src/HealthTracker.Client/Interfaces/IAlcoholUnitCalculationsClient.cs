namespace HealthTracker.Client.Interfaces
{
    public interface IAlcoholUnitCalculationsClient
    {
        Task<decimal> CalculateUnitsAsync(decimal abv, decimal volume);
        Task<decimal> UnitsPerShot(decimal abv);
        Task<decimal> UnitsPerPint(decimal abv);
        Task<decimal> UnitsPerSmallGlass(decimal abv);
        Task<decimal> UnitsPerMediumGlass(decimal abv);
         Task<decimal> UnitsPerLargeGlass(decimal abv);
    }
}