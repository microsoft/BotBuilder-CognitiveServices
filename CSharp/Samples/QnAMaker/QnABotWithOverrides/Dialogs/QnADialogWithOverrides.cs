using System;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Linq;
using System.Collections.Generic;

namespace QnABotWithOverrides.Dialogs
{
    [Serializable]
    [QnAMaker("set yout subscription key here", "set your kbid here", "I don't understand this right now! Try another query!", 0.50, 3)]
    public class QnADialogWithOverrides : QnAMakerDialog
    {
        // Override to also include the knowledgebase question with the answer on confident matches
        protected override async Task RespondFromQnAMakerResultAsync(IDialogContext context, IMessageActivity message, QnAMakerResults results)
        {
            if (results.Answers.Count > 0)
            {
                var response = "Here is the match from FAQ:  \r\n  Q: " + results.Answers.First().Questions.First() + "  \r\n A: " + results.Answers.First().Answer;
                await context.PostAsync(response);
            }
        }

        // Override to log matched Q&A before ending the dialog
        protected override async Task DefaultWaitNextMessageAsync(IDialogContext context, IMessageActivity message, QnAMakerResults results)
        {
            Console.WriteLine("KB Question: " + results.Answers.First().Questions.First());
            Console.WriteLine("KB Answer: " + results.Answers.First().Answer);
            await base.DefaultWaitNextMessageAsync(context, message, results);
        }

        // Override default message to offer assitance with a suggested action card
        protected override async Task RespondWithDefaultMessageAsync(IDialogContext context, IMessageActivity request)
        {
            var activity = ((Activity)context.Activity).CreateReply("Sorry, I don't understand this right now. Try another query!");
            activity.SuggestedActions = new SuggestedActions(actions:
                                            new List<CardAction>
                                            {
                                                new CardAction() {
                                                    Title = "Try asking Bing!",
                                                    Value = $"https://bing.com/search?q={request.Text}",
                                                    Type = ActionTypes.OpenUrl }
                                            });
            await context.PostAsync(activity);
        }
    }
}