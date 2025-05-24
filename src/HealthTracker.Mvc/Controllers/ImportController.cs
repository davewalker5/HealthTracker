using System.Text;
using HealthTracker.Client.Interfaces;
using HealthTracker.Mvc.Entities;
using HealthTracker.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthTracker.Mvc.Controllers
{
    [Authorize]
    public class ImportController : DataExchangeControllerBase
    {
        private readonly List<string> _validFileTypes = [".csv"];

        public ImportController(
            IBloodGlucoseMeasurementClient bloodGlucoseMeasurementClient,
            IBloodOxygenSaturationMeasurementClient bloodOxygenSaturationMeasurementClient,
            IBloodPressureMeasurementClient bloodPressurementMeasurementClient,
            IExerciseMeasurementClient exerciseMeasurementClient,
            IWeightMeasurementClient weightMeasurementClient,
            ILogger<WeightController> logger) : base(
                bloodGlucoseMeasurementClient,
                bloodOxygenSaturationMeasurementClient,
                bloodPressurementMeasurementClient,
                exerciseMeasurementClient,
                weightMeasurementClient,
                logger
            )
        {
        }

        /// <summary>
        /// Serve the data import page
        /// </summary>
        /// <param name="importType"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            var model = new ImportViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ImportViewModel model)
        {
            string fileName = "";
            string content = "";

            // If the model's nominally valid, perform some additional checks
            if (ModelState.IsValid)
            {
                // Check the data exchange type isn't the default selection, "None"
                _logger.LogDebug($"Import data type = {model.DataExchangeType}");
                if (model.DataExchangeType == DataExchangeType.None)
                {
                    ModelState.AddModelError("DataExchangeType", "You must select an import data type");
                }

                // Check the selected file is of a valid type, based on the extension
                fileName = model.ImportFile.FileName;
                _logger.LogDebug($"Import file name = {fileName}");

                // Check the selected file is of a supported type
                var extension = Path.GetExtension(fileName);
                if (!_validFileTypes.Contains(extension, StringComparer.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("ImportFile", $"'{extension}' is not a valid import file type");
                }
            }

            // If the model's still valid, read the file content
            if (ModelState.IsValid)
            {
                // Read file content to a stream or byte array
                using (var stream = new MemoryStream())
                {
                    await model.ImportFile.CopyToAsync(stream);

                    // You now have the file content as a byte array and convert to a string
                    var bytes = stream.ToArray();
                    _logger.LogDebug($"{bytes.Length} bytes read from {fileName}");

                    if (bytes.Length > 0)
                    {
                        content = Encoding.Default.GetString(bytes);
                    }
                    else
                    {
                        ModelState.AddModelError("ImportFile", $"'{fileName}' contains no data to import");
                    }   
                }
            }

            // If the model's still valid, proceed with the upload
            if (ModelState.IsValid)
            {
                // Request the import
                _logger.LogDebug($"Requesting import of {model.DataExchangeTypeName} data from {fileName}");
                await Client(model.DataExchangeType).ImportFromFileContentAsync(content);

                // Reset the model and set a confirmation message
                ModelState.Clear();
                model.Message = $"Import of {model.DataExchangeTypeName} data from {fileName} has been requested";
                model.DataExchangeType = DataExchangeType.None;
                model.ImportFile = default;
            }
            else
            {
                LogModelStateErrors(_logger);
            }

            return View(model);
        }
    }
}