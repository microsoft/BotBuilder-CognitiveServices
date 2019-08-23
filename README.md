# Status Update - ARCHIVED

As of August 2019, this repo is now archived. 

From now on, https://github.com/Microsoft/botframework-sdk repo will be used as hub, with pointers to all the different SDK languages, tools and samples repos.

# [Microsoft Cognitive Services](https://www.microsoft.com/cognitive-services) control for [Microsoft Bot Builder V3](https://github.com/Microsoft/BotBuilder-V3)

## V3 Deprecation Notification

Microsoft Bot Framework SDK V4 was released in September 2018, and since then we have shipped a few dot-release improvements. As announced previously, the V3  SDK is being retired with final long-term support ending on December 31st, 2019.
Accordingly, there will be no more development in this repo. **Existing V3 bot workloads will continue to run without interruption. We have no plans to disrupt any running workloads**.

We highly recommend that you start migrating your V3 bots to V4. In order to support this migration we have produced migration documentation and will provide extended support for migration initiatives (via standard channels such as Stack Overflow and Microsoft Customer Support).

For more information please refer to the following references:
* Migration Documentation: https://aka.ms/v3v4-bot-migration
* End of lifetime support announcement: https://aka.ms/bfmigfaq
* Primary V4 Repositories to develop Bot Framework bots
  * [Botbuilder for dotnet](https://github.com/microsoft/botbuilder-dotnet)
  * [Botbuilder for JS](https://github.com/microsoft/botbuilder-js) 
* QnA Maker Libraries were replaced with the following V4 libraries:
  * [Libraries for dotnet](https://github.com/Microsoft/botbuilder-dotnet/tree/master/libraries/Microsoft.Bot.Builder.AI.QnA)
  * [Libraries for JS](https://github.com/Microsoft/botbuilder-js/blob/master/libraries/botbuilder-ai/src/qnaMaker.ts)
* Azure Libraries were replaced with the following V4 libraries:
  * [Botbuilder for JS Azure](https://github.com/Microsoft/botbuilder-js/tree/master/libraries/botbuilder-azure)
  * [Botbuilder for dotnet Azure](https://github.com/Microsoft/botbuilder-dotnet/tree/master/libraries/Microsoft.Bot.Builder.Azure)

## Description

The cognitive services control makes consuming different Microsoft Cognitive Services easy for bots developed using [Microsoft Bot Builder](https://github.com/microsoft/botbuilder) SDK. The control is available for [C#](https://www.nuget.org/packages/Microsoft.Bot.Builder/) and [Node.js](https://www.npmjs.com/package/botbuilder) SDKs.

Currently the control has support for the following cognitive services:

- [QnA Maker](https://qnamaker.ai/): This service enables developers to build, train and publish a simple question and answer bot based on FAQ URLs, structured documents or editorial content. The bot can consume the QnA Maker dialog implemented by this control to forward the conversation to QnA Maker service and relay back the answers to the user.

- [LUIS.ai](https://luis.ai): Language Understanding Intelligent Service (LUIS) enables developers to build smart applications that can understand human language and react accordingly to user requests. There are times when you may want to link an intent to an action at client side (e.g.: in your Bot, or web app, or even a console app), with an easy binding logic for it, where you can also resolve complex things in order to fulfill an user's intent.

## More Information

Read these resources for more information about the Microsoft Bot Framework, Bot Builder SDK and Cognitive service:

* [Microsoft Bot Framework Overview](https://docs.microsoft.com/en-us/bot-framework/)
* [Microsoft Bot Framework Bot Builder SDK V3](https://github.com/Microsoft/BotBuilder-V3)
* [Microsoft Bot Framework Samples](https://github.com/Microsoft/BotBuilder-Samples/tree/v3-sdk-samples)
* [Microsoft Cognitive Services](https://www.microsoft.com/cognitive-services)
* [QnA Maker](https://qnamaker.ai/)
* [LUIS.ai](https://luis.ai/)
