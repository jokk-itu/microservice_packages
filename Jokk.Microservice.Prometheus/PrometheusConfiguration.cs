using System.Collections.Generic;

namespace Jokk.Microservice.Prometheus
{
    public class PrometheusConfiguration
    {
        public string SqlServerConnectionString { get; set; }

        #region Mongo

        public string MongoConnectionString { get; set; }

        public string MongoDatabase { get; set; }

        public string MongoUsername { get; set; }

        public string MongoPassword { get; set; }

        #endregion

        #region Neo4J

        public string Neo4JConnectionString { get; set; }

        public string Neo4JDatabase { get; set; }

        public string Neo4JUsername { get; set; }

        public string Neo4JPassword { get; set; }

        #endregion

        public IDictionary<string, string> Services { get; set; }
    }
}