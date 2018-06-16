namespace NSoldat.Lib
{
    public class RefreshPacket
    {
        public string[] PlayerName = new string[32];
        public byte[] Team = new byte[32];
        public long[] Kills = new long[32];
        public long[] Deaths = new long[32];
        public byte[] Ping = new byte[32];
        public byte[] Number = new byte[32];
        public byte[,] IP = new byte[32, 4];
        public long[] TeamScore = new long[4];
        public string MapName;
        public long TimeLimit;
        public long CurrentTime;
        public long KillLimit;
        public byte GameStyle;
    }
}