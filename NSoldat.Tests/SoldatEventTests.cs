using System;
using NSoldat.Lib;
using Xunit;
namespace NSoldat.Tests
{
    public class SingleEventParserTests
    {
        private readonly string[] _playerNames = { "Mandro", "[Hacker]" };

        [Fact]
        public void TestAddressIsRequestingGame() =>
            Test(
                "10.57.72.58:8080|2ECBFF6F425 requesting game...",
                EventType.AddressIsRequestingGame,
                ("address", "10.57.72.58:8080"),
                ("hwid", "2ECBFF6F425"));
        
        [Fact]
        public void TestPlayerJoiningGame() =>
            Test(
                "Mandro joining game (10.57.72.58:8080) HWID:2ECBFF6F425",
                EventType.PlayerJoiningGame,
                ("name", "Mandro"),
                ("address", "10.57.72.58:8080"),
                ("hwid", "2ECBFF6F425"));

        [Fact]
        public void TestPlayerJoinsTeam() =>
            Test(
                "Mandro has joined alpha team.",
                EventType.PlayerJoinsTeam,
                ("name", "Mandro"),
                ("team", "alpha"));

        [Fact]
        public void TestPlayerLeavesTeam() =>
            Test(
                "[Former-Player] has left alpha team.",
                EventType.PlayerLeavesTeam,
                ("formername", "[Former-Player]"),
                ("team", "alpha"));

        [Fact]
        public void TestPlayerKillsPlayer() =>
            Test(
                "(1) Mandro killed (0) Billy with Barrett M82A1",
                EventType.PlayerKillsPlayer,
                ("killer", "Mandro"),
                ("victim", "Billy"),
                ("weapon", "Barrett M82A1"),
                ("killerindex", "1"),
                ("victimindex", "0")
            );

        [Fact]
        public void TestPlayerCapturesFlag() =>
            Test(
                "Mandro captured the Blue Flag",
                EventType.PlayerCapturesFlag,
                ("name", "Mandro"),
                ("flag", "Blue"));

        [Fact]
        public void TestPlayerReturnsFlag() =>
            Test(
                "Mandro returned the Red Flag",
                EventType.PlayerReturnsFlag,
                ("name", "Mandro"),
                ("flag", "Red"));
        
        [Fact]
        public void TestPlayerScoresForTeam() =>
            Test(
                "Mandro scores for Alpha Team",
                EventType.PlayerScoresForTeam,
                ("name", "Mandro"),
                ("team", "Alpha"));

        [Fact]
        public void TestAddressIsRequestingGame_InvalidAddress() =>
            Test(
                "Some@crap|2ECBFF6F425 requesting game...",
                EventType.Unknown,
                ("input", "Some@crap|2ECBFF6F425 requesting game..."));

        [Fact]
        public void TestPlayerSays_HackAttempt() =>
            Test(
                "[Mandro] has joined alpha team.",
                EventType.PlayerSays,
                ("name", "Mandro"),
                ("what", "has joined alpha team."));

        [Fact]
        public void TestPlayerSays_Inception() =>
            Test(
                "[[Hacker]] [Hacker] Hi!",
                EventType.PlayerSays,
                ("name", "[Hacker]"),
                ("what", "[Hacker] Hi!"));

        [Fact]
        public void TestPlayerSays_HackAttempt2() =>
            Test(
                "[[Hacker]] has joined alpha team.",
                EventType.PlayerSays,
                ("name", "[Hacker]"),
                ("what", "has joined alpha team."));

        [Fact]
        public void Test_CommandRequest() =>
            Test(
                "/nextmap(10.57.72.134[[Hacker]])",
                EventType.CommandRequest,
                ("command", "nextmap"),
                ("ip", "10.57.72.134"),
                ("name", "[Hacker]")
                );

        [Fact]
        public void TestNextMap() =>
            Test(
                "Next map: ctf_Barbwire",
                EventType.NextMap,
                ("map", "ctf_Barbwire"));


        [Fact]
        public void TestTimeLeft() =>
            Test(
                "Time Left: 1 minutes",
                EventType.TimeLeft,
                ("timeleft", "1 minutes"));

        private void Test(string input, EventType expectedType, params (string, string)[] props)
        {
            // given
            var refreshPacket = new RefreshPacket() {PlayerName = _playerNames};

            // when
            var parsedData = SingleEventParser.Parse(input, refreshPacket);

            // then
            parsedData.EventType.AssertEquals(expectedType);
            parsedData.Props.AssertContains(props);
        }
    }
}