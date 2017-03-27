namespace QnAMakerSampleBot.Dialogs
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    [Serializable]
    public class BasicQnAMakerDialog : QnAMakerDialog
    {
        // Go to https://qnamaker.ai and feed data, train & publish your QnA Knowledgebase.
        public BasicQnAMakerDialog() : base(new QnAMakerService(new QnAMakerAttribute("set yout subscription key here", "set your kbid here", "I don't understand this right now! Try another query!", 0.50)))
        {
        }

        // Uncomment the two handlers below if you want to see an example of a 
        // custom handler to respond to QnA Maker Service scores
        // up to a maximum threshold.  This allows you to tailor responses
        // depending on the confidence level in the answer provided
        //[QnAMakerResponseHandler(0.7)]
        //public async Task MidScoreHandler(IDialogContext context, IMessageActivity message, QnAMakerResult result)
        //{
        //    await context.PostAsync($"I think I found an answer that can help you....{result.Answer}");
        //}

        //[QnAMakerResponseHandler(0.4)]
        //public async Task LowScoreHandler(IDialogContext context, IMessageActivity message, QnAMakerResult result)
        //{
        //    await context.PostAsync($"Sorry, I couldn't find an answer to your question");
        //}

        // Uncomment the code below if you wanna see an example on how to
        // break the QnA loop in order to have custom logic within your 
        // inherited dialog that you could be using as Root
        //protected override async Task RespondFromQnAMakerResultAsync(IDialogContext context, IMessageActivity message, QnAMakerResult result)
        //{
        //    if (result.Score == 0)
        //    {
        //        await context.PostAsync("Executing custom logic..");
        //    }
        //    else
        //    {
        //        await base.RespondFromQnAMakerResultAsync(context, message, result);
        //    }
        //}

        //protected override async Task DefaultWaitNextMessageAsync(IDialogContext context, IMessageActivity message, QnAMakerResult result)
        //{
        //    if (result != null && result.Score == 0)
        //    {
        //        PromptDialog.Confirm(context, PromptDialogResultAsync, "Do you want to see the services menu?");
        //    }
        //    else
        //    {
        //        await base.DefaultWaitNextMessageAsync(context, message, result);
        //    }
        //}

        //private async Task PromptDialogResultAsync(IDialogContext context, IAwaitable<bool> result)
        //{
        //    if (await result == true)
        //    {
        //        await context.PostAsync("Showing the menu..");

        //        // TODO: you can continue your custom logic here and finally go back to the QnA dialog loop using DefaultWaitNextMessageAsync()

        //        await this.DefaultWaitNextMessageAsync(context, null, null);
        //    }
        //    else
        //    {
        //        await this.DefaultWaitNextMessageAsync(context, null, null);
        //    }
        //}
    }
}