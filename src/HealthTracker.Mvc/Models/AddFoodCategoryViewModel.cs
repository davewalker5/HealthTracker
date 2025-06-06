namespace HealthTracker.Mvc.Models
{
    public class AddFoodCategoryViewModel : FoodCategoryViewModel
    {
        public string Message { get; set; } = "";

        public AddFoodCategoryViewModel()
        {
            FoodCategory.Id = 0;
            FoodCategory.Name = "";
        }
    }
}