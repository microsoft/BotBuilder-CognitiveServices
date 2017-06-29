namespace Microsoft.Bot.Builder.CognitiveServices.LuisActionBinding
{
    public interface ILuisContextualAction<T> : ILuisAction where T : ILuisAction
    {
        T Context { get; set; }
    }
}
