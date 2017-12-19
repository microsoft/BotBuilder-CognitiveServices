namespace LuisActions.Samples.Web.Models
{
    using System.Collections.Generic;
    using Microsoft.Bot.Builder.CognitiveServices.LuisActionBinding;

    public class ActionScaffoldViewModel
    {
        public IEnumerable<string> Fields { get; set; }

        public ILuisAction LuisAction { get; set; }
    }
}