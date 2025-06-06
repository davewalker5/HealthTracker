﻿namespace HealthTracker.Configuration.Interfaces
{
    public interface IConfigReader<T> where T : class
    {
        T Read(string jsonFileName);
    }
}
