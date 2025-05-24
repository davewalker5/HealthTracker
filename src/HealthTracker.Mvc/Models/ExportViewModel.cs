using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using HealthTracker.Mvc.Entities;

namespace HealthTracker.Mvc.Models
{
    public class ExportViewModel : DataExchangeViewModel
    {
        public string Action { get; set; }
        public string Message { get; set; }
        public string BackButtonLabel { get; set; } = "Cancel";
        public string PersonName { get; set; }

        [DisplayName("File Name")]
        [Required(ErrorMessage = "You must provide an export file name")]
        public string FileName { get; set; }

        [DisplayName("Person")]
        public int PersonId { get; set; }

        [DisplayName("From")]
        public DateTime From { get; set; }

        [DisplayName("To")]
        public DateTime To { get; set; }
    }
}