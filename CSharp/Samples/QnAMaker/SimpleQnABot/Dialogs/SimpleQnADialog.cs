using System;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;

namespace SimpleQnABot.Dialogs
{
    [Serializable]
    // Below method uses the V2 APIs : https://aka.ms/qnamaker-v2-apis. 
    // To use V4 stack, you also need to add the Endpoint hostname to the parameters below : https://aka.ms/qnamaker-v4-apis
    [QnAMaker("set yout subscription key here", "set your kbid here")]
    public class SimpleQnADialog : QnAMakerDialog
    {
    }
}