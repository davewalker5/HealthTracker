using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using HealthTracker.Mvc.Entities;

namespace HealthTracker.Mvc.Models
{
    public class DataExchangeViewModel
    {
        protected readonly Dictionary<DataExchangeType, string> _typeNameMap = new()
        {
            { DataExchangeType.None, "" },
            { DataExchangeType.SPO2, "% SPO2" },
            { DataExchangeType.BloodPressure, "Blood Pressure" },
            { DataExchangeType.Exercise, "Exercise" },
            { DataExchangeType.Glucose, "Glucose" },
            { DataExchangeType.Weight, "Weight" }
        };

        [DisplayName("Data Type")]
        public DataExchangeType DataExchangeType { get; set; }

        public string DataExchangeTypeName { get { return _typeNameMap[DataExchangeType]; } }
    }
}