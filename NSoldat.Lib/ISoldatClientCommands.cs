using System.Threading.Tasks;

namespace NSoldat.Lib
{
    public interface ISoldatClientCommands
    {
        Task SwapTeams();
        void SetTeam(int playerNumber, int teamNumber);
        Task SetTeam(string playerName, SoldatTeam team);
        void SetMap(string mapName);
        void Restart();
        void Say(string what);
        void SendRaw(string command);
    }
}