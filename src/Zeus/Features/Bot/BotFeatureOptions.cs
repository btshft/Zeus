﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Zeus.Features.Bot
{
    public class BotFeatureOptions
    {
        /// <summary>
        /// Telegram bot token.
        /// </summary>
        [Required(ErrorMessage = "Bot token is not set", AllowEmptyStrings = false)]
        public string Token { get; set; }

        /// <summary>
        /// Time wait before next try to get bot-updates if circuit breaks.
        /// </summary>
        public TimeSpan PollingDurationOfBreak { get; set; } 
            = TimeSpan.FromSeconds(15);

        /// <summary>
        /// Attempts before break the circuit.
        /// </summary>
        public int PollingAttemptsBeforeBreaking { get; set; } = 5;

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