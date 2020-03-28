namespace Zeus.Bot.Options
{
    public partial class BotOptions
    {
        public class AuthorizationOptions
        {
            /// <summary>
            /// Usernames without '@';
            /// </summary>
            public string[] Users { get; set; }
        }
    }
}