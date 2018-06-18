using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using NSoldat.Lib;

namespace NSoldat.MultiServerClient
{
    class ClientMonitor
    {
        public SoldatClient Client { get; private set; }


        public static ClientMonitor Run(SoldatClient client)
        {
            var connection = new ClientMonitor()
            {
                Client = client
            };

            Task.Run(connection.ReadLoop);

            return connection;
        }

        private async Task ReadLoop()
        {
            while (true)
            {
                Log(await Client.ReadLine());
                await Task.Delay(100);
            }
        }

        private void Log(string message)
        {
            Console.WriteLine($"{DateTime.Now.ToShortTimeString()} {Client.Address}:{Client.Port} - {message}");
        }
    }
}