## Overview
QnA Maker Dialog is to simplify the bot development using [QnA Maker](https://qnamaker.ai/) knowledge bases. The bot can consume the QnA Maker dialog as shown in the examples to forward the conversation to QnA Maker service and post the answer to the user in a seamless way.

## Prerequisites

The minimum prerequisites to run this sample are:
* Latest Node.js with NPM. Download it from [here](https://nodejs.org/en/download/).
* The Bot Framework Emulator. To install the Bot Framework Emulator, download it from [here](https://emulator.botframework.com/). Please refer to [this documentation article](https://github.com/microsoft/botframework-emulator/wiki/Getting-Started) to know more about the Bot Framework Emulator.
* **[Recommended]** Visual Studio Code for IntelliSense and debugging, download it from [here](https://code.visualstudio.com/) for free.

## Code Highlights
The package contains a Recognizer plugin for QnA Maker that allow interation with QnA Maker knowledge bases. This recognizer along with the QnA Maker dialog are useful for building QnA bots in few minutes.

#### QnA Recognizer
The QnA Recognizer takes in the following inputs:
* knowledgeBaseId - Your knowledge base id
* authKey - Your authorization key.
* top (optional) - Number of results returned.
* qnaThreshold (optional) - Confidence threshod;
* defaultMessage (optional) - Message to show if no results returned.
* endpointHostName (optional) - To be used with the V4 APIs and the GA stack. See [here](https://aka.ms/qnamaker-docs-changesfrompreview).
````
var recognizer = new cognitiveservices.QnAMakerRecognizer({
	knowledgeBaseId: 'set your kbid here', 
	authKey: 'set your authorization key here',
	top: 4});
````

#### QnA Maker Tools Library
Libraries of reusable parts can be developed by creating a new Library instance and adding dialogs just as you would to a bot. The QnA Maker Tools Library includes the feedback dialog. The default QnAMakerTools library contains an 'answerSelection' dialog which is invoked through the 'answerSelector' method, when there are multiple good matches above the specified threshold. One can define their own custom libraries including the 'answerSelector' method, which basically takes care of the dialog to execute to get the user selection from the top N QnA Maker responses. 

Note that, you'll need to initialize this and add it to the bot.libraries when the value of 'top' is set to >1 in QnA Maker Recognizer. 
````
var qnaMakerTools = new cognitiveservices.QnAMakerTools();
bot.library(qnaMakerTools.createLibrary());
````
If this is not added, the feedback dialog is unknown to the QnA Maker dialog and it falls back to the basic scenario where the top answer is directly sent back.

#### QnA Dialog
The QnA dialog takes in input Recognizer and QnA Maker tools along with few optional inputs (defaultMessage and qnaThreshold). This manages the bot conversations with the users.
````
var basicQnAMakerDialog = new cognitiveservices.QnAMakerDialog({
	recognizers: [recognizer],
	defaultMessage: 'No match! Try changing the query terms!',
	qnaThreshold: 0.3
});
````

## Bot Samples

#### Simple QnA bot
[QnAMakerSimpleBot](QnAMakerSimpleBot) demonstrates a simple QnA bot where the bot responds with the top answer above specified threshold. You just need to set up the recognizer and the dialog as follows and add it as the root dialog.
````
var recognizer = new cognitiveservices.QnAMakerRecognizer({
	knowledgeBaseId: 'set your kbid here', 
	authKey: 'set your authorization key here'});
	
var basicQnAMakerDialog = new cognitiveservices.QnAMakerDialog({
	recognizers: [recognizer],
	defaultMessage: 'No match! Try changing the query terms!',
	qnaThreshold: 0.3
});

bot.dialog('/', basicQnAMakerDialog);
````

#### QnA bot with Active Learning
Active learning can be enabled using the QnA Maker dialog by setting the optional value 'top' to some value greater than one and initializing the QnA Maker tools library. Then QnA Maker uses active learning to learn from the utterances that come into the system. In this process, QnAMaker responds with multiple relevant QnAs for low confidence scenarios and asks users to mark the correct response. The user feedback is logged and models are updated once the system has gathered enough examples. Users will be able to see improved responses based on the feedback received.

[QnAMakerBotWithActiveLearning](QnAMakerBotWithActiveLearning) demonstrates a QnA bot, where the high confidence answers are directly sent as bot response. In case the confidence is low but multiple good matches are found, it asks for options through a prompt and returns the answers based on the selection. This feedback is also sent back to the service for training the knowledge base with user utterences. 

The recognizer instance is set up setting up the optional parameter 'top' to some value greater than 1.
````
var recognizer = new cognitiveservices.QnAMakerRecognizer({
	knowledgeBaseId: 'set your kbid here', 
	authKey: 'set your authorization key here',
	top: 3});
````

The QnA Maker tools needs to be initialized and added to the bot.libraries. If this is not registered, the QnA dialog is unaware of the feedback dialog and it will behave as the simple QnA bot described in the previous section. 
````
var qnaMakerTools = new cognitiveservices.QnAMakerTools();
bot.library(qnaMakerTools.createLibrary());
````

Similar to the simple QnA bot, you now need to set up the QnA dialog with the recognizer and other optional parameters. In addition, you also specify the feedbackLib.

````
var basicQnAMakerDialog = new cognitiveservices.QnAMakerDialog({
	recognizers: [recognizer],
	defaultMessage: 'No match! Try changing the query terms!',
	qnaThreshold: 0.3,
	feedbackLib: qnaMakerTools
});

bot.dialog('/', basicQnAMakerDialog);
````

One can also specify their custom logic and add their own feedbackLib. [QnAMakerWithCustomTools](QnAMakerWithCustomTools) demonstrates how to achieve that. CustomQnAMakerTools.js is an example of how to customize the feedbackLib. Note that, you can also turn off the additional call to QnA Maker service for recording the feedback by just calling endDialog() instead of endDialogWithResult().

````
function CustomQnAMakerTools() {
        this.lib = new builder.Library('customQnAMakerTools');
        this.lib.dialog('answerSelection', [
            function (session, args) {
                var qnaMakerResult = args;
                session.dialogData.qnaMakerResult = qnaMakerResult;
                var questionOptions = [];
                qnaMakerResult.answers.forEach(function (qna) { questionOptions.push(qna.questions[0]); });
                var promptOptions = { listStyle: builder.ListStyle.button };
                builder.Prompts.choice(session, "There are multiple good matches. Please select from the following:", questionOptions, promptOptions);
            },
            function (session, results) {
                var qnaMakerResult = session.dialogData.qnaMakerResult;
                var filteredResult = qnaMakerResult.answers.filter(function (qna) { return qna.questions[0] === results.response.entity; });
                var selectedQnA = filteredResult[0];
                session.send(selectedQnA.answer);
                // The following ends the dialog and returns the selected response to the parent dialog, which logs the record in QnA Maker service
                // You can simply end the dialog, in case you don't want to learn from these selections using session.endDialog()
                session.endDialogWithResult(selectedQnA);
            },
        ]);
    }
````

#### QnA bot with function overrides for decorating responses and custom logging
[QnAMakerWithFunctionOverrides](QnAMakerWithFunctionOverrides) demonstrates a QnA bot, where the default methods are overridden to add some custom logic. One can override the 'respondFromQnAMakerResult' method and customize that as needed. In this example, the indexed question present in the QnA Maker knowledge base is also added whenever there is a confident match and the dialog returns the best matched answer. 
````
basicQnAMakerDialog.respondFromQnAMakerResult = function(session, qnaMakerResult){
	var result = qnaMakerResult;
	var response = 'Here is the match from FAQ:  \r\n  Q: ' + result.answers[0].questions[0] + '  \r\n A: ' + result.answers[0].answer;
	session.send(response);
}
````

One can also override the 'defaultWaitNextMessage' to add custom logging. In this example, the user query and the matched Q&A are just printed to console.
````
basicQnAMakerDialog.defaultWaitNextMessage = function(session, qnaMakerResult){
	if(session.privateConversationData.qnaFeedbackUserQuestion != null && qnaMakerResult.answers != null && qnaMakerResult.answers.length > 0 
		&& qnaMakerResult.answers[0].questions != null && qnaMakerResult.answers[0].questions.length > 0 && qnaMakerResult.answers[0].answer != null){
			console.log('User Query: ' + session.privateConversationData.qnaFeedbackUserQuestion);
			console.log('KB Question: ' + qnaMakerResult.answers[0].questions[0]);
			console.log('KB Answer: ' + qnaMakerResult.answers[0].answer);
		}
	session.endDialog();
}
````

#### QnA + LUIS bot
[QnAWithLUIS](QnAWithLUIS) demonstrates a bot using LUIS and QnA Maker. Both LUIS and QnA recognizers are first set up.
````
var qnarecognizer = new cognitiveservices.QnAMakerRecognizer({
	knowledgeBaseId: 'set your kbid here', 
	authKey: 'set your authorization key here',
    top: 4});

var model='set your luis model uri';
var recognizer = new builder.LuisRecognizer(model);
````
And then the [IntentDialog](https://docs.botframework.com/en-us/node/builder/chat/IntentDialog/) is initiated with both the recognizers as follows:
````
var intents = new builder.IntentDialog({ recognizers: [recognizer, qnarecognizer] });
bot.dialog('/', intents);

intents.matches('luisIntent1', builder.DialogAction.send('Inside LUIS Intent 1.'));

intents.matches('luisIntent2', builder.DialogAction.send('Inside LUIS Intent 2.'));

intents.matches('qna', [
    function (session, args, next) {
        var answerEntity = builder.EntityRecognizer.findEntity(args.entities, 'answer');
        session.send(answerEntity.entity);
    }
]);

intents.onDefault([
    function(session){
        session.send('Sorry!! No match!!');
	}
]);
````
By default the recognizers will be evaluated in parallel and the recognizer returning the intent with the highest score will be matched. 
