using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using System.Linq;

namespace QnABotWithCustomFeedback.Dialogs
{
    [Serializable]
    // Train and the feedback API only works with the QnAMaker V2 APIs: https://aka.ms/qnamaker-v2-apis
    [QnAMaker("set yout subscription key here", "set your kbid here", "I don't understand this right now! Try another query!", 0.50, 3)]
    public class QnABotWithCustomFeedback : QnAMakerDialog
    {
        protected override async Task QnAFeedbackStepAsync(IDialogContext context, QnAMakerResults qnaMakerResults)
        {
            // responding with the top answer when score is above some threshold
            if (qnaMakerResults.Answers.Count > 0 && qnaMakerResults.Answers.FirstOrDefault().Score > 0.75)
            {
                await context.PostAsync(qnaMakerResults.Answers.FirstOrDefault().Answer);
            }
            else
            {
                await base.QnAFeedbackStepAsync(context, qnaMakerResults);
            }
        }
    }
}