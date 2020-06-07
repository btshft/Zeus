using System.ComponentModel.DataAnnotations;

namespace Zeus.Features.Bot.Authorize
{
    public class BotAuthorizationFeatureOptions
    {
        [Required]
        public TrustedUsersOptions TrustedUsers { get; set; }

        public class TrustedUsersOptions
        {
            /// <summary>
            /// Users identifiers.
            /// </summary>
            public int[] AdministratorIds { get; set; }
        }
    }
}