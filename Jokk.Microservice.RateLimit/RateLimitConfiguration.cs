namespace Jokk.Microservice.RateLimit
{
    public class RateLimitConfiguration
    {
        public string Host { get; set; }
        
        public int Port { get; set; }
        
        public string Password { get; set; }

        public long MinuteMax {get; set;}
        
        public long HourMax { get; set; }
        
        public long DayMax { get; set; }
    }
}