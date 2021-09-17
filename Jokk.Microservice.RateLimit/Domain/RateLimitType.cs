using System;

namespace Jokk.Microservice.RateLimit
{
    internal class RateLimitType
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

        public override string ToString()
        {
            return $"Value: {Value} & End: {End}";
        }
    }
}