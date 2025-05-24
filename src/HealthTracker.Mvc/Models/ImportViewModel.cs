using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using HealthTracker.Mvc.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Models
{
    public class ImportViewModel : DataExchangeViewModel
    {
        public List<SelectListItem> ImportTypes { get; private set; } = [];

        [DisplayName("Selected File")]
        [Required(ErrorMessage = "You must select a file for import")]
        public IFormFile ImportFile { get; set; }

        public string Message { get; set; } = "";

        public ImportViewModel()
        {
            foreach (var importType in Enum.GetValues<DataExchangeType>())
            {
                var importTypeName = _typeNameMap[importType];
                ImportTypes.Add(new SelectListItem() { Text = $"{importTypeName}", Value = importType.ToString() });
            }
        }
    }
}