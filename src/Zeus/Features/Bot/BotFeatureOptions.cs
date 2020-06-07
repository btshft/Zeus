using System.ComponentModel.DataAnnotations;

namespace Zeus.v2.Features.Bot
{
    public class BotFeatureOptions
    {
        /// <summary>
        /// Telegram bot token.
        /// </summary>
        [Required(ErrorMessage = "Bot token is not set", AllowEmptyStrings = false)]
        public string Token { get; set; }

        /// <summary>
        /// Proxy options.
        /// </summary>
        public ProxyOptions Proxy { get; set; }

        /// <summary>
        /// Proxy options.
        /// </summary>
        public class ProxyOptions
        {
            public ProxyType Type { get; set; }

            public string Address { get; set; }

            public int Port { get; set; }

            public string Username { get; set; }

            public string Password { get; set; }
        }

        /// <summary>
        /// Type of proxy to use.
        /// </summary>
        public enum ProxyType
        {
            Http, Socks5
        }
    }
}