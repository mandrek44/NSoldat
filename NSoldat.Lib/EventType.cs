namespace NSoldat.Lib
{
    public enum EventType
    {
        Unknown,
        PlayerJoiningGame,
        PlayerJoinsTeam,
        PlayerKillsPlayer,
        PlayerCapturesFlag,
        PlayerReturnsFlag,
        PlayerSays,
        CommandRequest,
        TimeLeft,
        PlayerLeavesTeam,
        NextMap,
        ServerVersion,
        ServerInfo,
        AddressIsRequestingGame,
        PlayerScoresForTeam,

    }
}