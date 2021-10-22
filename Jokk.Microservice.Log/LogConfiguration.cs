using System.Collections.Generic;

namespace Jokk.Microservice.Log
{
    public class LogConfiguration
    {
        #region LogToConfiguration

        public bool LogToConsole { get; set; }
        public bool LogToUdp { get; set; }
        public bool LogToSeq { get; set; }
        public bool LogToElasticSearch { get; set; }

        #endregion

        #region AccessConfiguration

        public string SeqUrl { get; set; }

        public string ElasticSearchUrl { get; set; }
        public int UdpPort { get; set; }
        public string UdpHost { get; set; }

        #endregion

        public string Service { get; set; }

        public IDictionary<string, string> Overrides { get; set; }
    }
}