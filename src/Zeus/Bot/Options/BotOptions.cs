namespace Zeus.Bot.Options
{
    public partial class BotOptions
    {
        public string Token { get; set; }

        public ProxyOptions Proxy { get; set; }

        public AuthorizationOptions Authorization { get; set; }
    }
}