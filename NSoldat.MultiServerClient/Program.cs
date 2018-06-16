using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Binder;
using Newtonsoft.Json;
using NSoldat.Lib;

namespace NSoldat.MultiServerClient
{
    class MultiServerSettings
    {
        public class ServerSettings
        {
            public string Address { get; set; }
            public int Port { get; set; }
            public string Password { get; set; }
        }

        public ServerSettings[] Servers { get; set; }
    }

    class SoldatConnnection
    {
        public MultiServerSettings.ServerSettings Settings { get; private set; }
        public SoldatClient Client { get; private set; }
        
        public static SoldatConnnection Initilize(MultiServerSettings.ServerSettings settings, SoldatClient client)
        {
            var connection = new SoldatConnnection()
            {
                Settings = settings,
                Client = client
            };

            Task.Run(connection.ReadLoop);

            return connection;
        }

        private async Task ReadLoop()
        {
            while (true)
            {
                Log(Client.ReadRaw());
                await Task.Delay(100);
            }
        }

        private void Log(string message)
        {
            Console.WriteLine($"{DateTime.Now.ToShortTimeString()} {Settings.Address}:{Settings.Port} - {message}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.local.json", optional: true);

            var settings = new MultiServerSettings();
            builder.Build().Bind(settings);

            var soldatConnnections = settings.Servers.Select(
                serverSettings =>
                {
                    var client = new SoldatClient(new PacketParser());
                    client.Connect(serverSettings.Address, serverSettings.Port, serverSettings.Password);

                    return SoldatConnnection.Initilize(serverSettings, client);
                })
                .ToArray();
            
            while (true)
            {
                var command = Console.ReadLine();
                if (command == "quit")
                {
                    break;
                }

                foreach (var soldatConnnection in soldatConnnections)
                {
                    soldatConnnection.Client.SendRaw(command);
                }                
            }

        }
    }
}
