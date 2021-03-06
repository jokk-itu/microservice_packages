using System;

namespace Jokk.Microservice.Log.Exceptions
{
    public class EnvironmentException : Exception
    {
        public EnvironmentException() : base() {}
        
        public EnvironmentException(string message) : base(message) {}

        public EnvironmentException(string message, Exception inner) : base(message, inner) {}
    }
}