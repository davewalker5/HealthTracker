using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Identity
{
    [ExcludeFromCodeCoverage]
    public class User : HealthTrackerEntityBase
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
