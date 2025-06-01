namespace HealthTracker.Enumerations.Enumerations
{
    // The enum values are persisted to the database so it's essential their values
    // are fixed, even when new ones are added
    public enum TempBeverageMeasure
    {
        None = 0,
        Pint = 1,
        LargeGlass = 2,
        MediumGlass = 3,
        SmallGlass = 4,
        Shot = 5,
        HalfPint = 6
    }
}