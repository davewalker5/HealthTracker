﻿using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Manager.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class MalformedCommandLineException : Exception
    {
        public MalformedCommandLineException()
        {
        }

        public MalformedCommandLineException(string message) : base(message)
        {
        }

        public MalformedCommandLineException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}