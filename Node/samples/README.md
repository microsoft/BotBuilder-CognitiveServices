## Overview
QnA Maker Dialog is to simplify the bot development using QnA Maker knowledge bases. The bot can consume the QnA Maker dialog as shown in the examples to forward the conversation to QnA Maker service and post the answer to the user in a seamless way.

## Bot Samples

|**Example**     | **Description**                                   
| ---------------| ---------------------------------------------
|[QnAMakerSimpleBot](https://github.com/Microsoft/BotBuilder-CognitiveServices/tree/master/Node/samples/QnAMakerSimpleBot) | Demonstrates a simple QnA bot where the bot responds with the top answer above specified threshold.      
|[QnAMakerBotWithActiveLearning](https://github.com/Microsoft/BotBuilder-CognitiveServices/tree/master/Node/samples/QnAMakerBotWithActiveLearning) | Demonstrates a QnA bot, where the high confidence answers are directly sent as bot response. In case the confidence is low but multiple good matches are found, it asks for options through a prompt and returns the answers based on the selection. This feedback is also sent back to the service for training the knowledge base with user utterences.
|[QnAMakerWithFunctionOverrides](https://github.com/Microsoft/BotBuilder-CognitiveServices/tree/master/Node/samples/QnAMakerWithFunctionOverrides) | Demonstrates a QnA bot, where the default methods are overridden to add some custom logic. 
