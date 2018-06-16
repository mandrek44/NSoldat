using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NSoldat.Lib
{
    internal static class StreamExtensions
    {
        public static void Write(this Stream stream, string message)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);

            stream.Write(data, 0, data.Length);
        }

        public static void WriteLine(this Stream stream, string message)
        {
            stream.Write(message + "\n");
        }

        public static string Echo(this string message)
        {
            System.Console.WriteLine(message);
            return message;
        }

        public static async Task<string> ReadLine(this Stream stream)
        {
            var sb = new StringBuilder();
            byte[] buffer = new byte[1];

            while (true)
            {   
                var count = await stream.ReadAsync(buffer, 0, 1);
                if (count != 1)
                {
                    return sb.ToString();
                }

                var readString = Encoding.ASCII.GetString(new[] { (byte)buffer[0] });
                sb.Append(readString);

                if (readString == "\n")
                {
                    return sb.ToString();
                }
            }
        }

        public static async Task<bool> ReadUntil(this Stream stream, string textPresent)
        {
            for (int i = 0; i < 10; i++)
            {
                if ((await stream.ReadLine()).Echo() == textPresent)
                    return true;
            }

            return false;
        }
    }
}