
var restify = require('restify');
var builder = require('botbuilder');
var qnaMakerDialog = require('../lib/QnAMakerDialog');

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

//var modelUri = 'http://qnaservice.cloudapp.net/KBService.svc/GetAnswer?kbid=&question=';

var dialog = new qnaMakerDialog.QnAMakerDialog({
	knowledgeBaseId: 'set your kbid here', 
	subscriptionKey: 'set your subscription key here', 
	qnaThreshold: 0.0});

bot.dialog('/', dialog);
