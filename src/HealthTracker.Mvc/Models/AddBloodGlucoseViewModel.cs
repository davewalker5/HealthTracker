using HealthTracker.Mvc.Entities;

namespace HealthTracker.Mvc.Models
{
    public class AddBloodGlucoseViewModel : BloodGlucoseViewModel
    {
        public string Message { get; set; } = "";
    }
}