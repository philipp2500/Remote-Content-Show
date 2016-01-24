using System;

namespace Agent
{
    /// <summary>
    /// A custom exception used to indicate that a process took too long to start up.
    /// </summary>
    public class ProcessStartupException : Exception
    {
    }
}