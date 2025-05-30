using System.ComponentModel;
using HealthTracker.Enumerations.Enumerations;
using HealthTracker.Enumerations.Extensions;

namespace HealthTracker.Mvc.Models
{
    public class DataExchangeViewModel
    {
        [DisplayName("Data Type")]
        public DataExchangeType DataExchangeType { get; set; }

        public string DataExchangeTypeName { get { return DataExchangeType.ToName(); } }
    }
}