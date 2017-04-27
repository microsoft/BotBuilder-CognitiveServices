using System;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;

namespace QnABotWithActiveLearning.Dialogs
{
    [Serializable]
    [QnAMaker("set yout subscription key here", "set your kbid here", "I don't understand this right now! Try another query!", 0.50, 3)]
    public class QnADialogWithActiveLearning : QnAMakerDialog
    {
    }
}