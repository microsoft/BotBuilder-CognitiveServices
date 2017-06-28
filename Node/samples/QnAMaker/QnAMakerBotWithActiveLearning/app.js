var restify = require('restify');
var builder = require('botbuilder');
var cognitiveservices = require('../../../lib/botbuilder-cognitiveservices');

//=========================================================
// Bot Setup
//=========================================================

// Setup Restify Server
var server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function () {
    console.log('%s listening to %s', server.name, server.url); 
});
  
// Create chat bot
var connector = new builder.ChatConnector({
    appId: process.env.MICROSOFT_APP_ID,
    appPassword: process.env.MICROSOFT_APP_PASSWORD
});
var bot = new builder.UniversalBot(connector);
server.post('/api/messages', connector.listen());

//=========================================================
// Bots Dialogs
//=========================================================

var recognizer = new cognitiveservices.QnAMakerRecognizer({
    knowledgeBaseId: '67f6513c-8abc-4563-944e-07f5da2ff187', 
    subscriptionKey: '6cbf13a0503c472c805ab0354317f75e',
    top: 3});

var qnaMakerTools = new cognitiveservices.QnAMakerTools();
bot.library(qnaMakerTools.createLibrary());

var basicQnAMakerDialog = new cognitiveservices.QnAMakerDialog({
    recognizers: [recognizer],
    defaultMessage: 'No match! Try changing the query terms!',
    qnaThreshold: 0.3,
    feedbackLib: qnaMakerTools
});

bot.dialog('/', basicQnAMakerDialog);
