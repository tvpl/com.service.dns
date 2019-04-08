using com.service.dnsproxy.service.util;
using System.Diagnostics;
using System.Net.Sockets;
using System.ServiceProcess;

namespace com.service.dnsproxy.service
{
    public partial class DnsProxyService : ServiceBase
    {
        //private Timer _timer;

        public DnsProxyService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            UdpClient c = new UdpClient(53);
            c.BeginReceive(ClienteUdp.ClienteUdpCallback, c);
        }

        protected override void OnStop()
        {
        }

        private void eventLog_EntryWritten(object sender, EntryWrittenEventArgs e)
        {

        }
    }
}
