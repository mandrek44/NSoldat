namespace NSoldat.MultiServerClient
{
    class MultiServerSettings
    {
        public class ServerSettings
        {
            public string Address { get; set; }
            public int Port { get; set; }
            public string Password { get; set; }
        }

        public ServerSettings[] Servers { get; set; }
    }
}