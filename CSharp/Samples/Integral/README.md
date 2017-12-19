# Simple Bot with LUIS Action Binding Sample & QnA Maker Dialog

A simple bot combining LUIS Action Binding and QnA Maker Dialog, a bot that can answer common questions and also respond to user intents with pre-defined actions.

### Prerequisites

The minimum prerequisites to run this sample are:
* The latest update of Visual Studio 2015. You can download the community version [here](http://www.visualstudio.com) for free.
* The Bot Framework Emulator. To install the Bot Framework Emulator, download it from [here](https://emulator.botframework.com/). Please refer to [this documentation article](https://github.com/microsoft/botframework-emulator/wiki/Getting-Started) to know more about the Bot Framework Emulator.

### QnA Make Dialog
If you want to test this sample, you have to import the demo QnA Maker Knowledge base from [Smalltalk.tsv](Smalltalk.tsv) file to your [QnA Maker account](https://qnamaker.ai/).

You'll need to create a [new service instance](https://qnamaker.ai/Create) with the default values. Once created, use the *Replace Knowledge Base* option to upload the [Smalltalk](Smalltalk.tsv) model.

![Replace Knowledge Base](images/qnamaker-replace.png)

You can obtain these values by publishing the Knowledge base, using *Publish* button. Once it is published, you'll see a success message with a sample HTTP Request to QnA Maker service. You'll need the `KnowledgeID` and `Ocp-Api-Subscription-Key`.

![Knowledge Base Published](images/qnamaker-publish.png)

Now update the `QnAMakerSubscriptionKey` and `QnAMakerKnowledgeBaseId` appSettings in the [Web.config](Web.config#L18-L19).

### LUIS Application
If you want to test this sample, you have to import the pre-build [LUIS_MODEL.json](../LuisActions/LUIS_MODEL.json) file to your [LUIS account](https://luis.ai/).

The first step to using LUIS is to create or import an application. Go to the home page, www.luis.ai, and log in. After creating your LUIS account you'll be able to Import an Existing Application where can you can select a local copy of the LUIS_MODEL.json file and import it.

![Import an Existing Application](../LuisActions/images/prereqs-import.png)

Once you imported the application you'll need to "train" the model ([Training](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/train-test)) before you can "Publish" the model in an HTTP endpoint. For more information, take a look at [Publishing a Model](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/publishapp).

Finally, edit the [Web.config](Web.config#L14-L15) and update the `LuisSubscriptionKey` and `LuisApplicationId` appSettings with the values corresponding to your Subscription and Application.

### Code Highlights

QnA Maker can be used using Scorables, meaning all messages can be intercepted and validated first with the QnA Maker Knowledge base. If a question/answer pair is found matching the message's text, a reply will be sent automatically.

Check out how the [`QnAMakerModule` is registered](Global.asax.cs#L18-L22), which it internally [registers the `QnAMakerScorable`](../../Library/QnAMaker/QnAMaker/QnAMakerService/QnAMakerModule.cs#L73-L78) and `QnAMakerDialog`.

````C#
builder.RegisterModule(new QnAMakerModule(
    ConfigurationManager.AppSettings["QnAMakerSubscriptionKey"],
    ConfigurationManager.AppSettings["QnAMakerKnowledgeBaseId"],
    "I don't understand this right now! Try another query!",
    0.50));
````

LUIS Action Binding is implemented as the default dialog, [`LuisSamplesDialog`](Dialogs/LuisSamplesDialog.cs).

````C#
public async Task<HttpResponseMessage> Post([FromBody]Activity activity, CancellationToken token)
{
    if (activity.Type == ActivityTypes.Message)
    {
        await Conversation.SendAsync(activity, () => new LuisSamplesDialog());
    }
    else
    {
        HandleSystemMessage(activity);
    }
    var response = Request.CreateResponse(HttpStatusCode.OK);
    return response;
}
````

### More Information

* [LUIS Action Binding Documentation](../LuisActions/README.md)
* [QnA Maker Dialog Documentation](../QnAMaker/README.md)
