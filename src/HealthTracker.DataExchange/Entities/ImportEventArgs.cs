using System;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.DataExchange.Entities
{
    [ExcludeFromCodeCoverage]
    public class ImportEventArgs<T> : EventArgs where T : class
    {
        public long RecordCount { get; set; }
        public T Entity { get; set; }
    }
}
