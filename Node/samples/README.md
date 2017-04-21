## Overview
QnA Maker Dialog is to simplify the bot development using QnA Maker knowledge bases. The bot can consume the QnA Maker dialog as shown in the examples to forward the conversation to QnA Maker service and post the answer to the user in a seamless way.

## Prerequisites

The minimum prerequisites to run this sample are:
* Latest Node.js with NPM. Download it from [here](https://nodejs.org/en/download/).
* The Bot Framework Emulator. To install the Bot Framework Emulator, download it from [here](https://emulator.botframework.com/). Please refer to [this documentation article](https://github.com/microsoft/botframework-emulator/wiki/Getting-Started) to know more about the Bot Framework Emulator.
* **[Recommended]** Visual Studio Code for IntelliSense and debugging, download it from [here](https://code.visualstudio.com/) for free.

## Bot Samples

|**Example**     | **Description**                                   
| ---------------| ---------------------------------------------
|[QnAMakerSimpleBot](https://github.com/Microsoft/BotBuilder-CognitiveServices/tree/master/Node/samples/QnAMakerSimpleBot) | Demonstrates a simple QnA bot where the bot responds with the top answer above specified threshold.      
|[QnAMakerBotWithActiveLearning](https://github.com/Microsoft/BotBuilder-CognitiveServices/tree/master/Node/samples/QnAMakerBotWithActiveLearning) | Demonstrates a QnA bot, where the high confidence answers are directly sent as bot response. In case the confidence is low but multiple good matches are found, it asks for options through a prompt and returns the answers based on the selection. This feedback is also sent back to the service for training the knowledge base with user utterences.
|[QnAMakerWithFunctionOverrides](https://github.com/Microsoft/BotBuilder-CognitiveServices/tree/master/Node/samples/QnAMakerWithFunctionOverrides) | Demonstrates a QnA bot, where the default methods are overridden to add some custom logic. 

## Code Highlights
The package contains a Recognizer plugin for QnA Maker that allow interation with QnA Maker knowledge bases. This recognizer along with the QnA Maker dialog are useful for building QnA bots in few minutes.

#### QnA Recognizer
The QnA Recognizer takes in two inputs 'knowledgeBaseId' and 'subscriptionKey', which is used to interact with the QnA Maker service. There is an optional input 'top' which can be specified to fetch more than 1 response from the service. The default value for 'top' is 1.
````
var recognizer = new cognitiveservices.QnAMakerRecognizer({
	knowledgeBaseId: 'set your kbid here', 
	subscriptionKey: 'set your subscription key here',
	top: 4});
````

#### QnA Maker Tools Library
Libraries of reusable parts can be developed by creating a new Library instance and adding dialogs just as you would to a bot. The QnA Maker Tools Library includes the feedback dialog. The default QnAMakerTools library contains an 'answerSelection' dialog which is invoked through the 'answerSelector' method, when there are multiple good matches above the specified threshold. One can define their own custom libraries including the 'answerSelector' method, which basically takes care of the dialog to execute to get the user selection from the top N QnA Maker responses as demonstrated in [QnAMakerWithFunctionOverrides](https://github.com/Microsoft/BotBuilder-CognitiveServices/tree/master/Node/samples/QnAMakerWithFunctionOverrides).

Note that, you'll need to initialize this and add it to the bot.libraries when the value of 'top' is set to >1 in QnA Maker Recognizer. 
````
var qnaMakerTools = new cognitiveservices.QnAMakerTools();
bot.library(qnaMakerTools.createLibrary());
````
If this is not added, the feedback dialog is unknown to the QnA Maker dialog and it falls back to the basic scenario where the top answer is directly sent back.

#### QnA Dialog
The QnA dialog takes in input Recognizer and QnA Maker tools along with few optional inputs (defaultMessage and qnaThreshold). This manages the bot conversations with the users.

## Frequently Asked Questions
#### I want to include the knowledge base question along with the answer in bot responses. Is that possible?
Yes. You can override the 'respondFromQnAMakerResult' method and customize that as needed. [QnAMakerWithFunctionOverrides](https://github.com/Microsoft/BotBuilder-CognitiveServices/tree/master/Node/samples/QnAMakerWithFunctionOverrides) is a sample bot doing the same. 
````
basicQnAMakerDialog.respondFromQnAMakerResult = function(session, qnaMakerResult){
	var result = qnaMakerResult;
	session.send(result.answers[0].questions[0]);
	cognitiveservices.QnAMakerDialog.prototype.respondFromQnAMakerResult(session, qnaMakerResult);
}
````

#### I don't want to end the dialog after the response. Can I customize that?
Yes. You can simply override the 'defaultWaitNextMessage' as shown in [QnAMakerWithFunctionOverrides](https://github.com/Microsoft/BotBuilder-CognitiveServices/tree/master/Node/samples/QnAMakerWithFunctionOverrides). 
````
basicQnAMakerDialog.defaultWaitNextMessage = function(session, qnaMakerResult){
}
````
