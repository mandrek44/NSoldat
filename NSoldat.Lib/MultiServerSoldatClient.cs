using System;
using System.Linq;
using System.Threading.Tasks;

namespace NSoldat.Lib
{
    public class MultiServerSoldatClient : ISoldatClientCommands
    {
        public MultiServerSoldatClient(ISoldatClientCommands[] soldatClients)
        {
            SoldatClients = soldatClients;
        }

        public ISoldatClientCommands[] SoldatClients { get; private set; }

        public Task SwapTeams()
        {
            return ForAll(client => client.SwapTeams());
        }

        public void SetTeam(int playerNumber, int teamNumber)
        {
            ForAll(client => client.SetTeam(playerNumber, teamNumber));
        }

        public Task SetTeam(string playerName, SoldatTeam team)
        {
            return ForAll(client => client.SetTeam(playerName, team));
        }

        public void SetMap(string mapName)
        {
            ForAll(client => client.SetMap(mapName));
        }

        public void Restart()
        {
            ForAll(client => client.Restart());
        }

        public void Say(string what)
        {
            ForAll(client => client.Say(what));
        }

        public void SendRaw(string command)
        {
            ForAll(client => client.SendRaw(command));
        }
        
        private void ForAll(Action<ISoldatClientCommands> clientAction)
        {
            foreach (var soldatClient in SoldatClients)
            {
                clientAction(soldatClient);
            }
        }

        private Task ForAll(Func<ISoldatClientCommands, Task> clientAction)
        {
            return Task.WhenAll(SoldatClients.Select(clientAction).ToArray());            
        }
    }
}