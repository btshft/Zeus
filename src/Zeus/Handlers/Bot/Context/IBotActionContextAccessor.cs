namespace Zeus.Handlers.Bot.Context
{
    public interface IBotActionContextAccessor
    {
        BotActionContext Context { get; set; }
    }
}
