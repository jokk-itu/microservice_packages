using System.Collections.Generic;

namespace Jokk.Microservice.Prometheus
{
    public class PrometheusConfiguration
    {
        #region SQLServer
        public string SqlServerConnectionString { get; set; }
        
        #endregion

        #region Mongo

        public string MongoUri { get; set; }

        public string MongoUsername { get; set; }

        public string MongoPassword { get; set; }

        #endregion

        #region Neo4J

        public string Neo4JUri { get; set; }

        public string Neo4JUsername { get; set; }

        public string Neo4JPassword { get; set; }

        #endregion
        
        #region Redis
        
        public string RedisConnectionString { get; set; }
        
        public string RedisDatabase { get; set; }
        
        #endregion
        public IDictionary<string, string> Services { get; set; }
    }
}