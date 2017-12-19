namespace LuisActions.Samples.Web.Models
{
    using System.ComponentModel.DataAnnotations;
    using Microsoft.Bot.Builder.CognitiveServices.LuisActionBinding;

    public class QueryViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please provide something to evaluate (E.g.: \"What is the time in Miami?\"")]
        public string Query { get; set; }

        public ILuisAction LuisAction { get; set; }

        public string LuisActionType { get; set; }

        public bool HasIntent
        {
            get
            {
                return !string.IsNullOrWhiteSpace(StringCrypto.Decrypt(this.LuisActionType));
            }
        }
    }
}