using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using HealthTracker.Entities.Food;
using HealthTracker.Enumerations.Enumerations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Models
{
    public class PlannedMealViewModel : SelectedFiltersViewModel
    {
        public IList<SelectListItem> People { get; set; }
        public IList<SelectListItem> FoodSources { get; set; }
        public IList<SelectListItem> MealTypes { get; private set; }

        [DisplayName("Date")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "You must provide a date")]
        public DateTime Date { get; set; }

        public PlannedMeal PlannedMeal { get; set; }
        public string Action { get; set; }

        public PlannedMealViewModel()
        {
            // Build the list of meal types, starting with a default "empty" entry followed by the list of meal types
            // from the enumeration
            MealTypes = [ new SelectListItem() { Text = "", Value = "0" } ];
            foreach (var value in Enum.GetValues<MealType>())
            {
                MealTypes.Add(new SelectListItem() { Text = value.ToString(), Value = ((int)value).ToString() });
            }
        }

        /// <summary>
        /// Create a new scheduled meal
        /// </summary>
        public void CreatePlannedMeal()
            => SetPlannedMeal(new()
            {
                Meal = new(),
                Date = DateTime.Now
            });

        /// <summary>
        /// Create a new scheduled meal with a meal specified
        /// </summary>
        /// <param name="meal"></param>
        public void CreatePlannedMeal(Meal meal)
            => SetPlannedMeal(new()
            {
                Meal = meal,
                MealId = meal.Id,
                Date = DateTime.Now
            });

        /// <summary>
        /// Set the scheduled meal that's being edited/added
        /// </summary>
        /// <param name="plannedMeal"></param>
        public void SetPlannedMeal(PlannedMeal plannedMeal)
        {
            PlannedMeal = plannedMeal;
            Date = plannedMeal.Date;
        }

        /// <summary>
        /// Return a date from the date field
        /// </summary>
        /// <returns></returns>
        // TODO:
        public DateTime ScheduledDate()
            => Date;
    }
}