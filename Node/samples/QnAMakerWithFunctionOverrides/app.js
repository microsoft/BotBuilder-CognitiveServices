
var restify = require('restify');
var builder = require('botbuilder');
var cognitiveservices = require('../../lib/botbuilder-cognitiveservices');
var customQnAMakerTools = require('./CustomQnAMakerTools');

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
	knowledgeBaseId: 'set your kbid here', 
	subscriptionKey: 'set your subscription key here',
	top: 4});

var customQnAMakerTools = new customQnAMakerTools.CustomQnAMakerTools();
bot.library(customQnAMakerTools.createLibrary());
	
var basicQnAMakerDialog = new cognitiveservices.QnAMakerDialog({
	recognizers: [recognizer],
	defaultMessage: 'No match! Try changing the query terms!',
	qnaThreshold: 0.3,
	feedbackLib: customQnAMakerTools
});

// Override to also include the knowledgebase question on confident matches
basicQnAMakerDialog.respondFromQnAMakerResult = function(session, qnaMakerResult){
	var result = qnaMakerResult;
	session.send(result.answers[0].questions[0]);
	cognitiveservices.QnAMakerDialog.prototype.respondFromQnAMakerResult(session, qnaMakerResult);
}

// Override to not call .endDialog()
basicQnAMakerDialog.defaultWaitNextMessage = function(session, qnaMakerResult){
}

bot.dialog('/', basicQnAMakerDialog);