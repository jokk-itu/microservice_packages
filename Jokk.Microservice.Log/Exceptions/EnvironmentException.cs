using System;

namespace Jokk.Microservice.Log.Exceptions
{
    internal class EnvironmentException : Exception
    {
        public EnvironmentException()
        {
            
        }
        
        public EnvironmentException(string message) : base(message)
        {
            
        }

        public EnvironmentException(string message, Exception inner) : base(message, inner)
        {
            
        }
    }
}