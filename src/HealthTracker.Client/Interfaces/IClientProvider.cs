namespace HealthTracker.Client.Interfaces
{
    public interface IClientProvider
    {
        IActivityTypeClient ActivityTypeClient { get; }
        IAlcoholUnitCalculationsClient AlcoholUnitCalculationsClient { get; }
        IAuthenticationClient AuthenticationClient { get; }
        IBloodOxygenSaturationMeasurementClient BloodOxygenSaturationClient { get; }
        IBloodPressureMeasurementClient BloodPressureClient { get; }
        ICholesterolMeasurementClient CholesterolClient { get; }
        IExerciseMeasurementClient ExerciseClient { get; }
        IMedicationClient MedicationClient { get; }
        IMedicationTrackingClient MedicationTrackingClient { get; }
        IPersonClient PersonClient { get; }
        IPersonMedicationClient PersonMedicationClient { get; }
        IWeightMeasurementClient WeightClient { get; }
    }
}