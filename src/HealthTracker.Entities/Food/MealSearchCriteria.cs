namespace HealthTracker.Entities.Food
{
    public class MealSearchCriteria
    {
        public int? FoodSourceId { get; set; }
        public int? FoodCategoryId { get; set; }
        public string MealName { get; set; }
        public string FoodItemName { get; set; }
    }
}