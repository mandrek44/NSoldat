using System;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NSoldat.Lib;

namespace NSoldat.StatsHarvester
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables(prefix:"soldat_")
                .AddCommandLine(args);

            var configurationRoot = builder.Build();

            var serverAdress = configurationRoot["address"];
            var serverPort = int.Parse(configurationRoot["port"]);
            var password = configurationRoot["password"];

            var soldatClient = new SoldatClient(new PacketParser());
            soldatClient.Connect(serverAdress, serverPort, password);

            while (true)
            {
                var serverEvent = soldatClient.ReadNextEvent();
                Console.WriteLine(serverEvent);

                var jsonServerEvent = JsonConvert.SerializeObject(serverEvent);

                File.AppendAllText("output.json", ",\r\n" + jsonServerEvent);

                Thread.Sleep(500);
            }
        }
    }
}
