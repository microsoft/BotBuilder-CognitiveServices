# QnA Maker Dialog for Microsoft Bot Framework

## Overview
The following examples demonstrate how to use the QnA Maker Dialog your Microsoft Bot Framework bot in C#. 

## Prerequisites
You should create a knowledge base at [QnA Maker](https://qnamaker.ai).

## Code Highlights

### Usage
QnAMaker Dialog is distributed in a separate NuGet package called **Microsoft.Bot.Builder.CognitiveServices** for C#.

The **QnAMakerDialog** object contains the **StartAsync** and **MessageReceived** methods. When the dialog’s instantiated, the dialog’s StartAsync method runs and calls IDialogContext.Wait with the continuation delegate that’s called when there is a new message. In the initial case, there is an immediate message available (the one that launched the dialog) and the message is immediately passed to the **MessageReceived** method (in the **QnAMakerDialog** object).

The **MessageReceived** method calls your QnA Maker service and returns the response to the user.

The following parameters are passed when invoking the QnA Maker service.
+ Authorization Key - Each registered user on [QnA Maker](https://qnamaker.ai) is assigned an unique subscription key for metering.
+ Knowledge Base ID - Each knowledge base created is assigned a unique subscription key by the tool.
+ Default Message (optional) - Message to show if there is no match in the knowledge base.
+ Score Threshold (optional) - Threshold value of the match confidence score returned by the service. It ranges from 0-1. This is useful in controlling the relevance of the responses.
+ Number of results (optional) - Specify the TopN results to retrieve from the service.
+ Endpoint Hostname (optional) - If using the [V4 APIs](https://aka.ms/qnamaker-v4-apis) and the GA stack, you need to also pass the endpoint hostname.

### Calling the QnAMakerDialog
The example extends the QnAMakerDialog, and calls it with the required parameters.

````C#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;

//Inherit from the QnAMakerDialog
[Serializable]
public class BasicQnAMakerDialog : QnAMakerDialog
{        
	//Parameters to QnAMakerService are:
	//Compulsory: authKey, knowledgebaseId, 
	//Optional: defaultMessage, scoreThreshold[Range 0.0 – 1.0]
	public BasicQnAMakerDialog() : base(new QnAMakerService(new QnAMakerAttribute(Utils.GetAppSetting("QnASubscriptionKey"), Utils.GetAppSetting("QnAKnowledgebaseId"), "No good match in FAQ.", 0.5)))
	{}
}
````

## Sample Bot
You can find a sample bot that uses the QnAMakerDialog [here](https://github.com/Microsoft/BotBuilder-CognitiveServices/tree/master/CSharp/Samples/QnAMaker)

## More Information
Read these resources for more information about the Microsoft Bot Framework, Bot Builder SDK and QnA Maker:

* [Microsoft Bot Framework Overview](https://docs.microsoft.com/en-us/bot-framework/resources-links-help)
* [Microsoft Bot Framework Bot Builder SDK](https://github.com/Microsoft/BotBuilder)
* [Microsoft Bot Framework Samples](https://github.com/Microsoft/BotBuilder-Samples)
* [QnA Maker Documentation](https://qnamaker.ai/Documentation)

