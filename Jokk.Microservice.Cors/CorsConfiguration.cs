using System.Collections.Generic;

namespace Jokk.Microservice.Cors
{
    public class CorsConfiguration
    {
        public ICollection<string> Services { get; set; }

        public ICollection<string> Methods { get; set; }
    }
}