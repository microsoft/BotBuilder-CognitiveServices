using System;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;

namespace SimpleQnABot.Dialogs
{
    [Serializable]
    [QnAMaker("set yout subscription key here", "set your kbid here")]
    public class SimpleQnADialog : QnAMakerDialog
    {
    }
}