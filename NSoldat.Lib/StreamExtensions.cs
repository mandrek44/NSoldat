using System;
using System.IO;
using System.Text;

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

        public static string ReadLine(this Stream stream)
        {
            var sb = new StringBuilder();

            while (true)
            {
                var readByte = stream.ReadByte();
                if (readByte == -1)
                {
                    return sb.ToString();
                }

                var readString = Encoding.ASCII.GetString(new[] { (byte)readByte });
                sb.Append(readString);

                if (readString == "\n")
                {
                    return sb.ToString();
                }
            }
        }

        public static bool ReadUntil(this Stream stream, string textPresent)
        {
            for (int i = 0; i < 10; i++)
            {
                if (stream.ReadLine().Echo() == textPresent)
                    return true;
            }

            return false;
        }
    }
}