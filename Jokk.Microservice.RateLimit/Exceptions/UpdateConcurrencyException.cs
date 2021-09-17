using System;

namespace Jokk.Microservice.RateLimit
{
    internal class UpdateConcurrencyException : Exception
    {
        public UpdateConcurrencyException(string message) : base(message)
        {
            
        }

        public UpdateConcurrencyException(string message, Exception innerException) : base(message, innerException)
        {
            
        }
    }
}