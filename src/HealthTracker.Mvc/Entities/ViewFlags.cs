namespace HealthTracker.Mvc.Entities
{
    [Flags]
    public enum ViewFlags
    {
        None = 0x0,
        Editable = 0x01,
        Add = 0x02,
        IncludeInactive = 0x04,
        Export = 0x08,
        FutureDates = 0x10,
        SuppressNoMatches = 0x20,
        Purge = 0x40,
        ListView = Editable | Add | Export
    }
}