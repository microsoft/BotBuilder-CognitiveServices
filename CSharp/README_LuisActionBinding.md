# LUIS Action Binding

## Overview
The following examples demonstrate how to use LUIS Action Binding with your Microsoft Bot Framework bot in C#. 

## Prerequisites
You should create an account at [LUIS.ai](https://luis.ai). This is a free service under Cognitive Services with a limit of 10,000 transactions per month.

## Code Highlights

### Installation
LUIS Action Binding is distributed in a separate NuGet package called **Microsoft.Bot.Builder.CognitiveServices** for C#.

### What is LUIS Action Binding?

There are times when you may want to link an intent to an action at client side (e.g.: in your Bot, or web app, or even a console app), with an easy binding logic for it, where you can also resolve complex things in order to fulfill an user's intent. In the same way that you can define an intent at LUIS UI for your app, you can also specify requirements for this action to be triggered when bound to an intent at client side. These requirements are known as action members, and will match recognizable entities for the intent that the action maps to.

The framework for client side Action Binding supports only one action per intent. Each action includes members (fields) mapping to entities. These action members can be optional or required, the client framework will provide you the tools to validate the action's state in order to see if it can be fulfilled or if further information is required from the user.

As said, the framework provides contracts to allow defining your actions at client side, a way to bind them to LUIS intents, and additional objects that allows validating and filling mandatory missing entities before you can proceed with the action fulfillment in order to complete the flow.

## Sample Bot
You can find a simple sample bot that uses LUIS Action Binding [here](Samples/LuisActions.Samples.Bot).

A full description of all available samples can be found [here](Samples/LuisActionBinding/README.md).

## More Information
Read these resources for more information about the Microsoft Bot Framework, Bot Builder SDK and LUIS:

* [Microsoft Bot Framework Overview](https://docs.botframework.com/en-us/)
* [Microsoft Bot Framework Bot Builder SDK](https://github.com/Microsoft/BotBuilder)
* [Microsoft Bot Framework Samples](https://github.com/Microsoft/BotBuilder-Samples)
* [Enable language understanding with LUIS](https://docs.microsoft.com/en-us/bot-framework/dotnet/bot-builder-dotnet-luis-dialogs)
* [LUIS Help Docs](https://www.luis.ai/Help/)
* [Cognitive Services Documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/)