
var restify = require('restify');
var builder = require('botbuilder');
var cognitiveservices = require('../lib/botbuilder-cognitiveservices');

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

var dialog = new cognitiveservices.QnAMakerDialog({
	knowledgeBaseId: 'set your kbid here', 
	subscriptionKey: 'set your subscription key here', 
	defaultMessage: 'No match! Try changing the query terms!',
	qnaThreshold: 10.0});

bot.dialog('/', dialog);
