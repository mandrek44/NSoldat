using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace NSoldat.Lib
{
    public enum SoldatTeam
    {
        Alpha = 1,
        Bravo = 2,
        Charlie = 3,
        Delta = 4,
        NoTeam = 255
    }

    public class SoldatClient
    {
        private TcpClient _tcpClient;
        private readonly IPacketParser _packetParser;
        private readonly ConcurrentQueue<Action<Stream>> _commandsQueue = new ConcurrentQueue<Action<Stream>>();
        private readonly ConcurrentQueue<string> _readLinesQueue = new ConcurrentQueue<string>();

        public SoldatClient(IPacketParser packetParser)
        {
            _packetParser = packetParser;
        }

        public event Action<string> ServerLineReceived;

        public void Connect(string address, int port, string adminPassword)
        {
            _tcpClient?.Close();
            _tcpClient = new TcpClient("pila.fp.lan", port);

            var networkStream = _tcpClient.GetStream();

            networkStream.ReadLine().Result.Echo();

            networkStream.WriteLine(adminPassword);

            networkStream.ReadLine().Result.Echo();
            networkStream.ReadLine().Result.Echo();
            networkStream.ReadLine().Result.Echo();

            networkStream.WriteLine("INFO");
            networkStream.ReadLine().Result.Echo();

            //_tcpClient.GetStream().ReadTimeout = 1;

            Task.Run(() => ClientLoop());
        }

        private void ClientLoop()
        {
            var stream = _tcpClient.GetStream();
            var lineCompletion = new TaskCompletionSource<RefreshPacket>();

            while (true)
            {
                if (stream.DataAvailable)
                {
                    _readLinesQueue.Enqueue(stream.ReadLine().Result);
                }
                else if (_commandsQueue.TryDequeue(out var command))
                {
                    command(stream);
                }
                else
                {
                    Thread.Sleep(50);
                }
            }
        }

        private void AssertConnected()
        {
            if (_tcpClient == null)
            {
                throw new InvalidOperationException("Connect first");
            }
        }

        public async Task<RefreshPacket> Refresh()
        {
            var tcs = new TaskCompletionSource<RefreshPacket>();

            _commandsQueue.Enqueue(stream =>
            {
                stream.WriteLine("REFRESH");

                if (!stream.ReadUntil("REFRESH\r\n").Result)
                {
                    System.Console.WriteLine("Something went wrong");
                    tcs.SetResult(null);
                    return;
                }

                var refreshPacketLength = 1188;
                var packet = new byte[refreshPacketLength];
                var read = stream.Read(packet, 0, refreshPacketLength);
                if (read != refreshPacketLength)
                {
                    System.Console.WriteLine("Not everything read");
                }

                var refreshPacket = _packetParser.ParseRefreshPacket(packet, 0);
                tcs.SetResult(refreshPacket);

            });

            return await tcs.Task;
        }

        public async Task SwapTeams()
        {
            AssertConnected();

            const int noTeam = 0b11111111;

            var refreshPacket = await Refresh();
            if (refreshPacket == null)
            {
                return;
            }

            for (int playerIndex = 0; playerIndex < refreshPacket.Team.Length; ++playerIndex)
            {
                var team = (SoldatTeam)refreshPacket.Team[playerIndex];
                if (team == SoldatTeam.NoTeam)
                {
                    continue;
                }

                var playerNumber = refreshPacket.Number[playerIndex];
                if (team != SoldatTeam.Alpha)
                {
                    SetTeam(playerNumber, (int)SoldatTeam.Alpha);
                }
                else
                {
                    SetTeam(playerNumber, (int)SoldatTeam.Bravo);
                }
            }
        }

        public void SetTeam(int playerNumber, int teamNumber)
        {
            AssertConnected();

            _commandsQueue.Enqueue(stream => stream.WriteLine($"/setteam{teamNumber} {playerNumber}"));
        }

        public async Task SetTeam(string playerName, SoldatTeam team)
        {
            AssertConnected();

            var refreshPacket = await Refresh();

            _commandsQueue.Enqueue(stream =>
            {
                var playerNumber = refreshPacket.Number[refreshPacket.PlayerName.ToList().IndexOf(playerName)];
                var teamNumber = (int)team;
                stream.WriteLine($"/setteam{teamNumber} {playerNumber}");
            });
        }

        public async Task<ServerEvent> ReadNextEvent()
        {
            AssertConnected();

            var input = await ReadLine();
            var refresh = await Refresh();

            return SingleEventParser.Parse(input, refresh);
        }

        public void SendRaw(string command)
        {
            _commandsQueue.Enqueue(stream => stream.WriteLine(command));
        }

        public async Task<string> ReadLine()
        {
            return await Task.Run(async () =>
            {
                while (true)
                {
                    if (_readLinesQueue.TryDequeue(out var result))
                        return result;
                    await Task.Delay(100);
                }
            });
        }

        protected virtual void OnServerLineReceived(string line)
        {
            ServerLineReceived?.Invoke(line);
        }
    }
}