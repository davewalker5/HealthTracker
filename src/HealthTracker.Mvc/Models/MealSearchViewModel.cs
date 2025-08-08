using HealthTracker.Entities.Food;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Models
{
    public class MealSearchViewModel : MealListViewModel
    {
        public IList<SelectListItem> Sources { get; set; } = [];
        public IList<SelectListItem> Categories { get; set; } = [];
        public MealSearchCriteria Criteria { get; set; } = new();
    }
}