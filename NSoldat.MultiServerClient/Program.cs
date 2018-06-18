using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using NSoldat.Lib;

namespace NSoldat.MultiServerClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = LoadSettings();

            var clients = settings.Servers
                .Select(serverSettings => SoldatClient.CreateConnected(serverSettings.Address, serverSettings.Port, serverSettings.Password))
                .ToArray();

            var clientMonitors = clients.Select(ClientMonitor.Run).ToArray();

            var multiServerSoldatClient = new MultiServerSoldatClient(clients);
            var soldatTournament = new SoldatTournament();
            soldatTournament.Start(multiServerSoldatClient);

            while (true)
            {
                var command = Console.ReadLine();
                if (command == "quit")
                {
                    break;
                }

                if (command == "p")
                {
                    soldatTournament.TogglePause();
                }

                if (command == "swap")
                {
                    multiServerSoldatClient.SwapTeams();
                }
                else
                {
                    multiServerSoldatClient.SendRaw(command);
                }
            }
        }

        private static MultiServerSettings LoadSettings()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.local.json", optional: true);

            var settings = new MultiServerSettings();
            builder.Build().Bind(settings);
            return settings;
        }
    }
}
