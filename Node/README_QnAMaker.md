# QnA Maker Dialog for Microsoft Bot Framework

## Overview
The following examples demonstrate how to use the QnA Maker Dialog with your Microsoft Bot Framework bot in Node.js.

## Prerequisites
You should create a knowledge base at [QnA Maker](https://qnamaker.ai). This is a free tool under Cognitive Services with a limit of 10,000 transactions per month, 10 per minute.

## Code Highlights

### Installation
Install the botbuilder-cognitiveservices  module using npm.

`npm install --save botbuilder-cognitiveservices`

The following parameters are passed when invoking the QnA Maker service.
+ Authorization Key - Each registered user on [QnA Maker](https://qnamaker.ai) is assigned an unique subscription key for metering.
+ Knowledge Base ID - Each knowledge base created is assigned a unique subscription key by the tool.
+ Default Message (optional) - Message to show if there is no match in the knowledge base.
+ Score Threshold (optional) - Threshold value of the match confidence score returned by the service. It ranges from 0-1. This is useful in controlling the relevance of the responses.
+ Top (optional) - Number of results returned.
+ EndpointHostName (optional) - To be used with the V4 APIs and the GA stack. See [here](https://aka.ms/qnamaker-docs-changesfrompreview).

### Calling the QnAMakerDialog
The example extends the QnAMakerDialog, and calls it with the required parameters.

```js
var recognizer = new cognitiveservices.QnAMakerRecognizer({
	knowledgeBaseId: 'set your kbid here',
	authKey: 'set your authorization key here',
});

var BasicQnAMakerDialog = new cognitiveservices.QnAMakerDialog({
	recognizers: [recognizer],
	defaultMessage: 'No good match in FAQ.',
	qnaThreshold: 0.5
});
```

## Sample Bots
You can find a simple sample bot that uses the QnAMakerDialog [here](samples/QnAMaker/QnAMakerSimpleBot/app.js).

A list of more advanced samples can be found [here](samples/QnAMaker/README.md).

## More Information
Read these resources for more information about the Microsoft Bot Framework, Bot Builder SDK and QnA Maker:

* [Microsoft Bot Framework Overview](https://docs.microsoft.com/en-us/bot-framework/)
* [Microsoft Bot Framework Bot Builder SDK](https://github.com/Microsoft/BotBuilder)
* [Microsoft Bot Framework Samples](https://github.com/Microsoft/BotBuilder-Samples)
* [QnA Maker Documentation](https://qnamaker.ai/Documentation)

