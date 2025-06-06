using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Food
{
    [ExcludeFromCodeCoverage]
    public class FoodCategory : NamedEntity
    {
        public ICollection<FoodItem> FoodItems { get; set; } = new List<FoodItem>();
    }
}