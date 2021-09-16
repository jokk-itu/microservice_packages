using System;

namespace Jokk.Microservice.RateLimit
{
    public class RateLimitType
    {
        public long Value { get; set; }

        private DateTime End { get; set; }

        public void ResetSurpassedRate(TimeSpan offset)
        {
            if (DateTime.UtcNow < End) 
                return;
            
            Value = 0;
            End = DateTime.UtcNow + offset;
        }

        public void Increment()
        {
            Value += 1;
        }
    }
}