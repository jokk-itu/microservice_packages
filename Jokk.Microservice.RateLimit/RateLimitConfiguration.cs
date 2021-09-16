namespace Jokk.Microservice.RateLimit
{
    public class RateLimitConfiguration
    {
        public string RedisConnectionString { get; set; }
        
        public long MinuteMax {get; set;}
        
        public long HourMax { get; set; }
        
        public long DayMax { get; set; }
    }
}