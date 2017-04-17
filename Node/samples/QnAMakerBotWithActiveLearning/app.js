
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

var recognizer = new cognitiveservices.QnAMakerRecognizer({
	//knowledgeBaseId: 'set your kbid here', 
	//subscriptionKey: 'set your subscription key here'});
	knowledgeBaseId: 'c00db5e6-5802-4e32-bd1d-0cbf83c86da1', 
	subscriptionKey: '682875376ad54258acc921d95b4500c2',
	top: 3});
	
var basicQnAMakerDialog = new cognitiveservices.QnAMakerDialog({
	recognizers: [recognizer],
	defaultMessage: 'No match! Try changing the query terms!',
	qnaThreshold: 0.3
});


bot.dialog('/', basicQnAMakerDialog);
