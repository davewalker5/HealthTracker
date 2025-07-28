using HealthTracker.Client.Interfaces;
using HealthTracker.Enumerations.Enumerations;
using HealthTracker.Mvc.Interfaces;

namespace HealthTracker.Mvc.Controllers
{
    public abstract class DataExchangeControllerBase : HealthTrackerControllerBase
    {
        private readonly Dictionary<DataExchangeType, string> _controllerMap = new()
        {
            { DataExchangeType.SPO2, "BloodOxygenSaturation" },
            { DataExchangeType.BloodPressure, "BloodPressure" },
            { DataExchangeType.Exercise, "Exercise" },
            { DataExchangeType.Glucose, "BloodGlucose" },
            { DataExchangeType.Weight, "Weight" },
            { DataExchangeType.BeverageConsumption, "BeverageConsumption" },
            { DataExchangeType.FoodItems, "FoodItems" },
            { DataExchangeType.Meals, "Meals" },
            { DataExchangeType.MealConsumption, "MealConsumption" },
            { DataExchangeType.PlannedMeals, "PlannedMeal" }
        };

        protected readonly Dictionary<DataExchangeType, IImporterExporter> _clients = new();

        public DataExchangeControllerBase(
            IBloodGlucoseMeasurementClient bloodGlucoseMeasurementClient,
            IBloodOxygenSaturationMeasurementClient bloodOxygenSaturationMeasurementClient,
            IBloodPressureMeasurementClient bloodPressurementMeasurementClient,
            IExerciseMeasurementClient exerciseMeasurementClient,
            IWeightMeasurementClient weightMeasurementClient,
            IBeverageConsumptionMeasurementClient beverageConsumptionMeasurementClient,
            IFoodItemClient foodItemClient,
            IMealClient mealClient,
            IMealConsumptionMeasurementClient mealConsumptionMeasurementClient,
            IPlannedMealClient plannedMealClient,
            IPartialViewToStringRenderer renderer,
            ILogger logger) : base(renderer, logger)
        {
            _clients.Add(DataExchangeType.Glucose, bloodGlucoseMeasurementClient);
            _clients.Add(DataExchangeType.SPO2, bloodOxygenSaturationMeasurementClient);
            _clients.Add(DataExchangeType.BloodPressure, bloodPressurementMeasurementClient);
            _clients.Add(DataExchangeType.Exercise, exerciseMeasurementClient);
            _clients.Add(DataExchangeType.Weight, weightMeasurementClient);
            _clients.Add(DataExchangeType.BeverageConsumption, beverageConsumptionMeasurementClient);
            _clients.Add(DataExchangeType.FoodItems, foodItemClient);
            _clients.Add(DataExchangeType.Meals, mealClient);
            _clients.Add(DataExchangeType.MealConsumption, mealConsumptionMeasurementClient);
            _clients.Add(DataExchangeType.PlannedMeals, plannedMealClient);
        }

        /// <summary>
        /// Return the name of the controller associated a given data exchange type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected string ControllerName(DataExchangeType type)
            => _controllerMap[type];

        /// <summary>
        /// Return the API client associated with a given data exchange type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected IImporterExporter Client(DataExchangeType type)
            => _clients[type];

        /// <summary>
        /// Export a set of measurements
        /// </summary>
        /// <param name="type"></param>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        protected async Task ExportAsync(DataExchangeType type, int personId, DateTime? from, DateTime? to, string fileName)
            => await (_clients[type] as IMeasurementImporterExporter).ExportAsync(personId, from, to, fileName);

        /// <summary>
        /// Export a set of entities
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        protected async Task ExportAsync(DataExchangeType type, string fileName)
            => await (_clients[type] as IDataImporterExporter).ExportAsync(fileName);
    }
}