var restify = require('restify');
var builder = require('botbuilder');
var cognitiveservices = require('botbuilder-cognitiveservices');

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
server.post('/api/messages', connector.listen());

var bot = new builder.UniversalBot(connector);
bot.set('storage', new builder.MemoryBotStorage());         // Register in-memory state storage

//=========================================================
// Bots Dialogs
//=========================================================

var qnaRecognizer = new cognitiveservices.QnAMakerRecognizer({
    knowledgeBaseId: 'set your kbid here',
    authKey: 'set your authorization key here',
});

var LuisModelUrl = '';      // set your LUIS url with LuisActionBinding models (see samples/LuisActionBinding/LUIS_MODEL.json)
var luisRecognizer = new builder.LuisRecognizer(LuisModelUrl);

// Setup dialog
var intentsDialog = new builder.IntentDialog({ recognizers: [qnaRecognizer, luisRecognizer] });
bot.dialog('/', intentsDialog);

// QnA Maker
intentsDialog.matches('qna', (session, args, next) => {
    var answerEntity = builder.EntityRecognizer.findEntity(args.entities, 'answer');
    session.send(answerEntity.entity);
});

// LUIS Action Binding
var SampleActions = require('../LuisActionBinding/all');
cognitiveservices.LuisActionBinding.bindToBotDialog(bot, intentsDialog, LuisModelUrl, SampleActions);

// Default message
intentsDialog.onDefault(session => session.send('Sorry, I didn\'t understand that.'));