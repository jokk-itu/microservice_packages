using System;

namespace Jokk.Microservice.RateLimit.Exceptions
{
    public class UpdateConcurrencyException : Exception
    {
        public UpdateConcurrencyException(string message) : base(message)
        {
            
        }

        public UpdateConcurrencyException(string message, Exception innerException) : base(message, innerException)
        {
            
        }
    }
}