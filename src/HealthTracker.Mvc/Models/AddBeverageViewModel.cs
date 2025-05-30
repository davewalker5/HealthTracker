namespace HealthTracker.Mvc.Models
{
    public class AddBeverageViewModel : BeverageViewModel
    {
        public string Message { get; set; } = "";

        public AddBeverageViewModel()
        {
            Beverage.Id = 0;
            Beverage.Name = "";
            Beverage.TypicalABV = 0M;
        }
    }
}