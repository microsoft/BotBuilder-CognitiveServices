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
bot.set('storage', new builder.MemoryBotStorage());         // Register in-memory state storage
server.post('/api/messages', connector.listen());

//=========================================================
// Recognizers
//=========================================================

var qnarecognizer = new cognitiveservices.QnAMakerRecognizer({
    knowledgeBaseId: 'set your kbid here',
    authKey: 'set your authorization key here',
    top: 4});

var model='set your luis model uri';
var recognizer = new builder.LuisRecognizer(model);

//=========================================================
// Bot Dialogs
//=========================================================
var intents = new builder.IntentDialog({ recognizers: [recognizer, qnarecognizer] });
bot.dialog('/', intents);

intents.matches('luisIntent1', builder.DialogAction.send('Inside LUIS Intent 1.'));

intents.matches('luisIntent2', builder.DialogAction.send('Inside LUIS Intent 2.'));

intents.matches('qna', [
    function (session, args, next) {
        var answerEntity = builder.EntityRecognizer.findEntity(args.entities, 'answer');
        session.send(answerEntity.entity);
    }
]);

intents.onDefault([
    function(session){
        session.send('Sorry!! No match!!');
    }
]);