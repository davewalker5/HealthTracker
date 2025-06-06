namespace HealthTracker.Mvc.Models
{
    public class AddFoodSourceViewModel : FoodSourceViewModel
    {
        public string Message { get; set; } = "";

        public AddFoodSourceViewModel()
        {
            FoodSource.Id = 0;
            FoodSource.Name = "";
        }
    }
}