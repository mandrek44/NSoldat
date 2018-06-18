using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoldat.Lib
{
    public class ServerEvent
    {
        public RefreshPacket RefreshPacket { get; }

        public EventType EventType { get; }

        public Dictionary<string, string> Props { get; }

        public DateTime Timestamp { get; } = DateTime.UtcNow;

        public string this[string groupname] => Props[groupname];

        public ServerEvent(RefreshPacket refreshPacket, EventType type, params (string, string)[] props)
        {
            RefreshPacket = refreshPacket;
            EventType = type;
            Props = props.ToDictionary(p => p.Item1, p => p.Item2);
        }

        public static ServerEvent Unknown(string input)
            => new ServerEvent(null, EventType.Unknown, ("input", input));

        public static ServerEvent NoRefreshFrame(string input)
            => new ServerEvent(null, EventType.Unknown, ("input", "(No refresh frame) " + input));

        public override string ToString()
        {
            return $"{nameof(Timestamp)}: {Timestamp}, {nameof(EventType)}: {EventType}, {nameof(Props)}:\r\n {string.Join("\r\n",Props.Select(k => $"\t{k.Key} => {k.Value}"))}";
        }
    }
}