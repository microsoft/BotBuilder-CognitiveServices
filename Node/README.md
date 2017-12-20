# [Microsoft Cognitive Services](https://www.microsoft.com/cognitive-services) control for [Microsoft Bot Builder](https://github.com/microsoft/botbuilder)

The Cognitive Services control makes consuming different Microsoft Cognitive Services easy for bots developed using [Microsoft Bot Builder](https://github.com/microsoft/botbuilder) SDK.

Currently the control has support for the following cognitive services:

- [QnA Maker](https://qnamaker.ai/): This service enables developers to build, train and publish a simple question and answer bot based on FAQ URLs, structured documents or editorial content. The bot can consume the QnA Maker dialog implemented by this control to forward the conversation to QnA Maker service and relay back the answers to the user.

    **[View QnA Maker Dialog documentation](samples/QnAMaker/README.md)**

- [LUIS.ai](https://luis.ai): Language Understanding Intelligent Service (LUIS) enables developers to build smart applications that can understand human language and react accordingly to user requests. There are times when you may want to link an intent to an action at client side (e.g.: in your Bot, or web app, or even a console app), with an easy binding logic for it, where you can also resolve complex things in order to fulfill an user's intent.

    **[View LUIS Action Binding documentation](samples/LuisActionBinding/README.md)**