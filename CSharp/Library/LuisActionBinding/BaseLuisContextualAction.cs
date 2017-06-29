namespace Microsoft.Bot.Builder.CognitiveServices.LuisActionBinding
{
    using System;

    [Serializable]
    public abstract class BaseLuisContextualAction<T> : BaseLuisAction, ILuisContextualAction<T> where T : ILuisAction
    {
        public T Context { get; set; }
    }
}
