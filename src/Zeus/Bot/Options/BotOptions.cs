namespace Zeus.Bot.Options
{
    public partial class BotOptions
    {
        public string Token { get; set; }

        public ProxyOptions Proxy { get; set; }

        public SecurityOptions Security { get; set; }
    }
}