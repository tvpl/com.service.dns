using System;

namespace com.service.dnsproxy.service.models
{
    public class LogRequest
    {
        public string Destino { get; set; }
        public string Origem { get; set; }
        public DateTime DataHora { get; set; }
    }
}
