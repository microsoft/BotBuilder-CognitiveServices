namespace QnAMakerSampleBot.Dialogs
{
    using System;
    using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;

    [Serializable]
    public class BasicQnAMakerDialog : QnAMakerDialog
    {
        // Go to https://qnamaker.ai and feed data, train & publish your QnA Knowledgebase.
        public BasicQnAMakerDialog() : base(new QnAMakerService(new QnAMakerAttribute("set yout subscription key here", "set your kbid here", "I don't understand this right now! Try another query!", 0.50)))
        {
        }
    }
}