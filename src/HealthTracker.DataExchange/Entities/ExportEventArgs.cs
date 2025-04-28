using System;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.DataExchange.Entities
{
    [ExcludeFromCodeCoverage]
    public class ExportEventArgs<T> : EventArgs where T : class
    {
        public long RecordCount { get; set; }
        public T RecordSource { get; set; }
    }
}
