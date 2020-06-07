namespace Zeus.v2.Handlers.Bot.Abstractions
{
    public interface IBotActionFormat<TAction>
        where TAction : IBotAction
    {
        public bool TryParse(string message, out TAction action);
    }
}