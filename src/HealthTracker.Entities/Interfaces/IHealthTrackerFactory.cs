using Microsoft.EntityFrameworkCore;

namespace HealthTracker.Entities.Interfaces
{
    public interface IHealthTrackerFactory
    {
        DbContext Context { get; }
        IHealthTrackerLogger Logger { get; }

        IPersonManager People { get; }
        IBloodPressureMeasurementManager BloodPressureMeasurements { get; }
        IBloodOxygenSaturationMeasurementManager BloodOxygenSaturationMeasurements { get; }
        IBloodGlucoseMeasurementManager BloodGlucoseMeasurements { get; }
        IBloodPressureBandManager BloodPressureBands { get; }
        IBloodPressureAssessor BloodPressureAssessor { get; }
        IBloodOxygenSaturationBandManager BloodOxygenSaturationBands { get; }
        IBloodOxygenSaturationAssessor BloodOxygenSaturationAssessor { get; }
        ICholesterolMeasurementManager CholesterolMeasurements { get; }
        IWeightMeasurementManager WeightMeasurements { get; }
        IBMIBandManager BMIBands { get; }
        IActivityTypeManager ActivityTypes { get; }
        IExerciseMeasurementManager ExerciseMeasurements { get; }
        IExerciseCalculator ExerciseCalculator { get; }
        IUserManager Users { get; }
        IJobStatusManager JobStatuses { get; }

        IMedicationManager Medications { get; }
        IPersonMedicationManager PersonMedications { get; }
        IMedicationStockUpdater MedicationStockUpdater { get; }
        IMedicationActionGenerator MedicationActionGenerator { get; }

        IBloodPressureCalculator BloodPressureCalculator { get; }
        IBloodOxygenSaturationCalculator BloodOxygenSaturationCalculator { get; }
        IWeightCalculator WeightCalculator { get; }

        IAlcoholUnitsCalculator AlcoholUnitsCalculator { get; }
    }
}