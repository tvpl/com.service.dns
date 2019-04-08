using com.service.dnsproxy.service.models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;

namespace com.service.dnsproxy.service.util
{
    public static class ClienteUdp
    {
        private static readonly HttpClient client = new HttpClient();

        public static void ClienteUdpCallback(IAsyncResult ar)
        {
            Log entrada = new Log();
            entrada.DataHora = DateTime.Now;
            UdpClient cliente = ((UdpClient)ar.AsyncState);
            UdpClient servidor = new UdpClient();
            IPEndPoint epc = new IPEndPoint(IPAddress.Any, 53);
            IPEndPoint eps = new IPEndPoint(IPAddress.Any, 53);
            byte[] request = cliente.EndReceive(ar, ref epc);

            entrada.Origem = epc.Address;
            entrada.Destino = ParseDomainName(request);

            try
            {
                LogRequest logRequest = new LogRequest()
                {
                    Origem = entrada.Origem.ToString(),
                    Destino = entrada.ToString(),
                    DataHora = entrada.DataHora
                };

                var client = new RestClient("https://dnsproxy.azurewebsites.net");

                var requisicao = new RestRequest("api/Log", Method.POST);
                requisicao.AddJsonBody(logRequest);

                IRestResponse resposta = client.Execute(requisicao);

                if (!resposta.IsSuccessful)
                {
                    StreamWriter vWriter = new StreamWriter(@"c:\logs\log_envioServer_erro.txt", true);
                    vWriter.WriteLine("Sucesso? " + resposta.IsSuccessful + " -> Erro: " + resposta.ErrorMessage);
                    vWriter.Flush();
                    vWriter.Close();
                }
            }
            catch (Exception e)
            {
                StreamWriter vWriter2 = new StreamWriter(@"c:\logs\dns_envioServer_erro.txt", true);
                vWriter2.WriteLine(e);
                vWriter2.Flush();
                vWriter2.Close();
            }

            try
            {
                StreamWriter vWriter = new StreamWriter(@"c:\logs\log.txt", true);
                vWriter.WriteLine("entrada: {0}", entrada.DataHora.ToString(CultureInfo.InvariantCulture) + " -> " + entrada.Origem.ToString() + " -> " + entrada.ToString());
                vWriter.Flush();
                vWriter.Close();
            }
            catch (Exception e)
            {
                StreamWriter vWriter2 = new StreamWriter(@"c:\logs\dns_logfile_erro.txt", true);
                vWriter2.WriteLine(e);
                vWriter2.Flush();
                vWriter2.Close();
            }

            try
            {
                //Da continuidade ao processo repassando a requisição para um DNS verdadeiro
                servidor.Send(
                    request,
                    request.Length,
                    new IPEndPoint(new IPAddress(new byte[] { 8, 8, 8, 8 }), 53));
                byte[] response = servidor.Receive(ref eps);
                servidor.Close();

                cliente.Send(response, response.Length, epc);
                cliente.Close();
            }
            catch (Exception e)
            {
                StreamWriter vWriter2 = new StreamWriter(@"c:\logs\dns_servidorsend_erro.txt", true);
                vWriter2.WriteLine(e);
                vWriter2.Flush();
                vWriter2.Close();
            }

            UdpClient c = new UdpClient(53);
            c.BeginReceive(ClienteUdpCallback, c);
        }

        public static string[] ParseDomainName(byte[] request)
        {
            List<string> domainName = new List<string>();

            int i = 12;
            while (request[i] > 0)
            {
                domainName.Add(ASCIIEncoding.ASCII.GetString(request, i + 1, request[i]));
                i += request[i] + 1;
            }
            
            domainName.Reverse();
            return domainName.ToArray();
        }
    }
}