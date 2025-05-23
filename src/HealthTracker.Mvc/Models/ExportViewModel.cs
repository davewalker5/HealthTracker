using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using HealthTracker.Mvc.Entities;

namespace HealthTracker.Mvc.Models
{
    public class ExportViewModel
    {
        private readonly Dictionary<ExportType, string> _typeNameMap = new()
        {
            { ExportType.SPO2, "% SPO2" },
            { ExportType.BloodPressure, "Blood Pressure" },
            { ExportType.Exercise, "Exercise" },
            { ExportType.Glucose, "Glucose" },
            { ExportType.Weight, "Weight" }
        };

        public string Action { get; set; }
        public string Message { get; set; }
        public string BackButtonLabel { get; set; } = "Cancel";
        public string PersonName { get; set; }
        public string ExportTypeName { get { return _typeNameMap[ExportType]; }}

        [DisplayName("File Name")]
        [Required(ErrorMessage = "You must provide an export file name")]
        public string FileName { get; set; }

        [DisplayName("Person")]
        public int PersonId { get; set; }

        [DisplayName("From")]
        public DateTime From { get; set; }

        [DisplayName("To")]
        public DateTime To { get; set; }

        [DisplayName("Export Type")]
        public ExportType ExportType { get; set; }
    }
}