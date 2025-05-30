using HealthTracker.Client.Interfaces;
using HealthTracker.Enumerations.Enumerations;

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
            { DataExchangeType.Weight, "Weight" }
        };

        protected readonly Dictionary<DataExchangeType, IImporterExporter> _clients = new();
        protected readonly ILogger _logger;

        public DataExchangeControllerBase(
            IBloodGlucoseMeasurementClient bloodGlucoseMeasurementClient,
            IBloodOxygenSaturationMeasurementClient bloodOxygenSaturationMeasurementClient,
            IBloodPressureMeasurementClient bloodPressurementMeasurementClient,
            IExerciseMeasurementClient exerciseMeasurementClient,
            IWeightMeasurementClient weightMeasurementClient,
            ILogger logger)
        {
            _clients.Add(DataExchangeType.Glucose, bloodGlucoseMeasurementClient);
            _clients.Add(DataExchangeType.SPO2, bloodOxygenSaturationMeasurementClient);
            _clients.Add(DataExchangeType.BloodPressure, bloodPressurementMeasurementClient);
            _clients.Add(DataExchangeType.Exercise, exerciseMeasurementClient);
            _clients.Add(DataExchangeType.Weight, weightMeasurementClient);
            _logger = logger;
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
    }
}