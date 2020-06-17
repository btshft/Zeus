using System;
using System.ComponentModel.DataAnnotations;
using Zeus.Shared.Validation;

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
        /// Proxy options.
        /// </summary>
        public ProxyOptions Proxy { get; set; }

        /// <summary>
        /// Polling options.
        /// </summary>
        [Required, ValidateObject]
        public PollingOptions Polling { get; set; } = new PollingOptions();

        /// <summary>
        /// Authorization options.
        /// </summary>
        [Required, ValidateObject]
        public AuthorizationFeatureOptions Authorization { get; set; }

        public class PollingOptions
        {
            [Required]
            public CircuitBreakerOptions CircuitBreaker { get; set; }

            public class CircuitBreakerOptions
            {
                /// <summary>
                /// Time wait before next try to get bot-updates if circuit breaks.
                /// </summary>
                public TimeSpan DurationOfBreak { get; set; }
                    = TimeSpan.FromSeconds(15);

                /// <summary>
                /// Attempts before break the circuit.
                /// </summary>
                public int AttemptsBeforeBreaking { get; set; } = 5;
            }
        }

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

        public class AuthorizationFeatureOptions
        {
            [Required, ValidateObject]
            public TrustedUsersOptions TrustedUsers { get; set; }

            public class TrustedUsersOptions
            {
                /// <summary>
                /// Users identifiers.
                /// </summary>
                [Required]
                public int[] AdministratorIds { get; set; } = Array.Empty<int>();
            }
        }
    }
}