using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HealthTracker.Mvc.Models
{
    public class ExportViewModel : DataExchangeViewModel
    {
        public string Action { get; set; }
        public string Message { get; set; }
        public string BackButtonLabel { get; set; } = "Cancel";

        [DisplayName("File Name")]
        [Required(ErrorMessage = "You must provide an export file name")]
        public string FileName { get; set; }
    }
}