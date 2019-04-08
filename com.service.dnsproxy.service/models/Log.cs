using System;
using System.Net;

namespace com.service.dnsproxy.service.models
{
    public class Log
    {
        public string[] Destino { get; set; }
        public IPAddress Origem { get; set; }
        public DateTime DataHora { get; set; }

        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < Destino.Length - 1; i++)
                s += Destino[i] + "/";
            s += Destino[Destino.Length - 1];

            return s;
        }
    }
}