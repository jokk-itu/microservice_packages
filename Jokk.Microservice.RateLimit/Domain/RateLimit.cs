using System;

namespace Jokk.Microservice.RateLimit
{
    internal class RateLimit
    {
        private readonly RateLimitConfiguration _config;

        private RateLimitType Minute { get; set; }

        private RateLimitType Hour { get; set; }

        private RateLimitType Day { get; set; }

        public RateLimit(RateLimitConfiguration config)
        {
            _config = config;
        }

        public bool IsLimitReached()
        {
            ResetSurpassedRates();
            return Minute.Value > _config.MinuteMax
                            || Hour.Value > _config.HourMax
                            || Day.Value > _config.DayMax;
        }
        
        public void IncrementValues()
        {
            Minute.Increment();
            Hour.Increment();
            Day.Increment();
        }

        private void ResetSurpassedRates()
        {
            Minute.ResetSurpassedRate(TimeSpan.FromMinutes(1));
            Hour.ResetSurpassedRate(TimeSpan.FromHours(1));
            Day.ResetSurpassedRate(TimeSpan.FromDays(1));
        }

        public override string ToString()
        {
            return $"{Minute}\n, {Hour}\n, \n{Day}";
        }
    }
}