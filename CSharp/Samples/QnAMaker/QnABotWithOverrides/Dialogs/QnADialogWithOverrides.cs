using System;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Linq;

namespace QnABotWithOverrides.Dialogs
{
    [Serializable]
    //[QnAMaker("set yout subscription key here", "set your kbid here", "I don't understand this right now! Try another query!", 0.50, 3)]
    [QnAMaker("682875376ad54258acc921d95b4500c2", "c00db5e6-5802-4e32-bd1d-0cbf83c86da1", "I don't understand this right now! Try another query!", 0.50, 5)]
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
    }
}