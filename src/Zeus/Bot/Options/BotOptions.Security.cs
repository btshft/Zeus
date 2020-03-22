namespace Zeus.Bot.Options
{
    public partial class BotOptions
    {
        public class SecurityOptions
        {
            /// <summary>
            /// Usernames without '@';
            /// </summary>
            public string[] Administrators { get; set; }
        }
    }
}