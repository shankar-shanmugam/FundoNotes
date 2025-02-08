using System;

namespace FundooNotes.Helpers
{
    /// <summary>
    /// Custom exception class for throwing application specific exceptions 
    /// that can be caught and handled within the application
    /// </summary>

    public class AppException : Exception
    {
        public AppException() : base() { }
        public AppException(string message) : base(message) { }
    }
}
