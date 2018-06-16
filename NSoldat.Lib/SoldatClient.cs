using System;
using System.IO;
using System.Net.Sockets;

namespace NSoldat.Lib
{
    public class SoldatClient
    {
        private TcpClient _tcpClient;
        private readonly IPacketParser _packetParser;

        public SoldatClient(IPacketParser packetParser)
        {
            _packetParser = packetParser;
        }

        private Stream OutboundStream => _tcpClient.GetStream();

        public void Connect(string address, int port, string adminPassword)
        {
            _tcpClient?.Close();
            _tcpClient = new TcpClient("pila.fp.lan", 23073);

            var networkStream = _tcpClient.GetStream();

            networkStream.ReadLine().Echo();

            networkStream.WriteLine(adminPassword);

            networkStream.ReadLine().Echo();
            networkStream.ReadLine().Echo();
            networkStream.ReadLine().Echo();

            networkStream.WriteLine("INFO");
            networkStream.ReadLine().Echo();
        }

        private void AssertConnected()
        {
            if (_tcpClient == null)
            {
                throw new InvalidOperationException("Connect first");
            }
        }

        public RefreshPacket Refresh()
        {
            AssertConnected();
            var stream = OutboundStream;

            stream.WriteLine("REFRESH");

            if (!stream.ReadUntil("REFRESH\r\n"))
            {
                System.Console.WriteLine("Something went wrong");
                return null;
            }

            var refreshPacketLength = 1188;
            var packet = new byte[refreshPacketLength];
            var read = stream.Read(packet, 0, refreshPacketLength);
            if (read != refreshPacketLength)
            {
                System.Console.WriteLine("Not everything read");
            }

            var refreshPacket = _packetParser.ParseRefreshPacket(packet, 0);

            return refreshPacket;
        }

        public ServerEvent ReadNextEvent()
        {
            AssertConnected();
            var input = OutboundStream.ReadLine().Echo();
            var refresh = Refresh();

            return SingleEventParser.Parse(input, refresh);
        }
    }
}