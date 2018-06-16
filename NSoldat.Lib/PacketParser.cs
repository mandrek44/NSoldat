using System;

namespace NSoldat.Lib
{
    public interface IPacketParser
    {
        RefreshPacket ParseRefreshPacket(byte[] packet, int offset);
    }

    public class PacketParser : IPacketParser
    {
        public RefreshPacket ParseRefreshPacket(byte[] packet, int offset = 0)
        {
            int i = offset;
            byte length;
            RefreshPacket tempPacket = new RefreshPacket();

            for (int k = 0; k < 32; k++)
            {
                length = packet[i];
                i++;
                for (int j = 0; j < 24; j++)
                {
                    tempPacket.PlayerName[k] += Convert.ToChar(packet[i]);
                    i++;
                }
                tempPacket.PlayerName[k] = left(tempPacket.PlayerName[k], length);
            }
            for (int k = 0; k < 32; k++)
            {
                tempPacket.Team[k] = packet[i];
                i++;
            }
            for (int k = 0; k < 32; k++)
            {
                tempPacket.Kills[k] = packet[i] + (packet[i + 1] * 256);
                i += 2;
            }
            for (int k = 0; k < 32; k++)
            {
                tempPacket.Deaths[k] = packet[i] + (packet[i + 1] * 256);
                i += 2;
            }
            for (int k = 0; k < 32; k++)
            {
                tempPacket.Ping[k] = packet[i];
                i++;
            }
            for (int k = 0; k < 32; k++)
            {
                tempPacket.Number[k] = packet[i];
                i++;
            }
            for (int k = 0; k < 32; k++)
            {
                for (int j = 0; j < 4; j++)
                {
                    tempPacket.IP[k, j] = packet[i];
                    i++;
                }
            }
            for (int k = 0; k < 4; k++)
            {
                tempPacket.TeamScore[k] = packet[i] + (packet[i + 1] * 256);
                i += 2;
            }
            length = packet[i];
            i++;
            for (int k = 0; k < 16; k++)
            {
                tempPacket.MapName += Convert.ToChar(packet[i]);
                i++;
            }
            tempPacket.MapName = left(tempPacket.MapName, length);
            for (int k = 0; k < 4; k++)
            {
                //You can use getTime() to return the time limit as a string in "MM:SS" format.
                tempPacket.TimeLimit += Convert.ToInt64(packet[i] * (Math.Pow(256, k)));
                i++;
            }
            for (int k = 0; k < 4; k++)
            {
                //You can use getTime() to return the time left as a string in "MM:SS" format.
                tempPacket.CurrentTime += Convert.ToInt64(packet[i] * (Math.Pow(256, k)));
                i++;
            }
            tempPacket.KillLimit = packet[i] + (packet[i + 1] * 256);
            i += 2;
            tempPacket.GameStyle = packet[i];


            return tempPacket;
        }


        private string left(string word, int x)
        {
            if (x <= word.Length)
                word = word.Remove(x);
            return word;
        }

        private string getTime(long ticks)
        {
            string time;
            time = getMinutes(ticks) + ':' + getSeconds(ticks);
            return time;
        }

        private string getMinutes(long ticks)
        {
            int x;
            string minutes;
            x = ((Convert.ToInt32(ticks) / 60) / 60);
            minutes = Convert.ToString(x);
            return minutes;
        }

        private string getSeconds(long ticks)
        {
            int x;
            string seconds;
            x = ((Convert.ToInt32(ticks) / 60) % 60);
            seconds = Convert.ToString(x);
            if (seconds.Length == 1)
                seconds = '0' + seconds;
            return seconds;
        }
    }
}