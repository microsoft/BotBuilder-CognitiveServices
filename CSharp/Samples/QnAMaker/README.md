## Overview
QnA Maker Dialog is to simplify the bot development using [QnA Maker](https://qnamaker.ai/) knowledge bases. The bot can consume the QnA Maker dialog as shown in the examples to forward the conversation to QnA Maker service and post the answer to the user in a seamless way. The package contains QnAMakerService that allow interation with QnA Maker knowledge bases. This service along with the QnA Maker dialog or scorable are useful for building QnA bots in few minutes. 

## Bot Samples

#### Simple QnA bot
[SimpleQnABot](https://github.com/Microsoft/BotBuilder-CognitiveServices/tree/master/CSharp/Samples/QnAMaker/SimpleQnABot) demonstrates a simple QnA bot where the bot responds with the top answer above specified threshold. One just needs to add the knowledge base id and the subscription key as shown below in the Dialog code.
````
[Serializable]
[QnAMaker("set yout subscription key here", "set your kbid here")]
public class SimpleQnADialog : QnAMakerDialog
{
}
````
And then forward the call from the MessagesController:
````
public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
{
	if (activity.Type == ActivityTypes.Message)
	{
		await Conversation.SendAsync(activity, () => new Dialogs.SimpleQnADialog());
	}
	...
	...
}
````

#### QnA bot with Active Learning
Active learning can be enabled using the QnA Maker dialog by setting the optional parameter 'top' to some value greater than 1. Then QnA Maker uses active learning to learn from the utterances that come into the system. In this process, QnAMaker responds with multiple relevant QnAs for low confidence scenarios and asks users to mark the correct response. The user feedback is logged and models are updated once the system has gathered enough examples. Users will be able to see improved responses based on the feedback received.

[QnABotWithActiveLearning](https://github.com/Microsoft/BotBuilder-CognitiveServices/tree/master/CSharp/Samples/QnAMaker/QnABotWithActiveLearning) demonstrates a QnA bot, where the high confidence answers are directly sent as bot response. In case the confidence is low but multiple good matches are found, it asks for options through a prompt and returns the answers based on the selection. This feedback is also sent back to the service for training the knowledge base with user utterences. The optional 'top' parameter needs to be set to some value > 1. The 'top' represents the maximum number of responses that we will get from the QnA Maker Service. Everything else is similar to [SimpleQnABot](https://github.com/Microsoft/BotBuilder-CognitiveServices/tree/master/CSharp/Samples/QnAMaker/SimpleQnABot).
````
[Serializable]
[QnAMaker("set yout subscription key here", "set your kbid here", "I don't understand this right now! Try another query!", 0.50, 3)]
public class QnADialogWithActiveLearning : QnAMakerDialog
{
}
````

One can also specify their custom logic and override the QnAFeedbackStepAsync method. [QnABotWithCustomFeedback](https://github.com/Microsoft/BotBuilder-CognitiveServices/tree/master/CSharp/Samples/QnAMaker/QnABotWithCustomFeedback) demonstrates how to achieve that. 

````
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
````
Note that, once can also turn off the additional call to QnA Maker service for recording the feedback by just ending the dialog here and not calling the base QnAFeedbackStepAsync method.

#### QnA bot with function overrides for decorating responses and custom logging
[QnABotWithOverrides](https://github.com/Microsoft/BotBuilder-CognitiveServices/tree/master/CSharp/Samples/QnAMaker/QnABotWithOverrides) demonstrates a QnA bot, where the default methods are overridden to add some custom logic. One can override the 'RespondFromQnAMakerResultAsync' method and customize that as needed. In this example, the indexed question present in the QnA Maker knowledge base is also added whenever there is a confident match and the dialog returns the best matched answer. 
````
protected override async Task RespondFromQnAMakerResultAsync(IDialogContext context, IMessageActivity message, QnAMakerResults results)
{
	if (results.Answers.Count > 0)
	{
		var response = "Here is the match from FAQ:  \r\n  Q: " + results.Answers.First().Questions.First() + "  \r\n A: " + results.Answers.First().Answer;
		await context.PostAsync(response);
	}
}
````

One can also override the 'DefaultWaitNextMessageAsync' to add custom logging. In this example, the matched Q&A are just printed to console.
````
protected override async Task DefaultWaitNextMessageAsync(IDialogContext context, IMessageActivity message, QnAMakerResults results)
{
	Console.WriteLine("KB Question: " + results.Answers.First().Questions.First());
	Console.WriteLine("KB Answer: " + results.Answers.First().Answer);
	await base.DefaultWaitNextMessageAsync(context, message, results);
}
````
