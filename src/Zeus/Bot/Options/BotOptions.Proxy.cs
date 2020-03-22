namespace Zeus.Bot.Options
{
    public partial class BotOptions
    {
        public class ProxyOptions
        {
            public Socks5ProxyOptions Socks5 { get; set; }

            public HttpProxyOptions Http { get; set; }
        }

        public class HttpProxyOptions
        {
            public string Address { get; set; }

            public int Port { get; set; }

            public string Username { get; set; }

            public string Password { get; set; }
        }

        public class Socks5ProxyOptions
        {
            public string Address { get; set; }

            public int Port { get; set; }

            public string Username { get; set; }

            public string Password { get; set; }
        }
    }
}