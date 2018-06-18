using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace NSoldat.Lib
{
    public class SingleEventParser
    {
        private static ServerEvent ParseAddressIsRequestingGame(RefreshPacket refreshPacket, string input) => ParseByNamedRegex(
            refreshPacket, input, 
            @"(?<address>[\d\.:]+)\|(?<hwid>[a-zA-Z0-9]+) requesting game\.\.\.", 
            EventType.AddressIsRequestingGame);

        private static ServerEvent ParsePlayerJoiningGame(RefreshPacket refreshPacket, string input) => ParseByNamedRegex(
            refreshPacket, input,
            @"(?<name>.+) joining game \((?<address>[\d\.:]+)\) HWID:(?<hwid>[a-zA-Z0-9]+)",
            EventType.PlayerJoiningGame);

        private static ServerEvent ParsePlayerJoinsTeam(RefreshPacket refreshPacket, string input) => ParseByNamedRegex(
            refreshPacket, input,
            @"(?<name>.+) has joined (?<team>\w+) team\.",
            EventType.PlayerJoinsTeam);

        private static ServerEvent ParsePlayerLeavesTeam(RefreshPacket refreshPacket, string input) => ParseByNamedRegex(
            refreshPacket, input,
            @"(?<formername>.+) has left (?<team>\w+) team\.",
            EventType.PlayerLeavesTeam);

        private static ServerEvent ParsePlayerKillsPlayer(RefreshPacket refreshPacket, string input) => ParseByNamedRegex(
            refreshPacket, input,
            @"\((?<killerindex>\d+?)\) (?<killer>.+) killed \((?<victimindex>\d+?)\) (?<victim>.+) with (?<weapon>.+)",
            EventType.PlayerKillsPlayer);

        private static ServerEvent ParsePlayerCapturesFlag(RefreshPacket refreshPacket, string input) => ParseByNamedRegex(
            refreshPacket, input,
            @"(?<name>.+) captured the (?<flag>\w+) Flag",
            EventType.PlayerCapturesFlag);

        private static ServerEvent ParsePlayerReturnsFlag(RefreshPacket refreshPacket, string input) => ParseByNamedRegex(
            refreshPacket, input,
            @"(?<name>.+) returned the (?<flag>\w+) Flag",
            EventType.PlayerReturnsFlag);

        private static ServerEvent ParsePlayerScoresForTeam(RefreshPacket refreshPacket, string input) => ParseByNamedRegex(
            refreshPacket, input,
            @"(?<name>.+) scores for (?<team>\w+) Team",
            EventType.PlayerScoresForTeam);

        private static ServerEvent ParsePlayerSays(RefreshPacket refreshPacket, string input) => ParseByNamedRegex(
            refreshPacket, input,
            @"^\[(?<name>.+?)\] (?<what>.+)$",
            EventType.PlayerSays);

        private static ServerEvent ParseNextMap(RefreshPacket refreshPacket, string input) => ParseByNamedRegex(
            refreshPacket, input,
            @"Next map: (?<map>.+)",
            EventType.NextMap);

        private static ServerEvent ParseCommandRequest(RefreshPacket refreshPacket, string input) => ParseByNamedRegex(
            refreshPacket, input,
            @"^\/(?<command>\w+?)\((?<ip>[\d\.]+)\[(?<name>.+?)\]\)",
            EventType.CommandRequest);


        private static ServerEvent ParseTimeLeft(RefreshPacket refreshPacket, string input) => ParseByNamedRegex(
            refreshPacket, input,
            @"^Time Left: (?<timeleft>.+)",
            EventType.TimeLeft);

        private static ServerEvent ParseByNamedRegex(RefreshPacket refreshPacket, string input, string regexWithNamedGroups,
            EventType eventType)
        {
            var regex = new Regex(regexWithNamedGroups);
            var match = regex.Match(input);
            if (!match.Success)
                return null;

            var props = regex.GetGroupNames()
                .Except(new [] {"0"})
                .Select(name => (name, match.Groups[(string) name].Value))
                .ToArray();
            
            return new ServerEvent(refreshPacket, eventType, props);
        }

        public static ServerEvent Parse(string input, RefreshPacket refreshPacket)
        {
            if (refreshPacket?.PlayerName == null)
            {
                return ServerEvent.NoRefreshFrame(input);
            }

            Func<RefreshPacket, string, ServerEvent>[] handlers =
            {
                ParsePlayerLeavesTeam,
                ParsePlayerSays,
                ParseAddressIsRequestingGame,
                ParsePlayerJoiningGame,
                ParsePlayerJoinsTeam,
                ParsePlayerKillsPlayer,
                ParsePlayerCapturesFlag,
                ParsePlayerReturnsFlag,
                ParsePlayerScoresForTeam,
                ParseNextMap,
                ParseTimeLeft,
                ParseCommandRequest,
            };

            foreach (var handler in handlers)
            {
                var result = handler(refreshPacket, input);
                if (result == null)
                    continue;

                if (PlayerNameMatches(result, refreshPacket.PlayerName))
                    return result;
                else
                {
                    Console.WriteLine($"Tried {result.EventType} but player name doesn't match");
                }
            }
            
            return ServerEvent.Unknown(input);
        }

        private static bool PlayerNameMatches(ServerEvent parsedEvent, string[] playerNames)
        {
            return !parsedEvent.Props.ContainsKey("name")
                   || playerNames.Contains(parsedEvent["name"]);
        }
    }
}